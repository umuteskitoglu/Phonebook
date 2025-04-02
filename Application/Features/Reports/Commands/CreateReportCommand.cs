using MediatR;
using Application.Interfaces;
using Domain.Entities;
using Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Application.Features.Reports.Models;

namespace Application.Features.Reports.Commands
{
    public class CreateReportCommand : IRequest<Result<Guid>>
    {
    }

    public class CreateReportCommandHandler : IRequestHandler<CreateReportCommand, Result<Guid>>
    {
        private readonly IPhonebookDbContext _context;
        private readonly IMessageService _messageService;
        private const string REPORT_QUEUE = "report-processing";

        public CreateReportCommandHandler(IPhonebookDbContext context, IMessageService messageService)
        {
            _context = context;
            _messageService = messageService;
        }

        public async Task<Result<Guid>> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {

            try
            {
                // Create a new report
                var report = new Report();
                report.RequestedAt = DateTime.UtcNow;
                report.Status = ReportStatus.Preparing;
                report.Details = new List<ReportDetail>();

                // Save the report with details
                await _context.Reports.AddAsync(report, cancellationToken);
                var a = await _context.SaveChangesAsync(cancellationToken);

                // Create and send the message to RabbitMQ
                var message = new ReportProcessingMessage
                {
                    ReportId = report.Id,
                    RequestedAt = report.RequestedAt
                };

                await _messageService.PublishAsync(REPORT_QUEUE, message);

                return new Result<Guid>(true, "Report created successfully", report.Id);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
