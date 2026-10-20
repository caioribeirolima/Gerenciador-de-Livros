using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace GerenciadorLivro.Application.Commands.RegistrarEmprestimo
{
    public class RegistrarEmprestimoHandler : IRequestHandler<RegistrarEmprestimoCommand, Result<int>>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IEmprestimoRepository _emprestimoRepository;
        private readonly IReservaRepository _reservaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmprestimoSettings _emprestimoSettings;

        public RegistrarEmprestimoHandler(
            ILivroRepository livroRepository,
            IEmprestimoRepository emprestimoRepository,
            IReservaRepository reservaRepository,
            IUsuarioRepository usuarioRepository,
            IUnitOfWork unitOfWork,
            IOptions<EmprestimoSettings> emprestimoSettings)
        {
            _livroRepository = livroRepository;
            _emprestimoRepository = emprestimoRepository;
            _reservaRepository = reservaRepository;
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
            _emprestimoSettings = emprestimoSettings.Value;
        }

        public async Task<Result<int>> Handle(RegistrarEmprestimoCommand command, CancellationToken cancellationToken)
        {
            var livro = await _livroRepository.BuscarPorIdAsync(command.LivroId, cancellationToken);

            if (livro == null)
                return Result<int>.NaoEncontrado($"Livro de id {command.LivroId} não encontrado");

            if (!livro.Disponivel)
                return Result<int>.Conflito($"Livro de id {command.LivroId} não está disponível");

            var usuarioExiste = await _usuarioRepository.ExisteAsync(command.UsuarioId, cancellationToken);

            if (!usuarioExiste)
                return Result<int>.NaoEncontrado($"Usuário de id {command.UsuarioId} não encontrado");

            var reservaNotificadaDeOutro = await _reservaRepository
                .ExisteReservaNotificadaDeOutroUsuarioAsync(command.LivroId, command.UsuarioId, cancellationToken);

            if (reservaNotificadaDeOutro)
                return Result<int>.Conflito($"Livro de id {command.LivroId} está reservado para outro usuário");

            var reservaDoUsuario = await _reservaRepository
                .BuscarReservaPendenteDoUsuarioAsync(command.LivroId, command.UsuarioId, cancellationToken);

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

            livro.Disponivel = false;
            _emprestimoRepository.Adicionar(emprestimo);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<int>.Ok(emprestimo.Id);
        }
    }
}
