using System;
using System.Collections.Generic;

namespace DoAn.Models
{
    public partial class TblRole
    {
        public TblRole()
        {
            TblUsers = new HashSet<TblUser>();
        }

        public int IdRole { get; set; }
        public string SRole { get; set; } = null!;

        public virtual ICollection<TblUser> TblUsers { get; set; }
    }
}
