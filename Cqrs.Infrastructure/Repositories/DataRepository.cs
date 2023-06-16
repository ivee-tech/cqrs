using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Cqrs.Common.EntityFramework;
using Cqrs.Common.Exceptions;
using Cqrs.Application.Interfaces;
using Cqrs.Domain.Models;
using Cqrs.Infrastructure.Repositories.DbContexts;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using m = Cqrs.Domain.Models;
using e = Cqrs.Infrastructure.Repositories.Entities;
using System.Data.Common;
using Cqrs.Domain.Comparers;
using Cqrs.Infrastructure.Repositories.Comparers;
using Azure.Core;
using MediatR;

namespace Cqrs.Infrastructure.Repositories
{
    public partial class DataRepository : IDataRepository
    {
        private readonly DbContexts.DataDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private IDbContextTransaction _transaction;
        private bool _useTransaction;

        public DataRepository(IConfiguration configuration,
            DbContexts.DataDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }

        public async Task BeginTransaction(CancellationToken cancellationToken)
        {
            _transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadUncommitted, cancellationToken);
            _useTransaction = true;
        }

        public async Task CommitTransaction(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
            _useTransaction = false;
        }

        public async Task RollbackTransaction(CancellationToken cancellationToken)
        {
            await _transaction.RollbackAsync(cancellationToken);
            _useTransaction = false;
        }


        #region Client

        public async Task<Guid> CreateClient(m.Client client, CancellationToken cancellationToken)
        {
            if (!client.Id.HasValue)
            {
                client.Id = Guid.NewGuid();
            }
            var entity = _mapper.Map<e.Client>(client);

            _context.Clients.Add(entity);

            if (!_useTransaction)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return entity.Id;
        }

        public async Task DeleteClient(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _context.Clients.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(m.Client), id);
            }

            _context.Clients.Remove(entity);


            if (!_useTransaction)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<m.Client>> GetClients()
        {
            return _mapper.Map<IEnumerable<m.Client>>(await _context.Clients
                .ToListAsync());
        }

        public async Task<m.Client> GetClient(Guid id)
        {
            var entity = await _context.Clients
                .Where(x => x.Id == id)
                .Include(c => c.Addresses)
                .SingleOrDefaultAsync();

            return _mapper.Map<m.Client>(entity);
        }

        public async Task UpdateClient(Guid id, m.Client client, CancellationToken cancellationToken)
        {
            var entity = await _context.Clients.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(m.Client), id);
            }

            var updatedEntity = _mapper.Map<m.Client>(client);

            entity.FamilyName = updatedEntity.FamilyName;
            entity.FirstName = updatedEntity.FirstName;
            entity.MiddleNames = updatedEntity.MiddleNames;
            entity.DateOfBirth = updatedEntity.DateOfBirth;
            entity.Email = updatedEntity.Email;
            entity.Phone = updatedEntity.Phone;

            if (!_useTransaction)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        #endregion Client

        #region Address

        public async Task<Guid> CreateAddress(m.Address address, CancellationToken cancellationToken)
        {
            if (!address.Id.HasValue)
            {
                address.Id = Guid.NewGuid();
            }
            var entity = _mapper.Map<e.Address>(address);

            _context.Addresses.Add(entity);

            if (!_useTransaction)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }

            return entity.Id;
        }

        public async Task DeleteAddress(Guid id, CancellationToken cancellationToken)
        {
            var entity = await _context.Addresses.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(m.Address), id);
            }

            _context.Addresses.Remove(entity);


            if (!_useTransaction)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<m.Address>> GetAddresss()
        {
            return _mapper.Map<IEnumerable<m.Address>>(await _context.Addresses
                .ToListAsync());
        }

        public async Task<m.Address> GetAddress(Guid id)
        {
            var entity = await _context.Addresses
                .Where(x => x.Id == id)
                .SingleOrDefaultAsync();

            return _mapper.Map<m.Address>(entity);
        }

        public async Task UpdateAddress(Guid id, m.Address address, CancellationToken cancellationToken)
        {
            var entity = await _context.Addresses.FindAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(nameof(m.Address), id);
            }

            var updatedEntity = _mapper.Map<m.Address>(address);

            entity.Line1 = updatedEntity.Line1;
            entity.Line2 = updatedEntity.Line2;
            entity.Suburb = updatedEntity.Suburb;
            entity.State = updatedEntity.State;
            entity.Code = updatedEntity.Code;

            if (!_useTransaction)
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        #endregion Address

    }
}
