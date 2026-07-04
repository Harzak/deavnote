using Avalonia.Data;
using Avalonia.Interactivity;
using deavnote.app.ViewModels.Controls;
using LiveMarkdown.Avalonia;
using Microsoft.Extensions.DependencyInjection;

namespace deavnote.app.Controls;

internal partial class MarkdownTextBox : UserControl, IDisposable
{
    private readonly ObservableStringBuilder _markdownBuilder;
    private MarkdownTextBoxViewModel? _viewModel;

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<MarkdownTextBox, string>(nameof(Text), defaultValue: string.Empty, defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<MarkdownTextBoxViewModel?> InternalViewModelProperty =
        AvaloniaProperty.Register<MarkdownTextBox, MarkdownTextBoxViewModel?>(nameof(InternalViewModel));

    public string Text
    {
        get => this.GetValue(TextProperty);
        set => this.SetValue(TextProperty, value);
    }

    public MarkdownTextBoxViewModel? InternalViewModel
    {
        get => this.GetValue(InternalViewModelProperty);
        private set => this.SetValue(InternalViewModelProperty, value);
    }

    public MarkdownTextBox()
    {
        InitializeComponent();

        _markdownBuilder = new ObservableStringBuilder();

        this.MarkdownTexBox.TextChanged += this.OnMarkdownTexBoxTextChanged;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        IDialogService? dialogService = (Application.Current as App)?.Services?.GetService<IDialogService>();
        IDebounceActionFactory? debounceActionFactory = (Application.Current as App)?.Services?.GetService<IDebounceActionFactory>();
        if (dialogService is not null && debounceActionFactory is not null)
        {
            _viewModel = new MarkdownTextBoxViewModel(dialogService, debounceActionFactory);
            _viewModel.PropertyChanged += this.OnViewModelPropertyChanged;
            _viewModel.Text = this.Text;
            this.InternalViewModel = _viewModel;

            this.ApplyModeToUI(_viewModel.CurrentMode);
        }

        this.MarkdownRenderer.MarkdownBuilder = _markdownBuilder;
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        _viewModel?.PropertyChanged -= this.OnViewModelPropertyChanged;

        base.OnUnloaded(e);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty)
        {
            string newText = change.GetNewValue<string>() ?? string.Empty;
            this.MarkdownTexBox.Text = newText;
            _markdownBuilder.Clear();
            _markdownBuilder.Append(change.GetNewValue<string>());

            if (_viewModel is not null && !string.Equals(_viewModel.Text, newText, StringComparison.Ordinal))
            {
                _viewModel.Text = newText;
            }
        }
    }

    private void OnMarkdownTexBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_viewModel is not null)
        {
            _viewModel.InternalTextBoxValue = this.MarkdownTexBox.Text ?? string.Empty;
            _viewModel.OnTextBoxTextChanged();
        }
        else
        {
            this.Text = this.MarkdownTexBox.Text ?? string.Empty;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (string.Equals(e.PropertyName, nameof(MarkdownTextBoxViewModel.CurrentMode), StringComparison.Ordinal) && _viewModel is not null)
        {
            this.ApplyModeToUI(_viewModel.CurrentMode);
        }
        else if (string.Equals(e.PropertyName, nameof(MarkdownTextBoxViewModel.Text), StringComparison.Ordinal) && _viewModel is not null)
        {
            if (!string.Equals(this.Text, _viewModel.Text, StringComparison.Ordinal))
            {
                this.Text = _viewModel.Text;
            }
        }
    }

    private void ApplyModeToUI(EMarkdownTextBoxMode mode)
    {
        _markdownBuilder.Clear();

        switch (mode)
        {
            case EMarkdownTextBoxMode.Edit:
                this.MarkdownRenderer.IsVisible = false;
                this.MarkdownTexBox.IsVisible = true;
                this.ViewSplitter.IsVisible = false;
                this.MarkdownTexBox.Focus();

                Grid.SetColumn(this.MarkdownTexBox, 0);
                Grid.SetColumnSpan(this.MarkdownTexBox, 3);
                break;

            case EMarkdownTextBoxMode.View:
                this.MarkdownRenderer.IsVisible = true;
                this.MarkdownTexBox.IsVisible = false;
                this.ViewSplitter.IsVisible = false;
                this.MarkdownRenderer.Focus();

                Grid.SetColumn(this.MarkdownRenderer, 0);
                Grid.SetColumnSpan(this.MarkdownRenderer, 3);

                _markdownBuilder.Append(this.MarkdownTexBox.Text);
                break;

            case EMarkdownTextBoxMode.Split:
                this.MarkdownRenderer.IsVisible = true;
                this.MarkdownTexBox.IsVisible = true;
                this.ViewSplitter.IsVisible = true;
                this.MarkdownTexBox.Focus();

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
        if (_viewModel is not null)
        {
            _viewModel.PropertyChanged -= this.OnViewModelPropertyChanged;
            _viewModel.Dispose();
        }
    }
}