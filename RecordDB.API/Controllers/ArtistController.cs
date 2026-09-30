using Microsoft.AspNetCore.Mvc;
using RecordDB.Shared.DTOs;
using RecordDB.API.Models;
using RecordDB.API.Repositories;

namespace RecordDB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ArtistController(IArtistRepository artistRepository) : ControllerBase
    {
        private readonly IArtistRepository _artistRepository = artistRepository;

        // -----------------------------------------------------------------------
        // Mapping helpers
        // -----------------------------------------------------------------------

        private static ArtistDto ToDto(Artist a) => new()
        {
            ArtistId  = a.ArtistId,
            FirstName = a.FirstName,
            LastName  = a.LastName,
            Name      = a.Name,
            Biography = a.Biography
        };

        private static Artist FromCreateDto(CreateArtistDto dto) => new()
        {
            FirstName = dto.FirstName,
            LastName  = dto.LastName,
            Biography = dto.Biography
        };

        private static Artist FromUpdateDto(UpdateArtistDto dto) => new()
        {
            ArtistId  = dto.ArtistId,
            FirstName = dto.FirstName,
            LastName  = dto.LastName,
            Name      = dto.Name,
            Biography = dto.Biography
        };

        // -----------------------------------------------------------------------
        // GET endpoints
        // -----------------------------------------------------------------------

        /// <summary>Returns all artists with full details.</summary>
        /// <response code="200">List of artists.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ArtistDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetAll()
        {
            var artists = await _artistRepository.GetArtistsAsync();
            return Ok(artists.Select(ToDto));
        }

        /// <summary>Returns a list of all artists with their IDs and formatted names for dropdown menus.</summary>
        /// <response code="200">List of artists for dropdown menu.</response>
        [HttpGet("artist-list")]
        [HttpGet("list")]
        [ProducesResponseType(typeof(IEnumerable<ArtistDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetArtistList()
        {
            var artists = await _artistRepository.GetArtistListAsync();
            return Ok(artists.Select(ToDto));
        }

        /// <summary>Returns a single artist by ID.</summary>
        /// <param name="id">The artist's primary key.</param>
        /// <response code="200">The requested artist.</response>
        /// <response code="404">Artist not found.</response>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ArtistDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ArtistDto>> GetById(int id)
        {
            var artist = await _artistRepository.SelectAsync(id);
            if (artist is null) return NotFound();
            return Ok(ToDto(artist));
        }

        /// <summary>Returns all artists whose name contains the given partial string.</summary>
        /// <param name="name">Partial name to search for.</param>
        /// <response code="200">Matching artists (may be empty).</response>
        [HttpGet("search/{name}")]
        [ProducesResponseType(typeof(IEnumerable<ArtistDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> Search(string name)
        {
            var artists = await _artistRepository.GetArtistsByPartialNameAsync(name);
            return Ok(artists.Select(ToDto));
        }

        /// <summary>Returns all artists that have no biography.</summary>
        /// <response code="200">Artists without a biography.</response>
        [HttpGet("no-biography")]
        [ProducesResponseType(typeof(IEnumerable<ArtistDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ArtistDto>>> GetWithNoBiography()
        {
            var artists = await _artistRepository.GetArtistsWithNoBiographyAsync();
            return Ok(artists.Select(ToDto));
        }

        /// <summary>Returns a single artist with no biography, matched by name.</summary>
        /// <param name="name">Name to search for.</param>
        /// <response code="200">Matching artist.</response>
        /// <response code="404">No artist found.</response>
        [HttpGet("no-biography/{name}")]
        [ProducesResponseType(typeof(ArtistDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ArtistDto>> GetWithNoBiographyByName(string name)
        {
            var artist = await _artistRepository.GetArtistWithNoBiographyAsync(name);
            if (artist is null) return NotFound();
            return Ok(ToDto(artist));
        }

        /// <summary>Returns the artist associated with a given record.</summary>
        /// <param name="recordId">The record's primary key.</param>
        /// <response code="200">The artist for this record.</response>
        /// <response code="404">No artist found for this record.</response>
        [HttpGet("by-record/{recordId:int}")]
        [ProducesResponseType(typeof(ArtistDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ArtistDto>> GetByRecord(int recordId)
        {
            var artist = await _artistRepository.GetArtistByRecordIdAsync(recordId);
            if (artist is null) return NotFound();
            return Ok(ToDto(artist));
        }

        /// <summary>Returns the biography text for the artist of a given record.</summary>
        /// <param name="recordId">The record's primary key.</param>
        /// <response code="200">Biography text (may be empty string).</response>
        [HttpGet("biography/{recordId:int}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<ActionResult<string>> GetBiography(int recordId)
        {
            var bio = await _artistRepository.GetBiographyAsync(recordId);
            return Ok(bio);
        }

        /// <summary>Returns the ArtistId for a given first/last name pair.</summary>
        /// <param name="firstName">Artist's first name.</param>
        /// <param name="lastName">Artist's last name.</param>
        /// <response code="200">The ArtistId (0 if not found).</response>
        [HttpGet("artist-id")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetArtistId([FromQuery] string firstName, [FromQuery] string lastName)
        {
            var id = await _artistRepository.GetArtistIdAsync(firstName, lastName);
            return Ok(id);
        }

        // -----------------------------------------------------------------------
        // POST — Create
        // -----------------------------------------------------------------------

        /// <summary>Creates a new artist.</summary>
        /// <param name="dto">Artist details.</param>
        /// <response code="201">Artist created. Returns the new ArtistId.</response>
        /// <response code="400">Invalid request body or creation failed.</response>
        [HttpPost]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> Create([FromBody] CreateArtistDto dto)
        {
            var artist = FromCreateDto(dto);
            var newId = await _artistRepository.InsertAsync(artist);

            if (newId <= 0)
                return BadRequest("Artist could not be created. It may already exist.");

            return CreatedAtAction(nameof(GetById), new { id = newId }, newId);
        }

        // -----------------------------------------------------------------------
        // PUT — Update
        // -----------------------------------------------------------------------

        /// <summary>Updates an existing artist.</summary>
        /// <param name="id">Route ID — must match the ArtistId in the body.</param>
        /// <param name="dto">Updated artist fields.</param>
        /// <response code="204">Update successful.</response>
        /// <response code="400">ID mismatch or invalid body.</response>
        /// <response code="404">Artist not found.</response>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateArtistDto dto)
        {
            if (id != dto.ArtistId)
                return BadRequest("Route id does not match body ArtistId.");

            var existing = await _artistRepository.SelectAsync(id);
            if (existing is null) return NotFound();

            var artist = FromUpdateDto(dto);
            await _artistRepository.UpdateArtistAsync(artist);
            return NoContent();
        }

        // -----------------------------------------------------------------------
        // DELETE
        // -----------------------------------------------------------------------

        /// <summary>Deletes an artist by ID.</summary>
        /// <param name="id">The artist's primary key.</param>
        /// <response code="204">Deletion successful.</response>
        /// <response code="404">Artist not found.</response>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _artistRepository.SelectAsync(id);
            if (existing is null) return NotFound();

            await _artistRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
