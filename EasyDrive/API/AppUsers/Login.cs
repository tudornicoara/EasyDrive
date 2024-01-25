using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
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

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

            return Result<AppUserDto>.Success(_mapper.Map<AppUserDto>(user));
        }
    }
}