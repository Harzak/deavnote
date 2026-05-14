namespace deavnote.app.ViewModels
{
    internal sealed partial class SettingsViewModel : DialogViewModel<OperationResult>
    {
        private readonly IClipboardFormatRepository _clipboardFormatRepository;

        [ObservableProperty]
        public partial string ClipboardTemplateTimeEntry { get; set; }

        [ObservableProperty]
        public partial string ClipboardTemplateDay { get; set; }

        [ObservableProperty]
        public partial string ClipboardTemplateWeek { get; set; }

        internal override string Title => Strings.SettingsViewModel_Title;

        public SettingsViewModel(IClipboardFormatRepository clipboardFormatRepository)
        {
            ArgumentNullException.ThrowIfNull(clipboardFormatRepository);

            _clipboardFormatRepository = clipboardFormatRepository;

            this.ClipboardTemplateTimeEntry = string.Empty;
            this.ClipboardTemplateDay = string.Empty;
            this.ClipboardTemplateWeek = string.Empty;
        }

        public override async Task OnInitializedAsync()
        {
            this.ClipboardTemplateTimeEntry = await _clipboardFormatRepository.GetTemplateAsync(EJournalMode.TimeEntry).ConfigureAwait(false);
            this.ClipboardTemplateDay = await _clipboardFormatRepository.GetTemplateAsync(EJournalMode.Day).ConfigureAwait(false);
            this.ClipboardTemplateWeek = await _clipboardFormatRepository.GetTemplateAsync(EJournalMode.Week).ConfigureAwait(false);
        }

        [RelayCommand]
        private async Task Confirm()
        {
            base.ValidateAllProperties();

            if (base.HasErrors)
            {
                return;
            }

            await _clipboardFormatRepository.SetTemplateAsync(EJournalMode.TimeEntry, this.ClipboardTemplateTimeEntry).ConfigureAwait(false);
            await _clipboardFormatRepository.SetTemplateAsync(EJournalMode.Day, this.ClipboardTemplateDay).ConfigureAwait(false);
            await _clipboardFormatRepository.SetTemplateAsync(EJournalMode.Week, this.ClipboardTemplateWeek).ConfigureAwait(false);

            base.Close(OperationResult.Success());
        }

        [RelayCommand]
        private void Cancel()
        {
            base.Close(result: null);
        }
    }
}