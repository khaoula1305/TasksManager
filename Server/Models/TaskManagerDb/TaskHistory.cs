using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Server.Models.TaskManagerDb
{
    [Table("TaskHistory", Schema = "dbo")]
    public partial class TaskHistory
    {
        [Key]
        public Guid HistoryID { get; set; }

        public Guid? TaskID { get; set; }

        [Column("Action")]
        public string Action1 { get; set; }

        public Guid? UserID { get; set; }

        public DateTime? Timestamp { get; set; }

        public string DescriptionOfChange { get; set; }

        public Task Task { get; set; }

        public User User { get; set; }

    }
}