using System;
using System.Collections.Generic;

namespace DoAn.Models
{
    public partial class TblUser
    {
        public TblUser()
        {
            TblComments = new HashSet<TblComment>();
            TblFavoritePosts = new HashSet<TblFavoritePost>();
            TblFeedBacks = new HashSet<TblFeedBack>();
            TblRoomPosts = new HashSet<TblRoomPost>();
            TblSupports = new HashSet<TblSupport>();
        }

        public int IdUser { get; set; }
        public string TaiKhoan { get; set; } = null!;
        public string MatKhau { get; set; } = null!;
        public string Sdt { get; set; } = null!;
        public string HoTen { get; set; } = null!;
        public bool Gt { get; set; }
        public string CanCuoc { get; set; } = null!;
        public int IdRole { get; set; }

        public virtual TblRole IdRoleNavigation { get; set; } = null!;
        public virtual TblLessee? TblLessee { get; set; }
        public virtual ICollection<TblComment> TblComments { get; set; }
        public virtual ICollection<TblFavoritePost> TblFavoritePosts { get; set; }
        public virtual ICollection<TblFeedBack> TblFeedBacks { get; set; }
        public virtual ICollection<TblRoomPost> TblRoomPosts { get; set; }
        public virtual ICollection<TblSupport> TblSupports { get; set; }
    }
}
