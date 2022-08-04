using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace ApiManagerStudent.EF
{
    public partial class UserRefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string IpAddress { get; set; }
        public bool IsInvalidated { get; set; }
        public int TeacherId { get; set; }
        [NotMapped]
        public bool IsActive
        {
            get
            {
                return ExpirationDate > DateTime.UtcNow;
            }
        }
        public virtual Teacher Teacher { get; set; }
    }
}
