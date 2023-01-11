using API.Data;
using API.Entities;
using API.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.AppUsers;

public class List
{
    public class Query : IRequest<Result<List<AppUser>>>
    {
    }
    
    public class Handler : IRequestHandler<Query, Result<List<AppUser>>>
    {
        private readonly DataContext _context;

        public Handler(DataContext context)
        {
            _context = context;
        }
        
        public async Task<Result<List<AppUser>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var users = await _context.AppUsers.ToListAsync(cancellationToken: cancellationToken);
            return Result<List<AppUser>>.Success(users);
        }
    }
}