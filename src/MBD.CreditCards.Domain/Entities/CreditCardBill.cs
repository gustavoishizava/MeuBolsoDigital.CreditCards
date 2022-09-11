using System;
using System.Collections.Generic;
using System.Linq;
using MBD.CreditCards.Domain.Entities.Common;
using MBD.CreditCards.Domain.ValueObjects;
using MeuBolsoDigital.Core.Assertions;
using MeuBolsoDigital.Core.Interfaces.Entities;

namespace MBD.CreditCards.Domain.Entities
{
    public class CreditCardBill : BaseEntity, IAggregateRoot
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public Guid CreditCardId { get; private set; }
        public DateTime ClosesIn { get; private set; }
        public DateTime DueDate { get; private set; }
        public BillReference Reference { get; private set; }

        public decimal Balance => _transactions.Sum(x => x.Value);
        public IReadOnlyList<Transaction> Transactions => _transactions.AsReadOnly();

        protected CreditCardBill() { }

        internal CreditCardBill(Guid creditCardId, int paymentDay, int closingDay, int month, int year)
        {
            CreditCardId = creditCardId;
            Reference = new BillReference(month, year);
            SetDates(paymentDay, closingDay, month, year);
        }

        #region Bill

        private void SetDates(int paymentDay, int closingDay, int month, int year)
        {
            DomainAssertions.IsNotNull(Reference, "Informe a referência da fatura.");

            DueDate = Reference.GetDueDate(paymentDay);
            ClosesIn = Reference.GetClosingDate(closingDay);
            AdjustDueDate();
        }

        private void AdjustDueDate()
        {
            if (DueDate >= ClosesIn)
                return;

            DueDate = DueDate.AddMonths(1);
        }

        #endregion

        #region Transactions

        internal void AddTransaction(Guid transactionId, decimal value, DateTime createdAt)
        {
            DomainAssertions.IsFalse(ExistingTransaction(transactionId), $"Transação já existente. Id='{transactionId}'.");

            _transactions.Add(new Transaction(transactionId, Id, value, createdAt));
        }

        internal Transaction GetTransaction(Guid transactionId)
        {
            return _transactions.Find(x => x.Id == transactionId);
        }

        internal bool ExistingTransaction(Guid transactionId)
        {
            return _transactions.Any(x => x.Id == transactionId);
        }

        #endregion
    }
}