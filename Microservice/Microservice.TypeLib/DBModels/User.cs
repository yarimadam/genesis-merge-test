using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Microservice.TypeLib.DBModels
{
    [Keyless]
    [Table("user", Schema = "user_app")]
    public partial class User
    {
        [Column("id")]
        public Guid? Id { get; set; }
        [Column("email", TypeName = "character varying")]
        public string Email { get; set; }
    }
}
