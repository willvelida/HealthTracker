using AutoFixture;
using AutoMapper;
using FluentAssertions;
using HealthTracker.Activity.Common;
using HealthTracker.Activity.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Activity.Services.UnitTests
{
    public class ActivityServiceShould
    {
        private Mock<IMapper> _mapperMock;
        private Mock<IServiceBusRepository> _serviceBusRepoMock;
        private Mock<ICosmosDbRepository> _dbRepositoryMock;
        private Mock<ILogger<ActivityService>> _loggerMock;

        private ActivityService _sut;

        public ActivityServiceShould()
        {
            _mapperMock = new Mock<IMapper>();
            _serviceBusRepoMock = new Mock<IServiceBusRepository>();
            _loggerMock = new Mock<ILogger<ActivityService>>();
            _dbRepositoryMock = new Mock<ICosmosDbRepository>();
            _sut = new ActivityService(_mapperMock.Object, _serviceBusRepoMock.Object, _loggerMock.Object, _dbRepositoryMock.Object);
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateActivityDocumentFails()
        {
            // Arrange
            var fixture = new Fixture();
            var activity = fixture.Create<mdl.Activity>();
            activity.ActivityDate = "11/05/2022";
            _dbRepositoryMock.Setup(x => x.CreateActivityDocument(It.IsAny<mdl.ActivityEnvelope>())).ThrowsAsync(new Exception());

            // Action
            Func<Task> activityServiceAction = async () => await _sut.MapActivityEnvelopeAndSaveToDatabase(activity);

            // Assert
            await activityServiceAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task SuccessfullyInvokeCreateActivityDocumentWithValidEnvelope()
        {
            // Arrange
            var fixture = new Fixture();
            var activity = fixture.Create<mdl.Activity>();
            activity.ActivityDate = "11/05/2022";

            // Action
            Func<Task> activityServiceAction = async () => await _sut.MapActivityEnvelopeAndSaveToDatabase(activity);

            // Assert
            await activityServiceAction.Should().NotThrowAsync<Exception>();
        }

        [Fact]
        public async Task ThrowExceptionWhenSendMessageToQueueFails()
        {
            // Arrange
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();
            var activity = fixture.Create<mdl.Activity>();
            activity.ActivityDate = "11/05/2022";

            _mapperMock.Setup(x => x.Map(activityResponse, activity)).Returns(activity);

            _serviceBusRepoMock.Setup(x => x.SendMessageToQueue(It.IsAny<mdl.Activity>())).ThrowsAsync(new Exception());

            // Action
            Func<Task> activityServiceAction = async () => await _sut.MapAndSendActivityRecordToQueue(activity.ActivityDate, activityResponse);

            // Assert
            await activityServiceAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task SuccessfullyInvokeSendMessageToQueueWithValidActivityObject()
        {
            // Arrange
            var fixture = new Fixture();
            var activityResponse = fixture.Create<ActivityResponse>();
            var activity = fixture.Create<mdl.Activity>();
            activity.ActivityDate = "11/05/2022";

            _mapperMock.Setup(x => x.Map(activityResponse, activity)).Returns(activity);

            // Action
            Func<Task> activityServiceAction = async () => await _sut.MapAndSendActivityRecordToQueue(activity.ActivityDate, activityResponse);

            // Assert
            await activityServiceAction.Should().NotThrowAsync<Exception>();
        }
    }
}
