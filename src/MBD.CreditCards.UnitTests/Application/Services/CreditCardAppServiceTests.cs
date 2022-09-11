using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MBD.CreditCards.Application.Interfaces;
using MBD.CreditCards.Application.Requests;
using MBD.CreditCards.Application.Services;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Interfaces.Repositories;
using Moq.AutoMock;
using Xunit;
using Moq;
using MeuBolsoDigital.Core.Interfaces.Repositories;
using Bogus;
using AutoMapper;
using MBD.CreditCards.Application.AutoMapper;
using MBD.CreditCards.Domain.Entities.Common;

namespace MBD.CreditCards.UnitTests.Application.Services
{
    public class CreditCardAppServiceTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly ICreditCardAppService _appService;

        public CreditCardAppServiceTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<DomainToResponseProfile>());
            config.AssertConfigurationIsValid();
            _autoMocker.Use<IMapper>(config.CreateMapper());

            _appService = _autoMocker.CreateInstance<CreditCardAppService>();
        }

        [Fact]
        public async Task Create_InvalidRequest_ReturnFail()
        {
            // Arrange
            var bankAccountId = Guid.Empty;
            var name = string.Empty;
            var closingDay = 0;
            var dayOfPayment = 0;
            var limit = 0;
            var brand = Brand.AMERICANEXPRESS;

            var request = new CreateCreditCardRequest
            {
                BankAccountId = bankAccountId,
                Name = name,
                ClosingDay = closingDay,
                DayOfPayment = dayOfPayment,
                Limit = limit,
                Brand = brand
            };

            // Act
            var result = await _appService.CreateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.AddAsync(It.IsAny<CreditCard>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Create_InvalidBankAccountId_ReturnFail()
        {
            // Arrange
            var bankAccountId = Guid.NewGuid();
            var name = "Nubank";
            var closingDay = _faker.Random.Int(1, 10);
            var dayOfPayment = _faker.Random.Int(11, 30);
            var limit = _faker.Random.Decimal(100, 1000);
            var brand = Brand.AMERICANEXPRESS;

            var request = new CreateCreditCardRequest
            {
                BankAccountId = bankAccountId,
                Name = name,
                ClosingDay = closingDay,
                DayOfPayment = dayOfPayment,
                Limit = limit,
                Brand = brand
            };

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(request.BankAccountId))
                .ReturnsAsync((BankAccount)null);

            // Act
            var result = await _appService.CreateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);
            Assert.Equal("Conta bancária inválida.", result.Message);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(request.BankAccountId), Times.Once);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.AddAsync(It.IsAny<CreditCard>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Create_ReturnSuccess()
        {
            // Arrange
            var bankAccountId = Guid.NewGuid();
            var name = "Nubank";
            var closingDay = _faker.Random.Int(1, 10);
            var dayOfPayment = _faker.Random.Int(11, 30);
            var limit = _faker.Random.Decimal(100, 1000);
            var brand = Brand.AMERICANEXPRESS;

            var request = new CreateCreditCardRequest
            {
                BankAccountId = bankAccountId,
                Name = name,
                ClosingDay = closingDay,
                DayOfPayment = dayOfPayment,
                Limit = limit,
                Brand = brand
            };

            var bankAccount = new BankAccount(request.BankAccountId, Guid.NewGuid(), "Nu conta");

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(request.BankAccountId))
                .ReturnsAsync(bankAccount);

            // Act
            var result = await _appService.CreateAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Null(result.Message);
            Assert.NotEqual(Guid.Empty, result.Data.Id);
            Assert.Equal(request.BankAccountId, result.Data.BankAccountId);
            Assert.Equal(request.Name, result.Data.Name);
            Assert.Equal(request.ClosingDay, result.Data.ClosingDay);
            Assert.Equal(request.DayOfPayment, result.Data.DayOfPayment);
            Assert.Equal(request.Limit, result.Data.Limit);
            Assert.Equal(request.Brand, result.Data.Brand);
            Assert.Equal(Status.Active, result.Data.Status);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(request.BankAccountId), Times.Once);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.AddAsync(It.IsAny<CreditCard>()), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Update_InvalidRequest_ReturnFail()
        {
            // Arrange
            var id = Guid.Empty;
            var bankAccountId = Guid.Empty;
            var name = string.Empty;
            var closingDay = 0;
            var dayOfPayment = 0;
            var limit = 0;
            var brand = Brand.AMERICANEXPRESS;
            var status = Status.Active;

            var request = new UpdateCreditCardRequest
            {
                Id = id,
                BankAccountId = bankAccountId,
                Name = name,
                ClosingDay = closingDay,
                DayOfPayment = dayOfPayment,
                Limit = limit,
                Brand = brand,
                Status = status
            };

            // Act
            var result = await _appService.UpdateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<CreditCard>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Update_InvalidCreditCard_ReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();
            var bankAccountId = Guid.NewGuid();
            var name = "Nubank";
            var closingDay = _faker.Random.Int(1, 10);
            var dayOfPayment = _faker.Random.Int(11, 30);
            var limit = _faker.Random.Decimal(100, 1000);
            var brand = Brand.AMERICANEXPRESS;
            var status = Status.Active;

            var request = new UpdateCreditCardRequest
            {
                Id = id,
                BankAccountId = bankAccountId,
                Name = name,
                ClosingDay = closingDay,
                DayOfPayment = dayOfPayment,
                Limit = limit,
                Brand = brand,
                Status = status
            };

            _autoMocker.GetMock<ICreditCardRepository>()
                .Setup(x => x.GetByIdAsync(request.Id))
                .ReturnsAsync((CreditCard)null);

            // Act
            var result = await _appService.UpdateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);
            Assert.Equal("Cartão de crédito inválido.", result.Message);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetByIdAsync(request.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Never);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<CreditCard>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Update_InvalidBankAccount_ReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();
            var bankAccountId = Guid.NewGuid();
            var name = "Nubank";
            var closingDay = _faker.Random.Int(1, 10);
            var dayOfPayment = _faker.Random.Int(11, 30);
            var limit = _faker.Random.Decimal(100, 1000);
            var brand = Brand.AMERICANEXPRESS;
            var status = Status.Active;

            var request = new UpdateCreditCardRequest
            {
                Id = id,
                BankAccountId = bankAccountId,
                Name = name,
                ClosingDay = closingDay,
                DayOfPayment = dayOfPayment,
                Limit = limit,
                Brand = brand,
                Status = status
            };

            var bankAccount = new BankAccount(Guid.NewGuid(), Guid.NewGuid(), "Nu conta");
            var creditCard = new CreditCard(Guid.NewGuid(), bankAccount, request.Name, request.ClosingDay, request.DayOfPayment, request.Limit, request.Brand);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Setup(x => x.GetByIdAsync(request.Id))
                .ReturnsAsync(creditCard);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(request.BankAccountId))
                .ReturnsAsync((BankAccount)null);

            // Act
            var result = await _appService.UpdateAsync(request);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);
            Assert.Equal("Conta bancária inválida.", result.Message);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetByIdAsync(request.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<CreditCard>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Theory]
        [InlineData(Status.Active)]
        [InlineData(Status.Inactive)]
        public async Task Update_ReturnSuccess(Status status)
        {
            // Arrange
            var id = Guid.NewGuid();
            var bankAccountId = Guid.NewGuid();
            var name = "Nubank";
            var closingDay = _faker.Random.Int(1, 10);
            var dayOfPayment = _faker.Random.Int(11, 30);
            var limit = _faker.Random.Decimal(100, 1000);
            var brand = Brand.AMERICANEXPRESS;

            var request = new UpdateCreditCardRequest
            {
                Id = id,
                BankAccountId = bankAccountId,
                Name = name,
                ClosingDay = closingDay,
                DayOfPayment = dayOfPayment,
                Limit = limit,
                Brand = brand,
                Status = status
            };

            var bankAccount = new BankAccount(request.BankAccountId, Guid.NewGuid(), "Nu conta");
            var creditCard = new CreditCard(Guid.NewGuid(), bankAccount, "Name", 1, 5, 1, request.Brand);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Setup(x => x.GetByIdAsync(request.Id))
                .ReturnsAsync(creditCard);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Setup(x => x.GetByIdAsync(request.BankAccountId))
                .ReturnsAsync(bankAccount);

            // Act
            var result = await _appService.UpdateAsync(request);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Message);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetByIdAsync(request.Id), Times.Once);

            _autoMocker.GetMock<IBankAccountRepository>()
                .Verify(x => x.GetByIdAsync(It.IsAny<Guid>()), Times.Once);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.UpdateAsync(It.IsAny<CreditCard>()), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnSuccess()
        {
            // Arrange
            var bankAccount = new BankAccount(Guid.NewGuid(), Guid.NewGuid(), "Nu conta");
            var creditCard = new CreditCard(Guid.NewGuid(), bankAccount, "Name", 1, 5, 1, Brand.AMERICANEXPRESS);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Setup(x => x.GetAllAsync())
                .ReturnsAsync(new List<CreditCard>() { creditCard });

            // Act
            var result = await _appService.GetAllAsync();

            // Assert
            Assert.Single(result);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_NotFound_ReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();

            _autoMocker.GetMock<ICreditCardRepository>()
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((CreditCard)null);

            // Act
            var result = await _appService.GetByIdAsync(id);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Null(result.Data);
            Assert.Null(result.Message);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnSuccess()
        {
            // Arrange
            var bankAccount = new BankAccount(Guid.NewGuid(), Guid.NewGuid(), "Nu conta");
            var creditCard = new CreditCard(Guid.NewGuid(), bankAccount, "Name", 1, 5, 1, Brand.AMERICANEXPRESS);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Setup(x => x.GetByIdAsync(creditCard.Id))
                .ReturnsAsync(creditCard);

            // Act
            var result = await _appService.GetByIdAsync(creditCard.Id);

            // Assert
            Assert.True(result.Succeeded);
            Assert.NotNull(result.Data);
            Assert.Equal(creditCard.Id, result.Data.Id);
            Assert.Equal(creditCard.BankAccount.Id, result.Data.BankAccountId);
            Assert.Equal(creditCard.Name, result.Data.Name);
            Assert.Equal(creditCard.ClosingDay, result.Data.ClosingDay);
            Assert.Equal(creditCard.DayOfPayment, result.Data.DayOfPayment);
            Assert.Equal(creditCard.Limit, result.Data.Limit);
            Assert.Equal(creditCard.Brand, result.Data.Brand);
            Assert.Equal(creditCard.Status, result.Data.Status);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetByIdAsync(creditCard.Id), Times.Once);
        }

        [Fact]
        public async Task Remove_NotFound_ReturnFail()
        {
            // Arrange
            var id = Guid.NewGuid();

            _autoMocker.GetMock<ICreditCardRepository>()
                .Setup(x => x.GetByIdAsync(id))
                .ReturnsAsync((CreditCard)null);

            // Act
            var result = await _appService.RemoveAsync(id);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Message);
            Assert.NotEmpty(result.Message);
            Assert.Equal("Cartão de crédito inválido.", result.Message);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetByIdAsync(id), Times.Once);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.RemoveAsync(It.IsAny<CreditCard>()), Times.Never);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Remove_ReturnSuccess()
        {
            // Arrange
            var bankAccount = new BankAccount(Guid.NewGuid(), Guid.NewGuid(), "Nu conta");
            var creditCard = new CreditCard(Guid.NewGuid(), bankAccount, "Name", 1, 5, 1, Brand.AMERICANEXPRESS);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Setup(x => x.GetByIdAsync(creditCard.Id))
                .ReturnsAsync(creditCard);

            // Act
            var result = await _appService.RemoveAsync(creditCard.Id);

            // Assert
            Assert.True(result.Succeeded);
            Assert.Null(result.Message);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.GetByIdAsync(creditCard.Id), Times.Once);

            _autoMocker.GetMock<ICreditCardRepository>()
                .Verify(x => x.RemoveAsync(It.IsAny<CreditCard>()), Times.Once);

            _autoMocker.GetMock<IUnitOfWork>()
                .Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}