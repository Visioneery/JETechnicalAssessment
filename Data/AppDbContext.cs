using Microsoft.EntityFrameworkCore;

namespace JETechnicalAssessment.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();
}