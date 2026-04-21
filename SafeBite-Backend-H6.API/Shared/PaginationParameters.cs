namespace SafeBite_Backend_H6.API.Shared;

public class PaginationParameters
{
    private const int MaxPageSize = 20;
    public int Page
    {
        get => _page;
        set => _page = value <= 0 ? 1 : value;
        // sørger for at page altid er større end 0
    }
    private int _page = 1; // default page 

    private int _pageSize = 20; // default page size
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value <= 0 ? MaxPageSize : value > MaxPageSize ? MaxPageSize : value;
        // sørger for at pagesize ikke kan være større end maxpagesize, hvis det er det, så sætter den pagesize til maxpagesize
    }
}
