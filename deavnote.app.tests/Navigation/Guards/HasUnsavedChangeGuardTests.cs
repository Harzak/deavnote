using deavnote.app.Enums;
using deavnote.app.Navigation.Context;
using deavnote.app.Navigation.Guards;
using deavnote.utils.Results;
using System.Threading;
using System.Threading.Tasks;

namespace deavnote.app.tests.Navigation.Guards;

[TestClass]
public sealed class HasUnsavedChangeGuardTests
{
    private IDialogService _dialogService = null!;
    private IViewModel _targetViewModel = null!;
    private NavigationContext _context = null!;

    [TestInitialize]
    public void Initialize()
    {
        _dialogService = A.Fake<IDialogService>();
        _targetViewModel = A.Fake<IViewModel>();
        _context = new NavigationContext();
    }

    [TestMethod]
    public async Task CanNavigateAsync_SourceHasNoChanges_AllowsWithoutAskingConfirmation()
    {
        // Arrange
        HasUnsavedChangeGuard guard = new(_dialogService);
        IViewModel sourceViewModel = CreateViewModel(hasChanges: false);

        // Act
        NavigationGuardResult result = await guard.CanNavigateAsync(sourceViewModel, _targetViewModel, _context).ConfigureAwait(false);

        // Assert
        result.IsAllowed.Should().BeTrue();
        A.CallTo(() => _dialogService.ShowWindowAsync(A<ConfirmationViewModel>._)).MustNotHaveHappened();
    }

    [TestMethod]
    public async Task CanNavigateAsync_UserConfirmsSaveAndSaveSucceeds_AllowsNavigation()
    {
        // Arrange
        HasUnsavedChangeGuard guard = new(_dialogService);
        IViewModel sourceViewModel = CreateViewModel(hasChanges: true);
        INavigationStateDescriptor sourceState = sourceViewModel.NavigationState;
        A.CallTo(() => _dialogService.ShowWindowAsync(A<ConfirmationViewModel>._))
            .Returns(EConfirmationResult.Yes);
        A.CallTo(() => sourceState.SaveChangesAsync(A<CancellationToken>._))
            .Returns(Task.FromResult(OperationResult.Success()));

        // Act
        NavigationGuardResult result = await guard.CanNavigateAsync(sourceViewModel, _targetViewModel, _context).ConfigureAwait(false);

        // Assert
        result.IsAllowed.Should().BeTrue();
        A.CallTo(() => sourceState.SaveChangesAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    public async Task CanNavigateAsync_UserDeclinesSave_AllowsNavigationWithoutSaving()
    {
        // Arrange
        HasUnsavedChangeGuard guard = new(_dialogService);
        IViewModel sourceViewModel = CreateViewModel(hasChanges: true);
        INavigationStateDescriptor sourceState = sourceViewModel.NavigationState;
        A.CallTo(() => _dialogService.ShowWindowAsync(A<ConfirmationViewModel>._))
            .Returns(EConfirmationResult.No);

        // Act
        NavigationGuardResult result = await guard.CanNavigateAsync(sourceViewModel, _targetViewModel, _context).ConfigureAwait(false);

        // Assert
        result.IsAllowed.Should().BeTrue();
        A.CallTo(() => sourceState.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [TestMethod]
    public async Task CanNavigateAsync_UserCancelsConfirmation_CancelsNavigation()
    {
        // Arrange
        HasUnsavedChangeGuard guard = new(_dialogService);
        IViewModel sourceViewModel = CreateViewModel(hasChanges: true);
        INavigationStateDescriptor sourceState = sourceViewModel.NavigationState;
        A.CallTo(() => _dialogService.ShowWindowAsync(A<ConfirmationViewModel>._))
            .Returns(EConfirmationResult.Cancel);

        // Act
        NavigationGuardResult result = await guard.CanNavigateAsync(sourceViewModel, _targetViewModel, _context).ConfigureAwait(false);

        // Assert
        result.IsCanceled.Should().BeTrue();
        A.CallTo(() => sourceState.SaveChangesAsync(A<CancellationToken>._)).MustNotHaveHappened();
    }

    [TestMethod]
    public async Task CanNavigateAsync_SaveFails_DeniesNavigationWithSaveError()
    {
        // Arrange
        const string errorMessage = "Save failed";
        HasUnsavedChangeGuard guard = new(_dialogService);
        IViewModel sourceViewModel = CreateViewModel(hasChanges: true);
        INavigationStateDescriptor sourceState = sourceViewModel.NavigationState;
        A.CallTo(() => _dialogService.ShowWindowAsync(A<ConfirmationViewModel>._))
            .Returns(EConfirmationResult.Yes);
        A.CallTo(() => sourceState.SaveChangesAsync(A<CancellationToken>._))
            .Returns(Task.FromResult(OperationResult.Failure(errorMessage)));

        // Act
        NavigationGuardResult result = await guard.CanNavigateAsync(sourceViewModel, _targetViewModel, _context).ConfigureAwait(false);

        // Assert
        result.IsDenied.Should().BeTrue();
        result.Reason.Should().Be(errorMessage);
    }

    private static IViewModel CreateViewModel(bool hasChanges)
    {
        INavigationStateDescriptor navigationState = A.Fake<INavigationStateDescriptor>();
        IViewModel viewModel = A.Fake<IViewModel>();
        A.CallTo(() => navigationState.HasUnsavedChanges).Returns(hasChanges);
        A.CallTo(() => viewModel.NavigationState).Returns(navigationState);
        return viewModel;
    }
}
