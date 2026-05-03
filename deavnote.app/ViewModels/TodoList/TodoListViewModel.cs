namespace deavnote.app.ViewModels.TodoList;

internal sealed partial class TodoListViewModel : BaseViewModel, ITodoHost
{
    private readonly ITodoRepository _repository;
    private readonly INotificationService _notificationService;

    public override string Identifier { get; }

    [ObservableProperty]
    public partial ObservableCollection<TodoListItemViewModel> TodoItemsInProgress { get; set; }

    [ObservableProperty]
    public partial ObservableCollection<TodoListItemViewModel> TodoItemsCompleted { get; set; }

    public TodoListViewModel(ITodoRepository repository, INotificationService notificationService)
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(notificationService);

        _repository = repository;
        _notificationService = notificationService;

        this.Identifier = Guid.NewGuid().ToString();
        this.TodoItemsInProgress = [];
        this.TodoItemsCompleted = [];
    }

    public async override Task OnInitializedAsync()
    {
        IReadOnlyList<model.Entities.Todo> inProgressTodos = await _repository.GetAllAsync(ETodoStatus.InProgress).ConfigureAwait(false);
        if (inProgressTodos?.Count > 0)
        {
            foreach (model.Entities.Todo todo in inProgressTodos)
            {
                this.AddInProgressItem(todo);
            }
        }

        IReadOnlyList<model.Entities.Todo> completedTodos = await _repository.GetAllAsync(ETodoStatus.Completed).ConfigureAwait(false);
        if (completedTodos?.Count > 0)
        {
            foreach (model.Entities.Todo todo in completedTodos)
            {
                this.AddCompletedItem(todo);
            }
        }
    }

    [RelayCommand]
    private async Task AddTodoItem()
    {
        DateTime now = DateTime.UtcNow;
        model.Entities.Todo newTodo = new()
        {
            Code = Guid.NewGuid().ToString(),
            Name = string.Format(CultureInfo.InvariantCulture, "AUTO-{0}", now.Ticks),
            Status = ETodoStatus.InProgress,
            CreatedAtUtc = now,
            UpdatedAtUtc = now,
        };
        OperationResult result = await _repository.AddAsync(newTodo).ConfigureAwait(false);
        if (result.IsSuccess)
        {
            this.AddInProgressItem(newTodo);
            _notificationService.Show("Todo item created", ENotificationType.Success);
        }
        else
        {
            _notificationService.Show(result.ErrorMessage ?? "Failed to create todo item", ENotificationType.Error, durationMs: 0);
        }

    }

    public async Task OnNoteChangedAsync(model.Entities.Todo item)
    {
        await this.TrySaveItemAsync(item).ConfigureAwait(false);
    }

    public async Task OnStateChangedAsync(model.Entities.Todo item)
    {
        if (await this.TrySaveItemAsync(item).ConfigureAwait(false))
        {
            if (item.Status == ETodoStatus.Completed)
            {
                TodoListItemViewModel? toRemove = this.TodoItemsInProgress.FirstOrDefault(x => x.Identifier.EqualsOrdinalIgnoreCase(item.Id.ToStringInvariant()));
                if (toRemove != null)
                {
                    this.RemoveInProgressItem(toRemove);
                    this.AddCompletedItem(item);
                }
            }
            else
            {
                TodoListItemViewModel? toRemove = this.TodoItemsCompleted.FirstOrDefault(x => x.Identifier.EqualsOrdinalIgnoreCase(item.Id.ToStringInvariant()));
                if (toRemove != null)
                {
                    this.RemoveInProgressItem(toRemove);
                    this.AddInProgressItem(item);
                }
            }
        }
    }

    private void AddInProgressItem(model.Entities.Todo item)
    {
        TodoListItemViewModel itemViewModel = new(item, this);

        Dispatcher.UIThread.Post(() => this.TodoItemsInProgress.Add(itemViewModel));
    }

    private void AddCompletedItem(model.Entities.Todo item)
    {
        TodoListItemViewModel itemViewModel = new(item, this);
        Dispatcher.UIThread.Post(() => this.TodoItemsCompleted.Add(itemViewModel));
    }

    private void RemoveInProgressItem(TodoListItemViewModel item)
    {
        Dispatcher.UIThread.Post(() => this.TodoItemsInProgress.Remove(item));
    }

    private void RemoveCompletedItem(TodoListItemViewModel item)
    {
        Dispatcher.UIThread.Post(() => this.TodoItemsCompleted.Remove(item));
    }

    private async Task<bool> TrySaveItemAsync(model.Entities.Todo item)
    {
        OperationResult result = await _repository.UpdateAsync(item).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            _notificationService.Show("Todo item saved", ENotificationType.Success);
        }
        else
        {
            _notificationService.Show(result.ErrorMessage ?? "Failed to save todo item", ENotificationType.Error, durationMs: 0);
        }

        return result.IsSuccess;
    }
}