namespace Cqrs.Common.Interfaces
{
    public interface ICurrentStateService
    {
        string User { get; }
        string TransactionId { get; }

        void SetString(string name, string value);
        string GetString(string name);

        void SetObject(string name, object value);
        T GetObject<T>(string name);
    }
}
