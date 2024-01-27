using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.AppUsers;

public class Login
{
    public class Command : IRequest<Result<AppUserDto>>
    {
        public LoginDto? LoginDto { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, Result<AppUserDto>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public Handler(DataContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        public async Task<Result<AppUserDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.LoginDto == null)
            {
                return Result<AppUserDto>.Failure("User is null");
            }

            if (request.LoginDto.Email == null)
            {
                return Result<AppUserDto>.Failure("You didn't provide an email");
            }
            
            if (request.LoginDto.Password == null)
            {
                return Result<AppUserDto>.Failure("You didn't provide a password");
            }

            var user = await _context.AppUsers
                .SingleOrDefaultAsync(x => x.Email == request.LoginDto.Email, cancellationToken);

            if (user == null)
            {
                return Result<AppUserDto>.Failure("Unauthorized: Invalid Email");
            }

            if (user.PasswordHash == null || user.PasswordSalt == null)
            {
                return Result<AppUserDto>.Failure("Unauthorized: Problem when checking user's password");                
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac
                .ComputeHash(Encoding.UTF8.GetBytes(request.LoginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i])
                {
                    return Result<AppUserDto>.Failure("Unauthorized: Invalid password");
                }
            }

            var userDto = _mapper.Map<AppUserDto>(user);
            userDto.Token = _tokenService.CreateToken(user);

            return Result<AppUserDto>.Success(userDto);
        }
    }
}