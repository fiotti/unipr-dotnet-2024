
class FileDataAccessor : IDataAccessor
{
    public async Task<string> GetDataAsync(CancellationToken cancellationToken = default)
    {
        string data = await File.ReadAllTextAsync("data.txt", cancellationToken);
        return data;
    }
}
