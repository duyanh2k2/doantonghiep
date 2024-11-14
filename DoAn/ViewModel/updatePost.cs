using DoAn.Models;

namespace DoAn.ViewModel
{
    public class updatePost
    {
        public TblRoomPost roomPost { get; set; }
        public List<TblImage> imageList { get; set; }
        public TblUser User { get; set; }
    }
}
