namespace Application.Common.Ordering;

public class OrderingParams
{
    public OrderingParams(OrderingParams other)
    {
        OrderBy = other.OrderBy;
        OrderDesc = other.OrderDesc;
    }

    public OrderingParams(string? orderBy, bool? orderDesc)
    {
        OrderBy = orderBy;
        OrderDesc = orderDesc ?? false;
    }


    public string? OrderBy;
    public bool OrderDesc;
}
