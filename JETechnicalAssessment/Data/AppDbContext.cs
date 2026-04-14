using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace JETechnicalAssessment.Data;

[ExcludeFromCodeCoverage]
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<SearchHistory> SearchHistories => Set<SearchHistory>();
}