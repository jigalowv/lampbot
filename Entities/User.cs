using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace lampbot.Entities
{
    [Table("user")]
    public class User
    {
        [Key]
        [Column("u_id")]
        public ulong Id { get; set; }

        [Column("u_name")]
        public string Name { get; set; } = string.Empty;

        [Column("u_role")]
        public Role Role { get; set; } = Role.User;
    }
}