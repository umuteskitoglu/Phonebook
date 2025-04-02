using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports.Queries
{
    public class GetReportsQuery : IRequest<Result<List<Report>>>
    {
    }

    public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, Result<List<Report>>>
    {
        private readonly IPhonebookDbContext _context;

        public GetReportsQueryHandler(IPhonebookDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<Report>>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var reports = await _context.Reports
                    .OrderByDescending(r => r.RequestedAt)
                    .ToListAsync(cancellationToken);

                return new Result<List<Report>>(true, "Reports fetched successfully", reports);
            }
            catch (System.Exception ex)
            {
                return new Result<List<Report>>(false, $"Error fetching reports: {ex.Message}", null);
            }
        }
    }
}

