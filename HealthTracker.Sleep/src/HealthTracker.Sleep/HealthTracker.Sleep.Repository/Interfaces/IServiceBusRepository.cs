namespace HealthTracker.Sleep.Repository.Interfaces
{
    public interface IServiceBusRepository
    {
        Task SendMessageToQueue<T>(T messageContent);
    }
}
