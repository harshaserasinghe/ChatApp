namespace Chat.Service.Interfaces
{
    public interface IAzureRedisService
    {
        T GetEntity<T>(string key);
        void SetEntity<T>(string key, T entity);
        bool IsEntityExists(string key);
    }
}