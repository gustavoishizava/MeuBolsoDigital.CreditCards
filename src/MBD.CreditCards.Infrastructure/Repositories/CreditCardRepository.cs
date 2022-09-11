using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Interfaces.Repositories;
using MBD.CreditCards.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Identity;
using MongoDB.Driver;

namespace MBD.CreditCards.Infrastructure.Repositories
{
    public class CreditCardRepository : ICreditCardRepository
    {
        private readonly CreditCardContext _context;
        private readonly ILoggedUser _loggedUser;

        public CreditCardRepository(CreditCardContext context, ILoggedUser loggedUser)
        {
            _context = context;
            _loggedUser = loggedUser;
        }

        public async Task AddAsync(CreditCard entity)
        {
            await _context.CreditCards.AddAsync(entity);
        }

        public async Task<IEnumerable<CreditCard>> GetAllAsync()
        {
            return await _context.CreditCards.Collection.Find(Builders<CreditCard>.Filter.Where(x => x.TenantId == _loggedUser.UserId)).ToListAsync();
        }

        public async Task<CreditCard> GetByIdAsync(Guid id)
        {
            return await _context.CreditCards.Collection.Find(Builders<CreditCard>.Filter.Where(x => x.Id == id && x.TenantId == _loggedUser.UserId)).FirstOrDefaultAsync();
        }

        public async Task RemoveAsync(CreditCard entity)
        {
            await _context.CreditCards.RemoveAsync(Builders<CreditCard>.Filter.Where(x => x.Id == entity.Id), entity);
        }

        public async Task UpdateAsync(CreditCard entity)
        {
            await _context.CreditCards.UpdateAsync(Builders<CreditCard>.Filter.Where(x => x.Id == entity.Id && x.TenantId == entity.TenantId), entity);
        }
    }
}