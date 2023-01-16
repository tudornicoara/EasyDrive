using API.Data;
using API.DTOs;
using API.Entities;
using API.Helpers;
using AutoMapper;
using MediatR;

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

            var appUser = _mapper.Map<AppUser>(request.RegisterDto);
            
            _context.AppUsers.Add(appUser);

            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                return Result<Unit>.Failure("Failed to create app user");
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}