using AutoFixture;
using FluentAssertions;
using HealthTracker.Activity.Common;
using mdl = HealthTracker.Common.Models;
using HealthTracker.Activity.Repository.UnitTests.Helpers;
using HealthTracker.Common.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HealthTracker.Activity.Repository.UnitTests
{
    public class CosmosDbRepositoryShould
    {
        private Mock<CosmosClient> _mockCosmosClient;
        private Mock<Container> _mockContainer;
        private Mock<ILogger<CosmosDbRepository>> _mockLogger;
        private IOptions<Settings> _options;

        private CosmosDbRepository _sut;

        public CosmosDbRepositoryShould()
        {
            _mockCosmosClient = new Mock<CosmosClient>();
            _mockContainer = new Mock<Container>();
            _mockCosmosClient.Setup(x => x.GetContainer(It.IsAny<string>(), It.IsAny<string>())).Returns(_mockContainer.Object);
            _mockLogger = new Mock<ILogger<CosmosDbRepository>>();
            var testSettings = new Settings { DatabaseName = "databaseName", ContainerName = "containerName" };
            _options = Options.Create(testSettings);

            _sut = new CosmosDbRepository(_mockCosmosClient.Object, _options, _mockLogger.Object);
        }

        [Fact]
        public async Task AddActivityDocumentWhenCreateItemAsyncIsCalled()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivityDocument = fixture.Create<ActivityEnvelope>();

            _mockContainer.SetupCreateItemAsync<ActivityEnvelope>();

            // Act
            Func<Task> serviceAction = async () => await _sut.CreateActivityDocument(testActivityDocument);

            // Assert
            await serviceAction.Should().NotThrowAsync<Exception>();
            _mockContainer.Verify(x => x.CreateItemAsync(
                It.IsAny<ActivityEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateItemAsyncCallFails()
        {
            // Arrange
            var fixture = new Fixture();
            var testActivityDocument = fixture.Create<ActivityEnvelope>();

            _mockContainer.SetupCreateItemAsync<ActivityEnvelope>();
            _mockContainer.Setup(x => x.CreateItemAsync(
                It.IsAny<ActivityEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> serviceAction = async () => await _sut.CreateActivityDocument(testActivityDocument);

            // Assert
            await serviceAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task GetAllActivites()
        {
            // Arrange
            List<ActivityEnvelope> activityEnvelopes = new List<ActivityEnvelope>();
            var fixture = new Fixture();
            ActivityEnvelope activityEnvelope = fixture.Create<ActivityEnvelope>(); ;
            activityEnvelopes.Add(activityEnvelope);

            _mockContainer.SetupItemQueryIteratorMock(activityEnvelopes);
            _mockContainer.SetupItemQueryIteratorMock(new List<int> { activityEnvelopes.Count });

            // Act
            var response = await _sut.GetActivities();

            // Assert
            Assert.Equal(activityEnvelopes.Count, response.Count);
        }

        [Fact]
        public async Task GetAllActivies_NoResultsReturned()
        {
            // Arrange
            var emptyActivitiesList = new List<ActivityEnvelope>();

            var getActivities = _mockContainer.SetupItemQueryIteratorMock(emptyActivitiesList);
            getActivities.feedIterator.Setup(x => x.HasMoreResults).Returns(false);
            _mockContainer.SetupItemQueryIteratorMock(new List<int>() { 0 });

            // Act
            var response = await _sut.GetActivities();

            // Act
            Assert.Empty(response);
        }

        [Fact]
        public async Task CatchExceptionWhenCosmosThrowsExceptionWhenGetActivitiesIsCalled()
        {
            // Arrange
            _mockContainer.Setup(x => x.GetItemQueryIterator<ActivityEnvelope>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
                .Throws(new Exception());

            // Act
            Func<Task> responseAction = async () => await _sut.GetActivities();

            // Act
            await responseAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task GetActivityByDate()
        {
            // Arrange
            List<ActivityEnvelope> activityEnvelopes = new List<ActivityEnvelope>();
            ActivityEnvelope activityEnvelope = new ActivityEnvelope
            {
                Id = Guid.NewGuid().ToString(),
                DocumentType = "Test",
                Activity = new mdl.Activity
                {
                    CaloriesBurned = 4500,
                    ActivityDate = "31/12/2019"
                }
            };
            activityEnvelopes.Add(activityEnvelope);

            _mockContainer.SetupItemQueryIteratorMock(activityEnvelopes);
            _mockContainer.SetupItemQueryIteratorMock(new List<int> { activityEnvelopes.Count });

            var activityDate = activityEnvelope.Activity.ActivityDate;

            // Act
            var response = await _sut.GetActivityByDate(activityDate);

            // Assert
            Assert.Equal(activityDate, response.Activity.ActivityDate);
        }

        [Fact]
        public async Task GetActivityByDate_NoResultsReturned()
        {
            // Arrange
            var emptyActivitiesList = new List<mdl.ActivityEnvelope>();

            var getActivities = _mockContainer.SetupItemQueryIteratorMock(emptyActivitiesList);
            getActivities.feedIterator.Setup(x => x.HasMoreResults).Returns(false);
            _mockContainer.SetupItemQueryIteratorMock(new List<int>() { 0 });

            // Act
            var response = await _sut.GetActivityByDate("31/12/2019");

            // Act
            Assert.Null(response);
        }

        [Fact]
        public async Task CatchExceptionWhenCosmosThrowsExceptionWhenGetActivityByDateIsCalled()
        {
            // Arrange
            _mockContainer.Setup(x => x.GetItemQueryIterator<mdl.ActivityEnvelope>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
                .Throws(new Exception());

            // Act
            Func<Task> responseAction = async () => await _sut.GetActivityByDate("31/12/2019");

            // Act
            await responseAction.Should().ThrowAsync<Exception>();
        }
    }
}