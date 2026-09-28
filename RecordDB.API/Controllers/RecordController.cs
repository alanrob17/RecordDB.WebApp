using Microsoft.AspNetCore.Mvc;
using RecordDB.Shared.DTOs;
using RecordDB.API.Models;
using RecordDB.API.Repositories;

namespace RecordDB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RecordController(IRecordRepository recordRepository) : ControllerBase
    {
        private readonly IRecordRepository _recordRepository = recordRepository;

        // -----------------------------------------------------------------------
        // Mapping helpers
        // -----------------------------------------------------------------------

        private static Record FromCreateDto(CreateRecordDto dto) => new()
        {
            ArtistId  = dto.ArtistId,
            Name      = dto.Name,
            Field     = dto.Field,
            Recorded  = dto.Recorded,
            Label     = dto.Label,
            Pressing  = dto.Pressing,
            Rating    = dto.Rating,
            Discs     = dto.Discs,
            Media     = dto.Media,
            Bought    = dto.Bought,
            Cost      = dto.Cost,
            CoverName = dto.CoverName,
            Review    = dto.Review
        };

        private static ArtistRecordDto FromUpdateDto(UpdateRecordDto dto) => new()
        {
            RecordId  = dto.RecordId,
            ArtistId  = dto.ArtistId,
            Name      = dto.Name,
            Field     = dto.Field,
            Recorded  = dto.Recorded,
            Label     = dto.Label,
            Pressing  = dto.Pressing,
            Rating    = dto.Rating,
            Discs     = dto.Discs,
            Media     = dto.Media,
            Bought    = dto.Bought ?? DateTime.MinValue,
            Cost      = dto.Cost,
            Review    = dto.Review
        };

        // -----------------------------------------------------------------------
        // GET — collections
        // -----------------------------------------------------------------------

        /// <summary>Returns all records with artist details.</summary>
        /// <response code="200">List of artist+record projections.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDto>>> GetAll()
        {
            var records = await _recordRepository.SelectAsync();
            return Ok(records);
        }

        /// <summary>Returns all records filtered by the show flag.</summary>
        /// <param name="show">Show flag value (e.g. "Y" or "N").</param>
        /// <response code="200">Matching records.</response>
        [HttpGet("show")]
        [HttpGet("show/{show}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDto>>> GetByShow(string? show = "all")
        {
            var filter = string.IsNullOrWhiteSpace(show) ? "all" : show;
            var records = await _recordRepository.SelectRecordsShowAsync(filter);
            return Ok(records);
        }

        /// <summary>Returns all records by artist name.</summary>
        /// <param name="name">Artist name to search for.</param>
        /// <response code="200">Matching records (may be empty).</response>
        [HttpGet("by-artist/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDto>>> GetByArtistName(string name)
        {
            var records = await _recordRepository.GetRecordsByArtistNameAsync(name);
            return Ok(records);
        }

        /// <summary>Returns all records for a given recording year.</summary>
        /// <param name="year">The four-digit recording year.</param>
        /// <response code="200">Matching records.</response>
        [HttpGet("by-year/{year:int}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDto>>> GetByYear(int year)
        {
            var records = await _recordRepository.GetRecordsByYearAsync(year);
            return Ok(records);
        }

        /// <summary>Returns all records that have a review text.</summary>
        /// <response code="200">Records with reviews.</response>
        [HttpGet("reviews")]
        [ProducesResponseType(typeof(IEnumerable<RecordReviewDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<RecordReviewDto>>> GetReviews()
        {
            var records = await _recordRepository.SelectRecordReviewsAsync();
            return Ok(records);
        }

        /// <summary>Returns all records that are missing a review.</summary>
        /// <response code="200">Records without reviews.</response>
        [HttpGet("no-reviews")]
        [ProducesResponseType(typeof(IEnumerable<MissingReviewDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MissingReviewDto>>> GetMissingReviews()
        {
            var records = await _recordRepository.NoRecordReviewsAsync();
            return Ok(records);
        }

        /// <summary>Returns all records that have no associated tracks.</summary>
        /// <response code="200">Records with no tracks.</response>
        [HttpGet("no-tracks")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscDto>>> GetWithNoTracks()
        {
            var records = await _recordRepository.ListRecordsWithNoTracksAsync();
            return Ok(records);
        }

        /// <summary>Returns records with no tracks for a given artist name.</summary>
        /// <param name="name">Artist name.</param>
        /// <response code="200">Records with no tracks for this artist.</response>
        [HttpGet("no-tracks/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscDto>>> GetArtistRecordsWithNoTracks(string name)
        {
            var records = await _recordRepository.ArtistRecordsWithNoTracksAsync(name);
            return Ok(records);
        }

        /// <summary>Searches for tracks whose name contains the given partial string.</summary>
        /// <param name="name">Partial track name.</param>
        /// <response code="200">Matching track results with record/disc context.</response>
        [HttpGet("tracks/search/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscTrackDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscTrackDto>>> SearchTracks(string name)
        {
            var tracks = await _recordRepository.SelectTracksByPartialNameAsync(name);
            return Ok(tracks);
        }

        /// <summary>Returns all records for a given artist as flat Record objects.</summary>
        /// <param name="artistId">The artist's primary key.</param>
        /// <response code="200">Records for this artist.</response>
        [HttpGet("by-artist-id/{artistId:int}")]
        [ProducesResponseType(typeof(IEnumerable<Record>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Record>>> GetByArtistId(int artistId)
        {
            var records = await _recordRepository.GetArtistRecordsAsync(artistId);
            return Ok(records);
        }

        /// <summary>Returns per-artist total disc count and cost.</summary>
        /// <response code="200">List of artist totals.</response>
        [HttpGet("totals")]
        [ProducesResponseType(typeof(IEnumerable<Total>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Total>>> GetTotals()
        {
            var totals = await _recordRepository.GetTotalCostsAsync();
            return Ok(totals);
        }

        // -----------------------------------------------------------------------
        // GET — single record
        // -----------------------------------------------------------------------

        /// <summary>Returns a single record with artist details by ID.</summary>
        /// <param name="id">The record's primary key.</param>
        /// <response code="200">The requested record.</response>
        /// <response code="404">Record not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ArtistRecordDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ArtistRecordDto>> GetById(int id)
        {
            var record = await _recordRepository.SelectAsync(id);
            if (record is null) return NotFound();
            return Ok(record);
        }

        // -----------------------------------------------------------------------
        // GET — scalar counts
        // -----------------------------------------------------------------------

        /// <summary>Returns the total disc count for the given show flag.</summary>
        /// <param name="show">Show flag (e.g. "Y" or "N").</param>
        /// <response code="200">Disc count as a string.</response>
        [HttpGet("disc-count/{show}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetDiscCount(string show)
        {
            var count = await _recordRepository.CountDiscsAsync(show);
            return Ok(count);
        }

        /// <summary>Returns the number of records for a given artist.</summary>
        /// <param name="artistId">The artist's primary key.</param>
        /// <response code="200">Record count as a string.</response>
        [HttpGet("count/artist/{artistId:int}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetArtistRecordCount(int artistId)
        {
            var count = await _recordRepository.GetArtistNumberOfRecordsAsync(artistId);
            return Ok(count);
        }

        /// <summary>Returns the number of records for a given recording year.</summary>
        /// <param name="year">The four-digit recording year.</param>
        /// <response code="200">Record count as a string.</response>
        [HttpGet("count/year/{year:int}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetYearRecordCount(int year)
        {
            var count = await _recordRepository.GetRecordedYearNumberAsync(year);
            return Ok(count);
        }

        // -----------------------------------------------------------------------
        // POST — Create
        // -----------------------------------------------------------------------

        /// <summary>Creates a new record.</summary>
        /// <param name="dto">Record details.</param>
        /// <response code="201">Record created. Returns the new RecordId.</response>
        /// <response code="400">Invalid request or creation failed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> Create([FromBody] CreateRecordDto dto)
        {
            var record = FromCreateDto(dto);
            var newId = await _recordRepository.InsertAsync(record);

            if (newId <= 0)
                return BadRequest("Record could not be created. It may already exist.");

            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        // -----------------------------------------------------------------------
        // PUT — Update
        // -----------------------------------------------------------------------

        /// <summary>Updates an existing record.</summary>
        /// <param name="id">Route ID — must match the RecordId in the body.</param>
        /// <param name="dto">Updated record fields.</param>
        /// <response code="204">Update successful.</response>
        /// <response code="400">ID mismatch or invalid body.</response>
        /// <response code="404">Record not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateRecordDto dto)
        {
            if (id != dto.RecordId)
                return BadRequest("Route id does not match body RecordId.");

            var existing = await _recordRepository.SelectAsync(id);
            if (existing is null) return NotFound();

            var artistRecord = FromUpdateDto(dto);
            await _recordRepository.UpdateAsync(artistRecord);
            return NoContent();
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        /// <summary>Deletes a record by ID.</summary>
        /// <param name="id">The record's primary key.</param>
        /// <response code="204">Deletion successful.</response>
        /// <response code="404">Record not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _recordRepository.SelectAsync(id);
            if (existing is null) return NotFound();

            await _recordRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
