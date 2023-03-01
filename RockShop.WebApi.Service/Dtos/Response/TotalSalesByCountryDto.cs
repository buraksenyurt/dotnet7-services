namespace RockShop.WebApi.Service.Dtos.Response
{
    public class TotalSalesByCountryDto{
        public string? Country { get; set; }
        public decimal Total { get; set; }
    }
}