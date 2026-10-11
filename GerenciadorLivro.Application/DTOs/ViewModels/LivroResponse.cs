using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorLivro.Application.DTOs.ViewModels
{
    public class LivroResponse
    {

        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AnoPublicacao { get; set; }
        public bool Disponivel { get; set; } = true;
    }
}
