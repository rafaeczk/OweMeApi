namespace Application.Common.Pagination;

public record PaginationParams
{
    public PaginationParams(PaginationParams other)
    {
        PageNumber = other.PageNumber;
        PageSize = other.PageSize;
    }

    public PaginationParams(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }


    public int PageNumber;
    public int PageSize;
}
