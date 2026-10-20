namespace GerenciadorLivro.Application.Common
{
    public enum TipoErro
    {
        NaoClassificado,   // temporário: é o que todo Falha(string) atual vira
        Validacao,         // 400
        NaoEncontrado,     // 404
        Conflito           // 409
    }
}