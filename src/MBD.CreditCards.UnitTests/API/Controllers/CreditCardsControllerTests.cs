using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using MBD.CreditCards.API.Controllers;
using MBD.CreditCards.API.Models;
using MBD.CreditCards.Application.Interfaces;
using MBD.CreditCards.Application.Requests;
using MBD.CreditCards.Application.Responses;
using MBD.CreditCards.Domain.Entities;
using MBD.CreditCards.Domain.Entities.Common;
using MeuBolsoDigital.Application.Utils.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.AutoMock;
using Xunit;

namespace MBD.CreditCards.UnitTests.API.Controllers
{
    public class CreditCardsControllerTests
    {
        private readonly AutoMocker _autoMocker;
        private readonly Faker _faker;
        private readonly CreditCardsController _controller;

        public CreditCardsControllerTests()
        {
            _autoMocker = new AutoMocker();
            _faker = new Faker();
            _controller = _autoMocker.CreateInstance<CreditCardsController>();
        }

        [Fact]
        public async Task GetAll_Empty_ReturnNoContent()
        {
            // Arrante && Act
            var response = await _controller.GetAll() as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            _autoMocker.GetMock<ICreditCardAppService>().Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ReturnOk()
        {
            // Arrange
            var creditCard = new CreditCardResponse
            {
                Id = Guid.NewGuid(),
                BankAccountId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(10),
                ClosingDay = _faker.Random.Int(1, 30),
                DayOfPayment = _faker.Random.Int(1, 30),
                Limit = _faker.Finance.Amount(),
                Status = Status.Active,
                Brand = Brand.AMERICANEXPRESS
            };

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.GetAllAsync())
                       .ReturnsAsync(new List<CreditCardResponse> { creditCard });

            // Act
            var response = await _controller.GetAll() as ObjectResult;
            var value = response.Value as List<CreditCardResponse>;

            // Assert
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.NotNull(value);
            Assert.Equal(creditCard, value.First());
            serviceMock.Verify(x => x.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnNotFound()
        {
            // Arrange
            var id = Guid.NewGuid();

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.GetByIdAsync(id))
                       .ReturnsAsync(Result<CreditCardResponse>.Fail());

            // Act
            var response = await _controller.GetById(id) as NotFoundResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);
            serviceMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task GetById_ReturnOk()
        {
            // Arrange
            var id = Guid.NewGuid();

            var creditCard = new CreditCardResponse
            {
                Id = id,
                BankAccountId = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(10),
                ClosingDay = _faker.Random.Int(1, 30),
                DayOfPayment = _faker.Random.Int(1, 30),
                Limit = _faker.Finance.Amount(),
                Status = Status.Active,
                Brand = Brand.AMERICANEXPRESS
            };

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.GetByIdAsync(id))
                       .ReturnsAsync(Result<CreditCardResponse>.Success(creditCard));

            // Act
            var response = await _controller.GetById(id) as ObjectResult;
            var value = response.Value as CreditCardResponse;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
            Assert.Equal(creditCard, value);
            serviceMock.Verify(x => x.GetByIdAsync(id), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnBadRequest()
        {
            // Arrange
            var request = new CreateCreditCardRequest
            {
                BankAccountId = Guid.NewGuid(),
                ClosingDay = _faker.Random.Int(1, 30),
                DayOfPayment = _faker.Random.Int(1, 30),
                Limit = _faker.Finance.Amount(),
                Name = _faker.Random.AlphaNumeric(10),
                Brand = Brand.VISA
            };

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.CreateAsync(request))
                       .ReturnsAsync(Result<CreditCardResponse>.Fail(errorMessage));

            // Act
            var response = await _controller.Create(request) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
            serviceMock.Verify(x => x.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Create_ReturnCreated()
        {
            // Arrange
            var request = new CreateCreditCardRequest
            {
                BankAccountId = Guid.NewGuid(),
                ClosingDay = _faker.Random.Int(1, 30),
                DayOfPayment = _faker.Random.Int(1, 30),
                Limit = _faker.Finance.Amount(),
                Name = _faker.Random.AlphaNumeric(10),
                Brand = Brand.VISA
            };

            var creditCard = new CreditCardResponse
            {
                Id = Guid.NewGuid(),
                BankAccountId = request.BankAccountId,
                Name = request.Name,
                ClosingDay = request.ClosingDay,
                DayOfPayment = request.DayOfPayment,
                Limit = request.Limit,
                Brand = request.Brand,
                Status = Status.Active
            };

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.CreateAsync(request))
                       .ReturnsAsync(Result<CreditCardResponse>.Success(creditCard));

            // Act
            var response = await _controller.Create(request) as CreatedResult;
            var value = response.Value as CreditCardResponse;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status201Created, response.StatusCode);
            Assert.Equal(creditCard, value);
            Assert.Equal($"api/credit-cards/{creditCard.Id}", response.Location);
            serviceMock.Verify(x => x.CreateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnBadRequest()
        {
            // Arrange
            var request = new UpdateCreditCardRequest
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(10),
                ClosingDay = _faker.Random.Int(1, 30),
                DayOfPayment = _faker.Random.Int(1, 30),
                Limit = _faker.Finance.Amount(),
                BankAccountId = Guid.NewGuid(),
                Status = Status.Active,
                Brand = Brand.AMERICANEXPRESS
            };

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.UpdateAsync(request))
                       .ReturnsAsync(Result<CreditCardResponse>.Fail(errorMessage));

            // Act
            var response = await _controller.Update(request) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
            serviceMock.Verify(x => x.UpdateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Update_ReturnNoContent()
        {
            // Arrange
            var request = new UpdateCreditCardRequest
            {
                Id = Guid.NewGuid(),
                Name = _faker.Random.AlphaNumeric(10),
                ClosingDay = _faker.Random.Int(1, 30),
                DayOfPayment = _faker.Random.Int(1, 30),
                Limit = _faker.Finance.Amount(),
                BankAccountId = Guid.NewGuid(),
                Status = Status.Active,
                Brand = Brand.AMERICANEXPRESS
            };

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.UpdateAsync(request))
                       .ReturnsAsync(Result<CreditCardResponse>.Success());

            // Act
            var response = await _controller.Update(request) as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            serviceMock.Verify(x => x.UpdateAsync(request), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnBadRequest()
        {
            // Arrange
            var id = Guid.NewGuid();

            var errorMessage = Guid.NewGuid().ToString();

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.RemoveAsync(id))
                       .ReturnsAsync(Result<CreditCardResponse>.Fail(errorMessage));

            // Act
            var response = await _controller.Delete(id) as ObjectResult;
            var value = response.Value as ErrorModel;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status400BadRequest, response.StatusCode);
            Assert.Equal(errorMessage, value.Errors.First());
            serviceMock.Verify(x => x.RemoveAsync(id), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnNoContent()
        {
            // Arrange
            var id = Guid.NewGuid();

            var serviceMock = _autoMocker.GetMock<ICreditCardAppService>();

            serviceMock.Setup(x => x.RemoveAsync(id))
                       .ReturnsAsync(Result<CreditCardResponse>.Success());

            // Act
            var response = await _controller.Delete(id) as NoContentResult;

            // Assert
            Assert.NotNull(response);
            Assert.Equal(StatusCodes.Status204NoContent, response.StatusCode);
            serviceMock.Verify(x => x.RemoveAsync(id), Times.Once);
        }
    }
}