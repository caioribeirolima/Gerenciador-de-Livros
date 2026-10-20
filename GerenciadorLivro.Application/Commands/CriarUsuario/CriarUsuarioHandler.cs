using GerenciadorLivro.Application.Common;
using GerenciadorLivro.Core.Entities;
using GerenciadorLivro.Core.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorLivro.Application.Commands.CriarUsuario
{
    public class CriarUsuarioHandler : IRequestHandler<CriarUsuarioCommand, Result<int>>
    {
        // O CPF é dado pessoal (LGPD): a mensagem não o repete.
        private const string CpfJaCadastrado = "Já existe um usuário cadastrado com este CPF";

        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CriarUsuarioHandler(IUsuarioRepository usuarioRepository, IUnitOfWork unitOfWork)
        {
            _usuarioRepository = usuarioRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<int>> Handle(CriarUsuarioCommand command, CancellationToken cancellationToken)
        {
            // 123.456.789-09 e 12345678909 são o mesmo CPF: compara e grava sempre só os dígitos.
            var cpf = Cpf.Normalizar(command.Cpf);

            if (await _usuarioRepository.ExisteComCpfAsync(cpf, cancellationToken))
                return Result<int>.Conflito(CpfJaCadastrado);

            var usuario = new Usuario(command.Nome.Trim(), command.Email.Trim(), cpf);
            _usuarioRepository.Adicionar(usuario);

            try
            {
                await _unitOfWork.CommitAsync(cancellationToken);
            }
            catch (DbUpdateException)
            {
                // A checagem acima e o Commit não são atômicos: outra requisição pode ter gravado o mesmo CPF
                // no meio. O índice único barra e chega aqui. Só vira Conflito se a causa for essa;
                // qualquer outra falha do banco continua sendo erro de verdade (500).
                _usuarioRepository.Remover(usuario);

                if (!await _usuarioRepository.ExisteComCpfAsync(cpf, cancellationToken))
                    throw;

                return Result<int>.Conflito(CpfJaCadastrado);
            }

            return Result<int>.Ok(usuario.Id);
        }
    }
}
