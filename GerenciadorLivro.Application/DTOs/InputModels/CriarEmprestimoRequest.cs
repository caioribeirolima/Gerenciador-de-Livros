using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GerenciadorLivro.Application.DTOs.InputModels
{
    public class CriarEmprestimoRequest
    {
        public int LivroId { get; set; }
        public int UsuarioId { get; set; }
    }
}
