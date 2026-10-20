using System.Text.RegularExpressions;

namespace GerenciadorLivro.Application.Common
{
    public static class Cpf
    {
        // 11 dígitos, com ou sem máscara (000.000.000-00). Letras e pontuação fora do padrão são recusadas.
        // [0-9] em vez de \d porque \d também casa dígitos de outros alfabetos; \z em vez de $ porque $ aceita "\n" no fim.
        private static readonly Regex Formato = new(@"^[0-9]{3}\.?[0-9]{3}\.?[0-9]{3}-?[0-9]{2}\z", RegexOptions.Compiled);

        /// <summary>Só os dígitos. É a forma que o sistema grava e compara.</summary>
        public static string Normalizar(string? cpf)
            => new string((cpf ?? string.Empty).Where(c => c >= '0' && c <= '9').ToArray());

        public static bool EhValido(string? cpf)
        {
            if (cpf is null || !Formato.IsMatch(cpf))
                return false;

            var digitos = Normalizar(cpf);

            // 111.111.111-11 passa na conta dos dígitos verificadores, mas não é um CPF.
            if (digitos.Distinct().Count() == 1)
                return false;

            return CalcularDigito(digitos, 9) == digitos[9] - '0'
                && CalcularDigito(digitos, 10) == digitos[10] - '0';
        }

        /// <summary>Esconde o início e o fim: 52998224725 vira ***.982.247-**. Devolve null se não tiver 11 dígitos.</summary>
        public static string? Mascarar(string? cpf)
        {
            var digitos = Normalizar(cpf);
            return digitos.Length == 11 ? $"***.{digitos[3..6]}.{digitos[6..9]}-**" : null;
        }

        // Pesos decrescentes a partir de (quantos + 1): 10..2 para o 1º dígito, 11..2 para o 2º.
        private static int CalcularDigito(string digitos, int quantos)
        {
            var soma = 0;
            for (var i = 0; i < quantos; i++)
                soma += (digitos[i] - '0') * (quantos + 1 - i);

            var resto = soma * 10 % 11;
            return resto == 10 ? 0 : resto;
        }
    }
}
