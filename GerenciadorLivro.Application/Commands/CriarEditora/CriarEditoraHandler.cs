using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;

namespace GerenciadorLivro.Application.Commands.CriarEditora
{
    public class CriarEditoraHandler : IRequestHandler<CriarEditoraCommand, Result<int>>
    {
        private readonly IEditoraRepository _editoraRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CriarEditoraHandler(
            IEditoraRepository editoraRepository,
            IUnitOfWork unitOfWork)
        {
            _editoraRepository = editoraRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result<int>> Handle(CriarEditoraCommand command, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(command.Nome))
            {
                return Result<int>.Falha("Nome da editora inválido");
            }

            var nome = command.Nome.Trim();

            if (nome.Length > 100)
            {
                return Result<int>.Falha("Nome não pode ter mais que 100 caracteres");
            }

            var exists = await _editoraRepository
                .ExisteComNomeAsync(nome, cancellationToken);

            if (exists)
            {
                return Result<int>.Falha($"Editora com nome {nome} já existe");
            }

            var editora = new Editora
            {
                Nome = nome,
            };

            _editoraRepository.Adicionar(editora);
            await _unitOfWork.CommitAsync(cancellationToken);

            return Result<int>.Ok(editora.Id);
        }
    }
}
