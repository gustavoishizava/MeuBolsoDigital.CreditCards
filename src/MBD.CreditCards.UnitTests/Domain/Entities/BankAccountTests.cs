using System;
using MBD.CreditCards.Domain.Entities;
using Xunit;

namespace MBD.CreditCards.UnitTests.Domain.Entities
{
    public class BankAccountTests
    {
        [Fact]
        public void Create_ReturnSuccess()
        {
            // Arrange
            var id = Guid.NewGuid();
            var tentantId = Guid.NewGuid();
            var description = Guid.NewGuid().ToString();

            // Act
            var bankAccount = new BankAccount(id, tentantId, description);

            // Assert
            Assert.Equal(id, bankAccount.Id);
            Assert.Equal(tentantId, bankAccount.TenantId);
            Assert.Equal(description, bankAccount.Description);
        }
    }
}