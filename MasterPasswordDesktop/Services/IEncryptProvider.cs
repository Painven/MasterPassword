namespace MasterPasswordDesktop.Infrastructure
{
    public interface IEncryptProvider
    {
        bool Check(string source, string key);
    }
}