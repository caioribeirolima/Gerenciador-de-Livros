using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.ExcluirEditora
{
    public class ExcluirEditoraHandler : IRequestHandler<ExcluirEditoraCommand, Result>
    {
        private readonly IEditoraRepository _editoraRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILivroRepository _livroRepository;
        public ExcluirEditoraHandler(
            IEditoraRepository editoraRepository,
            ILivroRepository livroRepository,
            IUnitOfWork unitOfWork)
        {
            _editoraRepository = editoraRepository;
            _livroRepository = livroRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ExcluirEditoraCommand command, CancellationToken cancellationToken)
        {
            var editora = await _editoraRepository.BuscarPorIdAsync(command.Id, cancellationToken);

            if (editora == null)
            {
                return Result.Falha("Editora não existe");
            }

            var possuiLivros = await _livroRepository.ExisteLivroNaEditora(command.Id, cancellationToken);

            if (possuiLivros)
            {
                return Result.Falha("Existem livros vinculados a esta editora (incluindo livros excluídos)");
            }

            _editoraRepository.ExcluirAsync(editora);

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
