using FluentValidation;

namespace Cqrs.Application.Clients
{
    public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
    {
        public UpdateClientCommandValidator()
        {
            RuleFor(command => command.Client)
                .NotEmpty().WithMessage("Empty client.");
        }
    }
}
