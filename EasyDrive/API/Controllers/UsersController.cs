using API.AppUsers;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsersController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetActivities()
    {
        return HandleResult(await Mediator.Send(new List.Query()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateActivity(AppUser appUser)
    {
        return HandleResult(await Mediator.Send(new Create.Command {AppUser = appUser}));
    }
}