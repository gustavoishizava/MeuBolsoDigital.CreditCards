using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Interfaces.Repositories;

namespace MBD.CreditCards.Infrastructure.Repositories
{
    public class CreditCardRepository : ICreditCardRepository
    {
        public Task AddAsync(CreditCard entity)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CreditCard>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CreditCard> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(CreditCard entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(CreditCard entity)
        {
            throw new NotImplementedException();
        }
    }
}