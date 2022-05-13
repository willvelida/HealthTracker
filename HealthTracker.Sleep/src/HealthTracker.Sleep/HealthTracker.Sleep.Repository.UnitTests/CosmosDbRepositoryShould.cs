using AutoFixture;
using FluentAssertions;
using HealthTracker.Common.Models;
using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Repository.UnitTests.Helpers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HealthTracker.Sleep.Repository.UnitTests
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

            _sut = new CosmosDbRepository(_mockCosmosClient.Object, _mockLogger.Object, _options);
        }

        [Fact]
        public async Task AddSleepDocumentWhenCreateItemAsyncIsCalled()
        {
            // Arrange
            var fixture = new Fixture();
            var testSleepEnvelope = fixture.Create<SleepEnvelope>();

            _mockContainer.SetupCreateItemAsync<SleepEnvelope>();

            // Act
            Func<Task> serviceAction = async () => await _sut.CreateSleepDocument(testSleepEnvelope);

            // Assert
            await serviceAction.Should().NotThrowAsync<Exception>();
            _mockContainer.Verify(x => x.CreateItemAsync(
                It.IsAny<SleepEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ThrowExceptionWhenCreateItemAsyncCallFails()
        {
            // Arrange
            var fixture = new Fixture();
            var testSleepEnvelope = fixture.Create<SleepEnvelope>();

            _mockContainer.SetupCreateItemAsync<SleepEnvelope>();
            _mockContainer.Setup(x => x.CreateItemAsync(
                It.IsAny<SleepEnvelope>(),
                It.IsAny<PartitionKey>(),
                It.IsAny<ItemRequestOptions>(),
                It.IsAny<CancellationToken>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> serviceAction = async () => await _sut.CreateSleepDocument(testSleepEnvelope);

            // Assert
            await serviceAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task GetAllSleepRecords()
        {
            // Arrange
            List<SleepEnvelope> sleepEnvelopes = new List<SleepEnvelope>();
            var fixture = new Fixture();
            SleepEnvelope sleepEnvelope = fixture.Create<SleepEnvelope>(); ;
            sleepEnvelopes.Add(sleepEnvelope);

            _mockContainer.SetupItemQueryIteratorMock(sleepEnvelopes);
            _mockContainer.SetupItemQueryIteratorMock(new List<int> { sleepEnvelopes.Count });

            // Act
            var response = await _sut.GetAllSleepDocuments();

            // Assert
            Assert.Equal(sleepEnvelopes.Count, response.Count);
        }

        [Fact]
        public async Task GetAllSleepRecords_NoResultsReturned()
        {
            // Arrange
            var emptySleepList = new List<SleepEnvelope>();

            var getSleeps = _mockContainer.SetupItemQueryIteratorMock(emptySleepList);
            getSleeps.feedIterator.Setup(x => x.HasMoreResults).Returns(false);
            _mockContainer.SetupItemQueryIteratorMock(new List<int>() { 0 });

            // Act
            var response = await _sut.GetAllSleepDocuments();

            // Act
            Assert.Empty(response);
        }

        [Fact]
        public async Task CatchExceptionWhenCosmosThrowsExceptionWhenGetAllSleepsIsCalled()
        {
            // Arrange
            _mockContainer.Setup(x => x.GetItemQueryIterator<SleepEnvelope>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
                .Throws(new Exception());

            // Act
            Func<Task> responseAction = async () => await _sut.GetAllSleepDocuments();

            // Act
            await responseAction.Should().ThrowAsync<Exception>();
        }

        [Fact]
        public async Task GetSleepByDate()
        {
            // Arrange
            List<SleepEnvelope> sleepEnvelopes = new List<SleepEnvelope>();
            SleepEnvelope sleepEnvelope = new SleepEnvelope
            {
                Id = Guid.NewGuid().ToString(),
                DocumentType = "Test",
                Sleep = new HealthTracker.Common.Models.Sleep
                {
                    SleepDate = "31/12/2019"
                }
            };
            sleepEnvelopes.Add(sleepEnvelope);

            _mockContainer.SetupItemQueryIteratorMock(sleepEnvelopes);
            _mockContainer.SetupItemQueryIteratorMock(new List<int> { sleepEnvelopes.Count });

            var sleepDate = sleepEnvelope.Sleep.SleepDate;

            // Act
            var response = await _sut.GetSleepEnvelopeByDate(sleepDate);

            // Assert
            Assert.Equal(sleepDate, response.Sleep.SleepDate);
        }

        [Fact]
        public async Task GetActivityByDate_NoResultsReturned()
        {
            // Arrange
            var emptySleepList = new List<SleepEnvelope>();

            var getActivities = _mockContainer.SetupItemQueryIteratorMock(emptySleepList);
            getActivities.feedIterator.Setup(x => x.HasMoreResults).Returns(false);
            _mockContainer.SetupItemQueryIteratorMock(new List<int>() { 0 });

            // Act
            var response = await _sut.GetSleepEnvelopeByDate("31/12/2019");

            // Act
            Assert.Null(response);
        }

        [Fact]
        public async Task CatchExceptionWhenCosmosThrowsExceptionWhenGetActivityByDateIsCalled()
        {
            // Arrange
            _mockContainer.Setup(x => x.GetItemQueryIterator<SleepEnvelope>(
                It.IsAny<QueryDefinition>(),
                It.IsAny<string>(),
                It.IsAny<QueryRequestOptions>()))
                .Throws(new Exception());

            // Act
            Func<Task> responseAction = async () => await _sut.GetSleepEnvelopeByDate("31/12/2019");

            // Act
            await responseAction.Should().ThrowAsync<Exception>();
        }
    }
}
