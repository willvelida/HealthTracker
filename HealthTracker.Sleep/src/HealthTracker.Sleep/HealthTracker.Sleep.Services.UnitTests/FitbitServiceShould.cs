using FluentAssertions;
using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HealthTracker.Sleep.Services.UnitTests
{
    public class FitbitServiceShould
    {
        private Mock<IOptions<Settings>> _optionsMock;
        private Mock<IKeyVaultRepository> _keyVaultRepoMock;
        private Mock<HttpClient> _httpClientMock;
        private Mock<ILogger<FitbitService>> _loggerMock;

        private FitbitService _sut;

        public FitbitServiceShould()
        {
            _optionsMock = new Mock<IOptions<Settings>>();
            _keyVaultRepoMock = new Mock<IKeyVaultRepository>();
            _httpClientMock = new Mock<HttpClient>();
            _loggerMock = new Mock<ILogger<FitbitService>>();
            _sut = new FitbitService(_keyVaultRepoMock.Object, _httpClientMock.Object, _loggerMock.Object, _optionsMock.Object);
        }

        [Fact]
        public async Task CatchAndThrowExceptionWhenRetrieveSecretFromKeyVaultThrowsException()
        {
            // Arrange
            _keyVaultRepoMock.Setup(x => x.RetrieveSecretFromKeyVault(It.IsAny<string>())).ThrowsAsync(new Exception());

            // Act
            Func<Task> fitbitServiceAction = async () => await _sut.GetSleepResonse("08/05/2022");

            // Assert
            await fitbitServiceAction.Should().ThrowAsync<Exception>();
        }
    }
}
