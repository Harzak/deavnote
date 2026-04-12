namespace deavnote.repository.Interfaces;

public interface IClipboardFormatRepository
{
    Task<string> GetTemplateAsync(EJournalContext context, CancellationToken cancellationToken = default);
}