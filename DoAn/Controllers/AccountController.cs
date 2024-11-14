using DoAn.Authen;
using DoAn.Models;
using DoAn.Services;
using Microsoft.AspNetCore.Mvc;
using Twilio.TwiML.Messaging;

namespace DoAn.Controllers
{
    public class AccountController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly SmsService _smsService;
        public AccountController(DoAnTotNghiepContext context, SmsService smsService)
        {
            _context = context;
            _smsService = smsService;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(TblUser user)
        {
            if(user.TaiKhoan !="" && user.MatKhau !="")
            {
                var u1 = _context.TblUsers.Where(u=>u.TaiKhoan.Equals(user.TaiKhoan) && u.MatKhau.Equals(user.MatKhau)).FirstOrDefault();
                if (u1 != null)
                {
                    HttpContext.Session.SetString("Name", u1.HoTen.ToString());
                    HttpContext.Session.SetInt32("Role", u1.IdRole);
                    HttpContext.Session.SetInt32("IdUser", u1.IdUser);
                    //return RedirectToAction("SendOtp");
                    if (u1.IdRole == 3)
                    {
                        return RedirectToAction("Index", "AdminUser");
                    }
                    else
                    {
                        return RedirectToAction("SendOtp");
                    }
                }
                //ViewBag.TaiKhoan = "Tài khoản mật khẩu không chính xác";
                TempData["error"] = "Tài khoản mật khẩu không chính xác";
            }
            return View("Index");
        }
        public IActionResult Logout() {
            HttpContext.Session.Clear();
            HttpContext.Session.Remove("Name");
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("IdUser");
            HttpContext.Session.Remove("OTP_Verified");
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> SendOtp()
        {
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Lưu OTP vào TempData, Session hoặc cơ sở dữ liệu
            HttpContext.Session.SetString("OtpCode", otpCode);
            HttpContext.Session.SetString("OtpExpireTime", DateTime.Now.AddMinutes(1).ToString()); // OTP hết hạn sau 5 phút

            // Số điện thoại của người dùng
            var phoneNumber = "+84398759537";  // Thay bằng số điện thoại người dùng

            // Gửi OTP qua SMS hoặc email (ví dụ với SMS)
            await _smsService.SendSmsAsync(phoneNumber, $"Mã OTP của bạn là: {otpCode}");

            // Chuyển hướng người dùng đến trang nhập mã OTP
            return RedirectToAction("Otp");
        }
        [HttpGet]
        public IActionResult Otp()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Otp(string otpcode)
        {
            var savedOtp = HttpContext.Session.GetString("OtpCode");
            var expireTimeString = HttpContext.Session.GetString("OtpExpireTime");

            // Chuyển đổi expireTimeString thành DateTime
            if (DateTime.TryParse(expireTimeString, out var expireTime) &&
                savedOtp == otpcode && DateTime.Now <= expireTime)
            {
                // OTP đúng và chưa hết hạn, xác minh thành công
                HttpContext.Session.Remove("OtpCode"); // Xóa OTP sau khi sử dụng
                HttpContext.Session.Remove("OtpExpireTime");
                HttpContext.Session.SetString("OTP_Verified", "true");
                var role = HttpContext.Session.GetInt32("Role");
                if (role == 1) 
                {
                    return RedirectToAction("Index","Home");
                }
                else
                {

                    return RedirectToAction("PostManager","QuanLy");
                }

            }
            else
            {
                TempData["error"] = "Mã otp không chính xác hoặc đã hết hiệu lực";
                return View();
            }
        }
        [HttpGet]
        public IActionResult DangKy()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangKy(TblUser user)
        {
            if (user.TaiKhoan != "" && user.MatKhau != "" && user.MatKhau.Length>=8 && user.Sdt != "" && user.HoTen != "" && user.Gt != null && user.CanCuoc !="" && user.IdRole!=0) 
            {
                var u1= _context.TblUsers.FirstOrDefault(u => u.TaiKhoan.Equals(user.TaiKhoan));
                if (u1==null)
                {
                    _context.TblUsers.Add(user);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Đăng ký tài khoản thành công!";
                    return RedirectToAction("Index");
                }
                    ViewBag.message = "Tài khoản đã tồn tại";
            }
            return View();
        }
        [AuthenLogin]
        [AuthenOtp]
        [HttpGet]
        public IActionResult UpdatePass()
        {
            var id = HttpContext.Session.GetInt32("IdUser");
            ViewBag.id = id;
            return View();
        }
        [AuthenLogin]
        [AuthenOtp]
        [HttpPost]
        public IActionResult UpdatePass(int IdUser, string currentPassword, string MatKhau)
        {
            if(IdUser > 0 && MatKhau != "" && MatKhau.Length>=8 && currentPassword !="" && currentPassword.Equals(MatKhau)==false)
            {
                var p = _context.TblUsers.Where(u=>u.IdUser==IdUser && u.MatKhau.Equals(currentPassword)).FirstOrDefault();
                if (p != null)
                { 
                    p.MatKhau = MatKhau;
                    _context.TblUsers.Update(p);
                    _context.SaveChanges(true);
                    TempData["Success"] = "Đổi mật khẩu thành công.";
                    return RedirectToAction("UpdatePass");
                }
            }
            TempData["Error"] = "Đổi mật khẩu thất bại.";
            return View();
        }
        private string GenerateRandomString(int length)
        {
            string guid = Guid.NewGuid().ToString("N");
            return guid.Substring(0, length);
        }
        [HttpGet]
        public IActionResult RecoverPass()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RecoverPass(string User)
        {
            if (User != "")
            {
                var u = _context.TblUsers.Where(u=>u.TaiKhoan.Equals(User)).FirstOrDefault();
                if(u != null)
                {
                    var phoneNumber = "+84398759537";
                    var mess = GenerateRandomString(8);
                    u.MatKhau = mess;
                    _context.TblUsers.Update(u);
                    _context.SaveChanges(true);
                    await _smsService.SendSmsAsync(phoneNumber, $"Mật khẩu của bạn là: {mess}");
                    TempData["success"] = "Lấy lại mật khẩu thành công";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "Vui lòng nhập đúng tên tài khoản của bạn";
            return View();
        }
    }
}
