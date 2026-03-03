namespace Plugin.Abstractions
{
    public interface IPlugin
    {
        void Register(IServiceCollection services);
    }
}