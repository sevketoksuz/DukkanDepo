using System.Globalization;
using DukkanDepo.Application.Abstractions.Charts;
using LiveChartsCore;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;

namespace DukkanDepo.Presentation.ViewModels.Charts;

public sealed class ChartsViewModel
{
    public ISeries[] AylikTutarSeri { get; private set; } = [];

    public ISeries[] AylikIskontoluTutarSeri { get; private set; } = [];

    public ICartesianAxis[] AylikXAxes { get; private set; } = [];

    public ISeries[] HaftalikTutarSeri { get; private set; } = [];

    public ISeries[] HaftalikIskontoluTutarSeri { get; private set; } = [];

    public ICartesianAxis[] HaftalikXAxes { get; private set; } = [];

    public ICartesianAxis[] ParaYAxes { get; private set; } = [];

    public ChartsViewModel(IChartsDataProvider provider)
    {
        ArgumentNullException.ThrowIfNull(provider);

        var culture = CultureInfo.CurrentCulture;
        var report = provider.GetReport();

        AylikXAxes =
        [
            new Axis
            {
                Labels = report.Monthly.Labels,
                LabelsRotation = 45
            }
        ];

        AylikTutarSeri =
        [
            new ColumnSeries<double>
            {
                Name = "Aylık Tutar",
                Values = report.Monthly.TutarValues,
                MaxBarWidth = 36
            }
        ];

        AylikIskontoluTutarSeri =
        [
            new LineSeries<double>
            {
                Name = "Aylık İskontolu Tutar",
                Values = report.Monthly.IskontoluTutarValues,
                GeometrySize = 0,
                LineSmoothness = 1
            }
        ];

        HaftalikXAxes =
        [
            new Axis
            {
                Labels = report.Weekly.Labels,
                LabelsRotation = 45
            }
        ];

        HaftalikTutarSeri =
        [
            new ColumnSeries<double>
            {
                Name = "Haftalık Tutar",
                Values = report.Weekly.TutarValues,
                MaxBarWidth = 36
            }
        ];

        HaftalikIskontoluTutarSeri =
        [
            new LineSeries<double>
            {
                Name = "Haftalık İskontolu Tutar",
                Values = report.Weekly.IskontoluTutarValues,
                GeometrySize = 0,
                LineSmoothness = 1
            }
        ];

        ParaYAxes =
        [
            new Axis
            {
                Labeler = value => value.ToString("C0", culture),
                MinStep = 1
            }
        ];
    }
}