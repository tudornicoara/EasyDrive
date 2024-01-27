using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.AppUsers;

public class Create
{
    public class Command : IRequest<Result<AppUserDto>>
    {
        public RegisterDto? RegisterDto { get; set; }
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
            if (request.RegisterDto == null)
                return Result<AppUserDto>.Failure("User is null");

            if (request.RegisterDto.Password == null)
                return Result<AppUserDto>.Failure("You didn't provide a password");

            var existingUser = await _context.AppUsers
                .FirstOrDefaultAsync(x => x.Email == request.RegisterDto.Email, cancellationToken);

            if (existingUser != null)
            {
                return Result<AppUserDto>.Failure("Email already used");  
            }
            
            using var hmac = new HMACSHA512();

            var appUser = _mapper.Map<AppUser>(request.RegisterDto);
            appUser.PasswordHash = hmac
                .ComputeHash(Encoding.UTF8.GetBytes(request.RegisterDto.Password));
            appUser.PasswordSalt = hmac.Key;

            _context.AppUsers.Add(appUser);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            var userDto = _mapper.Map<AppUserDto>(appUser);
            userDto.Token = _tokenService.CreateToken(appUser);

            return !result
                ? Result<AppUserDto>.Failure("Failed to create app user")
                : Result<AppUserDto>.Success(userDto);
        }
    }
}