using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;

namespace deavnote.app.Services;

/// <summary>
/// Opens an Avalonia <see cref="Window"/> for a given <see cref="DialogViewModel{TResult}"/>
/// The correct <see cref="Window"/> is resolved by the <see cref="ViewLocator"/> convention.
/// </summary>
internal sealed class DialogService : IDialogService
{
    /// <inheritdoc/>
    public async Task<TResult?> ShowDialogAsync<TResult>(DialogViewModel<TResult> viewModel)
    {
        ArgumentNullException.ThrowIfNull(viewModel);

        Window? owner = (Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

        if (owner is null)
        {
            return default;
        }

        TaskCompletionSource<TResult?> tcs = new();
        viewModel.CloseDialog = result =>
        {
            tcs.TrySetResult(result);
        };

        Window dialog = new()
        {
            Content = viewModel,
            Title = viewModel.Title,
            SizeToContent = SizeToContent.WidthAndHeight,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = true,
        };

        dialog.Closed += (_, _) => tcs.TrySetResult(default);

        await dialog.ShowDialog(owner).ConfigureAwait(false);

        return await tcs.Task.ConfigureAwait(false);
    }
}
