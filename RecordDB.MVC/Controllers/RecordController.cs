using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RecordDB.MVC.Models;
using RecordDB.MVC.Services;
using RecordDB.Shared.DTOs;

namespace RecordDB.MVC.Controllers
{
    /// <summary>
    /// MVC controller for Record (Album) operations.
    /// All data access goes through <see cref="IRecordService"/> and <see cref="IArtistService"/>.
    /// </summary>
    public class RecordController(IRecordService recordService, IArtistService artistService) : Controller
    {
        // -----------------------------------------------------------------------
        // Helpers
        // -----------------------------------------------------------------------

        private async Task PopulateArtistsDropdownAsync(int? selectedArtistId = null)
        {
            var artists = await artistService.GetAllAsync();
            var selectList = artists
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Value    = a.ArtistId.ToString(),
                    Text     = a.Name ?? $"{a.FirstName} {a.LastName}".Trim(),
                    Selected = selectedArtistId.HasValue && a.ArtistId == selectedArtistId.Value
                })
                .ToList();

            ViewBag.Artists = selectList;
        }

        // -----------------------------------------------------------------------
        // Index — list all records with pagination and search
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Index(int page = 1, string? search = null)
        {
            const int pageSize = 20;

            var all = (await recordService.GetAllAsync()).ToList();

            // Optional search by album name, artist name, or label
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                all = all.Where(r => 
                    (r.Name != null && r.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (r.ArtistName != null && r.ArtistName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Label != null && r.Label.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Field != null && r.Field.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    r.Recorded.ToString().Contains(term)
                ).ToList();
            }

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            // Clamp page to valid range
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var vm = new PaginatedViewModel<ArtistRecordDto>
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
        // Details — single record by ID
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Details(int id)
        {
            var record = await recordService.GetByIdAsync(id);
            if (record is null) return NotFound();
            return View(record);
        }

        // -----------------------------------------------------------------------
        // Create — new record form
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Create(int? artistId = null)
        {
            await PopulateArtistsDropdownAsync(artistId);

            var model = new CreateRecordDto
            {
                ArtistId = artistId ?? 0,
                Recorded = DateTime.Now.Year,
                Discs    = 1,
                Media    = "CD"
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRecordDto dto)
        {
            if (!ModelState.IsValid)
            {
                await PopulateArtistsDropdownAsync(dto.ArtistId);
                return View(dto);
            }

            var newId = await recordService.CreateAsync(dto);
            TempData["Success"] = $"Record \"{dto.Name}\" was created successfully.";
            return RedirectToAction(nameof(Details), new { id = newId });
        }

        // -----------------------------------------------------------------------
        // Edit — update existing record
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Edit(int id)
        {
            var record = await recordService.GetByIdAsync(id);
            if (record is null) return NotFound();

            await PopulateArtistsDropdownAsync(record.ArtistId);

            var dto = new UpdateRecordDto
            {
                RecordId  = record.RecordId,
                ArtistId  = record.ArtistId,
                Name      = record.Name ?? string.Empty,
                Field     = record.Field,
                Recorded  = record.Recorded,
                Label     = record.Label,
                Pressing  = record.Pressing,
                Rating    = record.Rating,
                Discs     = record.Discs,
                Media     = record.Media,
                Bought    = record.Bought == DateTime.MinValue ? null : record.Bought,
                Cost      = record.Cost,
                CoverName = record.CoverName,
                Review    = record.Review
            };

            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateRecordDto dto)
        {
            if (id != dto.RecordId) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateArtistsDropdownAsync(dto.ArtistId);
                return View(dto);
            }

            await recordService.UpdateAsync(id, dto);
            TempData["Success"] = $"Record \"{dto.Name}\" was updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // -----------------------------------------------------------------------
        // Delete — confirmation & execution
        // -----------------------------------------------------------------------

        public async Task<IActionResult> Delete(int id)
        {
            var record = await recordService.GetByIdAsync(id);
            if (record is null) return NotFound();
            return View(record);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await recordService.DeleteAsync(id);
            TempData["Success"] = "Record deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
