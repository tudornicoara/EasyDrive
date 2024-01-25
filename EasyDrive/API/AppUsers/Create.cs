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

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
        public RegisterDto? RegisterDto { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public Handler(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            if (request.RegisterDto == null)
                return Result<Unit>.Failure("User is null");

            if (request.RegisterDto.Password == null)
                return Result<Unit>.Failure("You didn't provide a password");

            var existingUser = await _context.AppUsers
                .FirstOrDefaultAsync(x => x.Email == request.RegisterDto.Email, cancellationToken);

            if (existingUser != null)
            {
                return Result<Unit>.Failure("Email already used");  
            }
            
            using var hmac = new HMACSHA512();

            var appUser = _mapper.Map<AppUser>(request.RegisterDto);
            appUser.PasswordHash = hmac
                .ComputeHash(Encoding.UTF8.GetBytes(request.RegisterDto.Password));
            appUser.PasswordSalt = hmac.Key;

            _context.AppUsers.Add(appUser);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            return !result
                ? Result<Unit>.Failure("Failed to create app user")
                : Result<Unit>.Success(Unit.Value);
        }
    }
}