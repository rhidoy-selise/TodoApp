using System.Text.Json.Serialization;

namespace TodoApp.Utils;

public class Paging
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const string Desc = "-";
    public int Page { get; set; } = DefaultPage;
    public int PerPage { get; set; } = DefaultPageSize;
    private string _orderBy = Desc + "id";

    public string OrderBy
    {
        get => GetOrder(_orderBy);
        set => _orderBy = GetOrder(value);
    }

    [JsonIgnore] public int Total { get; set; }


    public int GetOffset()
    {
        if (Page == 0)
        {
            return Page * PerPage;
        }

        return (Page - 1) * PerPage;
    }

    private static string GetOrder(string orderBy)
    {
        if (orderBy[0] == '-')
        {
            orderBy = orderBy[1..] + " desc";
        }

        return orderBy;
    }

    public Paging SetPage(int page)
    {
        Page = page;
        return this;
    }

    public Paging SetPerPage(int perPage)
    {
        PerPage = perPage;
        return this;
    }

    public Paging SetSortByDesc(string sortBy)
    {
        OrderBy = sortBy + Desc;
        return this;
    }
}
