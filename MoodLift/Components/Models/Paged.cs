namespace MoodLift.Components.Models
{
    /// <summary>
    /// Represents a single page of items from a larger collection, including paging metadata such as the current page
    /// number, page size, total item count, and navigation information.
    /// </summary>
    /// <remarks>Use this class to encapsulate the results of a paged query, such as when displaying search
    /// results or lists in a paginated UI. The paging properties provide information for navigation and display, while
    /// the <see cref="PageItems"/> property contains the items for the current page. This class is immutable and
    /// thread-safe.</remarks>
    /// <typeparam name="T">The type of items contained in the paged collection.</typeparam>
    public sealed class Paged<T>
    {
        /// <summary>
        /// Gets the current page number (1-based). The minimum value is 1, representing the first page.
        /// </summary>
        public int Page { get; }

        /// <summary>
        /// Gets the number of items to display per page. This value determines how many items are included in <see cref="PageItems"/>.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Gets the total number of items in the source collection, before paging was applied.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Gets the total number of pages available based on the <see cref="TotalCount"/> and <see cref="PageSize"/>.
        /// </summary>
        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalCount / PageSize));

        /// <summary>
        /// Gets a value indicating whether there is a previous page available.
        /// </summary>
        public bool HasPrev => Page > 1;

        /// <summary>
        /// Gets a value indicating whether there is a next page available.
        /// </summary>
        public bool HasNext => Page < TotalPages;

        /// <summary>
        /// Gets the collection of items for the current page.
        /// </summary>
        public IReadOnlyList<T> PageItems { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Paged{T}"/> class.
        /// </summary>
        /// <param name="items">The items for the current page.</param>
        /// <param name="page">The current page number (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="total">The total number of items in the source collection.</param>
        private Paged(IReadOnlyList<T> items, int page, int pageSize, int total)
        { PageItems = items; Page = page; PageSize = pageSize; TotalCount = total; }

        /// <summary>
        /// Creates a new paged collection from a source enumerable.
        /// </summary>
        /// <param name="src">The source collection to page.</param>
        /// <param name="page">The desired page number (1-based). If less than 1, it will be set to 1.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A new <see cref="Paged{T}"/> instance containing the requested page of items and associated metadata.</returns>
        public static Paged<T> Create(IEnumerable<T> src, int page, int pageSize)
        {
            var list = src.ToList();
            var total = list.Count;
            var p = Math.Max(1, page);
            var skip = (p - 1) * pageSize;
            return new Paged<T>(list.Skip(skip).Take(pageSize).ToList(), p, pageSize, total);
        }
    }
}
