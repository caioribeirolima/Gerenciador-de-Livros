using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarAutor
{
    public class CriarAutorHandler : IRequestHandler<CriarAutorCommand, Result<int>>
    {
        private readonly IAutorRepository _autorRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CriarAutorHandler(
            IAutorRepository autorRepository,
            IUnitOfWork unitOfWork)
        {
            _autorRepository = autorRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CriarAutorCommand command, CancellationToken cancellationToken)
        {
            var nome = command.Nome.Trim();

            var exists = await _autorRepository
                .ExisteComNomeAsync(nome, cancellationToken);

            if (exists)
            {
                return Result<int>.Conflito($"Autor com nome {nome} já registrado");
            }

            var autor = new Autor
            {
                Nome = nome,
            };

            _autorRepository.Adicionar(autor);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<int>.Ok(autor.Id);
        }
    }
}
