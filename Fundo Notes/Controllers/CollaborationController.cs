using BussinesLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;
using ModelLayer.Collaboration;
using ModelLayer.ResponseModel;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CollaborationController : ControllerBase
    {
        private readonly ICollaboration _collabbl;
        private readonly ILogger<CollaborationController> _logger;

        public CollaborationController(ICollaboration collabbl, ILogger<CollaborationController> logger)
        {
            _collabbl = collabbl;
            _logger = logger;
        }
        [Authorize]
        [HttpPost]

        public async Task<IActionResult> AddCollaborator(int noteid, [FromBody] CollaborationRequestModel model)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);
                await _collabbl.AddCollaborator(noteid, model, userId);
                _logger.LogInformation("Collabarator Added");
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Collaboration Successfull",
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return NotFound(response);
            }

        }
        [Authorize]
        [HttpDelete("{collab_id}")]

        public async Task<IActionResult> RemoveCollaborator(int collab_id)
        {
            try
            {
                await _collabbl.RemoveCollaborator(collab_id);
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Collaborator removed successfully",
                    Data = null
                };
                _logger.LogInformation("Collaborator removed successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request {ex.Message}");
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                return BadRequest(response);
            }
        }
        [Authorize]
        [HttpGet()]

        public async Task<IActionResult> GetCollabbyid()
        {
            try
            {
                var collab = await _collabbl.GetCollaboration();
                var response = new ResponseModel<IEnumerable<CollabInfoModel>>
                {
                    Success = true,
                    Message = "Collaborators Fetched Successfully",
                    Data = collab
                };
                _logger.LogInformation("Collaborators Fetched Successfully");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Invalid Request {ex.Message}");
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                };
                return Ok(response);

            }

        }
    }
}