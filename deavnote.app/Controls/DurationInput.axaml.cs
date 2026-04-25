using Avalonia.Data;
using Avalonia.Interactivity;

namespace deavnote.app.Controls;

internal sealed partial class DurationInput : UserControl
{
    private bool _isUpdatingText;

    public static readonly StyledProperty<TimeSpan> ValueProperty =
        AvaloniaProperty.Register<DurationInput, TimeSpan>(
            nameof(Value),
            TimeSpan.Zero,
            defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<TimeSpan> DefaultValueProperty =
        AvaloniaProperty.Register<DurationInput, TimeSpan>(nameof(DefaultValue), TimeSpan.Zero);

    public static readonly StyledProperty<TimeSpan> IncrementProperty =
        AvaloniaProperty.Register<DurationInput, TimeSpan>(nameof(Increment), TimeSpan.FromMinutes(15));

    public TimeSpan Value
    {
        get => this.GetValue(ValueProperty);
        set => this.SetValue(ValueProperty, value);
    }

    public TimeSpan DefaultValue
    {
        get => this.GetValue(DefaultValueProperty);
        set => this.SetValue(DefaultValueProperty, value);
    }

    public TimeSpan Increment
    {
        get => this.GetValue(IncrementProperty);
        set => this.SetValue(IncrementProperty, value);
    }

    public DurationInput()
    {
        InitializeComponent();

        this.DurationTextBox.Text = FormatDuration(this.Value);
        this.DurationTextBox.LostFocus += OnDurationTextBoxLostFocus;
        this.DecreaseButton.Click += OnDecreaseButtonClick;
        this.IncreaseButton.Click += OnIncreaseButtonClick;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            this.UpdateText(change.GetNewValue<TimeSpan>());
        }
    }

    private void OnDurationTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        if (_isUpdatingText)
        {
            return;
        }

        this.Value = TryParseDuration(this.DurationTextBox.Text, out TimeSpan duration)
            ? duration
            : this.DefaultValue;
        this.UpdateText(this.Value);
    }

    private void OnDecreaseButtonClick(object? sender, RoutedEventArgs e)
    {
        TimeSpan increment = this.GetEffectiveIncrement();
        TimeSpan value = this.GetValidCurrentValue() - increment;

        if (value < TimeSpan.Zero)
        {
            value = TimeSpan.Zero;
        }

        this.Value = value;
    }

    private void OnIncreaseButtonClick(object? sender, RoutedEventArgs e)
    {
        this.Value = this.GetValidCurrentValue() + this.GetEffectiveIncrement();
    }

    private TimeSpan GetValidCurrentValue()
    {
        return TryParseDuration(this.DurationTextBox.Text, out TimeSpan duration)
            ? duration
            : this.Value;
    }

    private TimeSpan GetEffectiveIncrement()
    {
        return this.Increment > TimeSpan.Zero
            ? this.Increment
            : TimeSpan.FromMinutes(15);
    }

    private void UpdateText(TimeSpan value)
    {
        string text = FormatDuration(value);

        if (string.Equals(this.DurationTextBox.Text, text, StringComparison.Ordinal))
        {
            return;
        }

        _isUpdatingText = true;
        try
        {
            this.DurationTextBox.Text = text;
        }
        finally
        {
            _isUpdatingText = false;
        }
    }

    private static bool TryParseDuration(string? text, out TimeSpan duration)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            duration = TimeSpan.Zero;
            return false;
        }

        string sanitizedText = text.Replace('_', '0');
        string[] parts = sanitizedText.Split(':');

        if (parts.Length != 2
            || !int.TryParse(parts[0], CultureInfo.InvariantCulture, out int hours)
            || !int.TryParse(parts[1], CultureInfo.InvariantCulture, out int minutes)
            || hours < 0
            || minutes is < 0 or > 59)
        {
            duration = TimeSpan.Zero;
            return false;
        }

        duration = new TimeSpan(hours, minutes, 0);
        return true;
    }

    private static string FormatDuration(TimeSpan duration)
    {
        if (duration < TimeSpan.Zero)
        {
            duration = TimeSpan.Zero;
        }

        int totalHours = (int)duration.TotalHours;
        return FormattableString.Invariant($"{totalHours:00}:{duration.Minutes:00}");
    }
}
