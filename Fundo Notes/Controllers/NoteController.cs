using BussinesLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using ModelLayer.Notes;
using ModelLayer.ResponseModel;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FundooNotes.Controllers
{
    [Route("api/note")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly INoteBusinessLayer _notesBL;
        private readonly IDistributedCache _cache;
        private readonly ILogger<NoteController> _logger;

        public NoteController(INoteBusinessLayer notesBL, IDistributedCache cache, ILogger<NoteController> logger)
        {
            _notesBL = notesBL;
            _cache = cache;
            _logger = logger;
        }

        [Authorize]
        [HttpPost()]
        public async Task<IActionResult> CreateNote(CreateNoteRequest createNoteRequest,int Label_id)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);

                await _notesBL.CreateNote(createNoteRequest, userId,Label_id);
                var response = new ResponseModel<string>
                {
                    Success = true,
                    Message = "Note Created Successfully",

                };
                _cache.Remove($"Notes_active_{userId}");
                _cache.Remove($"Notes_archived_{userId}");
                _cache.Remove($"Notes_trash_{userId}");
                _logger.LogInformation("Note Created Successfully");
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
                return Ok(response);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> DisplayNote()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);
                var key = $"Notes_active_{userId}";
                var cachedNote = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cachedNote))
                {
                    var notesList = JsonConvert.DeserializeObject<List<NoteResponse>>(cachedNote);
                    if (notesList.Count() != 0) {
                        var response = new ResponseModel<IEnumerable<NoteResponse>>
                        {
                            Success = true,
                            Message = "Active Note Fetched Successfully from cache",
                            Data = notesList
                        };
                        _logger.LogInformation("Active Note Fetched Successfully from cache");
                        return Ok(response);
                    }
                }
                var notes = await _notesBL.GetAllNoteAsync(userId);
                if (notes != null)
                {
                    var serializedNote = JsonConvert.SerializeObject(notes);
                    await _cache.SetStringAsync(key, serializedNote, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    });
                    //await _cache.SetStringAsync(key, JsonConvert.SerializeObject(notes), TimeSpan.FromMinutes(10));
                    var response = new ResponseModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Active Note Fetched Successfully from DB",
                        Data = notes
                    };
                    _logger.LogInformation("Active Note Fetched Successfully from DB");
                    return Ok(response);
                }
                return Ok();
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
        [HttpGet("archived")]
        public async Task<IActionResult> DisplayArchivedNotes()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);
                var key = $"Notes_archived_{userId}";
                var cachedNote = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cachedNote))
                {
                    var notesList = JsonConvert.DeserializeObject<List<NoteResponse>>(cachedNote);
                    if (notesList.Count() != 0)
                    {
                        var response = new ResponseModel<IEnumerable<NoteResponse>>
                        {
                            Success = true,
                            Message = "Archived Notes Fetched Successfully from cache",
                            Data = notesList
                        };
                        _logger.LogInformation("Archived Notes Fetched Successfully from cache");
                        return Ok(response);
                    }
                }
                var notes = await _notesBL.GetAllArchivedNoteAsync(userId);
                if (notes != null)
                {
                    var serializedNote = JsonConvert.SerializeObject(notes);
                    await _cache.SetStringAsync(key, serializedNote, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    });
                    //await _cache.SetStringAsync(key, JsonConvert.SerializeObject(notes), TimeSpan.FromMinutes(10));
                    var response = new ResponseModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Archived Notes Fetched Successfully from DB",
                        Data = notes
                    };
                    _logger.LogInformation("Archived Notes Fetched Successfully from DB");
                    return Ok(response);
                }
                return Ok();
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
        [HttpGet("trash")]
        public async Task<IActionResult> DisplayDeletedNotes()
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);
                var key = $"Notes_trash_{userId}";
                var cachedNote = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(cachedNote))
                {
                    var notesList = JsonConvert.DeserializeObject<List<NoteResponse>>(cachedNote);
                    if (notesList.Count() != 0) 
                    {
                        var response = new ResponseModel<IEnumerable<NoteResponse>>
                        {
                            Success = true,
                            Message = "Trashed Notes Fetched Successfully from cache",
                            Data = notesList
                        };
                        _logger.LogInformation("Trashed Notes Fetched Successfully from cache");
                        return Ok(response);
                    }
                }
                var notes = await _notesBL.GetAllDeletedNoteAsync(userId);
                if (notes != null)
                {
                    var serializedNote = JsonConvert.SerializeObject(notes);
                    await _cache.SetStringAsync(key, serializedNote, new DistributedCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                    });
                    //await _cache.SetStringAsync(key, JsonConvert.SerializeObject(notes), TimeSpan.FromMinutes(10));
                    var response = new ResponseModel<IEnumerable<NoteResponse>>
                    {
                        Success = true,
                        Message = "Trashed Notes Fetched Successfully from DB",
                        Data = notes
                    };
                    _logger.LogInformation("Trashed Notes Fetched Successfully from DB");
                    return Ok(response);
                }
                return Ok();
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
        [HttpPut("{note_id}")]
        public async Task<IActionResult> UpdateNote(int note_id, CreateNoteRequest updateone)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);
                await _notesBL.UpdateNote(note_id, userId, updateone);
                var response = new ResponseModel<NoteResponse>
                {
                    Success = true,
                    Message = "Note updated successfully",
                    Data = null

                };
                _cache.Remove($"Notes_active_{userId}");
                _cache.Remove($"Notes_archived_{userId}");
                _cache.Remove($"Notes_trash_{userId}");
                _logger.LogInformation("Note updated successfully");
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
        [HttpDelete("{note_id}")]
        public async Task<IActionResult> DeleteNote(int note_id)
        {
            try
            {

                var userIdClaim = User.FindFirstValue("UserId");
                int userId = Convert.ToInt32(userIdClaim);
                await _notesBL.DeleteNote(note_id, userId);
                _cache.Remove($"Notes_active_{userId}");
                _cache.Remove($"Notes_archived_{userId}");
                _cache.Remove($"Notes_trash_{userId}");
                return Ok(new ResponseModel<string>
                {
                    Success = true,
                    Message = "Note deleted successfully",
                    Data = null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = null
                });
            }
        }
    }
}