using API.Data;
using API.Entities;
using API.Helpers;
using MediatR;

namespace API.AppUsers;

public class Details
{
    public class Command : IRequest<Result<AppUser>>
    {
        public Guid? Id { get; set; }
    }
    
    public class Handler : IRequestHandler<Command, Result<AppUser>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }

        public async Task<Result<AppUser>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _context.AppUsers.FindAsync(request.Id);

            return user == null
                ? Result<AppUser>.Failure("Failed to find app user")
                : Result<AppUser>.Success(user);
        }
    }
}