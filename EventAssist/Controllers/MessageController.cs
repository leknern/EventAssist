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
    public class MessageController(IMessageService messageService) : ControllerBase
    {
        [HttpGet]
        public IActionResult LoadMessages(int chatId, int index, int limit)
        {
            try
            {
                List<MessageResponse> response = messageService.GetMessages(chatId, index, limit);
                Log.Information("Loaded {Count} messages for chat {ChatId}.", response.Count, chatId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while loading messages for chat {ChatId}.", chatId);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetMessages(int chatId)
        {
            try
            {
                List<MessageResponse> response = messageService.GetMessages(chatId);
                Log.Information("Retrieved {Count} messages for chat {ChatId}.", response.Count, chatId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting messages for chat {ChatId}.", chatId);
                return BadRequest(ex.Message);
            }
        }
    }
}
