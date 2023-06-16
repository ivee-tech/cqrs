namespace Cqrs.Common.Data
{
    public enum ChangeState
    {
        None = 0,
        Add = 1,
        Update = 2,
        Delete = 3,
        Patch = 4
    }
}
