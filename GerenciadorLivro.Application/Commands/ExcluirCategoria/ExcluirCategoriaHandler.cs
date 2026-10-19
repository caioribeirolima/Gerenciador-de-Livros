using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.ExcluirCategoria
{
    public class ExcluirCategoriaHandler : IRequestHandler<ExcluirCategoriaCommand, Result>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly ILivroRepository _livroRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ExcluirCategoriaHandler(
            ICategoriaRepository categoriaRepository,
            ILivroRepository livroRepository,
            IUnitOfWork unitOfWork)
        {
            _categoriaRepository = categoriaRepository;
            _livroRepository = livroRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ExcluirCategoriaCommand command, CancellationToken cancellationToken)
        {
            var categoria = await _categoriaRepository
                .BuscarPorIdAsync(command.Id, cancellationToken);

            if (categoria == null)
            {
                return Result.Falha("Categoria não existe");
            }

            var possuiLivros = await _livroRepository
                .VerificarLivroPorCategoria(command.Id, cancellationToken);


            if (possuiLivros)
            {
                return Result.Falha("Existem livros vinculados a esta categoria (incluindo livros excluídos)");
            }

            _categoriaRepository.ExcluirAsync(categoria);

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
