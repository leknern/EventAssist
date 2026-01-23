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
    public class ChatController(IChatService chatService, IAuthService authService) : ControllerBase
    {
        [HttpGet]
        public IActionResult TryGetChat()
        {
            try
            {
                int userId = authService.GetUserId(User);
                ChatResponse? response = chatService.TryGetChat(userId);
                Log.Information("User {UserId} retrieved chat.", userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting chat for user.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetChats()
        {
            try
            {
                List<ChatResponse> response = chatService.GetChats();
                Log.Information("Retrieved {Count} chats.", response.Count);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while getting chats.");
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> OpenChatAsync()
        {
            try
            {
                int userId = authService.GetUserId(User);
                ChatResponse response = await chatService.AddChatAsync(userId);
                Log.Information("User {UserId} opened chat {ChatId}.", userId, response.Id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while opening chat.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> TakeOverChatAsync(int chatId)
        {
            try
            {
                int userId = authService.GetUserId(User);
                await chatService.TakeOverChatAsync(chatId, userId);
                Log.Information("User {UserId} took over chat {ChatId}.", userId, chatId);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while taking over chat {ChatId}.", chatId);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> RequestHumanSupportAsync(int chatId)
        {
            try
            {
                int userId = authService.GetUserId(User);
                await chatService.RequestHumanSupportAsync(chatId, userId);
                Log.Information("User {UserId} requested human support for chat {ChatId}.", userId, chatId);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while requesting human support for chat {ChatId}.", chatId);
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCustomerSupportComment(CustomerSupportCommentRequest request)
        {
            try
            {
                int userId = authService.GetUserId(User);
                await chatService.UpdateCustomerSupportCommentAsync(request, userId);
                Log.Information("User {UserId} updated customer support comment for chat {ChatId}.", userId, request.ChatId);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while updating customer support comment.");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> CloseChatAsync(int chatId)
        {
            try
            {
                int userId = authService.GetUserId(User);
                await chatService.CloseChatAsync(chatId, userId);
                Log.Information("User {UserId} closed chat {ChatId}.", userId, chatId);
                return Ok();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while closing chat {ChatId}.", chatId);
                return BadRequest(ex.Message);
            }
        }
    }
}
