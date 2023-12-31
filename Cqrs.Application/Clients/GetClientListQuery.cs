using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Application.Interfaces;
using Cqrs.Domain.Models;

namespace Cqrs.Application.Clients
{
    public class GetClientListQuery : IRequest<IEnumerable<Client>>
    {

        public GetClientListQuery()
        {
        }

    }

    public class GetAppListQueryHandler : IRequestHandler<GetClientListQuery, IEnumerable<Client>>
    {
        private readonly IDataRepository _repository;

        public GetAppListQueryHandler(IDataRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Client>> Handle(GetClientListQuery request, CancellationToken cancellationToken)
        {
            return await _repository.GetClients();
        }
    }
}
