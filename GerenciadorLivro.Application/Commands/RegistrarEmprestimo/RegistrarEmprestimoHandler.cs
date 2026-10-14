using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GerenciadorLivro.Application.Commands.RegistrarEmprestimo
{
    public class RegistrarEmprestimoHandler : IRequestHandler<RegistrarEmprestimoCommand, Result<int>>
    {
        private readonly IAppDbContext _dbContext;
        private readonly EmprestimoSettings _emprestimoSettings;
        public RegistrarEmprestimoHandler(IAppDbContext dbContext, IOptions<EmprestimoSettings> emprestimoSettings)
        {
            _dbContext = dbContext;
            _emprestimoSettings = emprestimoSettings.Value;
        }

        public async Task<Result<int>> Handle(RegistrarEmprestimoCommand command, CancellationToken cancellationToken)
        {
            var livro = await _dbContext.Livros
                .Where(l => l.Id == command.LivroId)
                .FirstOrDefaultAsync(cancellationToken);

            if (livro == null)
            {
                return Result<int>.Falha($"Livro de id {command.LivroId} não encontrado");
            }

            if (!livro.Disponivel)
            {
                return Result<int>.Falha($"Livro de id {command.LivroId} não está disponível");
            }

            var usuarioExiste = await _dbContext.Usuarios
                .AnyAsync(u => u.Id == command.UsuarioId, cancellationToken);

            if (!usuarioExiste)
            {
                return Result<int>.Falha($"Usuário de id {command.UsuarioId} não encontrado");
            }

            var reservaNotificadaDeOutro = await _dbContext.Reservas
                .AnyAsync(r => r.LivroId == command.LivroId
                            && r.Status == StatusReserva.Notificada
                            && r.UsuarioId != command.UsuarioId, cancellationToken);

            if (reservaNotificadaDeOutro)
            {
                return Result<int>.Falha($"Livro de id {command.LivroId} está reservado para outro usuário");
            }

            // Se quem está pegando o livro é o dono da reserva, ela foi atendida.
            var reservaDoUsuario = await _dbContext.Reservas
                .Where(r => r.LivroId == command.LivroId
                         && r.UsuarioId == command.UsuarioId
                         && (r.Status == StatusReserva.Notificada || r.Status == StatusReserva.Ativa))
                .FirstOrDefaultAsync(cancellationToken);

            if (reservaDoUsuario != null)
            {
                reservaDoUsuario.Status = StatusReserva.Atendida;
                reservaDoUsuario.DataEncerramento = DateTime.Now;
            }

            var emprestimo = new Emprestimo
            {
                LivroId = command.LivroId,
                UsuarioId = command.UsuarioId,
                DataEmprestimo = DateTime.Now,
                DataDevolucaoPrevista = DateTime.Now.AddDays(_emprestimoSettings.DiasParaDevolucao)
            };

            // Livro e reserva já estão rastreados; só o empréstimo novo precisa ser adicionado.
            livro.Disponivel = false;
            _dbContext.Emprestimos.Add(emprestimo);

            // Empréstimo, livro e reserva vão juntos no mesmo SaveChanges (uma transação).
            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<int>.Ok(emprestimo.Id);
        }
    }
}
