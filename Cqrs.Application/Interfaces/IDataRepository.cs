using Azure.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Domain.Models;
using m = Cqrs.Domain.Models;

namespace Cqrs.Application.Interfaces
{
    public interface IDataRepository
    {
        Task BeginTransaction(CancellationToken cancellationToken);
        Task CommitTransaction(CancellationToken cancellationToken);
        Task RollbackTransaction(CancellationToken cancellationToken);

        Task<IEnumerable<Client>> GetClients();
        Task<Client> GetClient(Guid id);
        Task<Guid> CreateClient(Client client, CancellationToken cancellationToken);
        Task UpdateClient(Guid id, Client client, CancellationToken cancellationToken);
        Task DeleteClient(Guid id, CancellationToken cancellationToken);

        Task<IEnumerable<Address>> GetAddresss();
        Task<Address> GetAddress(Guid id);
        Task<Guid> CreateAddress(Address address, CancellationToken cancellationToken);
        Task UpdateAddress(Guid id, Address address, CancellationToken cancellationToken);
        Task DeleteAddress(Guid id, CancellationToken cancellationToken);
    }
}
