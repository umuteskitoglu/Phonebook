using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports.Queries
{
    public class GetReportByIdQuery : IRequest<Result<Report>>
    {
        public Guid Id { get; set; }
    }

    public class GetReportByIdQueryHandler : IRequestHandler<GetReportByIdQuery, Result<Report>>
    {
        private readonly IPhonebookDbContext _context;

        public GetReportByIdQueryHandler(IPhonebookDbContext context)
        {
            _context = context;
        }

        public async Task<Result<Report>> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var report = await _context.Reports
                    .Include(r => r.Details)
                    .FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);

                if (report == null)
                {
                    return new Result<Report>(false, $"Report with ID {request.Id} not found", null);
                }

                return new Result<Report>(true, "Report fetched successfully", report);
            }
            catch (Exception ex)
            {
                return new Result<Report>(false, $"Error fetching report: {ex.Message}", null);
            }
        }
    }
} 