using FluentValidation;

namespace GerenciadorLivro.Application.Commands.CancelarReserva
{
    public class CancelarReservaCommandValidator : AbstractValidator<CancelarReservaCommand>
    {
        public CancelarReservaCommandValidator()
        {
            RuleFor(x => x.ReservaId)
                .GreaterThan(0).WithMessage("O Id da reserva deve ser maior que zero.");
        }
    }
}
