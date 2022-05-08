namespace HealthTracker.Activity.Repository.Interfaces
{
    public interface IServiceBusRepository
    {
        Task SendMessageToQueue<T>(T messageContent);
    }
}
