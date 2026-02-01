using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Algora.Domain.Enums;
using Microsoft.AspNetCore.Http;
using Algora.Application.Persistence;
using Microsoft.AspNetCore.Http;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Algora.Application.Features.Tasks;

public record GetMyTasksQuery : IRequest<TasksListResponse>;

public record TasksListResponse(List<TaskResponse> Tasks);

public class GetMyTasksHandler : IRequestHandler<GetMyTasksQuery, TasksListResponse>
{
    private readonly AlgoraDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetMyTasksHandler(AlgoraDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TasksListResponse> Handle(GetMyTasksQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = Guid.Parse(_httpContextAccessor.HttpContext!.User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var tasks = await _context.UserTasks
            .Where(t => t.AssignedToStudentId == currentUserId)
            .OrderByDescending(t => t.CreatedAt)
            .Select(t => new TaskResponse(t.Id, t.Title, t.Status, t.DueDate, t.RequiredCount))
            .ToListAsync(cancellationToken);

        return new TasksListResponse(tasks);
    }
}







