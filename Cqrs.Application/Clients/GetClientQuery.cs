using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Application.Interfaces;
using Cqrs.Domain.Models;

namespace Cqrs.Application.Clients
{
    public class GetClientQuery : IRequest<Client>
    {
        public Guid Id { get; set; }

        public GetClientQuery(Guid id)
        {
            Id = id;
        }
    }

    public class GetAppQueryHandler : IRequestHandler<GetClientQuery, Client>
    {
        private readonly IDataRepository _repository;

        public GetAppQueryHandler(IDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<Client> Handle(GetClientQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetClient(request.Id);
        }
    }
}
