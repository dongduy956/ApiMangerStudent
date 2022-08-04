using System;
using System.Collections.Generic;

#nullable disable

namespace ApiManagerStudent.EF
{
    public partial class Teacher
    {
        public Teacher()
        {
            UserRefreshTokens = new HashSet<UserRefreshToken>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Image { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int? IdRole { get; set; }

        public virtual Role IdRoleNavigation { get; set; }
        public virtual ICollection<UserRefreshToken> UserRefreshTokens { get; set; }
    }
}
