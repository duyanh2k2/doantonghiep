namespace DoAn.Models
{
    public class LoadUserResponse
    {
        public int code { get; set; }
        public List<User> u { get; set; }
        public string msg { get; set; }
    }

    public class User
    {
        public int IdUser { get; set; }
        public string HoTen { get; set; }
    }
}
