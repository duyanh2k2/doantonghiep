using System;
using System.Collections.Generic;

namespace DoAn.Models
{
    public partial class TblFeedBack
    {
        public int IdFeedBack { get; set; }
        public string NoiDung { get; set; } = null!;
        public int IdUser { get; set; }
        public int IdComment { get; set; }

        public virtual TblComment IdCommentNavigation { get; set; } = null!;
        public virtual TblUser IdUserNavigation { get; set; } = null!;
    }
}
