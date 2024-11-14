namespace DoAn.Models
{
    public class addCommentResponse
    {
        public int code { get; set; }
        public List<TblComment> comments { get; set; }
        public string msg { get; set; }
    }
}

