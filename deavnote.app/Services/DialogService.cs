namespace deavnote.app.Services;

/// <summary>
/// Opens an Avalonia <see cref="Window"/> for a given <see cref="DialogViewModel{TResult}"/>
/// The correct <see cref="Window"/> is resolved by the <see cref="ViewLocator"/> convention.
/// </summary>
internal sealed class DialogService : IDialogService
{
    /// <inheritdoc/>
    public async Task<TResult?> ShowWindowAsync<TResult>(DialogViewModel<TResult> viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        Window? owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        if (owner is null)
        {
            return default;
        }   

        Window dialog = new()
        {
            Content = viewModel,
            Title = viewModel.Title,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = true,
        };


        TaskCompletionSource<TResult?> tcs = new();
        viewModel.CloseDialog = result =>
        {
            tcs.TrySetResult(result);
            dialog.Close();
        };

        dialog.Closed += (_, _) => tcs.TrySetResult(default);

        var e = await dialog.ShowDialog<TResult>(owner).ConfigureAwait(false);

        return await tcs.Task.ConfigureAwait(false);
    }
}
