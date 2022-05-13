using Microsoft.Azure.Cosmos;
using Moq;
using System.Collections.Generic;
using System.Threading;

namespace HealthTracker.Sleep.Repository.UnitTests.Helpers
{
    public static class CosmosExtensions
    {
        public static Mock<ItemResponse<T>> SetupCreateItemAsync<T>(this Mock<Container> containerMock)
        {
            var itemResponseMock = new Mock<ItemResponse<T>>();

            containerMock
                .Setup(x => x.CreateItemAsync(
                    It.IsAny<T>(),
                    It.IsAny<PartitionKey>(),
                    It.IsAny<ItemRequestOptions>(),
                    It.IsAny<CancellationToken>()))
                .Callback((T item, PartitionKey? partitionKey, ItemRequestOptions opts, CancellationToken ct) => itemResponseMock.Setup(x => x.Resource).Returns(null))
                .ReturnsAsync((T item, PartitionKey? partitionKey, ItemRequestOptions opts, CancellationToken ct) => itemResponseMock.Object);

            return itemResponseMock;
        }

        public static (Mock<FeedResponse<T>> feedResponseMock, Mock<FeedIterator<T>> feedIterator) SetupItemQueryIteratorMock<T>(this Mock<Container> containerMock, IEnumerable<T> itemsToReturn)
        {
            var feedRepsonseMock = new Mock<FeedResponse<T>>();
            feedRepsonseMock.Setup(x => x.Resource).Returns(itemsToReturn);
            var iteratorMock = new Mock<FeedIterator<T>>();
            iteratorMock.SetupSequence(x => x.HasMoreResults).Returns(true).Returns(false);
            iteratorMock.Setup(x => x.ReadNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(feedRepsonseMock.Object);
            containerMock.Setup(x => x.GetItemQueryIterator<T>(It.IsAny<QueryDefinition>(), It.IsAny<string>(), It.IsAny<QueryRequestOptions>()))
                .Returns(iteratorMock.Object);

            return (feedRepsonseMock, iteratorMock);
        }
    }
}
