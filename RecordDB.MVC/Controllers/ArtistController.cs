using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        // EditArtist — update artist with dropdown selector
        // -----------------------------------------------------------------------

        private async Task PopulateArtistListDropdownAsync(int? selectedArtistId = null)
        {
            var artists = (await artistService.GetArtistListAsync()).ToList();

            var selectList = artists.Select(a =>
            {
                var isPlaceholder = a.ArtistId == 0;
                var text = isPlaceholder
                    ? "-- Select an Artist to Edit --"
                    : (a.Name ?? $"{a.LastName}, {a.FirstName}".Trim());

                return new SelectListItem
                {
                    Value    = a.ArtistId.ToString(),
                    Text     = text,
                    Selected = selectedArtistId.HasValue && a.ArtistId == selectedArtistId.Value
                };
            }).ToList();

            ViewBag.ArtistList = selectList;
        }

        [HttpGet]
        public async Task<IActionResult> EditArtist(int? id = null)
        {
            await PopulateArtistListDropdownAsync(id);

            if (!id.HasValue || id.Value <= 0)
            {
                return View(new UpdateArtistDto());
            }

            var artist = await artistService.GetByIdAsync(id.Value);
            if (artist is null)
            {
                TempData["Error"] = $"Artist with ID #{id.Value} not found.";
                return RedirectToAction(nameof(EditArtist));
            }

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
        public async Task<IActionResult> EditArtist(UpdateArtistDto dto)
        {
            if (dto.ArtistId <= 0)
            {
                ModelState.AddModelError(string.Empty, "Please select an artist to edit.");
                await PopulateArtistListDropdownAsync(null);
                return View(dto);
            }

            if (!ModelState.IsValid)
            {
                await PopulateArtistListDropdownAsync(dto.ArtistId);
                return View(dto);
            }

            try
            {
                await artistService.UpdateAsync(dto.ArtistId, dto);
                TempData["Success"] = $"Artist '{dto.Name}' updated successfully.";
                return RedirectToAction(nameof(EditArtist), new { id = dto.ArtistId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Failed to update artist: {ex.Message}");
                await PopulateArtistListDropdownAsync(dto.ArtistId);
                return View(dto);
            }
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

        // -----------------------------------------------------------------------
        // NoBiography — list artists who don't have a biography
        // -----------------------------------------------------------------------

        [HttpGet]
        [Route("Artist/NoBiography")]
        [Route("NoBiography")]
        public async Task<IActionResult> NoBiography(int page = 1, string? search = null)
        {
            const int pageSize = 20;

            // Calls api/artist/no-biography -> ArtistRepository.GetArtistsWithNoBiographyAsync() -> up_SelectArtistsWithNoBiography
            var all = (await artistService.GetWithNoBiographyAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                all = all.Where(a =>
                    (a.Name != null && a.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (a.FirstName != null && a.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (a.LastName != null && a.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

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

            return View("~/Views/Artist/NoBiography.cshtml", vm);
        }
    }
}
