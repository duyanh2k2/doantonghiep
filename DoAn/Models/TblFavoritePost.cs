﻿using System;
using System.Collections.Generic;

namespace DoAn.Models
{
    public partial class TblFavoritePost
    {
        public int IdFavoritePost { get; set; }
        public int IdUser { get; set; }
        public int IdRoomPost { get; set; }

        public virtual TblRoomPost IdRoomPostNavigation { get; set; } = null!;
        public virtual TblUser IdUserNavigation { get; set; } = null!;
    }
}
