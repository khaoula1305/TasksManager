using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Server.Models.TaskManagerDb
{
    [Table("Tasks", Schema = "dbo")]
    public partial class Task
    {
        [Key]
        public Guid TaskID { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid? CreatorUserID { get; set; }

        public Guid? AssignedToUserID { get; set; }

        public string Status { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? DueDate { get; set; }

        public int? Priority { get; set; }

        public ICollection<Notification> Notifications { get; set; }

        public ICollection<TaskHistory> TaskHistories { get; set; }

        public User User { get; set; }

        public User User1 { get; set; }

        public ICollection<TaskSubPoint> TaskSubPoints { get; set; }

        public ICollection<TimeLog> TimeLogs { get; set; }

    }
}