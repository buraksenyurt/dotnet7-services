using RockShop.Shared;

namespace RockShop.OData.Client.Mvc.Models.OData;

/*
    This model using for OData Query results.
    "@odata.context":"http:.........",
    "value": [
        {
            
        },
    ]

*/
public class Albums
{
    public Album[]? Value { get; set; }
}