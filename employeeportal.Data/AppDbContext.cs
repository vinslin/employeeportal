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

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

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

        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.UserName).HasColumnType("varchar(100)").IsRequired();
            builder.Property(x => x.Email).HasColumnType("varchar(200)").IsRequired();
            builder.Property(x => x.PasswordHash).HasColumnType("varchar(500)").IsRequired();
            builder.Property(x => x.IsActive).HasDefaultValue(true).IsRequired();
            builder.Property(x => x.CreatedDate).HasDefaultValueSql("SYSUTCDATETIME()").IsRequired();

            builder.HasIndex(x => x.UserName).IsUnique().HasDatabaseName("UQ_Users_UserName");
            builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("UQ_Users_Email");
        });

        modelBuilder.Entity<Role>(builder =>
        {
            builder.ToTable("Roles");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasColumnType("varchar(50)").IsRequired();
            builder.Property(x => x.Description).HasColumnType("varchar(250)");

            builder.HasIndex(x => x.Name).IsUnique().HasDatabaseName("UQ_Roles_Name");
        });

        modelBuilder.Entity<UserRole>(builder =>
        {
            builder.ToTable("UserRoles");
            builder.HasKey(x => new { x.UserId, x.RoleId });

            builder.HasOne(x => x.User)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_UserRoles_Users");

            builder.HasOne(x => x.Role)
                .WithMany(x => x.UserRoles)
                .HasForeignKey(x => x.RoleId)
                .HasConstraintName("FK_UserRoles_Roles");
        });

        base.OnModelCreating(modelBuilder);
    }
}
