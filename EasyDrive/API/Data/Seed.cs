using API.Entities;

namespace API.Data;

public static class Seed
{
    public static async Task SeedData(DataContext context)
    {
        if (!context.AppUsers.Any())
        {
            var users = new List<AppUser>
            {
                new AppUser
                {
                    Name = "Ronald",
                    Surname = "McDonald",
                    Gender = "Male",
                    City = "Washington DC",
                    Country = "USA",
                    Password = "Pa$$w0rd"
                }
            };

            await context.AppUsers.AddRangeAsync(users);
            await context.SaveChangesAsync();   
        }
    }
}