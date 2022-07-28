namespace SimpleECommerce.Common.Abstractions.Domain
{
    public interface IHaveIdentity<out TId> : IHaveIdentity
    {
        new TId Id { get; }
    }

    public interface IHaveIdentity
    {
        object Id { get; }
    }
}
