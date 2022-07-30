namespace SimpleECommerce.Common.Abstractions.Domain
{
    public interface IHaveSoftDelete
    {
        bool IsDeleted { get; set; }
    }
}