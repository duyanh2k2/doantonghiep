using System;
using System.Collections.Generic;

namespace DoAn.Models
{
    public partial class TblRoomPost
    {
        public TblRoomPost()
        {
            TblComments = new HashSet<TblComment>();
            TblFavoritePosts = new HashSet<TblFavoritePost>();
            TblImages = new HashSet<TblImage>();
            TblLessees = new HashSet<TblLessee>();
        }

        public int IdRoomPost { get; set; }
        public string TieuDe { get; set; } = null!;
        public string MoTa { get; set; } = null!;
        public DateTime NgayDang { get; set; }
        public double? GiaTien { get; set; }
        public double? DienTich { get; set; }
        public int IdUser { get; set; }
        public string DiaChi { get; set; } = null!;

        public virtual TblUser IdUserNavigation { get; set; } = null!;
        public virtual ICollection<TblComment> TblComments { get; set; }
        public virtual ICollection<TblFavoritePost> TblFavoritePosts { get; set; }
        public virtual ICollection<TblImage> TblImages { get; set; }
        public virtual ICollection<TblLessee> TblLessees { get; set; }
    }
}
