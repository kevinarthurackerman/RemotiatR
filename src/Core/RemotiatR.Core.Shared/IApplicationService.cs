namespace RemotiatR.Shared
{
    public interface IApplicationService<TService>
    {
        TService Value { get; }
    }
}
