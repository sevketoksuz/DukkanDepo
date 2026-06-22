namespace DukkanDepo.Application.Models;

public sealed class ChartSeriesData
{
    public string[] Labels { get; init; } = [];
    public double[] TutarValues { get; init; } = [];
    public double[] IskontoluTutarValues { get; init; } = [];
}

public sealed class ChartsReport
{
    public ChartSeriesData Monthly { get; init; } = new();
    public ChartSeriesData Weekly { get; init; } = new();
}