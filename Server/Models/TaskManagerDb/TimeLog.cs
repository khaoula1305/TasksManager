using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Server.Models.TaskManagerDb
{
    [Table("TimeLogs", Schema = "dbo")]
    public partial class TimeLog
    {
        [Key]
        public Guid TimeLogID { get; set; }

        public Guid? UserID { get; set; }

        public Guid? TaskID { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? TotalTime { get; set; }

        public Task Task { get; set; }

        public User User { get; set; }

    }
}