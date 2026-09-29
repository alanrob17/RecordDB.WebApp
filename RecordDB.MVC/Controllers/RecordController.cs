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
    public class RecordController(IRecordService recordService, IArtistService artistService, ITrackService trackService) : Controller
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

            // Optional search by album name, artist name, or recorded year
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                all = all.Where(r => 
                    (r.Name != null && r.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (r.ArtistName != null && r.ArtistName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Recorded > 0 && r.Recorded.ToString().Contains(term))
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
        // ByArtist — records for one specific artist (used by Artist/Search results)
        // -----------------------------------------------------------------------

        public async Task<IActionResult> ByArtist(int artistId, string artistName, int page = 1)
        {
            const int pageSize = 20;

            // Use the dedicated API endpoint — much faster than fetching all and filtering
            var all = (await recordService.GetByArtistNameAsync(artistName)).ToList();

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var vm = new PaginatedViewModel<ArtistRecordDto>
            {
                Items       = all.Skip((page - 1) * pageSize).Take(pageSize),
                SearchTerm  = artistName,           // re-used so Index view shows the filter badge
                CurrentPage = page,
                TotalPages  = totalPages,
                TotalCount  = totalCount,
                PageSize    = pageSize
            };

            // Pass the artist identity so the Index view can display a better heading
            ViewBag.ByArtistId   = artistId;
            ViewBag.ByArtistName = artistName;

            return View("Index", vm);
        }

        // -----------------------------------------------------------------------
        // GetRecords — specialized record list by show filter or artist
        // -----------------------------------------------------------------------

        [HttpGet]
        [Route("Record/GetRecords/{id?}")]
        [Route("Records/GetRecords/{id?}")]
        [Route("GetRecords/{id?}")]
        public async Task<IActionResult> GetRecords(string? id, string? show, int? artistId, int page = 1)
        {
            const int pageSize = 20;
            List<ArtistRecordDto> all;
            string headerTitle;

            if (artistId.HasValue && artistId.Value > 0)
            {
                // Task<Artist?> SelectAsync(int artistId) via up_ArtistSelectById
                var artist = await artistService.GetByIdAsync(artistId.Value);
                if (artist is null) return NotFound();

                var artistName = artist.Name ?? $"{artist.FirstName} {artist.LastName}".Trim();
                all = (await recordService.GetByArtistNameAsync(artistName)).ToList();
                headerTitle = $"Records by {artistName}";

                ViewBag.ArtistId   = artistId.Value;
                ViewBag.ArtistName = artistName;
            }
            else
            {
                // Task<List<ArtistRecordDto>> SelectRecordsShowAsync(string show) via up_RecordSelectShowCore
                var showParam = !string.IsNullOrWhiteSpace(id) 
                    ? id.Trim() 
                    : (!string.IsNullOrWhiteSpace(show) ? show.Trim() : "all");

                all = (await recordService.GetRecordsShowAsync(showParam)).ToList();
                headerTitle = GetHeaderTitle(showParam);

                ViewBag.Show = showParam;
            }

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var vm = new PaginatedViewModel<ArtistRecordDto>
            {
                Items       = all.Skip((page - 1) * pageSize).Take(pageSize),
                CurrentPage = page,
                TotalPages  = totalPages,
                TotalCount  = totalCount,
                PageSize    = pageSize
            };

            ViewBag.HeaderTitle = headerTitle;
            return View("~/Views/Record/GetRecords.cshtml", vm);
        }

        private static string GetHeaderTitle(string show)
        {
            return show.ToLowerInvariant() switch
            {
                "all"            => "All Records and CD's",
                "cd"             => "All CD's",
                "records"        => "All Records",
                "dvds"           => "All DVD's",
                "blurays"        => "All Blurays",
                "2022"           => "All Records bought in 2022",
                "2021"           => "All Records bought in 2021",
                "2020"           => "All Records bought in 2020",
                "2019"           => "All Records bought in 2019",
                "2018"           => "All Records bought in 2018",
                "2017"           => "All Records bought in 2017",
                "1111"           => "Indispensible Records",
                "rock"           => "Rock Albums",
                "blues"          => "Blues Albums",
                "jazz"           => "Jazz Albums",
                "classical"      => "Classical Albums",
                "soundtrack"     => "Soundtrack Albums",
                "country"        => "Country Albums",
                "rockdesc"       => "Rock Albums by date",
                "bluesdesc"      => "Blues Albums by date",
                "jazzdesc"       => "Jazz Albums by date",
                "classicaldesc"  => "Classical Albums by date",
                "soundtrackdesc" => "Soundtrack Albums by date",
                "countrydesc"    => "Country Albums by date",
                _ when int.TryParse(show, out var year) && year >= 1900 && year <= 2100 => $"All Records bought in {year}",
                _ => $"{show} Records"
            };
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
        // RecordView — rich showcase page for a record with tracks, bio, and review
        // -----------------------------------------------------------------------

        public async Task<IActionResult> RecordView(int id)
        {
            // 1. Fetch record using RecordRepository.SelectAsync(id) -> up_RecordSelectByIdCore
            var record = await recordService.GetByIdAsync(id);
            if (record is null) return NotFound();

            // 2. Ensure Artist Biography is loaded if not already present in the projection
            if (string.IsNullOrWhiteSpace(record.Biography) && record.ArtistId > 0)
            {
                var artist = await artistService.GetByIdAsync(record.ArtistId);
                if (artist != null && !string.IsNullOrWhiteSpace(artist.Biography))
                {
                    record.Biography = artist.Biography;
                }
            }

            // 3. Fetch tracks using TrackRepository.SelectTracksByRecordAsync(name) -> up_GetArtistRecordTracks
            var tracks = !string.IsNullOrWhiteSpace(record.Name)
                ? (await trackService.GetByRecordAsync(record.Name)).ToList()
                : [];

            var vm = new RecordShowViewModel
            {
                Record = record,
                Tracks = tracks
            };

            return View("~/Views/Record/RecordView.cshtml", vm);
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

        // -----------------------------------------------------------------------
        // Search — filtered search by partial Record.Name
        // -----------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Search(string? recordName = null, int? selectedRecordId = null)
        {
            var vm = new RecordSearchViewModel
            {
                RecordName = recordName?.Trim(),
                SelectedRecordId = selectedRecordId
            };

            if (!string.IsNullOrWhiteSpace(recordName))
            {
                var term = recordName.Trim();
                var all = await recordService.GetAllAsync(); // up_RecordSelectAll
                vm.Results = all
                    .Where(r => r.Name != null && r.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(r => r.Name)
                    .ToList();
                vm.Searched = true;
            }

            if (selectedRecordId.HasValue && selectedRecordId.Value > 0)
            {
                vm.SelectedRecord = await recordService.GetByIdAsync(selectedRecordId.Value); // up_RecordSelectByIdCore
            }

            return View("~/Views/Record/Search.cshtml", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(RecordSearchViewModel vm, string? actionType = null)
        {
            // If the user clicked "View Selected Record", navigate directly to RecordView
            if (actionType == "view" && vm.SelectedRecordId.HasValue && vm.SelectedRecordId.Value > 0)
            {
                return RedirectToAction(nameof(RecordView), new { id = vm.SelectedRecordId.Value });
            }

            // Otherwise, performing a search
            if (string.IsNullOrWhiteSpace(vm.RecordName))
            {
                ModelState.AddModelError(nameof(vm.RecordName), "Please enter a partial record name to search.");
                return View("~/Views/Record/Search.cshtml", vm);
            }

            var searchTerm = vm.RecordName.Trim();
            var allRecords = await recordService.GetAllAsync(); // up_RecordSelectAll
            vm.Results = allRecords
                .Where(r => r.Name != null && r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(r => r.Name)
                .ToList();
            vm.Searched = true;

            // Clear previous selection when a new search is performed
            vm.SelectedRecordId = null;
            vm.SelectedRecord = null;

            return View("~/Views/Record/Search.cshtml", vm);
        }

        // -----------------------------------------------------------------------
        // Recorded — search and list records by recording year
        // -----------------------------------------------------------------------

        [HttpGet]
        [Route("Record/Recorded")]
        [Route("Records/Recorded")]
        [Route("Recorded")]
        public async Task<IActionResult> Recorded(string? year, int page = 1)
        {
            const int pageSize = 20;
            List<ArtistRecordDto> records = [];
            int? parsedYear = null;

            if (!string.IsNullOrWhiteSpace(year) && int.TryParse(year.Trim(), out var y) && y > 0)
            {
                parsedYear = y;
                // Task<List<ArtistRecordDto>> GetRecordsByYearAsync(int recorded) via up_GetRecordsByYear
                var result = await recordService.GetByYearAsync(y);
                records = result.ToList();
            }

            var totalCount = records.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

            var vm = new PaginatedViewModel<ArtistRecordDto>
            {
                Items       = records.Skip((page - 1) * pageSize).Take(pageSize),
                SearchTerm  = parsedYear?.ToString() ?? year?.Trim(),
                CurrentPage = page,
                TotalPages  = totalPages,
                TotalCount  = totalCount,
                PageSize    = pageSize
            };

            ViewBag.Year = parsedYear;
            ViewBag.Searched = !string.IsNullOrWhiteSpace(year);

            return View("~/Views/Record/Recorded.cshtml", vm);
        }

        // -----------------------------------------------------------------------
        // RecordReviews — list all records with reviews, displaying Artist.Name, Record.Name, Record.Review
        // -----------------------------------------------------------------------

        [HttpGet]
        [Route("Record/RecordReviews")]
        [Route("Records/RecordReviews")]
        [Route("RecordReviews")]
        public async Task<IActionResult> RecordReviews(int page = 1, string? search = null)
        {
            const int pageSize = 20;

            // Calls api/record/record-reviews -> RecordRepository.SelectRecordReviews() -> up_SelectRecordReviews2
            var all = (await recordService.GetRecordReviewsAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                all = all.Where(r =>
                    (r.Name != null && r.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (r.ArtistName != null && r.ArtistName.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (r.Review != null && r.Review.Contains(term, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            var totalCount = all.Count;
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

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

            return View("~/Views/Record/RecordReviews.cshtml", vm);
        }
    }
}
