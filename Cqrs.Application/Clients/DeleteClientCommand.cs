using FluentValidation.Results;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Application.Interfaces;

namespace Cqrs.Application.Clients
{
    public class DeleteClientCommand : IRequest
    {
        public Guid Id { get; private set; }

        public DeleteClientCommand(Guid id)
        {
            Id = id;
        }

        public ValidationResult Validate()
        {
            return new DeleteClientCommandValidator().Validate(this);
        }
    }

    public class DeleteAppCommandHandler : IRequestHandler<DeleteClientCommand>
    {
        private readonly IDataRepository _repository;

        public DeleteAppCommandHandler(IDataRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
        {
            await _repository.DeleteClient(request.Id, cancellationToken);
        }
    }
}
