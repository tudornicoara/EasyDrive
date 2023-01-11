using API.Entities;

namespace API.Data;

public static class Seed
{
    public static async Task SeedData(DataContext context)
    {
        var users = new List<AppUser>
        {
            new AppUser
            {
                Name = "Ronald",
                Surname = "McDonald",
                Gender = "Male",
                City = "Washington DC",
                Country = "USA"
            }
        };

        await context.AppUsers.AddRangeAsync(users);
        await context.SaveChangesAsync();
    }
}