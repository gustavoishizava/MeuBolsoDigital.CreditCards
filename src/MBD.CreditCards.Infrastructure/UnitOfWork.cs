using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MBD.CreditCards.Infrastructure.Context;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.CreditCards.Infrastructure
{
    [ExcludeFromCodeCoverage]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CreditCardContext _context;

        public UnitOfWork(CreditCardContext context)
        {
            _context = context;
            _context.StartTransaction();
        }

        public async Task<bool> CommitAsync()
        {
            await _context.CommitAsync();
            return true;
        }
    }
}