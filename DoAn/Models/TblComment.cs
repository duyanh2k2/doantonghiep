using System;
using System.Collections.Generic;

namespace DoAn.Models
{
    public partial class TblComment
    {
        public TblComment()
        {
            TblFeedBacks = new HashSet<TblFeedBack>();
        }

        public int IdComment { get; set; }
        public string NoiDung { get; set; } = null!;
        public int IdUser { get; set; }
        public int IdRoomPost { get; set; }

        public virtual TblRoomPost IdRoomPostNavigation { get; set; } = null!;
        public virtual TblUser IdUserNavigation { get; set; } = null!;
        public virtual ICollection<TblFeedBack> TblFeedBacks { get; set; }
    }
}
