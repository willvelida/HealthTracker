using AutoFixture;
using AutoMapper;
using FluentAssertions;
using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Sleep.Services.UnitTests
{
    public class SleepServiceShould
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IServiceBusRepository> _serviceBusRepoMock;
        private Mock<ICosmosDbRepository> _dbRepositoryMock;
        private Mock<ILogger<SleepService>> _loggerMock;

        private SleepService _sut;

        public SleepServiceShould()
        {
            _mapperMock = new Mock<IMapper>();
            _serviceBusRepoMock = new Mock<IServiceBusRepository>();
            _loggerMock = new Mock<ILogger<SleepService>>();
            _dbRepositoryMock = new Mock<ICosmosDbRepository>();
            _sut = new SleepService(_mapperMock.Object, _serviceBusRepoMock.Object, _loggerMock.Object, _dbRepositoryMock.Object);
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateSleepDocumentFails()
        {
            // Arrange
            var fixture = new Fixture();
            var sleep = fixture.Create<mdl.Sleep>();
            sleep.SleepDate = "11/05/2022";
            _dbRepositoryMock.Setup(x => x.CreateSleepDocument(It.IsAny<mdl.SleepEnvelope>())).ThrowsAsync(new Exception());

            // Action
            Func<Task> sleepServiceAction = async () => await _sut.MapSleepEnvelopeAndSaveToDatabase(sleep);

            // Assert
            await sleepServiceAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task SuccessfullyInvokeCreateSleepDocumentWithValidEnvelope()
        {
            // Arrange
            var fixture = new Fixture();
            var sleep = fixture.Create<mdl.Sleep>();
            sleep.SleepDate = "11/05/2022";

            // Action
            Func<Task> sleepServiceAction = async () => await _sut.MapSleepEnvelopeAndSaveToDatabase(sleep);

            // Assert
            await sleepServiceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenSendMessageToQueueFails()
        {
            // Arrange
            var fixture = new Fixture();
            var sleepResponse = fixture.Create<SleepResponse>();
            var sleep = fixture.Create<mdl.Sleep>();
            sleep.SleepDate = "11/05/2022";

            _mapperMock.Setup(x => x.Map(sleepResponse, sleep)).Returns(sleep);

            _serviceBusRepoMock.Setup(x => x.SendMessageToQueue(It.IsAny<mdl.Sleep>())).ThrowsAsync(new Exception());

            // Action
            Func<Task> sleepServiceAction = async () => await _sut.MapAndSendSleepRecordToQueue(sleep.SleepDate, sleepResponse);

            // Assert
            await sleepServiceAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task SuccessfullyInvokeSendMessageToQueueWithValidSleepObject()
        {
            // Arrange
            var fixture = new Fixture();
            var sleepResponse = fixture.Create<SleepResponse>();
            var sleep = fixture.Create<mdl.Sleep>();
            sleep.SleepDate = "11/05/2022";

            _mapperMock.Setup(x => x.Map(sleepResponse, sleep)).Returns(sleep);

            // Action
            Func<Task> sleepServiceAction = async () => await _sut.MapAndSendSleepRecordToQueue(sleep.SleepDate, sleepResponse);

            // Assert
            await sleepServiceAction.Should().NotThrowAsync<Exception>();
        }
    }
}
