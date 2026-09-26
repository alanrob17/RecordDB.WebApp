using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordDB.MVC.Models;
using RecordDB.MVC.Services;
using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Controllers
{
    /// <summary>
    /// MVC controller for Track CRUD operations.
    /// All data access goes through <see cref="ITrackService"/> (typed HttpClient → RecordDB.API).
    /// Disc select lists are populated via <see cref="IDiscService"/>.
    /// </summary>
    public class TrackController(ITrackService trackService, IDiscService discService) : Controller
    {
        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        /// <summary>Formats a raw track length in seconds to m:ss display.</summary>
        public static string FormatLength(int? seconds)
        {
            if (seconds is null or <= 0) return "—";
            var ts = TimeSpan.FromSeconds(seconds.Value);
            return ts.Hours > 0
                ? $"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}"
                : $"{ts.Minutes}:{ts.Seconds:D2}";
        }

        /// <summary>Populates ViewBag.Discs with a sorted dropdown of all discs.</summary>
        private async Task PopulateDiscsDropdownAsync(int? selectedDiscId = null)
        {
            var discs = await discService.GetAllAsync();
            ViewBag.Discs = discs
                .OrderBy(d => d.ArtistName)
                .ThenBy(d => d.Name)
                .ThenBy(d => d.DiscNo)
                .Select(d => new SelectListItem
                {
                    Value    = d.DiscId.ToString(),
                    Text     = $"{d.ArtistName} – {d.Name} (Disc {d.DiscNo})",
                    Selected = selectedDiscId.HasValue && d.DiscId == selectedDiscId.Value
                })
                .ToList();
        }

        // -----------------------------------------------------------------------
        // Index — list all tracks with pagination and search
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            const int pageSize = 25;

            var all = string.IsNullOrWhiteSpace(search)
                ? (await trackService.GetAllAsync()).ToList()
                : (await trackService.SearchByNameAsync(search.Trim())).ToList();

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var vm = new PaginatedViewModel<ArtistRecordDiscTrackDto>
            {
                Items       = all.Skip((page - 1) * pageSize).Take(pageSize),
                SearchTerm  = search,
                CurrentPage = page,
                TotalPages  = totalPages,
                TotalCount  = totalCount,
                PageSize    = pageSize
            };

            ViewBag.FormatLength = (Func<int?, string>)FormatLength;
            return View(vm);
        }

        // -----------------------------------------------------------------------
        // Details — single track
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Details(int id)
        {
            var track = await trackService.GetByIdAsync(id);
            if (track is null) return NotFound();
            ViewBag.FormattedLength = FormatLength(track.TrackLength);
            return View(track);
        }

        // -----------------------------------------------------------------------
        // Create — new track
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Create(int? discId = null)
        {
            await PopulateDiscsDropdownAsync(discId);
            var model = new CreateTrackDto
            {
                DiscId  = discId ?? 0,
                TrackNo = 1
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrackDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDiscsDropdownAsync(dto.DiscId);
                return View(dto);
            }

            var newId = await trackService.CreateAsync(dto);
            TempData["Success"] = $"Track \"{dto.Name}\" created successfully.";
            return RedirectToAction(nameof(Details), new { id = newId });
        }

        // -----------------------------------------------------------------------
        // Edit — update existing track
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Edit(int id)
        {
            var track = await trackService.GetByIdAsync(id);
            if (track is null) return NotFound();

            // TrackId and DiscId are int? on the read DTO — guard against null
            if (track.TrackId is null) return NotFound();

            await PopulateDiscsDropdownAsync(track.DiscId);

            var dto = new UpdateTrackDto
            {
                TrackId     = track.TrackId.Value,
                DiscId      = track.DiscId,
                TrackNo     = track.TrackNo ?? 1,
                Name        = track.TrackName,
                TrackLength = track.TrackLength,
                Extended    = track.Extended
            };

            ViewBag.AlbumName  = track.Name;
            ViewBag.ArtistName = track.ArtistName;
            ViewBag.DiscNo     = track.DiscNo;
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTrackDto dto)
        {
            if (id != dto.TrackId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateDiscsDropdownAsync(dto.DiscId);
                return View(dto);
            }

            await trackService.UpdateAsync(id, dto);
            TempData["Success"] = $"Track \"{dto.Name}\" updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // -----------------------------------------------------------------------
        // Delete — confirm + execute
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Delete(int id)
        {
            var track = await trackService.GetByIdAsync(id);
            if (track is null) return NotFound();
            ViewBag.FormattedLength = FormatLength(track.TrackLength);
            return View(track);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await trackService.DeleteAsync(id);
            TempData["Success"] = "Track deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
