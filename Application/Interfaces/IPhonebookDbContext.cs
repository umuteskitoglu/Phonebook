using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Interfaces
{
    public interface IPhonebookDbContext
    {
        DbSet<Contact> Contacts { get; set; }
        DbSet<ContactInformation> ContactInformation { get; set; }
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
} 