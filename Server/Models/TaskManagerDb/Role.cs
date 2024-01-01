using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TaskManager.Server.Models.TaskManagerDb
{
    [Table("Roles", Schema = "dbo")]
    public partial class Role
    {
        [Key]
        public Guid RoleID { get; set; }

        [Required]
        public string RoleName { get; set; }

        public ICollection<User> Users { get; set; }

    }
}