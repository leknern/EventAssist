using EventAssist.Models.DTOs;

namespace EventAssist.Services.Interfaces
{
    public interface IEventService
    {
        List<EventResponse> GetEventsByUserId(int userId);
        EventResponse AddEvent(EventRequest request, int userId);
        Task<EventResponse> AddEventAiAgentAsync(EventRequest request, int userId);
        EventResponse UpdateEventDescription(EventDescriptionRequest request, int userId);
        void RemoveEvent(int eventId, int userId);
    }
}
