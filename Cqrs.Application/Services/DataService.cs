using Azure.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Application.Interfaces;
using Cqrs.Domain.Helpers;
using Cqrs.Domain.Models;
using m = Cqrs.Domain.Models;

namespace Cqrs.Application.Services
{
    public class DataService : IDataService
    {

        private readonly IDataRepository _repository;

        public DataService(IDataRepository repository)
        {
            _repository = repository;
        }

        public async Task UpdateClient(Guid id, Client client, CancellationToken cancellationToken)
        {
            await _repository.BeginTransaction(cancellationToken);
            try
            {
                var existingClient = await _repository.GetClient(id);
                var existingAddreses = existingClient.Addresses;
                var items = CompareListsHelper.CompareById<Address>(client.Addresses, existingAddreses);
                foreach (var a in items.Item1)
                {
                    await _repository.CreateAddress(a, cancellationToken);
                }
                foreach (var a in items.Item2)
                {
                    await _repository.UpdateAddress(a.Id.Value, a, cancellationToken);
                }
                foreach (var a in items.Item3)
                {
                    await _repository.DeleteAddress(a.Id.Value, cancellationToken);
                }
                await _repository.UpdateClient(id, client, cancellationToken);

                await _repository.CommitTransaction(cancellationToken);
            }
            catch(Exception ex) 
            {
                await _repository.RollbackTransaction(cancellationToken);
                throw;
            }
        }

    }
}
