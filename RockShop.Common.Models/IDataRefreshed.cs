namespace RockShop.Shared
{
    public interface IDataRefreshed
    {
        DateTimeOffset LastRefreshed { get; set; }
    }
}