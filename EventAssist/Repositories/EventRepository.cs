using EventAssist.Contexts;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;

namespace EventAssist.Repositories
{
    public class EventRepository(AppDbContext context) : IEventRepository
    {
        public List<EventRecord> GetEventsByUserId(int userId)
        {
            return [.. context.Events.Where(e => e.UserRecordId == userId)];
        }

        public EventRecord GetEventById(int eventId)
        {
            return context.Events.First(e => e.Id == eventId);
        }

        public void AddEvent(EventRecord record)
        {
            context.Events.Add(record);
            context.SaveChanges();
        }

        public void UpdateEvent(EventRecord record)
        {
            context.Events.Update(record);
            context.SaveChanges();
        }

        public void RemoveEvent(EventRecord record)
        {
            context.Events.Remove(record);
            context.SaveChanges();
        }
    }
}
