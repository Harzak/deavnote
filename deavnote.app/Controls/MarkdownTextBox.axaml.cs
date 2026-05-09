using Avalonia.Data;
using Avalonia.Interactivity;
using LiveMarkdown.Avalonia;

namespace deavnote.app.Controls;

internal partial class MarkdownTextBox : UserControl, IDisposable
{
    private readonly ObservableStringBuilder _markdownBuilder;
    private readonly DebounceAction _affectTextProperty;
    private EMarkdownTextBoxMode _currentMode;

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MarkdownTextBox, string>(nameof(Text), defaultValue: string.Empty, defaultBindingMode: BindingMode.TwoWay);

    public string Text
    {
        get => this.GetValue(TextProperty);
        set => this.SetValue(TextProperty, value);
    }

    public MarkdownTextBox()
    {
        InitializeComponent();

        _markdownBuilder = new ObservableStringBuilder();
        _affectTextProperty = new DebounceAction(
            action: () => Dispatcher.UIThread.Post(() => this.Text = this.MarkdownTexBox.Text ?? string.Empty),
            delayMs: 500);

        this.ViewModeButton.Click +=OnViewModeButtonClick;
        this.EditModeButton.Click += OnEditModeButtonClick;
        this.SplitModeButton.Click += OnSplitModeButtonClick;
        this.MarkdownTexBox.TextChanged += this.OnMarkdownTexBoxTextChanged;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        this.ChangeMode(EMarkdownTextBoxMode.Edit);

        MarkdownRenderer.MarkdownBuilder = _markdownBuilder;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty)
        {
            this.MarkdownTexBox.Text = change.GetNewValue<string>();
            _markdownBuilder.Clear();
            _markdownBuilder.Append(change.GetNewValue<string>());
        }
    }

    private void OnMarkdownTexBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        _affectTextProperty.Execute();
    }

    private void OnEditModeButtonClick(object? sender, RoutedEventArgs e)
    {
        this.ChangeMode(EMarkdownTextBoxMode.Edit);
    }

    private void OnViewModeButtonClick(object? sender, RoutedEventArgs e)
    {
        this.ChangeMode(EMarkdownTextBoxMode.View);
    }

    private void OnSplitModeButtonClick(object? sender, RoutedEventArgs e)
    {
        this.ChangeMode(EMarkdownTextBoxMode.Split);
    }

    private void ChangeMode(EMarkdownTextBoxMode mode)
    {
        if (_currentMode == mode) return;
        _currentMode = mode;

        _markdownBuilder.Clear();
        switch (mode)
        {
            case EMarkdownTextBoxMode.Edit:
                this.MarkdownRenderer.IsVisible = false;
                this.MarkdownTexBox.IsVisible = true;
                this.ViewSplitter.IsVisible = false;

                Grid.SetColumn(this.MarkdownTexBox, 0);
                Grid.SetColumnSpan(this.MarkdownTexBox, 3);
                break;

            case EMarkdownTextBoxMode.View:
                this.MarkdownRenderer.IsVisible = true;
                this.MarkdownTexBox.IsVisible = false;
                this.ViewSplitter.IsVisible = false;

                Grid.SetColumn(this.MarkdownRenderer, 0);
                Grid.SetColumnSpan(this.MarkdownRenderer, 3);

                _markdownBuilder.Append(this.MarkdownTexBox.Text);
                break;

            case EMarkdownTextBoxMode.Split:
                this.MarkdownRenderer.IsVisible = true;
                this.MarkdownTexBox.IsVisible = true;
                this.ViewSplitter.IsVisible = true;

                Grid.SetColumn(this.MarkdownTexBox, 0);
                Grid.SetColumnSpan(this.MarkdownTexBox, 1);

                Grid.SetColumn(this.ViewSplitter, 1);

                Grid.SetColumn(this.MarkdownRenderer, 2);
                Grid.SetColumnSpan(this.MarkdownRenderer, 1);

                _markdownBuilder.Append(this.MarkdownTexBox.Text);
                break;

            default:
                throw new NotSupportedException(mode.ToString());
        }
    }

    public void Dispose()
    {
        _affectTextProperty?.Dispose();
    }
}