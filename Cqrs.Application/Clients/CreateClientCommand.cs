using FluentValidation;
using FluentValidation.Results;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Application.Interfaces;
using Cqrs.Domain.Models;

namespace Cqrs.Application.Clients
{
    public class CreateClientCommand : IRequest<Guid>
    {
        public Client Client { get; private set; }

        public CreateClientCommand(Client client)
        {
            Client = client;
        }

        public ValidationResult Validate()
        {
            return new CreateClientCommandValidator().Validate(this);
        }
    }

    public class CreateAppCommandHandler : IRequestHandler<CreateClientCommand, Guid>
    {
        private readonly IDataRepository _repository;

        public CreateAppCommandHandler(IDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(CreateClientCommand request, CancellationToken cancellationToken)
        {
            return await _repository.CreateClient(request.Client, cancellationToken);
        }
    }
}

