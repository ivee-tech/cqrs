using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Domain.Models;
using m = Cqrs.Domain.Models;

namespace Cqrs.Application.Interfaces
{
    public interface IDataService
    {
        Task UpdateClient(Guid id, Client client, CancellationToken cancellationToken);
    }
}
