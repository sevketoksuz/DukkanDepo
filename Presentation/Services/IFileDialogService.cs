namespace DukkanDepo.Presentation.Services;

public interface IFileDialogService
{
    string? PickSaveFile(
        string title,
        string filter,
        string fileName,
        string? defaultExt = null);

    string? PickOpenFile(
        string title,
        string filter);
}