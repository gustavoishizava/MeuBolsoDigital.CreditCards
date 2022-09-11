using System;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Entities.Common;

namespace MBD.CreditCards.Application.Responses
{
    public class CreditCardResponse
    {
        public Guid Id { get; set; }
        public Guid BankAccountId { get; set; }
        public string Name { get; set; }
        public int ClosingDay { get; set; }
        public int DayOfPayment { get; set; }
        public decimal Limit { get; set; }
        public Brand Brand { get; set; }
        public Status Status { get; set; }
    }
}