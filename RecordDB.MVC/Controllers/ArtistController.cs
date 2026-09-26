using Microsoft.AspNetCore.Mvc;
using RecordDB.MVC.Models;
using RecordDB.MVC.Services;
using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Controllers
{
    /// <summary>
    /// MVC controller for Artist CRUD operations.
    /// All data access goes through <see cref="IArtistService"/> (typed HttpClient → RecordDB.API).
    /// </summary>
    public class ArtistController(IArtistService artistService) : Controller
    {
        // -----------------------------------------------------------------------
        // Index — list all artists
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            const int pageSize = 20;

            var all = string.IsNullOrWhiteSpace(search)
                ? (await artistService.GetAllAsync()).ToList()
                : (await artistService.SearchAsync(search.Trim())).ToList();

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Clamp page to valid range
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var vm = new PaginatedViewModel<ArtistDto>
            {
                Items       = all.Skip((page - 1) * pageSize).Take(pageSize),
                SearchTerm  = search,
                CurrentPage = page,
                TotalPages  = totalPages,
                TotalCount  = totalCount,
                PageSize    = pageSize
            };

            return View(vm);
        }

        // -----------------------------------------------------------------------
        // Details — single artist
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Details(int id)
        {
            var artist = await artistService.GetByIdAsync(id);
            if (artist is null) return NotFound();
            return View(artist);
        }

        // -----------------------------------------------------------------------
        // Create
        // -----------------------------------------------------------------------

        public IActionResult Create() => View(new CreateArtistDto());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateArtistDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await artistService.CreateAsync(dto);
            TempData["Success"] = "Artist created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------------------------------------------------------
        // Edit — update existing artist
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Edit(int id)
        {
            var artist = await artistService.GetByIdAsync(id);
            if (artist is null) return NotFound();

            var dto = new UpdateArtistDto
            {
                ArtistId  = artist.ArtistId,
                FirstName = artist.FirstName,
                LastName  = artist.LastName,
                Name      = artist.Name,
                Biography = artist.Biography
            };
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateArtistDto dto)
        {
            if (id != dto.ArtistId) return BadRequest();
            if (!ModelState.IsValid) return View(dto);

            await artistService.UpdateAsync(id, dto);
            TempData["Success"] = "Artist updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------------------------------------------------------
        // Delete — confirm + execute
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Delete(int id)
        {
            var artist = await artistService.GetByIdAsync(id);
            if (artist is null) return NotFound();
            return View(artist);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await artistService.DeleteAsync(id);
            TempData["Success"] = "Artist deleted.";
            return RedirectToAction(nameof(Index));
        }

        // -----------------------------------------------------------------------
        // Search — find artist by FirstName / LastName, then show their records
        // -----------------------------------------------------------------------

        [HttpGet]
        public IActionResult Search()
            => View(new ArtistSearchViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(ArtistSearchViewModel vm)
        {
            // Build the search term from whichever fields the user filled in
            var term = string.Join(" ",
                new[] { vm.FirstName?.Trim(), vm.LastName?.Trim() }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

            if (string.IsNullOrWhiteSpace(term))
            {
                ModelState.AddModelError(string.Empty, "Please enter at least a first name or last name.");
                return View(vm);
            }

            var results = (await artistService.SearchAsync(term)).ToList();

            // Single match — go straight to their records without an extra click
            if (results.Count == 1)
            {
                var artist = results[0];
                return RedirectToAction("ByArtist", "Record", new
                {
                    artistId   = artist.ArtistId,
                    artistName = artist.Name ?? $"{artist.FirstName} {artist.LastName}".Trim()
                });
            }

            // Multiple (or zero) matches — let the user pick
            vm.Results = results;
            vm.Searched = true;
            return View(vm);
        }
    }
}
