using System;
using System.Threading.Tasks;
using MBD.CreditCards.Domain.Entities;

namespace MBD.CreditCards.Domain.Interfaces.Repositories
{
    public interface IBankAccountRepository
    {
        Task AddAsync(BankAccount bankAccount);
        Task<BankAccount> GetByIdAsync(Guid id);
    }
}