using FluentValidation.Results;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Application.Interfaces;
using Cqrs.Domain.Helpers;
using Cqrs.Domain.Models;

namespace Cqrs.Application.Clients
{
    public class UpdateClientCommand : IRequest
    {
        public Guid Id { get; private set; }
        public Client Client { get; private set; }

        public UpdateClientCommand(Guid id, Client client)
        {
            Id = id;
            Client = client;
        }

        public ValidationResult Validate()
        {
            return new UpdateClientCommandValidator().Validate(this);
        }
    }

    public class UpdateAppCommandHandler : IRequestHandler<UpdateClientCommand>
    {
        private readonly IDataService _svc;

        public UpdateAppCommandHandler(IDataService svc)
        {
            _svc = svc;
        }

        public async Task Handle(UpdateClientCommand request, CancellationToken cancellationToken)
        {
            await _svc.UpdateClient(request.Id, request.Client, cancellationToken);
        }
    }
}
