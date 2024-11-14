namespace DoAn.Models
{
    public class CommentResponse
    {
        public int code { get; set; }
        public List<TblComment> comments { get; set; }
        public string msg { get; set; }
    }
}
