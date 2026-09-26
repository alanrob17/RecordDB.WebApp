using Microsoft.AspNetCore.Mvc;
using RecordDB.Shared.DTOs;
using RecordDB.API.Models;
using RecordDB.API.Repositories;

namespace RecordDB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class TrackController(ITrackRepository trackRepository) : ControllerBase
    {
        private readonly ITrackRepository _trackRepository = trackRepository;

        // -----------------------------------------------------------------------
        // Mapping helpers
        // -----------------------------------------------------------------------

        private static Track FromCreateDto(CreateTrackDto dto) => new()
        {
            DiscId      = dto.DiscId,
            TrackNo     = dto.TrackNo,
            Name        = dto.Name,
            TrackLength = dto.TrackLength,
            Extended    = dto.Extended
        };

        private static Track FromUpdateDto(UpdateTrackDto dto) => new()
        {
            TrackId     = dto.TrackId,
            DiscId      = dto.DiscId,
            TrackNo     = dto.TrackNo,
            Name        = dto.Name,
            TrackLength = dto.TrackLength,
            Extended    = dto.Extended
        };

        // -----------------------------------------------------------------------
        // GET endpoints
        // -----------------------------------------------------------------------

        /// <summary>Returns all tracks with artist, record, and disc context.</summary>
        /// <response code="200">List of track entities.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscTrackDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscTrackDto>>> GetAll()
        {
            var tracks = await _trackRepository.SelectAllTrackEntitiesAsync();
            return Ok(tracks);
        }

        /// <summary>Returns a single track with context by track ID.</summary>
        /// <param name="id">The track's primary key.</param>
        /// <response code="200">The requested track.</response>
        /// <response code="404">Track not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ArtistRecordDiscTrackDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ArtistRecordDiscTrackDto>> GetById(int id)
        {
            var track = await _trackRepository.SelectTrackByIdAsync(id);
            if (track is null) return NotFound();
            return Ok(track);
        }

        /// <summary>Searches for tracks matching a partial track name.</summary>
        /// <param name="name">Partial track name to search for.</param>
        /// <response code="200">Matching track entities (may be empty).</response>
        [HttpGet("search/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscTrackDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscTrackDto>>> Search(string name)
        {
            var tracks = await _trackRepository.SelectTracksByPartialNameAsync(name);
            return Ok(tracks);
        }

        /// <summary>Returns tracks for records by artist name.</summary>
        /// <param name="name">Artist name to search for.</param>
        /// <response code="200">Matching track entities.</response>
        [HttpGet("by-artist/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscTrackDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscTrackDto>>> GetByArtist(string name)
        {
            var tracks = await _trackRepository.SelectArtistRecordTracksAsync(name);
            return Ok(tracks);
        }

        /// <summary>Returns tracks for a specific record by record name.</summary>
        /// <param name="name">Record name to search for.</param>
        /// <response code="200">Matching track entities.</response>
        [HttpGet("by-record/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscTrackDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscTrackDto>>> GetByRecord(string name)
        {
            var tracks = await _trackRepository.SelectTracksByRecordAsync(name);
            return Ok(tracks);
        }

        /// <summary>Returns the total number of tracks for a given record ID.</summary>
        /// <param name="recordId">The record's primary key.</param>
        /// <response code="200">Track count.</response>
        [HttpGet("count/record/{recordId:int}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetTrackCountByRecord(int recordId)
        {
            var count = await _trackRepository.GetTrackNumberAsync(recordId);
            return Ok(count);
        }

        /// <summary>Checks whether a disc has existing tracks and returns the count.</summary>
        /// <param name="discId">The disc ID to check.</param>
        /// <response code="200">Number of tracks on the disc (0 if none).</response>
        [HttpGet("check/disc/{discId:int}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> CheckForTracks(int discId)
        {
            var count = await _trackRepository.CheckForTracksAsync(discId);
            return Ok(count);
        }

        // -----------------------------------------------------------------------
        // POST — Create
        // -----------------------------------------------------------------------

        /// <summary>Creates a single new track.</summary>
        /// <param name="dto">Track details.</param>
        /// <response code="201">Track created. Returns the new TrackId.</response>
        /// <response code="400">Invalid request or creation failed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> Create([FromBody] CreateTrackDto dto)
        {
            var track = FromCreateDto(dto);
            var newId = await _trackRepository.InsertTrackAsync(track);

            if (newId <= 0)
                return BadRequest("Track could not be created.");

            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        /// <summary>Bulk-inserts multiple tracks for a disc using a Table-Valued Parameter.</summary>
        /// <param name="dtos">List of tracks to insert.</param>
        /// <response code="204">Tracks inserted successfully.</response>
        /// <response code="400">Empty list or invalid request.</response>
        [HttpPost("bulk")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> BulkCreate([FromBody] IEnumerable<CreateTrackDto> dtos)
        {
            var trackList = dtos?.Select(FromCreateDto).ToList();
            if (trackList is null || trackList.Count == 0)
                return BadRequest("Track collection cannot be empty.");

            await _trackRepository.BulkInsertTracksAsync(trackList);
            return NoContent();
        }

        // -----------------------------------------------------------------------
        // PUT — Update
        // -----------------------------------------------------------------------

        /// <summary>Updates an existing track.</summary>
        /// <param name="id">Route ID — must match the TrackId in the body.</param>
        /// <param name="dto">Updated track fields.</param>
        /// <response code="204">Update successful.</response>
        /// <response code="400">ID mismatch or invalid body.</response>
        /// <response code="404">Track not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTrackDto dto)
        {
            if (id != dto.TrackId)
                return BadRequest("Route id does not match body TrackId.");

            var existing = await _trackRepository.SelectTrackByIdAsync(id);
            if (existing is null) return NotFound();

            var track = FromUpdateDto(dto);
            await _trackRepository.UpdateTrackAsync(track);
            return NoContent();
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        /// <summary>Deletes a track by ID.</summary>
        /// <param name="id">The track's primary key.</param>
        /// <response code="204">Deletion successful.</response>
        /// <response code="404">Track not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _trackRepository.SelectTrackByIdAsync(id);
            if (existing is null) return NotFound();

            await _trackRepository.DeleteTrackAsync(id);
            return NoContent();
        }
    }
}
