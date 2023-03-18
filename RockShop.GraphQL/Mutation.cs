using RockShop.Shared;

namespace RockShop.GraphQL;

// Input for a mutation. Add[EntityName]Input
public record AddArtistInput(
    string Name
);

// Returned document for a mutation. Add[EntityName]Payload
public class AddArtistPayload
{
    public Artist Artist { get; }
    public AddArtistPayload(Artist artist)
    {
        Artist = artist;
    }
}

public class Mutation
{
    // Mutation function. Add[EntityName]
    public async Task<AddArtistPayload> AddArtistAsync(AddArtistInput input, ChinookDbContext db)
    {
        Artist artist = new()
        {
            Name = input.Name
        };
        db.Artists.Add(artist);
        int affectedRows = await db.SaveChangesAsync();
        return new AddArtistPayload(artist);
    }
}