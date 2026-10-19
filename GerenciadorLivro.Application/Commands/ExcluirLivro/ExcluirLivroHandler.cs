using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.ExcluirLivro
{
    public class ExcluirLivroHandler : IRequestHandler<ExcluirLivroCommand, Result>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExcluirLivroHandler(
            ILivroRepository livroRepository,
            IUnitOfWork unitOfWork)
        {
            _livroRepository = livroRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ExcluirLivroCommand request, CancellationToken cancellationToken)
        {
            var livro = await _livroRepository.BuscarPorIdAsync(request.Id, cancellationToken);

            if (livro is null)
                return Result.Falha($"Livro de id {request.Id} não encontrado");

            _livroRepository.Excluir(livro);

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
