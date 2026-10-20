using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Repositories;
using MediatR;

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
                return Result.NaoEncontrado("Editora não existe");
            }

            var possuiLivros = await _livroRepository.ExisteLivroNaEditoraAsync(command.Id, cancellationToken);

            if (possuiLivros)
            {
                return Result.Conflito("Existem livros vinculados a esta editora (incluindo livros excluídos)");
            }

            _editoraRepository.Excluir(editora);

            await _unitOfWork.CommitAsync(cancellationToken);
            return Result.Ok();
        }
    }
}
