
interface IServerUploader
{
    Task UploadToServerAsync(string data, CancellationToken cancellationToken = default);
}
