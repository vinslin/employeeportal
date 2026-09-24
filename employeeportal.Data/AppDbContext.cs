using employeeportal.Entity.Entities;
using Microsoft.EntityFrameworkCore;

namespace employeeportal.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<EmployeeProject> EmployeeProjects => Set<EmployeeProject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>(builder =>
        {
            builder.ToTable("Employees");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnType("varchar(100)").IsRequired();
        });

        modelBuilder.Entity<Project>(builder =>
        {
            builder.ToTable("Project");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnType("varchar(200)");
            builder.Property(x => x.Summary).HasColumnType("varchar(500)");
            builder.Property(x => x.Client).HasColumnType("varchar(200)");
        });

        modelBuilder.Entity<EmployeeProject>(builder =>
        {
            builder.ToTable("EmployeeProject");
            builder.HasKey(x => new { x.EmployeeId, x.ProjectId });

            builder.HasOne(x => x.Employee)
                .WithMany(x => x.EmployeeProjects)
                .HasForeignKey(x => x.EmployeeId)
                .HasConstraintName("FK_EmployeeProject_Employee");

            builder.HasOne(x => x.Project)
                .WithMany(x => x.EmployeeProjects)
                .HasForeignKey(x => x.ProjectId)
                .HasConstraintName("FK_EmployeeProject_Project");
        });

        base.OnModelCreating(modelBuilder);
    }
}
