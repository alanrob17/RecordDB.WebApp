using Microsoft.AspNetCore.Mvc;
using RecordDB.Shared.DTOs;
using RecordDB.API.Models;
using RecordDB.API.Repositories;

namespace RecordDB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DiscController(IDiscRepository discRepository) : ControllerBase
    {
        private readonly IDiscRepository _discRepository = discRepository;

        // -----------------------------------------------------------------------
        // Mapping helpers
        // -----------------------------------------------------------------------

        private static Disc FromCreateDto(CreateDiscDto dto) => new()
        {
            RecordId     = dto.RecordId,
            DiscNo       = dto.DiscNo,
            FreeDbDiscId = dto.FreeDbDiscId,
            FreeDbId     = dto.FreeDbId,
            Length       = dto.Length
        };

        private static Disc FromUpdateDto(UpdateDiscDto dto) => new()
        {
            DiscId       = dto.DiscId,
            DiscNo       = dto.DiscNo,
            FreeDbDiscId = dto.FreeDbDiscId,
            FreeDbId     = dto.FreeDbId,
            Length       = dto.Length
        };

        // -----------------------------------------------------------------------
        // GET endpoints
        // -----------------------------------------------------------------------

        /// <summary>Returns all discs with artist and record context.</summary>
        /// <response code="200">List of disc entities.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscDto>>> GetAll()
        {
            var discs = await _discRepository.SelectAllDiscEntitiesAsync();
            return Ok(discs);
        }

        /// <summary>Returns a single disc with artist and record context by disc ID.</summary>
        /// <param name="id">The disc's primary key.</param>
        /// <response code="200">The requested disc.</response>
        /// <response code="404">Disc not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ArtistRecordDiscDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ArtistRecordDiscDto>> GetById(int id)
        {
            var disc = await _discRepository.SelectSingleDiscAsync(id);
            if (disc is null) return NotFound();
            return Ok(disc);
        }

        /// <summary>Returns all discs for records whose name contains the given string.</summary>
        /// <param name="name">Partial record name to search for.</param>
        /// <response code="200">Matching disc entities (may be empty).</response>
        [HttpGet("search/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistRecordDiscDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistRecordDiscDto>>> SearchByRecordName(string name)
        {
            var discs = await _discRepository.GetDiscRecordsByRecordNameAsync(name);
            return Ok(discs);
        }

        // -----------------------------------------------------------------------
        // POST — Create
        // -----------------------------------------------------------------------

        /// <summary>Creates a new disc.</summary>
        /// <param name="dto">Disc details.</param>
        /// <response code="201">Disc created. Returns the new DiscId.</response>
        /// <response code="400">Invalid request or creation failed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> Create([FromBody] CreateDiscDto dto)
        {
            var disc = FromCreateDto(dto);
            var newId = await _discRepository.InsertDiscAsync(disc);

            if (newId <= 0)
                return BadRequest("Disc could not be created.");

            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        // -----------------------------------------------------------------------
        // PUT — Update
        // -----------------------------------------------------------------------

        /// <summary>Updates an existing disc.</summary>
        /// <param name="id">Route ID — must match the DiscId in the body.</param>
        /// <param name="dto">Updated disc fields.</param>
        /// <response code="204">Update successful.</response>
        /// <response code="400">ID mismatch or invalid body.</response>
        /// <response code="404">Disc not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDiscDto dto)
        {
            if (id != dto.DiscId)
                return BadRequest("Route id does not match body DiscId.");

            var existing = await _discRepository.SelectSingleDiscAsync(id);
            if (existing is null) return NotFound();

            var disc = FromUpdateDto(dto);
            await _discRepository.UpdateDiscAsync(disc);
            return NoContent();
        }

        // -----------------------------------------------------------------------
        // PATCH — Update length only
        // -----------------------------------------------------------------------

        /// <summary>Updates only the length of a disc.</summary>
        /// <param name="id">The disc's primary key.</param>
        /// <param name="dto">The new length value (null to clear).</param>
        /// <response code="204">Update successful.</response>
        /// <response code="404">Disc not found.</response>
        [HttpPatch("{id:int}/length")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateLength(int id, [FromBody] UpdateDiscLengthDto dto)
        {
            var existing = await _discRepository.SelectSingleDiscAsync(id);
            if (existing is null) return NotFound();

            await _discRepository.UpdateDiscLengthAsync(id, dto.Length);
            return NoContent();
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        /// <summary>Deletes a disc by ID.</summary>
        /// <param name="id">The disc's primary key.</param>
        /// <response code="204">Deletion successful.</response>
        /// <response code="404">Disc not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _discRepository.SelectSingleDiscAsync(id);
            if (existing is null) return NotFound();

            await _discRepository.DeleteDiscAsync(id);
            return NoContent();
        }
    }
}
