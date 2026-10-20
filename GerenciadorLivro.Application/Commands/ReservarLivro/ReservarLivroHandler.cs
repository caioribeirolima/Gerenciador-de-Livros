using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.ReservarLivro
{
    public class ReservarLivroHandler : IRequestHandler<ReservarLivroCommand, Result<int>>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IEmprestimoRepository _emprestimoRepository;
        private readonly IReservaRepository _reservaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReservarLivroHandler(
            ILivroRepository livroRepository,
            IEmprestimoRepository emprestimoRepository,
            IReservaRepository reservaRepository,
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork)
        {
            _livroRepository = livroRepository;
            _emprestimoRepository = emprestimoRepository;
            _reservaRepository = reservaRepository;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(ReservarLivroCommand command, CancellationToken cancellationToken)
        {
            var livro = await _livroRepository.BuscarPorIdAsync(command.LivroId, cancellationToken);

            if (livro == null)
            {
                return Result<int>.NaoEncontrado($"Livro de id {command.LivroId} não encontrado");
            }

            var usuarioExiste = await _usuarioRepository.ExisteAsync(command.UsuarioId, cancellationToken);

            if (!usuarioExiste)
            {
                return Result<int>.NaoEncontrado($"Usuário de id {command.UsuarioId} não encontrado");
            }

            var guardadoParaOutroUsuario = await _reservaRepository.ExisteReservaNotificadaDeOutroUsuarioAsync(command.LivroId, command.UsuarioId, cancellationToken);

            if (livro.Disponivel && !guardadoParaOutroUsuario)
            {
                return Result<int>.Conflito($"Livro de id {command.LivroId} está disponível. Registre o empréstimo em vez de reservar");
            }

            var reservaDuplicada = await _reservaRepository.ExisteReservaPendenteAsync(command.LivroId, command.UsuarioId, cancellationToken);

            if (reservaDuplicada)
            {
                return Result<int>.Conflito($"Usuário de id {command.UsuarioId} já possui uma reserva pendente para o livro de id {command.LivroId}");
            }

            var usuarioEstaComLivro = await _emprestimoRepository.ExisteEmprestimoAbertoDoUsuarioAsync(command.LivroId, command.UsuarioId, cancellationToken);

            if (usuarioEstaComLivro)
            {
                return Result<int>.Conflito($"Usuário de id {command.UsuarioId} já está com o livro de id {command.LivroId} emprestado");
            }

            var reserva = new Reserva(command.LivroId, command.UsuarioId, DateTime.Now);
            _reservaRepository.Adicionar(reserva);

            try
            {
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                _reservaRepository.RemoverReserva(reserva);

                var duplicadaAgora = await _reservaRepository
                    .ExisteReservaPendenteAsync(command.LivroId, command.UsuarioId, cancellationToken);

                if (!duplicadaAgora)
                {
                    throw;
                }

                return Result<int>.Conflito($"Usuário de id {command.UsuarioId} já possui uma reserva pendente para o livro de id {command.LivroId}");
            }

            return Result<int>.Ok(reserva.Id);
        }
    }
}
