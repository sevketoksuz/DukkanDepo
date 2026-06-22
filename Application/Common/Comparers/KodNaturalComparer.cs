namespace DukkanDepo.Application.Common.Comparers;

public sealed class KodNaturalComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        x = (x ?? string.Empty).Trim();
        y = (y ?? string.Empty).Trim();

        if (ReferenceEquals(x, y))
            return 0;

        if (x.Length == 0)
            return -1;

        if (y.Length == 0)
            return 1;

        var xs = x.Split('-', 2);
        var ys = y.Split('-', 2);

        var xPrefix = xs[0];
        var yPrefix = ys[0];

        var prefixCompare = StringComparer.OrdinalIgnoreCase.Compare(xPrefix, yPrefix);

        if (prefixCompare != 0)
            return prefixCompare;

        long xNumber = 0;
        long yNumber = 0;

        var xHasNumber = xs.Length == 2 && long.TryParse(xs[1], out xNumber);
        var yHasNumber = ys.Length == 2 && long.TryParse(ys[1], out yNumber);

        if (xHasNumber && yHasNumber)
        {
            var numberCompare = xNumber.CompareTo(yNumber);

            if (numberCompare != 0)
                return numberCompare;
        }

        return StringComparer.OrdinalIgnoreCase.Compare(x, y);
    }
}