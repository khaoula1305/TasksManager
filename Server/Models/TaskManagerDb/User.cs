using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Server.Models.TaskManagerDb
{
    [Table("Users", Schema = "dbo")]
    public partial class User
    {
        [Key]
        public Guid UserID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        public string Email { get; set; }

        public Guid? RoleID { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public ICollection<TaskHistory> TaskHistories { get; set; }

        public ICollection<Task> Tasks { get; set; }

        public ICollection<Task> Tasks1 { get; set; }

        public ICollection<TimeLog> TimeLogs { get; set; }

        public Role Role { get; set; }

    }
}