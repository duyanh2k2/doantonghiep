using DoAn.Authen;
using DoAn.Models;
using DoAn.Services;
using DoAn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Twilio.TwiML.Voice;

namespace DoAn.Controllers
{
    [AuthenOtp]
    public class QuanLyController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        private readonly SmsService _smsService;
        public QuanLyController(DoAnTotNghiepContext context, SmsService smsService)
        {
            _context = context;
            _smsService = smsService;
        }
        [AuthenRole("2")]
        public IActionResult Index()
        {
            var idUs = HttpContext.Session.GetInt32("IdUser") ?? 0;
            if (idUs > 0)
            {
                var u = _context.TblUsers.Where(u=>u.IdUser == idUs).FirstOrDefault();
                if (u != null)
                {
                    var addRoom = new AddRoom
                    {
                        User = u,
                    };
                    return View(addRoom);
                }
            }
            return View();
        }
        [AuthenRole("2")]
        [HttpPost]
        public async Task<IActionResult> addRoomPost(AddRoom room, List<IFormFile> images) 
        {
            double dt = (double)((room.roomPost.DienTich!=null) ? room.roomPost.DienTich:0);
            double gt = (double)((room.roomPost.GiaTien != null) ? room.roomPost.GiaTien : 0);
            if (room.roomPost.DiaChi != "" && room.roomPost.TieuDe != "" && room.roomPost.MoTa != "" && dt > 0 && images != null && images.Count>0)
            {
                var idUs = HttpContext.Session.GetInt32("IdUser") ?? 0;
                var r = new TblRoomPost
                {
                    TieuDe = room.roomPost.TieuDe,
                    MoTa = room.roomPost.MoTa,
                    DiaChi = room.roomPost.DiaChi,
                    NgayDang = DateTime.Now,
                    GiaTien = gt,
                    DienTich = dt,
                    IdUser = idUs,
                };
                _context.TblRoomPosts.Add(r);
                _context.SaveChanges();
                
                var r1 = _context.TblRoomPosts.Where(r => r.TieuDe.Equals(room.roomPost.TieuDe) && r.MoTa.Equals(room.roomPost.MoTa) && r.DiaChi.Equals(room.roomPost.DiaChi)).FirstOrDefault();
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                foreach (var image in images)
                {
                    if (image.Length > 0)
                    {
                        var filePath = Path.Combine(uploadPath, image.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await image.CopyToAsync(stream);
                            var i = new TblImage
                            {
                                IdRoomPost = r1.IdRoomPost,
                                HinhAnh = $"/uploads/{image.FileName}",
                            };
                            _context.TblImages.Add(i);
                            _context.SaveChanges();
                        }
                    }
                }
                TempData["success"] = "Đăng bài thành công";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Đăng bài thất bại";
            return RedirectToAction("Index");
        }
        [AuthenRole("2")]
        public IActionResult PostManager()
        {
            return View();
        }
        [AuthenLogin]
        public IActionResult UserInfo()
        {
            var idUs = HttpContext.Session.GetInt32("IdUser")??0;
 
            var u = _context.TblUsers.Where(u=>u.IdUser==idUs).FirstOrDefault();

            return View(u);
        }
        [AuthenLogin]
        [HttpPost]
        public IActionResult UpdetInfo(TblUser user)
        {
            if(user.TaiKhoan!="" && user.Sdt!="" && user.HoTen!=""&& user.CanCuoc!="") 
            {
                var idUs = HttpContext.Session.GetInt32("IdUser") ?? 0;
                if (idUs > 0)
                {
                    var u = _context.TblUsers.Where(u => u.IdUser == idUs).FirstOrDefault();
                    if (u != null)
                    {
                        u.Sdt = user.Sdt;
                        u.HoTen = user.HoTen;
                        u.Gt=user.Gt;
                        u.CanCuoc = user.CanCuoc;
                        _context.TblUsers.Update(u);
                        _context.SaveChanges();
                        TempData["success"] = "Thay đổi thông tin thành công";
                        return RedirectToAction("UserInfo");
                    }
                }
            }
            TempData["error"] = "Thay đổi thông tin thất bại";
            return View();
        }
        [AuthenRole("2")]
        [HttpGet]
        public IActionResult updateRoomPost(int id)
        {
            if (id > 0)
            {
                var idUs = HttpContext.Session.GetInt32("IdUser") ?? 0;
                var r = _context.TblRoomPosts.Where(x=>x.IdRoomPost == id).FirstOrDefault();
                var i = _context.TblImages.Where(x => x.IdRoomPost == id).ToList();
                if (r != null && idUs>0)
                {
                    var u = _context.TblUsers.Where(u => u.IdUser == idUs).FirstOrDefault();
                    var ur = new updatePost
                    {
                        roomPost = r,
                        imageList = i,
                        User =u ,
                    };
                    return View(ur);
                }
            }
            return RedirectToAction("PostManager", "QuanLy");
        }
        [AuthenRole("2")]
        [HttpPost]
        public async Task<IActionResult> updateRoomPost(updatePost room)
        {
            double dt = (double)((room.roomPost.DienTich != null) ? room.roomPost.DienTich : 0);
            double gt = (double)((room.roomPost.GiaTien != null) ? room.roomPost.GiaTien : 0);
            if (room.roomPost.IdRoomPost>0 && room.roomPost.DiaChi != "" && room.roomPost.TieuDe != "" && room.roomPost.MoTa != "" && dt > 0)
            {
                var r = await _context.TblRoomPosts.FindAsync(room.roomPost.IdRoomPost);
                if (r!=null)
                {
                    r.TieuDe = room.roomPost.TieuDe;
                    r.MoTa = room.roomPost.MoTa;
                    r.DiaChi = room.roomPost.DiaChi;
                    r.DienTich = dt; // Cập nhật diện tích
                    r.GiaTien = gt;
                    await _context.SaveChangesAsync();
                    TempData["success"] = "Cập nhật thành công";
                    return RedirectToAction("PostManager","QuanLy");
                }
            }
            TempData["error"] = "Cập nhật thất bại";
            return RedirectToAction("PostManager","QuanLy");
        }
        [AuthenRole("2")]
        [HttpGet]
        public JsonResult loadRoomPost()
        {
            var idUs = HttpContext.Session.GetInt32("IdUser")??0;
            if (idUs > 0) 
            {
                var r = _context.TblRoomPosts.Where(r => r.IdUser == idUs).ToList();
                return Json(new { code = 200, r = r, msg = "Lấy dữ liệu thành công" });
            }
            return Json(new { code = 400, msg = "Lấy dữ liệu thất bại" });
        }
        [AuthenRole("2")]
        [HttpGet]
        public async Task<JsonResult> loadLess(int id)
        {
            if(id > 0)
            {
                var less = await _context.TblLessees.Where(i=>i.IdRoomPost == id).FirstOrDefaultAsync();
                if (less != null)
                {
                    return Json(new { code = 200,l=less, msg = "Phòng trọ đã có người thuê" });
                }
                else
                {
                    return Json(new { code = 201,msg = "Phòng trọ chưa có người thuê" });
                }
            }
            return Json(new { code = 202, msg = "Lấy dữ liệu thất bại"});
        }
        [AuthenRole("2")]
        [HttpPost]
        public async  Task<JsonResult> removeRoomPost(int id)
        {
            if (id > 0)
            {
                var r = await _context.TblRoomPosts.Where(r => r.IdRoomPost == id).FirstOrDefaultAsync();
                if (r != null)
                {
                    var l = await _context.TblLessees.FirstOrDefaultAsync(x => x.IdRoomPost == id);
                    if (l == null)
                    {
                        var i = await _context.TblImages.Where(i => i.IdRoomPost == r.IdRoomPost).ToListAsync();
                        var f = await _context.TblFavoritePosts.Where(i => i.IdRoomPost == r.IdRoomPost).ToListAsync();
                        _context.TblImages.RemoveRange(i);
                        _context.TblFavoritePosts.RemoveRange(f);
                        await _context.SaveChangesAsync();
                        _context.TblRoomPosts.Remove(r);
                        await _context.SaveChangesAsync();
                        return Json(new { code = 200, msg = "Xóa bài đăng thành công" });
                    }
                    else
                    {
                        return Json(new { code = 202, msg = "Vui lòng xóa người thuê" });
                    }
                }
            }
            return Json(new { code = 201, msg = "Xóa bài đăng thất bại" });
        }
        [AuthenRole("2")]
        [HttpGet]
        public async Task<JsonResult> loadUserLess(int idUs)
        {
            if(idUs > 0)
            {
                var u = await _context.TblUsers.Where(u=>u.IdUser==idUs).FirstOrDefaultAsync();
                return Json(new {code=200,u=u,msg="Lấy dữ liệu thành công"});
            }
            return Json(new { code = 201, msg = "Lấy dữ liệu thất bại" });
        }
        [AuthenRole("2")]
        [HttpPost]
        public JsonResult addLess(int idUs,int idRoomPost)
        {
            try
            {
                if (idUs > 0 && idRoomPost > 0)
                {
                    var l = _context.TblLessees.Where(x=>x.IdUser == idUs && x.IdRoomPost==idRoomPost).FirstOrDefault();
                    if (l == null)
                    {
                        var l1 = new TblLessee
                        {
                            IdUser = idUs,
                            IdRoomPost = idRoomPost,
                        };
                        _context.TblLessees.Add(l1);
                        _context.SaveChanges();
                        return Json(new { code = 200,l=l1, msg = "Thêm người thuê thành công" });
                    }
                }
                return Json(new { code = 201, msg = "Phòng này đã có người thuê" });
            }
            catch (Exception ex)
            {
                return Json(new { code=500, msg = ex.InnerException?.Message});
            }

        }
        [AuthenRole("2")]
        [HttpPost]
        public async Task<JsonResult> removeLess(int idLess, string otp)
        {
            if(idLess > 0)
            {
                var savedOtp = HttpContext.Session.GetString("Otp");
                var expireTimeString = HttpContext.Session.GetString("OtpTime");

                // Chuyển đổi expireTimeString thành DateTime
                if (DateTime.TryParse(expireTimeString, out var expireTime) &&
                    savedOtp == otp  && DateTime.Now <= expireTime)
                {
                    HttpContext.Session.Remove("Otp"); // Xóa OTP sau khi sử dụng
                    HttpContext.Session.Remove("OtpTime");
                    var l = await _context.TblLessees.Where(x => x.IdLessee == idLess).FirstOrDefaultAsync();
                    if (l != null)
                    {
                        _context.TblLessees.Remove(l);
                        await _context.SaveChangesAsync();
                        return Json(new { code = 200, msg = "Xóa người thuê thành công" });
                    }
                }
                return Json(new { code = 201, msg = "Mã xác nhận không đúng hoặc đã hết hạn" });

            }
            return Json(new { code = 202, msg = "Xóa người thuê thất bại" });
        }
        [HttpGet]
        public async Task<JsonResult> sendOtp(int id)
        {
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Lưu OTP vào TempData, Session hoặc cơ sở dữ liệu
            HttpContext.Session.SetString("Otp", otpCode);
            var time = DateTime.Now.AddMinutes(1).ToString();
            HttpContext.Session.SetString("OtpTime", DateTime.Now.AddMinutes(1).ToString()); // OTP hết hạn sau 5 phút

            // Số điện thoại của người dùng
            var phoneNumber = "+84398759537";  // Thay bằng số điện thoại người dùng

            // Gửi OTP qua SMS hoặc email (ví dụ với SMS)
            await _smsService.SendSmsAsync(phoneNumber, $"Mã xác nhận của bạn là: {otpCode}");
            return Json(new { code = 200, otp = otpCode,time=time, id=id, msg = "Gửi mã xác nhận thành công" });
        }
        [AuthenRole("2")]
        [HttpPost]
        public async Task<IActionResult> UploadSingleImage(IFormFile image, int id)
        {
            if (image == null || image.Length == 0)
            {
                return Json(new { success = false, message = "No image uploaded." });
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = Path.GetFileName(image.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // Lưu đường dẫn vào cơ sở dữ liệu
            var imageUpload = new TblImage
            {
                HinhAnh = $"/uploads/{fileName}",
                IdRoomPost = id,
            };
            _context.TblImages.Add(imageUpload);
            await _context.SaveChangesAsync();

            return Json(new { success = true, imagePath = $"/uploads/{fileName}",idImage= imageUpload.IdImage});
        }
        [AuthenRole("2")]
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int id)
        {
            if (id<0)
            {
                return Json(new { success = false, message = "Hình ảnh không tồn tại" });
            }

            try
            {
                var img = await _context.TblImages.Where(x=>x.IdImage == id).FirstOrDefaultAsync();
                //var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", img.HinhAnh.TrimStart('/'));
                //if (System.IO.File.Exists(filePath))
                //{
                //    System.IO.File.Delete(filePath); // Xóa file trên server

                // Xóa thông tin khỏi cơ sở dữ liệu nếu cần
                if (img != null)
                {
                    _context.TblImages.Remove(img);
                    await _context.SaveChangesAsync(true);
                    return Json(new { success = true });
                }
                return Json(new { success = false });
                //}
                //else
                //{
                //    return Json(new { success = false, message = "Không tìm thấy ảnh" });
                //}
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error deleting image: " + ex.Message });
            }
        }
    }
}
