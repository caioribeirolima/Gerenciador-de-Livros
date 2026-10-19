using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.EditarCategoria
{
    public class EditarCategoriaHandler : IRequestHandler<EditarCategoriaCommand, Result>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _unitOfWork;
        public EditarCategoriaHandler(
            ICategoriaRepository categoriaRepository,
            IUnitOfWork unitOfWork) 
        {
            _categoriaRepository = categoriaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(EditarCategoriaCommand command, CancellationToken cancellationToken)
        {
            var nomeLimpo = command.Nome.Trim();

            if (string.IsNullOrEmpty(nomeLimpo))
                return Result.Falha("Nome da categoria inválido");

            var categoria = await _categoriaRepository
                .BuscarPorIdAsync(command.Id, cancellationToken);

            if (categoria == null)
                return Result.Falha("Categoria inexistente");

            if (!string.Equals(categoria.Nome, nomeLimpo, StringComparison.OrdinalIgnoreCase))
            {
                var existsCategoria = await _categoriaRepository
                    .ExisteComNomeAsync(nomeLimpo, cancellationToken);

                if (existsCategoria)
                    return Result.Falha($"Categoria com nome {nomeLimpo} já existe");
            }

            categoria.Nome = nomeLimpo;

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
