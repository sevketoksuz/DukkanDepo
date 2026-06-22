using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DukkanDepo.Presentation.Behaviours;

public static class NumericText
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(NumericText),
            new PropertyMetadata(false, OnIsEnabledChanged));

    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsEnabledProperty, value);
    }

    public static bool GetIsEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    public static readonly DependencyProperty AllowDecimalProperty =
        DependencyProperty.RegisterAttached(
            "AllowDecimal",
            typeof(bool),
            typeof(NumericText),
            new PropertyMetadata(true));

    public static void SetAllowDecimal(DependencyObject obj, bool value)
    {
        obj.SetValue(AllowDecimalProperty, value);
    }

    public static bool GetAllowDecimal(DependencyObject obj)
    {
        return (bool)obj.GetValue(AllowDecimalProperty);
    }

    public static readonly DependencyProperty MaxDecimalPlacesProperty =
        DependencyProperty.RegisterAttached(
            "MaxDecimalPlaces",
            typeof(int),
            typeof(NumericText),
            new PropertyMetadata(2));

    public static void SetMaxDecimalPlaces(DependencyObject obj, int value)
    {
        obj.SetValue(MaxDecimalPlacesProperty, value);
    }

    public static int GetMaxDecimalPlaces(DependencyObject obj)
    {
        return (int)obj.GetValue(MaxDecimalPlacesProperty);
    }

    private static void OnIsEnabledChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs eventArgs)
    {
        if (dependencyObject is not TextBox textBox)
            return;

        if ((bool)eventArgs.NewValue)
        {
            textBox.PreviewTextInput += OnPreviewTextInput;
            textBox.PreviewKeyDown += OnPreviewKeyDown;
            DataObject.AddPastingHandler(textBox, OnPaste);
        }
        else
        {
            textBox.PreviewTextInput -= OnPreviewTextInput;
            textBox.PreviewKeyDown -= OnPreviewKeyDown;
            DataObject.RemovePastingHandler(textBox, OnPaste);
        }
    }

    private static void OnPreviewKeyDown(object sender, KeyEventArgs eventArgs)
    {
        if (eventArgs.Key is Key.Back or Key.Delete or Key.Tab or Key.Left or Key.Right or Key.Home or Key.End or Key.Enter)
            return;

        if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            return;
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs eventArgs)
    {
        if (sender is not TextBox textBox)
            return;

        var allowDecimal = GetAllowDecimal(textBox);
        var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        var incoming = eventArgs.Text;

        if (!allowDecimal)
        {
            eventArgs.Handled = !Regex.IsMatch(incoming, @"^\d+$");
            return;
        }

        if (!Regex.IsMatch(incoming, @"^\d+$") &&
            incoming != "," &&
            incoming != "." &&
            incoming != separator)
        {
            eventArgs.Handled = true;
            return;
        }

        var normalizedIncoming = incoming is "," or "."
            ? separator
            : incoming;

        var proposedText = GetProposedText(textBox, normalizedIncoming);

        eventArgs.Handled = !IsValidDecimal(
            proposedText,
            GetMaxDecimalPlaces(textBox));
    }

    private static void OnPaste(object sender, DataObjectPastingEventArgs eventArgs)
    {
        if (sender is not TextBox textBox)
            return;

        if (!eventArgs.SourceDataObject.GetDataPresent(DataFormats.UnicodeText))
        {
            eventArgs.CancelCommand();
            return;
        }

        var text = eventArgs.SourceDataObject.GetData(DataFormats.UnicodeText) as string ?? string.Empty;
        var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        text = text
            .Trim()
            .Replace("₺", string.Empty)
            .Replace("$", string.Empty)
            .Replace("€", string.Empty)
            .Replace(",", separator)
            .Replace(".", separator);

        var proposedText = GetProposedText(textBox, text);

        if (!IsValidDecimal(proposedText, GetMaxDecimalPlaces(textBox)))
            eventArgs.CancelCommand();
    }

    private static string GetProposedText(TextBox textBox, string incoming)
    {
        var text = textBox.Text ?? string.Empty;

        if (textBox.SelectionLength > 0)
            text = text.Remove(textBox.SelectionStart, textBox.SelectionLength);

        return text.Insert(textBox.CaretIndex, incoming);
    }

    private static bool IsValidDecimal(string value, int maxDecimalPlaces)
    {
        value = value.Trim();

        if (string.IsNullOrEmpty(value))
            return true;

        var separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        if (value.Split([separator], StringSplitOptions.None).Length - 1 > 1)
            return false;

        if (!decimal.TryParse(value, NumberStyles.Any, CultureInfo.CurrentCulture, out _) &&
            !decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out _))
        {
            return false;
        }

        var separatorIndex = value.IndexOf(separator, StringComparison.Ordinal);

        if (separatorIndex >= 0)
        {
            var fraction = value[(separatorIndex + separator.Length)..];

            if (fraction.Length > maxDecimalPlaces)
                return false;
        }

        return true;
    }
}