using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordDB.MVC.Models;
using RecordDB.MVC.Services;
using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Controllers
{
    /// <summary>
    /// MVC controller for Disc CRUD operations.
    /// All data access goes through <see cref="IDiscService"/> (typed HttpClient → RecordDB.API).
    /// The artist/record select lists are populated via <see cref="IRecordService"/>.
    /// </summary>
    public class DiscController(IDiscService discService, IRecordService recordService) : Controller
    {
        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        /// <summary>Populates ViewBag.Records with a sorted dropdown of all albums.</summary>
        private async Task PopulateRecordsDropdownAsync(int? selectedRecordId = null)
        {
            var records = await recordService.GetAllAsync();
            ViewBag.Records = records
                .OrderBy(r => r.ArtistName)
                .ThenBy(r => r.Name)
                .Select(r => new SelectListItem
                {
                    Value    = r.RecordId.ToString(),
                    Text     = $"{r.ArtistName} – {r.Name}",
                    Selected = selectedRecordId.HasValue && r.RecordId == selectedRecordId.Value
                })
                .ToList();
        }

        /// <summary>Formats a raw length in seconds to mm:ss display string.</summary>
        private static string FormatLength(int? seconds)
        {
            if (seconds is null or <= 0) return "—";
            var ts = TimeSpan.FromSeconds(seconds.Value);
            return ts.Hours > 0
                ? $"{(int)ts.TotalHours}:{ts.Minutes:D2}:{ts.Seconds:D2}"
                : $"{ts.Minutes}:{ts.Seconds:D2}";
        }

        // -----------------------------------------------------------------------
        // Index — list all discs with pagination and search
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            const int pageSize = 20;

            var all = string.IsNullOrWhiteSpace(search)
                ? (await discService.GetAllAsync()).ToList()
                : (await discService.SearchByRecordNameAsync(search.Trim())).ToList();

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var vm = new PaginatedViewModel<ArtistRecordDiscDto>
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
        // Details — single disc
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Details(int id)
        {
            var disc = await discService.GetByIdAsync(id);
            if (disc is null) return NotFound();
            ViewBag.FormattedLength = FormatLength(disc.Length);
            return View(disc);
        }

        // -----------------------------------------------------------------------
        // Create — new disc
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Create(int? recordId = null)
        {
            await PopulateRecordsDropdownAsync(recordId);
            var model = new CreateDiscDto
            {
                RecordId = recordId ?? 0,
                DiscNo   = 1
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDiscDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRecordsDropdownAsync(dto.RecordId);
                return View(dto);
            }

            var newId = await discService.CreateAsync(dto);
            TempData["Success"] = $"Disc #{dto.DiscNo} created successfully.";
            return RedirectToAction(nameof(Details), new { id = newId });
        }

        // -----------------------------------------------------------------------
        // Edit — update existing disc
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Edit(int id)
        {
            var disc = await discService.GetByIdAsync(id);
            if (disc is null) return NotFound();

            await PopulateRecordsDropdownAsync(disc.RecordId);

            var dto = new UpdateDiscDto
            {
                DiscId       = disc.DiscId,
                DiscNo       = disc.DiscNo,
                FreeDbDiscId = disc.FreeDbDiscId,
                FreeDbId     = disc.FreeDbId,
                Length       = disc.Length
            };

            ViewBag.RecordName = disc.Name;
            ViewBag.ArtistName = disc.ArtistName;
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateDiscDto dto)
        {
            if (id != dto.DiscId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateRecordsDropdownAsync();
                return View(dto);
            }

            await discService.UpdateAsync(id, dto);
            TempData["Success"] = $"Disc #{dto.DiscNo} updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // -----------------------------------------------------------------------
        // Delete — confirm + execute
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Delete(int id)
        {
            var disc = await discService.GetByIdAsync(id);
            if (disc is null) return NotFound();
            ViewBag.FormattedLength = FormatLength(disc.Length);
            return View(disc);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await discService.DeleteAsync(id);
            TempData["Success"] = "Disc deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
