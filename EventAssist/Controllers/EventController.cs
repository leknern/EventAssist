using EventAssist.Models.DTOs;
using EventAssist.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace EventAssist.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Policy = "RequireFullToken")]
    public class EventController(IEventService eventService, IAuthService authService) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetEvents()
        {
            try
            {
                int userId = authService.GetUserId(User);
                List<EventResponse> response = eventService.GetEventsByUserId(userId);
                Log.Information("User {UserId} retrieved {Count} events.", userId, response.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting events.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult AddEvent(EventRequest request)
        {
            try
            {
                int userId = authService.GetUserId(User);
                EventResponse response = eventService.AddEvent(request, userId);
                Log.Information("User {UserId} added event {EventId}.", userId, response.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while adding event.");
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch]
        public IActionResult UpdateEvent(EventDescriptionRequest request)
        {
            try
            {
                int userId = authService.GetUserId(User);
                EventResponse response = eventService.UpdateEventDescription(request, userId);
                Log.Information("User {UserId} updated event {EventId}.", userId, response.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating event.");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult RemoveEvent(int eventId)
        {
            try
            {
                int userId = authService.GetUserId(User);
                eventService.RemoveEvent(eventId, userId);
                Log.Information("User {UserId} removed event {EventId}.", userId, eventId);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while removing event {EventId}.", eventId);
                return BadRequest(ex.Message);
            }
        }
    }
}
