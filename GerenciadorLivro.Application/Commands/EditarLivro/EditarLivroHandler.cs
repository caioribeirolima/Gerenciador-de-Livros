using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.EditarLivro
{
    public class EditarLivroHandler : IRequestHandler<EditarLivroCommand, Result>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IUnitOfWork _unitOfWork;

        public EditarLivroHandler(
            ILivroRepository livroRepository,
            IUnitOfWork unitOfWork)
        {
            _livroRepository = livroRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(EditarLivroCommand request, CancellationToken cancellationToken)
        {
            var livro = await _livroRepository.BuscarPorIdAsync(request.Id, cancellationToken);

            if (livro is null)
                return Result.NaoEncontrado($"Livro de id {request.Id} não encontrado");

            livro.Titulo = request.Titulo;
            livro.AnoPublicacao = request.AnoPublicacao;
            livro.CategoriaId = request.CategoriaId;
            livro.EditoraId = request.EditoraId;

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
