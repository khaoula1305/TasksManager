using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using TaskManager.Server.Models.TaskManagerDb;

namespace TaskManager.Server.Data
{
    public partial class TaskManagerDbContext : DbContext
    {
        public TaskManagerDbContext()
        {
        }

        public TaskManagerDbContext(DbContextOptions<TaskManagerDbContext> options) : base(options)
        {
        }

        partial void OnModelBuilding(ModelBuilder builder);

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Notification>()
              .HasOne(i => i.Task)
              .WithMany(i => i.Notifications)
              .HasForeignKey(i => i.TaskID)
              .HasPrincipalKey(i => i.TaskID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Notification>()
              .HasOne(i => i.User)
              .WithMany(i => i.Notifications)
              .HasForeignKey(i => i.UserID)
              .HasPrincipalKey(i => i.UserID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TaskHistory>()
              .HasOne(i => i.Task)
              .WithMany(i => i.TaskHistories)
              .HasForeignKey(i => i.TaskID)
              .HasPrincipalKey(i => i.TaskID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TaskHistory>()
              .HasOne(i => i.User)
              .WithMany(i => i.TaskHistories)
              .HasForeignKey(i => i.UserID)
              .HasPrincipalKey(i => i.UserID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Task>()
              .HasOne(i => i.User)
              .WithMany(i => i.Tasks)
              .HasForeignKey(i => i.AssignedToUserID)
              .HasPrincipalKey(i => i.UserID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Task>()
              .HasOne(i => i.User1)
              .WithMany(i => i.Tasks1)
              .HasForeignKey(i => i.CreatorUserID)
              .HasPrincipalKey(i => i.UserID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>()
              .HasOne(i => i.Task)
              .WithMany(i => i.TaskSubPoints)
              .HasForeignKey(i => i.TaskID)
              .HasPrincipalKey(i => i.TaskID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TimeLog>()
              .HasOne(i => i.Task)
              .WithMany(i => i.TimeLogs)
              .HasForeignKey(i => i.TaskID)
              .HasPrincipalKey(i => i.TaskID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TimeLog>()
              .HasOne(i => i.User)
              .WithMany(i => i.TimeLogs)
              .HasForeignKey(i => i.UserID)
              .HasPrincipalKey(i => i.UserID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.User>()
              .HasOne(i => i.Role)
              .WithMany(i => i.Users)
              .HasForeignKey(i => i.RoleID)
              .HasPrincipalKey(i => i.RoleID);

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Notification>()
              .Property(p => p.NotificationID)
              .HasDefaultValueSql(@"(newid())");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Role>()
              .Property(p => p.RoleID)
              .HasDefaultValueSql(@"(newid())");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TaskHistory>()
              .Property(p => p.HistoryID)
              .HasDefaultValueSql(@"(newid())");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Task>()
              .Property(p => p.TaskID)
              .HasDefaultValueSql(@"(newid())");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint>()
              .Property(p => p.SubPointID)
              .HasDefaultValueSql(@"(newid())");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TimeLog>()
              .Property(p => p.TimeLogID)
              .HasDefaultValueSql(@"(newid())");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.User>()
              .Property(p => p.UserID)
              .HasDefaultValueSql(@"(newid())");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Notification>()
              .Property(p => p.DateCreated)
              .HasColumnType("datetime");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TaskHistory>()
              .Property(p => p.Timestamp)
              .HasColumnType("datetime");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Task>()
              .Property(p => p.CreationDate)
              .HasColumnType("datetime");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.Task>()
              .Property(p => p.DueDate)
              .HasColumnType("datetime");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TimeLog>()
              .Property(p => p.StartTime)
              .HasColumnType("datetime");

            builder.Entity<TaskManager.Server.Models.TaskManagerDb.TimeLog>()
              .Property(p => p.EndTime)
              .HasColumnType("datetime");
            this.OnModelBuilding(builder);
        }

        public DbSet<TaskManager.Server.Models.TaskManagerDb.Notification> Notifications { get; set; }

        public DbSet<TaskManager.Server.Models.TaskManagerDb.Role> Roles { get; set; }

        public DbSet<TaskManager.Server.Models.TaskManagerDb.TaskHistory> TaskHistories { get; set; }

        public DbSet<TaskManager.Server.Models.TaskManagerDb.Task> Tasks { get; set; }

        public DbSet<TaskManager.Server.Models.TaskManagerDb.TaskSubPoint> TaskSubPoints { get; set; }

        public DbSet<TaskManager.Server.Models.TaskManagerDb.TimeLog> TimeLogs { get; set; }

        public DbSet<TaskManager.Server.Models.TaskManagerDb.User> Users { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new BlankTriggerAddingConvention());
        }
    
    }
}