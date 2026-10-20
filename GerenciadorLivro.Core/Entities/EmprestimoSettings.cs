namespace GerenciadorLivro.Core.Entities
{
    public class EmprestimoSettings
    {
        public int DiasParaDevolucao {  get; set; }
        public double MultaPorDiaAtraso { get; set; }
        public int MaximoEmprestimosAtivos { get; set; } = 3;
    }
}
