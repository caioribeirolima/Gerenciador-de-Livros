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
                return Result.Falha($"Empréstimo de id {command.EmprestimoId} não encontrado");
            }

            if (emprestimo.DataDevolucaoReal != null)
            {
                return Result.Falha($"Empréstimo de id {command.EmprestimoId} já foi devolvido");
            }

            var livro = await _livroRepository.BuscarPorIdExcluidosAsync(emprestimo.LivroId, cancellationToken);

            if (livro == null)
            {
                return Result.Falha($"Livro vinculado ao empréstimo de id {emprestimo.LivroId} não foi encontrado");
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
