namespace RecordDB.MVC.Models
{
    /// <summary>
    /// Wraps a page of items with all the metadata needed to render
    /// a block-based pagination control (e.g. 20 items/page, 15 buttons/block).
    /// </summary>
    public class PaginatedViewModel<T>
    {
        // -----------------------------------------------------------------------
        // Data
        // -----------------------------------------------------------------------

        /// <summary>The items on the current page.</summary>
        public IEnumerable<T> Items { get; init; } = [];

        /// <summary>Optional search term used to filter items.</summary>
        public string? SearchTerm { get; init; }

        // -----------------------------------------------------------------------
        // Page metadata
        // -----------------------------------------------------------------------

        public int CurrentPage  { get; init; }
        public int TotalPages   { get; init; }
        public int TotalCount   { get; init; }
        public int PageSize     { get; init; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage     => CurrentPage < TotalPages;

        // -----------------------------------------------------------------------
        // Block metadata (sliding window of page buttons)
        // -----------------------------------------------------------------------

        /// <summary>Number of page-number buttons shown at once.</summary>
        public int BlockSize => 15;

        /// <summary>Which block of page buttons we're currently in (1-based).</summary>
        public int CurrentBlock => (int)Math.Ceiling((double)CurrentPage / BlockSize);

        /// <summary>Total number of blocks.</summary>
        public int TotalBlocks  => (int)Math.Ceiling((double)TotalPages  / BlockSize);

        /// <summary>First page number shown in the current block.</summary>
        public int BlockStart   => (CurrentBlock - 1) * BlockSize + 1;

        /// <summary>Last page number shown in the current block.</summary>
        public int BlockEnd     => Math.Min(CurrentBlock * BlockSize, TotalPages);

        /// <summary>First page of the previous block (for the ‹ Prev 15 button).</summary>
        public int PreviousBlockFirstPage => (CurrentBlock - 2) * BlockSize + 1;

        /// <summary>First page of the next block (for the Next 15 › button).</summary>
        public int NextBlockFirstPage => CurrentBlock * BlockSize + 1;

        public bool HasPreviousBlock => CurrentBlock > 1;
        public bool HasNextBlock     => CurrentBlock < TotalBlocks;
    }
}
