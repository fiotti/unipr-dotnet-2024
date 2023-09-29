
class HttpServerUploader : IServerUploader
{
    public Task UploadToServerAsync(string data, CancellationToken cancellationToken = default)
    {
        // TODO
        return Task.Delay(3000, cancellationToken);
    }
}
