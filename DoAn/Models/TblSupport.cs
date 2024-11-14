using System;
using System.Collections.Generic;

namespace DoAn.Models
{
    public partial class TblSupport
    {
        public int IdSupport { get; set; }
        public string TuKhoa { get; set; } = null!;
        public string TraLoi { get; set; } = null!;
        public int IdUser { get; set; }

        public virtual TblUser IdUserNavigation { get; set; } = null!;
    }
}
