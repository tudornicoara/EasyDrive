using API.Data;
using API.Entities;
using API.Helpers;
using MediatR;

namespace API.AppUsers;

public class Create
{
    public class Command : IRequest<Result<Unit>>
    {
        public AppUser? AppUser { get; set; }
    }

    public class Handler : IRequestHandler<Command, Result<Unit>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }
        
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            _context.AppUsers.Add(request.AppUser);
            
            var result = await _context.SaveChangesAsync(cancellationToken) > 0;

            if (!result)
            {
                return Result<Unit>.Failure("Failed to create app user");
            }

            return Result<Unit>.Success(Unit.Value);
        }
    }
}