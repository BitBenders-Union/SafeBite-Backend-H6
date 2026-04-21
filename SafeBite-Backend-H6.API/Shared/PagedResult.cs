namespace SafeBite_Backend_H6.API.Shared;

public class PagedResult<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedResult<TResult> Map<TResult>(Func<T, TResult> mapper)
    {
        return new PagedResult<TResult>
        {
            Data = Data.Select(mapper),
            Page = Page,
            PageSize = PageSize,
            TotalCount = TotalCount,
            TotalPages = TotalPages
        };
    }


    // Func<T, TResult> - tager T og returnere TResult. den bliver stored i variablen mapper
    // Data.Select(maper). Select tager hver item i Data og anvender mapper på den.
    // microsofts dokumentaiton for func<T, tresult> delegate: https://learn.microsoft.com/en-us/dotnet/api/system.func-2?view=net-11.0
    // og for select : https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.select?view=net-11.0
}
