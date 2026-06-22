using System.Globalization;
using DukkanDepo.Application.Models;
using DukkanDepo.Domain.Entities;

namespace DukkanDepo.Application.Services.Charts;

public static class ChartsReportCalculator
{
    public static ChartsReport Calculate(
        IQueryable<Urun> query,
        CultureInfo culture)
    {
        var data = query.ToList();

        var monthly = data
            .GroupBy(x => new
            {
                x.Tarih.Year,
                x.Tarih.Month
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .Select(g => new
            {
                Label = $"{g.Key.Month:00}.{g.Key.Year}",
                Tutar = g.Sum(i => (double)(i.Tutar ?? 0m)),
                IskontoluTutar = g.Sum(i => (double)(i.IskontoluTutar ?? 0m))
            })
            .ToList();

        int GetWeek(DateTime date)
        {
            var calendar = culture.Calendar;

            return calendar.GetWeekOfYear(
                date,
                CalendarWeekRule.FirstFourDayWeek,
                DayOfWeek.Monday);
        }

        var weekly = data
            .GroupBy(x => new
            {
                x.Tarih.Year,
                Week = GetWeek(x.Tarih)
            })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Week)
            .Select(g => new
            {
                Label = $"W{g.Key.Week:00}-{g.Key.Year}",
                Tutar = g.Sum(i => (double)(i.Tutar ?? 0m)),
                IskontoluTutar = g.Sum(i => (double)(i.IskontoluTutar ?? 0m))
            })
            .ToList();

        return new ChartsReport
        {
            Monthly = new ChartSeriesData
            {
                Labels = monthly.Select(x => x.Label).ToArray(),
                TutarValues = monthly.Select(x => x.Tutar).ToArray(),
                IskontoluTutarValues = monthly.Select(x => x.IskontoluTutar).ToArray()
            },
            Weekly = new ChartSeriesData
            {
                Labels = weekly.Select(x => x.Label).ToArray(),
                TutarValues = weekly.Select(x => x.Tutar).ToArray(),
                IskontoluTutarValues = weekly.Select(x => x.IskontoluTutar).ToArray()
            }
        };
    }
}