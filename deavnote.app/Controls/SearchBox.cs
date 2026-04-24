using System.Windows.Input;
using System.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace deavnote.app.Controls;

/// <summary>
/// Custom search control
/// </summary>
internal sealed class SearchBox : TemplatedControl
{
    private TextBox? _textBox;
    private ListBox? _listBox;
    private Popup? _popup;
    private Button? _clearButton;
    private bool _isSelecting;

    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<SearchBox, string?>(nameof(Text), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<string?> WatermarkProperty =
        AvaloniaProperty.Register<SearchBox, string?>(nameof(Watermark));

    public static readonly StyledProperty<object?> SelectedItemProperty =
        AvaloniaProperty.Register<SearchBox, object?>(nameof(SelectedItem), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty =
        AvaloniaProperty.Register<SearchBox, IDataTemplate?>(nameof(ItemTemplate));

    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty =
        AvaloniaProperty.Register<SearchBox, IEnumerable?>(nameof(ItemsSource));

    public static readonly StyledProperty<double> MaxDropDownHeightProperty =
        AvaloniaProperty.Register<SearchBox, double>(nameof(MaxDropDownHeight), 400);

    public static readonly StyledProperty<bool> IsDropDownOpenProperty =
        AvaloniaProperty.Register<SearchBox, bool>(nameof(IsDropDownOpen), defaultBindingMode: Avalonia.Data.BindingMode.TwoWay);

    public static readonly StyledProperty<ICommand?> ClearCommandProperty =
        AvaloniaProperty.Register<SearchBox, ICommand?>(nameof(ClearCommand));

    public string? Text
    {
        get => this.GetValue(TextProperty);
        set => this.SetValue(TextProperty, value);
    }

    public string? Watermark
    {
        get => this.GetValue(WatermarkProperty);
        set => this.SetValue(WatermarkProperty, value);
    }

    public object? SelectedItem
    {
        get => this.GetValue(SelectedItemProperty);
        set => this.SetValue(SelectedItemProperty, value);
    }

    public IDataTemplate? ItemTemplate
    {
        get => this.GetValue(ItemTemplateProperty);
        set => this.SetValue(ItemTemplateProperty, value);
    }

    public IEnumerable? ItemsSource
    {
        get => this.GetValue(ItemsSourceProperty);
        set => this.SetValue(ItemsSourceProperty, value);
    }

    public double MaxDropDownHeight
    {
        get => this.GetValue(MaxDropDownHeightProperty);
        set => this.SetValue(MaxDropDownHeightProperty, value);
    }

    public bool IsDropDownOpen
    {
        get => this.GetValue(IsDropDownOpenProperty);
        set => this.SetValue(IsDropDownOpenProperty, value);
    }

    public ICommand? ClearCommand
    {
        get => this.GetValue(ClearCommandProperty);
        set => this.SetValue(ClearCommandProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        DetachEventHandlers();

        _textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        _listBox = e.NameScope.Find<ListBox>("PART_ListBox");
        _popup = e.NameScope.Find<Popup>("PART_Popup");
        _clearButton = e.NameScope.Find<Button>("PART_ClearButton");

        AttachEventHandlers();
    }

    private void AttachEventHandlers()
    {
        if (_textBox is not null)
        {
            _textBox.AddHandler(GotFocusEvent, OnTextBoxGotFocus, RoutingStrategies.Bubble);
            _textBox.AddHandler(LostFocusEvent, OnTextBoxLostFocus, RoutingStrategies.Bubble);
            _textBox.AddHandler(KeyDownEvent, OnTextBoxKeyDown, RoutingStrategies.Tunnel);
            _textBox.PropertyChanged += OnTextBoxPropertyChanged;
        }

        if (_clearButton is not null)
        {
            _clearButton.Click += OnClearButtonClick;
        }

        _listBox?.AddHandler(InputElement.PointerPressedEvent, OnListBoxPointerPressed, RoutingStrategies.Bubble, handledEventsToo: true);
    }

    private void DetachEventHandlers()
    {
        if (_textBox is not null)
        {
            _textBox.RemoveHandler(GotFocusEvent, OnTextBoxGotFocus);
            _textBox.RemoveHandler(LostFocusEvent, OnTextBoxLostFocus);
            _textBox.RemoveHandler(KeyDownEvent, OnTextBoxKeyDown);
            _textBox.PropertyChanged -= OnTextBoxPropertyChanged;
        }

        if (_clearButton is not null)
        {
            _clearButton.Click -= OnClearButtonClick;
        }

        _listBox?.RemoveHandler(InputElement.PointerPressedEvent, OnListBoxPointerPressed);
    }

    private void OnTextBoxGotFocus(object? sender, GotFocusEventArgs e)
    {
        this.IsDropDownOpen = true;
    }

    private void OnTextBoxLostFocus(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            if (_textBox is not null && !_textBox.IsFocused && (_listBox is null || !_listBox.IsFocused))
            {
                this.IsDropDownOpen = false;
            }
        }, DispatcherPriority.Input);
    }

    private void OnTextBoxPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != TextBox.TextProperty || _isSelecting)
        {
            return;
        }

