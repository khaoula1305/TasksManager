using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Server.Models.TaskManagerDb
{
    [Table("TaskSubPoints", Schema = "dbo")]
    public partial class TaskSubPoint
    {
        [Key]
        public Guid SubPointID { get; set; }

        public Guid? TaskID { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public Task Task { get; set; }

    }
}