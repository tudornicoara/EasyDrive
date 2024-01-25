using API.AppUsers;
using API.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsersController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return HandleResult(await Mediator.Send(new List.Query()));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(RegisterDto registerDto)
    {
        return HandleResult(await Mediator.Send(new Create.Command {RegisterDto = registerDto}));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        return HandleResult(await Mediator.Send(new Details.Command { Id = Guid.Parse(id) }));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        return HandleResult(await Mediator.Send(new Login.Command {LoginDto = loginDto}));
    }
}