        string? newValue = e.GetNewValue<string?>();
        this.Text = newValue;
    }

    private void OnTextBoxKeyDown(object? sender, KeyEventArgs e)
    {
        if (_listBox is null || _listBox.ItemCount == 0)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Down:
                {
                    int currentIndex = _listBox.SelectedIndex;
                    if (currentIndex < _listBox.ItemCount - 1)
                    {
                        _listBox.SelectedIndex = currentIndex + 1;
                    }
                    _listBox.ScrollIntoView(_listBox.SelectedIndex);
                    e.Handled = true;
                    break;
                }
            case Key.Up:
                {
                    int currentIndex = _listBox.SelectedIndex;
                    if (currentIndex > 0)
                    {
                        _listBox.SelectedIndex = currentIndex - 1;
                    }
                    _listBox.ScrollIntoView(_listBox.SelectedIndex);
                    e.Handled = true;
                    break;
                }
            case Key.Enter:
                {
                    CommitSelection();
                    e.Handled = true;
                    break;
                }
            case Key.Escape:
                {
                    this.IsDropDownOpen = false;
                    _listBox.SelectedIndex = -1;
                    e.Handled = true;
                    break;
                }
        }
    }

    private void OnListBoxPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            CommitSelection();
        }, 
        DispatcherPriority.Input);
    }

    private void CommitSelection()
    {
        if (_listBox is null || _listBox.SelectedIndex < 0)
        {
            return;
        }

        object? selected = _listBox.SelectedItem;
        _isSelecting = true;
        try
        {
            this.SelectedItem = selected;
            this.IsDropDownOpen = false;
            _listBox.SelectedIndex = -1;
        }
        finally
        {
            _isSelecting = false;
        }
    }

    private void OnClearButtonClick(object? sender, RoutedEventArgs e)
    {
        _isSelecting = true;
        try
        {
            this.Text = string.Empty;
            if (_textBox is not null)
            {
                _textBox.Text = string.Empty;
            }
            this.SelectedItem = null;
            this.IsDropDownOpen = false;

            if (this.ClearCommand is not null && this.ClearCommand.CanExecute(null))
            {
                this.ClearCommand.Execute(null);
            }
        }
        finally
        {
            _isSelecting = false;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == TextProperty && !_isSelecting && _textBox is not null)
        {
            string? newText = change.GetNewValue<string?>();
            if (_textBox.Text != newText)
            {
                _textBox.Text = newText;
            }
        }

        if (change.Property == ItemsSourceProperty && _listBox is not null)
        {
            _listBox.ItemsSource = change.GetNewValue<IEnumerable?>();
            _listBox.SelectedIndex = -1;
        }
    }
}
