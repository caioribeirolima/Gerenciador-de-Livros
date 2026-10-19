using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.AdicionarAutorAoLivro
{
    public class AdicionarAutorAoLivroHandler : IRequestHandler<AdicionarAutorAoLivroCommand, Result>
    {
        private readonly IAutorRepository _autorRepository;
        private readonly ILivroRepository _livroRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AdicionarAutorAoLivroHandler(
            IAutorRepository autorRepository,
            ILivroRepository livroRepository,
            IUnitOfWork unitOfWork)
        {
            _autorRepository = autorRepository;
            _livroRepository = livroRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AdicionarAutorAoLivroCommand request, CancellationToken cancellationToken)
        {
            var livro = await _livroRepository
                .BuscarPorIdIncluindoAutoresAsync(request.LivroId, cancellationToken);

            if (livro is null)
                return Result.Falha($"Livro de id {request.LivroId} não encontrado");

            var autor = await _autorRepository
                .BuscarPorIdAsync(request.AutorId, cancellationToken);

            if (autor is null)
                return Result.Falha($"Autor de id {request.AutorId} não encontrado");

            if (livro.Autores.Any(a => a.Id == autor.Id))
                return Result.Falha($"O autor {request.AutorId} já está associado ao livro {request.LivroId}");

            _livroRepository.AdicionarAutorAoLivro(livro, autor);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result.Ok();
        }
    }
}
