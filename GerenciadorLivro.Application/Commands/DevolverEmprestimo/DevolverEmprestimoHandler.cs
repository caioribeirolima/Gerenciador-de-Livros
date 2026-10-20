using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Notifications.LivroDevolvido;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.DevolverEmprestimo
{
    public class DevolverEmprestimoHandler : IRequestHandler<DevolverEmprestimoCommand, Result>
    {
        private readonly IPublisher _publisher;
        private readonly IEmprestimoRepository _emprestimoRepository;
        private readonly ILivroRepository _livroRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DevolverEmprestimoHandler(
            IPublisher publisher, 
            IEmprestimoRepository emprestimoRepository,
            ILivroRepository livroRepository,
            IUnitOfWork unitOfWork)
        {
            _publisher = publisher;
            _emprestimoRepository = emprestimoRepository;
            _livroRepository = livroRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(DevolverEmprestimoCommand command, CancellationToken cancellationToken)
        {
            var emprestimo = await _emprestimoRepository.BuscarPorIdAsync(command.EmprestimoId, cancellationToken);

            if (emprestimo == null)
            {
                return Result.NaoEncontrado($"Empréstimo de id {command.EmprestimoId} não encontrado");
            }

            if (emprestimo.DataDevolucaoReal != null)
            {
                return Result.Conflito($"Empréstimo de id {command.EmprestimoId} já foi devolvido");
            }

            var livro = await _livroRepository.BuscarPorIdIncluindoExcluidosAsync(emprestimo.LivroId, cancellationToken);

            if (livro == null)
            {
                // A busca já inclui livros excluídos (soft delete), então null aqui significa que o
                // empréstimo aponta para um livro que não existe mais na tabela: dado corrompido.
                // O cliente não tem como corrigir isso (nem é erro dele), então não é 404/409:
                // é falha de integridade do servidor. Exceção -> 500 pelo UseExceptionHandler.
                throw new InvalidOperationException(
                    $"Inconsistência de dados: o empréstimo {emprestimo.Id} referencia o livro {emprestimo.LivroId}, que não existe.");
            }

            emprestimo.DataDevolucaoReal = DateTime.Now;
            livro.Disponivel = true;

            await _unitOfWork.CommitAsync(cancellationToken);

            await _publisher.Publish(new LivroDevolvidoNotification(
                emprestimo.Id,
                emprestimo.LivroId,
                emprestimo.DataDevolucaoPrevista,
                emprestimo.DataDevolucaoReal.Value), cancellationToken);

            return Result.Ok();
        }
    }
}
