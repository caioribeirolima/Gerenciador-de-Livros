using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarCategoria
{
    public class CriarCategoriaHandler : IRequestHandler<CriarCategoriaCommand, Result<int>>
    {
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarCategoriaHandler(
            ICategoriaRepository categoriaRepository,
            IUnitOfWork unitOfWork) 
        {
            _categoriaRepository = categoriaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CriarCategoriaCommand command, CancellationToken cancellationToken)
        {

            var stringLimpa = command.Nome.Trim();

            if (string.IsNullOrEmpty(stringLimpa))
            {
                return Result<int>.Falha("Nome da categoria inválido");
            }

            var existsCategoria = await _categoriaRepository
                .ExisteComNomeAsync(stringLimpa, cancellationToken);


            if (existsCategoria)
            {
                return Result<int>.Falha("Uma categoria já está cadastrada com este nome");
            }

            var categoria = new Categoria
            {
                Nome = stringLimpa,
            };

            _categoriaRepository.Adicionar(categoria);
            
            await _unitOfWork.CommitAsync(cancellationToken);
            return Result<int>.Ok(categoria.Id);


        }
    }
}
