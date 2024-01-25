using API.AppUsers;
using API.DTOs;
using MediatR;

namespace API.Data;

public static class Seed
{
    public static async Task SeedData(DataContext context, IMediator mediator)
    {
        if (!context.AppUsers.Any())
        {
            var users = new List<RegisterDto>
            {
                new()
                {
                    Name = "Ronald",
                    Surname = "McDonald",
                    Email = "ronald.mcdonald@test.com",
                    Gender = "Male",
                    City = "Washington DC",
                    Country = "USA",
                    Password = "Pa$$w0rd"
                }
            };

            foreach (var user in users)
            {
                await mediator.Send(new Create.Command { RegisterDto = user });   
            }
        }
    }
}