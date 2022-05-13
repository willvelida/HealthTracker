using AutoMapper;
using HealthTracker.Sleep.Common.Models;
using HealthTracker.Sleep.Repository.Interfaces;
using HealthTracker.Sleep.Services.Interfaces;
using Microsoft.Extensions.Logging;
using mdl = HealthTracker.Common.Models;

namespace HealthTracker.Sleep.Services
{
    public class SleepService : ISleepService
    {
        private readonly IMapper _mapper;
        private readonly IServiceBusRepository _serviceBusRepository;
        private readonly ILogger<SleepService> _logger;

        public SleepService(IMapper mapper, IServiceBusRepository serviceBusRepository, ILogger<SleepService> logger)
        {
            _mapper = mapper;
            _serviceBusRepository = serviceBusRepository;
            _logger = logger;
        }

        public async Task MapAndSendSleepRecordToQueue(string date, SleepResponse sleepResponse)
        {
            try
            {
                var sleep = new mdl.Sleep();
                sleep.SleepDate = date;
                _mapper.Map(sleepResponse, sleep);

                await _serviceBusRepository.SendMessageToQueue(sleep);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(MapAndSendSleepRecordToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
