using DukkanDepo.Application.Models;

namespace DukkanDepo.Application.Abstractions.Charts;

public interface IChartsDataProvider
{
    ChartsReport GetReport();
}