using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Application.Common.Interfaces;
using GerenciadorLivro.Application.Notifications.LivroCriado;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.CriarLivro
{
    public class CriarLivroHandler : IRequestHandler<CriarLivroCommand, Result<int>>
    {
        private readonly ILivroRepository _livroRepository;
        private readonly IAutorRepository _autorRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublisher _publisher;

        public CriarLivroHandler(
            ILivroRepository livroRepository,
            IAutorRepository autorRepository,
            IUnitOfWork unitOfWork,
            IPublisher publisher)
        {
            _publisher = publisher;
            _autorRepository = autorRepository;
            _livroRepository = livroRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CriarLivroCommand command, CancellationToken cancellationToken)
        {
            if (command.AnoPublicacao > DateTime.Now.Year)
                return Result<int>.Falha($"Livro com ano de publicação {command.AnoPublicacao} inválido");

            var autores = await _autorRepository
                .ListarPorIdAsync(command.AutorIds, cancellationToken);

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
