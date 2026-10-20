using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace GerenciadorLivro.Application.Commands.RenovarEmprestimo
{
    public class RenovarEmprestimoHandler : IRequestHandler<RenovarEmprestimoCommand, Result<int>>
    {
        private readonly IEmprestimoRepository _emprestimoRepository;
        private readonly ILivroRepository _livroRepository;
        private readonly IReservaRepository _reservaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly EmprestimoSettings _emprestimoSettings;

        public RenovarEmprestimoHandler(
            IEmprestimoRepository emprestimoRepository,
            ILivroRepository livroRepository,
            IReservaRepository reservaRepository,
            IUnitOfWork unitOfWork,
            IOptions<EmprestimoSettings> emprestimoSettings)
        {
            _emprestimoRepository = emprestimoRepository;
            _livroRepository = livroRepository;
            _reservaRepository = reservaRepository;
            _unitOfWork = unitOfWork;
            _emprestimoSettings = emprestimoSettings.Value;
        }

        public async Task<Result<int>> Handle(RenovarEmprestimoCommand command, CancellationToken cancellationToken)
        {
            var emprestimo = await _emprestimoRepository.BuscarPorIdAsync(command.EmprestimoId, cancellationToken);

            if (emprestimo == null)
            {
                return Result<int>.Falha($"Empréstimo de id {command.EmprestimoId} não encontrado");
            }

            var hoje = DateTime.Now;
            var motivo = emprestimo.MotivoParaNaoRenovar(hoje);

            if (motivo != null)
            {
                return Result<int>.Falha(motivo);
            }

            // BuscarPorIdAsync respeita o filtro global, então devolve null para livro excluído.
            var livro = await _livroRepository.BuscarPorIdAsync(emprestimo.LivroId, cancellationToken);

            if (livro == null)
            {
                return Result<int>.Falha($"Livro de id {emprestimo.LivroId} foi excluído do acervo. O empréstimo não pode ser renovado, apenas devolvido");
            }

            var reservadoParaOutro = await _reservaRepository.ExisteReservaPendenteDeOutroUsuarioAsync(emprestimo.LivroId, emprestimo.UsuarioId, cancellationToken);

            if (reservadoParaOutro)
            {
                return Result<int>.Falha($"Livro de id {emprestimo.LivroId} possui reserva de outro usuário e não pode ser renovado");
            }

            emprestimo.Renovar(_emprestimoSettings.DiasParaDevolucao, hoje);

            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<int>.Ok(emprestimo.Id);
        }
    }
}
