using System.Threading.Tasks;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.CreditCards.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public Task<bool> CommitAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}