using EventAssist.Hubs;
using EventAssist.Models.DTOs;
using EventAssist.Models.Records;
using EventAssist.Repositories.Interfaces;
using EventAssist.Services.Interfaces;
using Mapster;
using Microsoft.AspNetCore.SignalR;

namespace EventAssist.Services
{
    public class EventService(IEventRepository eventRepository, IHubContext<EventHub> eventHubContext) : IEventService
    {
        public List<EventResponse> GetEventsByUserId(int userId)
        {
            List<EventRecord> records = eventRepository.GetEventsByUserId(userId);
            return records.Adapt<List<EventResponse>>();
        }

        public EventResponse AddEvent(EventRequest request, int userId)
        {
            EventRecord record = request.Adapt<EventRecord>();
            record.UserRecordId = userId;
            eventRepository.AddEvent(record);

            return record.Adapt<EventResponse>();
        }

        public EventResponse UpdateEventDescription(EventDescriptionRequest request, int userId)
        {
            EventRecord record = eventRepository.GetEventById(request.Id);

            if (record.UserRecordId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission.");
            }

            record.Description = request.Description;
            eventRepository.UpdateEvent(record);

            return record.Adapt<EventResponse>();
        }

        public void RemoveEvent(int eventId, int userId)
        {
            EventRecord record = eventRepository.GetEventById(eventId);

            if (record.UserRecordId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission.");
            }

            eventRepository.RemoveEvent(record);
        }

        public async Task<EventResponse> AddEventAiAgentAsync(EventRequest request, int userId)
        {
            EventRecord record = request.Adapt<EventRecord>();
            record.UserRecordId = userId;
            eventRepository.AddEvent(record);

            await eventHubContext.Clients.User(userId.ToString())
                .SendAsync("ReceiveNewEvent", record.Adapt<EventResponse>());

            return record.Adapt<EventResponse>();
        }
    }
}
