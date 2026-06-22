using DukkanDepo.Application.Abstractions.Summary;
using DukkanDepo.Application.Models;
using DukkanDepo.Application.Services.Summary;
using DukkanDepo.Domain.Entities;

namespace DukkanDepo.Infrastructure.Reporting.Summary;

public sealed class EfSummaryDataProvider : ISummaryDataProvider
{
    private readonly Func<IQueryable<Urun>> _queryFactory;

    public EfSummaryDataProvider(Func<IQueryable<Urun>> queryFactory)
    {
        _queryFactory = queryFactory;
    }

    public SummaryReport GetReport()
    {
        return SummaryReportCalculator.Calculate(
            _queryFactory(),
            DateTime.Now);
    }
}