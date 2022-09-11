using System;
using System.Threading.Tasks;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Interfaces.Repositories;
using MBD.CreditCards.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MongoDB.Driver;

namespace MBD.CreditCards.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly CreditCardContext _context;
        private readonly ILoggedUser _loggedUser;

        public BankAccountRepository(CreditCardContext context, ILoggedUser loggedUser)
        {
            _context = context;
            _loggedUser = loggedUser;
        }

        public async Task AddAsync(BankAccount bankAccount)
        {
            await _context.BankAccounts.AddAsync(bankAccount);
        }

        public async Task<BankAccount> GetByIdAsync(Guid id)
        {
            return await _context.BankAccounts.Collection.Find(Builders<BankAccount>.Filter.Where(x => x.Id == id && x.TenantId == _loggedUser.UserId)).FirstOrDefaultAsync();
        }
    }
}