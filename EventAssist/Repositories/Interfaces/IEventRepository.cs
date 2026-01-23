using EventAssist.Models.Records;

namespace EventAssist.Repositories.Interfaces
{
    public interface IEventRepository
    {
        List<EventRecord> GetEventsByUserId(int userId);
        EventRecord GetEventById(int eventId);
        void AddEvent(EventRecord record);
        void UpdateEvent(EventRecord record);
        void RemoveEvent(EventRecord record);
    }
}
