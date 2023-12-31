using FluentValidation;

namespace Cqrs.Application.Clients
{
    public class DeleteClientCommandValidator : AbstractValidator<DeleteClientCommand>
    {
        public DeleteClientCommandValidator()
        {
            RuleFor(command => command.Id)
                .NotEmpty().WithMessage("Empty client Id.");
        }
    }
}

