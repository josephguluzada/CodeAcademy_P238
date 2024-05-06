namespace PustokMVC.Helpers
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(List<T> values, int count, int pageSize, int page )
        {
            this.AddRange(values);
            ActivePage = page;
            TotalPageCount =  (int)Math.Ceiling(count / (double)pageSize); // 6 -> 2 = 3 || 7 -> 2 = 4
        }


        public int TotalPageCount { get; set; }
        public int ActivePage { get; set; }
        public bool HasNext => ActivePage < TotalPageCount;
        public bool HasPrev => ActivePage > 1;


        public static PaginatedList<T> Create(IQueryable<T> values, int pageSize, int page)
        {
            return new PaginatedList<T>(values.Skip((page - 1) * pageSize).Take(pageSize).ToList(), values.Count(), pageSize, page);
        }
    }
}
