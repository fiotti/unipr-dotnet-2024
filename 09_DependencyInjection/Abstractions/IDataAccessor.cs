
interface IDataAccessor
{
    Task<string> GetDataAsync(CancellationToken cancellationToken = default);
}
