using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Notifications.LivroCriado;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarLivro
{
    public class CriarLivroHandler : IRequestHandler<CriarLivroCommand, Result<int>>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IAutorRepository _autorRepository;
        private readonly ICategoriaRepository _categoriaRepository;
        private readonly IEditoraRepository _editoraRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public CriarLivroHandler(
            ILivroRepository livroRepository,
            IAutorRepository autorRepository,
            ICategoriaRepository categoriaRepository,
            IEditoraRepository editoraRepository,
            IUnitOfWork unitOfWork,
            IPublisher publisher)
        {
            _publisher = publisher;
            _autorRepository = autorRepository;
            _categoriaRepository = categoriaRepository;
            _editoraRepository = editoraRepository;
            _livroRepository = livroRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CriarLivroCommand command, CancellationToken cancellationToken)
        {
            // Recurso que o cliente referenciou e não existe -> 404 (antes: a FK estourava no Commit e virava 500).
            if (await _categoriaRepository.BuscarPorIdAsync(command.CategoriaId, cancellationToken) == null)
                return Result<int>.NaoEncontrado($"Categoria de id {command.CategoriaId} não encontrada");

            if (await _editoraRepository.BuscarPorIdAsync(command.EditoraId, cancellationToken) == null)
                return Result<int>.NaoEncontrado($"Editora de id {command.EditoraId} não encontrada");

            var autores = await _autorRepository
                .ListarPorIdsAsync(command.AutorIds, cancellationToken);

            // Antes, ids de autor inexistentes eram ignorados em silêncio e o livro nascia com menos autores.
            // O validator garante que AutorIds não tem repetidos, então a contagem é comparável.
            if (autores.Count != command.AutorIds.Count)
            {
                var faltando = command.AutorIds.Except(autores.Select(a => a.Id));
                return Result<int>.NaoEncontrado($"Autor(es) de id {string.Join(", ", faltando)} não encontrado(s)");
            }

            var livro = new Livro
            {
                Titulo = command.Titulo,
                AnoPublicacao = command.AnoPublicacao,
                CategoriaId = command.CategoriaId,
                EditoraId = command.EditoraId,
                Autores = autores
            };

            _livroRepository.Adicionar(livro);
            await _unitOfWork.CommitAsync(cancellationToken);

            await _publisher.Publish(new LivroCriadoNotification(livro.Id, livro.Titulo), cancellationToken);

            return Result<int>.Ok(livro.Id);
        }
    }
}
