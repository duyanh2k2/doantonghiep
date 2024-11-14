using System;
using System.Collections.Generic;

namespace DoAn.Models
{
    public partial class TblImage
    {
        public int IdImage { get; set; }
        public int IdRoomPost { get; set; }
        public string HinhAnh { get; set; } = null!;

        public virtual TblRoomPost IdRoomPostNavigation { get; set; } = null!;
    }
}
