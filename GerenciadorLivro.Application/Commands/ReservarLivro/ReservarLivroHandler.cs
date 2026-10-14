using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.ReservarLivro
{
    public class ReservarLivroHandler : IRequestHandler<ReservarLivroCommand, Result<int>>
    {
        private readonly IAppDbContext _dbContext;
        public ReservarLivroHandler(IAppDbContext dbContext) => _dbContext = dbContext;

        public async Task<Result<int>> Handle(ReservarLivroCommand command, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .AsNoTracking()
                .Where(l => l.Id == command.LivroId)
                .Select(l => new { l.Id, l.Disponivel })
                .FirstOrDefaultAsync(cancellationToken);

            if (livro == null)
            {
                return Result<int>.Falha($"Livro de id {command.LivroId} não encontrado");
            }

            var usuarioExiste = await _dbContext.Usuarios
                .AnyAsync(u => u.Id == command.UsuarioId, cancellationToken);

            if (!usuarioExiste)
            {
                return Result<int>.Falha($"Usuário de id {command.UsuarioId} não encontrado");
            }

            // Um livro devolvido fica Disponivel = true, mas guardado para quem foi Notificado.
            // Para os demais usuários ele continua indisponível, então eles podem entrar na fila.
            var guardadoParaOutroUsuario = await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == command.LivroId
                            && r.Status == StatusReserva.Notificada
                            && r.UsuarioId != command.UsuarioId, cancellationToken);

            if (livro.Disponivel && !guardadoParaOutroUsuario)
            {
                return Result<int>.Falha($"Livro de id {command.LivroId} está disponível. Registre o empréstimo em vez de reservar");
            }

            var reservaDuplicada = await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == command.LivroId
                            && r.UsuarioId == command.UsuarioId
                            && (r.Status == StatusReserva.Ativa || r.Status == StatusReserva.Notificada), cancellationToken);

            if (reservaDuplicada)
            {
                return Result<int>.Falha($"Usuário de id {command.UsuarioId} já possui uma reserva pendente para o livro de id {command.LivroId}");
            }

            var usuarioEstaComLivro = await _dbContext.Emprestimos
                .AnyAsync(e => e.LivroId == command.LivroId
                            && e.UsuarioId == command.UsuarioId
                            && e.DataDevolucaoReal == null, cancellationToken);

            if (usuarioEstaComLivro)
            {
                return Result<int>.Falha($"Usuário de id {command.UsuarioId} já está com o livro de id {command.LivroId} emprestado");
            }

            var reserva = new Reserva(command.LivroId, command.UsuarioId, DateTime.Now);
            _dbContext.Reservas.Add(reserva);

            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                // Duas requisições iguais ao mesmo tempo passam pela checagem acima e o índice único
                // filtrado barra a segunda. Mas DbUpdateException também é conexão caída, FK, timeout...
                // Só tratamos como "duplicada" se a reserva pendente realmente existe; o resto sobe.
                _dbContext.Reservas.Remove(reserva); // solta a reserva que falhou do rastreamento

                var duplicadaAgora = await _dbContext.Reservas
                    .AsNoTracking()
                    .AnyAsync(r => r.LivroId == command.LivroId
                                && r.UsuarioId == command.UsuarioId
                                && (r.Status == StatusReserva.Ativa || r.Status == StatusReserva.Notificada), cancellationToken);

                if (!duplicadaAgora)
                {
                    throw;
                }

                return Result<int>.Falha($"Usuário de id {command.UsuarioId} já possui uma reserva pendente para o livro de id {command.LivroId}");
            }

            return Result<int>.Ok(reserva.Id);
        }
    }
}
