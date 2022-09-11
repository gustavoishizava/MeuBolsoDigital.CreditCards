using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.CreditCards.Domain.Entities;
using MeuBolsoDigital.Core.Interfaces.Repositories;

namespace MBD.CreditCards.Domain.Interfaces.Repositories
{
    public interface ICreditCardRepository : IBaseRepository<CreditCard>
    {
        Task<IEnumerable<CreditCard>> GetAllAsync();
    }
}