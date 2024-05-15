using BussinesLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Label;
using Repository.Entity;
using System.Security.Claims;
using ModelLayer.ResponseModel;

namespace FundooNotes.Controllers
{
    [Route("api/label")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelBusinessLayer labelbl;
        private readonly ILogger<LabelController> _logger;
        public LabelController(ILabelBusinessLayer label, ILogger<LabelController> logger)
        {
            labelbl = label;
            _logger = logger;
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddLabel(CreateLabel label)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);
                await labelbl.CreateLabel(label, userId);
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Label created."

                };
                _logger.LogInformation("Label created ");
                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                };
                _logger.LogError(ex.Message);
                return NotFound(response);
            }
        }
        [Authorize]
        [HttpDelete("{label_id}")]
        public async Task<IActionResult> Removelabel(int label_id)
        {
            try
            {
                await labelbl.DeleteLabel(label_id);
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Label deleted"

                };
                _logger.LogInformation("Label deleted");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [Authorize]
        [HttpPut("{label_id}")]
        public async Task<IActionResult> UpdateLabel(CreateLabel label, int label_id)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);

                await labelbl.UpdateLabel(label, label_id, userId);
                var response = new ResponseModel<String>
                {
                    Success = true,
                    Message = "Label Updated",
                    Data = null

                };
                _logger.LogInformation("Label Updated");
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAllLabel()
        {
            try
            {
                _logger.LogInformation("getnotesbyid");
                var label = await labelbl.GetAllLabels();
                _logger.LogInformation("All Labels retrieved successfully");
                return Ok(new ResponseModel<IEnumerable<LabelEntity>>
                {
                    Success = true,
                    Message = "All Labels retrieved successfully",
                    Data = label
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
        [Authorize]
        [HttpGet("{label_id}")]
        public async Task<IActionResult> GetAllNotebyLabelId(int label_id)
        {
            try
            {
                var label = await labelbl.GetAllNotesbyLabelId(label_id);
                _logger.LogInformation("Label retrieved successfully by its id");
                return Ok(new ResponseModel<object>
                {
                    Success = true,
                    Message = "Label retrieved successfully by its id",
                    Data = label
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }

    }
}