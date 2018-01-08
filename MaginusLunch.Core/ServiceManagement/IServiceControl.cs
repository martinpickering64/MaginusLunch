namespace MaginusLunch.Core.ServiceManagement
{
    public interface IServiceControl
    {
        bool Start();
        bool Stop();
    }
}
