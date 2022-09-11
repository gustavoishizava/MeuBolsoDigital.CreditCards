using System;
using System.Threading.Tasks;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Interfaces.Repositories;

namespace MBD.CreditCards.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        public void Add(BankAccount bankAccount)
        {
            throw new NotImplementedException();
        }

        public Task<BankAccount> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}