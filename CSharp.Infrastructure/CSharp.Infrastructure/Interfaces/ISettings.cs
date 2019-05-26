namespace CSharp.Infrastructure.Interfaces
{
    public interface ISettings
    {
        void Put<TSetting>(TSetting setting);

        TSetting Obtain<TSetting>();
    }
}
