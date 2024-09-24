
class DataUploader(IDataAccessor dataAccessor, IServerUploader serverUploader) : IDataUploader
{
    public async Task UploadDataToServerAsync(CancellationToken cancellationToken = default)
    {
        string data = await dataAccessor.GetDataAsync(cancellationToken);
        await serverUploader.UploadToServerAsync(data, cancellationToken);
    }
}
