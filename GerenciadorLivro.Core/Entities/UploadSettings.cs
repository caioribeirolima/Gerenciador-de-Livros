namespace GerenciadorLivro.Core.Entities
{
    public class UploadSettings
    {
        public int TamanhoMaximoMB { get; set; }
        public List<string> ExtensoesPermitidas { get; set; } = new List<string>();
    }
}
