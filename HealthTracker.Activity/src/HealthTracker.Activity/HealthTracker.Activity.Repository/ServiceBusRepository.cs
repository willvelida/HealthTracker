﻿using Azure.Messaging.ServiceBus;
using HealthTracker.Activity.Common;
using HealthTracker.Activity.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace HealthTracker.Activity.Repository
{
    public class ServiceBusRepository : IServiceBusRepository
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<ServiceBusRepository> _logger;
        private readonly Settings _settings;

        public ServiceBusRepository(ServiceBusClient serviceBusClient, ILogger<ServiceBusRepository> logger, IOptions<Settings> options)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _settings = options.Value;
        }

        public async Task SendMessageToQueue<T>(T messageContent)
        {
            try
            {
                ServiceBusSender serviceBusSender = _serviceBusClient.CreateSender(_settings.ActivityQueueName);
                var messageAsJson = JsonConvert.SerializeObject(messageContent);
                await serviceBusSender.SendMessageAsync(new ServiceBusMessage(messageAsJson));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception thrown in {nameof(SendMessageToQueue)}: {ex.Message}");
                throw;
            }
        }
    }
}
