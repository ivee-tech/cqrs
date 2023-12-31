using FluentValidation;

namespace Cqrs.Application.Clients
{
    public class CreateClientCommandValidator : AbstractValidator<CreateClientCommand>
    {
        public CreateClientCommandValidator()
        {
            RuleFor(command => command.Client)
                .NotEmpty().WithMessage("Empty client.");
        }
    }
}
