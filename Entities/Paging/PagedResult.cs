namespace backend_marsh_project.Entities.Paging
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; } = [];
        public PaginationMetadata Metadata { get; set; } = new();
    }
}
