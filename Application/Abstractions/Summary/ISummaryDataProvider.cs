using DukkanDepo.Application.Models;

namespace DukkanDepo.Application.Abstractions.Summary;

public interface ISummaryDataProvider
{
    SummaryReport GetReport();
}