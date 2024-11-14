namespace DoAn.Models
{
    public class FeedBackResponse
    {
        public int code { get; set; }
        public string msg { get; set; }
        public List<TblFeedBack> feedbacks { get; set; }  // Danh sách phản hồi (nếu có)
    }
}
