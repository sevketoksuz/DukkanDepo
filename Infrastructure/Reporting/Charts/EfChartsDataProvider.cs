using System.Globalization;
using DukkanDepo.Application.Abstractions.Charts;
using DukkanDepo.Application.Models;
using DukkanDepo.Application.Services.Charts;
using DukkanDepo.Domain.Entities;

namespace DukkanDepo.Infrastructure.Reporting.Charts;

public sealed class EfChartsDataProvider : IChartsDataProvider
{
    private readonly Func<IQueryable<Urun>> _queryFactory;

    public EfChartsDataProvider(Func<IQueryable<Urun>> queryFactory)
    {
        _queryFactory = queryFactory;
    }

    public ChartsReport GetReport()
    {
        return ChartsReportCalculator.Calculate(
            _queryFactory(),
            CultureInfo.CurrentCulture);
    }
}