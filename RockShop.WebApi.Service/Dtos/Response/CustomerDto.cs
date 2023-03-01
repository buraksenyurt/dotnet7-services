namespace RockShop.WebApi.Service.Dtos.Response
{
    public class CustomerDto
    {
        public int CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }

    }
}