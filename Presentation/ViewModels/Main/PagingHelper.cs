namespace DukkanDepo.Presentation.ViewModels.Main;

internal static class PagingHelper
{
    public static (List<T> PageItems, int TotalPages) Paginate<T>(
        List<T> items,
        int page,
        int pageSize)
    {
        if (pageSize <= 0)
            pageSize = 20;

        var total = items.Count;
        var totalPages = (int)Math.Ceiling((double)total / pageSize);

        if (page < 1)
            page = 1;

        if (page > totalPages && totalPages > 0)
            page = totalPages;

        var pageItems = items
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return (pageItems, totalPages);
    }
}