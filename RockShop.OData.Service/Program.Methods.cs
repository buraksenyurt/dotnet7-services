using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using RockShop.Shared;
partial class Program
{
    static IEdmModel GetEdmModelForMusics()
    {
        ODataConventionModelBuilder builder = new();
        builder.EntitySet<Album>("Albums");
        builder.EntitySet<Artist>("Artists");
        builder.EntitySet<Track>("Tracks");
        return builder.GetEdmModel();
    }
}