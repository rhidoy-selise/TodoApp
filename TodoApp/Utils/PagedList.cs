using System.Text.Json.Serialization;

namespace TodoApp.Utils;

public sealed class PagedList<T>
{
    public List<T> Content { get; } = new();

    public int TotalPages => PerPage == 0 ? Paging.DefaultPage : (int)Math.Ceiling((double)TotalElements / PerPage);

    public int TotalElements { get; set; }

    public bool IsLast => Page == TotalPages;

    public int Page { get; set; }

    public int PerPage { get; set; }

    public bool IsFirst => Page == Paging.DefaultPage;

    public int NumberOfElements => Content.Count;

    public bool IsEmpty => Content.Count == 0;

    private PagedList()
    {
    }

    public static PagedList<T> Build(int total, int page, int perPage)
    {
        PagedList<T> pagedList = new PagedList<T>
        {
            Page = page,
            PerPage = perPage,
            TotalElements = total
        };
        return pagedList;
    }

    public static PagedList<T> GetPagedList(List<T> results, Paging paging)
    {
        return GetPagedList(results, paging.Total, paging.Page, paging.PerPage);
    }

    public static PagedList<T> GetPagedList(List<T> results, int total, int page, int perPage)
    {
        var pagedList = Build(total, page, perPage);
        if (results != null)
        {
            pagedList.Content.AddRange(results);
        }

        return pagedList;
    }
}

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
