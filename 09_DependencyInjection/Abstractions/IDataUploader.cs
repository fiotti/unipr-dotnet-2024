
interface IDataUploader
{
    Task UploadDataToServerAsync(CancellationToken cancellationToken = default);
}
