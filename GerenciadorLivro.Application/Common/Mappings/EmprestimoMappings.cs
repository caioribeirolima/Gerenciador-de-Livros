using GerenciadorLivro.Application.DTOs.ViewModels;
using GerenciadorLivro.Core.Entities;

namespace GerenciadorLivro.Application.Common.Mappings
{
    public static class EmprestimoMappings
    {
        public static IQueryable<EmprestimoResponse> ProjectToResponse(this IQueryable<Emprestimo> query)
        {
            return query.Select(e => new EmprestimoResponse
            {
                Id = e.Id,
                LivroTitulo = e.Livro != null ? e.Livro.Titulo : string.Empty,
                UsuarioNome = e.Usuario != null ? e.Usuario.Nome : string.Empty,
                DataEmprestimo = e.DataEmprestimo,
                DataDevolucaoPrevista = e.DataDevolucaoPrevista,
                DataDevolucaoReal = e.DataDevolucaoReal,
                Renovacoes = e.Renovacoes
            });
        }
    }
}