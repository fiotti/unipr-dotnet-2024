
class DataUploader : IDataUploader
{
    private readonly IDataAccessor _dataAccessor;
    private readonly IServerUploader _serverUploader;

    public DataUploader(IDataAccessor dataAccessor, IServerUploader serverUploader)
    {
        _dataAccessor = dataAccessor;
        _serverUploader = serverUploader;
    }

    public async Task UploadDataToServerAsync(CancellationToken cancellationToken = default)
    {
        string data = await _dataAccessor.GetDataAsync(cancellationToken);
        await _serverUploader.UploadToServerAsync(data, cancellationToken);
    }
}
