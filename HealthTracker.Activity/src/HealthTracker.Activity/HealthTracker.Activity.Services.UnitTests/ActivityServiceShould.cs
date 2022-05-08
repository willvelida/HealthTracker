using AutoMapper;
using HealthTracker.Activity.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _dbRepositoryMock = new Mock<ICosmosDbRepository>();
            _sut = new ActivityService(_mapperMock.Object, _serviceBusRepoMock.Object, _loggerMock.Object, _dbRepositoryMock.Object);
        }
    }
}
