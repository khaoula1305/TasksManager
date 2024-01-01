using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Server.Models.TaskManagerDb
{
    [Table("Notifications", Schema = "dbo")]
    public partial class Notification
    {
        [Key]
        public Guid NotificationID { get; set; }

        public Guid? UserID { get; set; }

        public Guid? TaskID { get; set; }

        public string NotificationType { get; set; }

        public string NotificationText { get; set; }

        public DateTime? DateCreated { get; set; }

        public bool? IsRead { get; set; }

        public Task Task { get; set; }

        public User User { get; set; }

    }
}