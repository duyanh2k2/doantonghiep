using DoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static System.Net.Mime.MediaTypeNames;
using Twilio.TwiML.Voice;
using Microsoft.EntityFrameworkCore;
using DoAn.Authen;

namespace DoAn.Controllers
{
    [AuthenRole("3")]
    public class AdminRoomPostController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        public AdminRoomPostController(DoAnTotNghiepContext context)
        {
            _context = context;
        }
        
        public IActionResult Index()
        {
            var posts = _context.TblRoomPosts.ToList();
            return View(posts);
        }
        public IActionResult Create()
        {
            var users = _context.TblUsers.Select(u => new
            {
                Id = u.IdUser,
                Name = u.HoTen
            }).ToList();

            // Tạo RoomPostViewModel và thêm danh sách người dùng vào ViewBag
            ViewBag.Users = new SelectList(users, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TblRoomPost model, List<IFormFile> image)
        {
            if (model.DiaChi != "" && model.TieuDe != "" && model.MoTa != "" && model.GiaTien > 0 && model.DienTich>0 && image!=null && image.Count>0)
            {
                // Xử lý dữ liệu và lưu vào database
                var roomPost = new TblRoomPost
                {
                    TieuDe = model.TieuDe,
                    MoTa = model.MoTa,
                    NgayDang = DateTime.Now,
                    GiaTien = model.GiaTien,
                    DienTich = model.DienTich,
                    DiaChi = model.DiaChi,
                    IdUser = model.IdUser
                };
                _context.TblRoomPosts.Add(roomPost);
                _context.SaveChanges();
                // Xử lý hình ảnh
                    foreach (var file in image)
                    {
                        // Lưu ảnh (có thể lưu vào thư mục hoặc database)
                        // Ví dụ: lưu vào thư mục "wwwroot/images"
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        // Lưu thông tin ảnh vào database
                        var roomImage = new TblImage
                        {
                            IdRoomPost = roomPost.IdRoomPost, // Giả sử bạn đã có IdRoomPost khi tạo bài viết
                            HinhAnh = $"/uploads/{file.FileName}"
                        };
                        _context.TblImages.Add(roomImage);
                        _context.SaveChanges();
                    }
                TempData["SuccessMessage"] = "Bài viết đã được thêm thành công!";
                return RedirectToAction(nameof(Index)); // Chuyển hướng đến danh sách bài viết sau khi lưu thành công
            }
            // Nếu model không hợp lệ, hiển thị lại form
            ViewBag.Users = new SelectList(_context.TblUsers, "IdUser", "HoTen", model.IdUser);
            return View(model);
        }
        public async Task <IActionResult> Edit(int id)
        {
            var roomPost = await _context.TblRoomPosts.Include(x=>x.TblImages).FirstOrDefaultAsync(x=>x.IdRoomPost==id);
            if (roomPost == null)
            {
                TempData["ErrorMessage"] = "Bài viết không tồn tại.";
                return RedirectToAction("Index");
            }
            ViewBag.Users = new SelectList(_context.TblUsers, "IdUser", "HoTen");
            return View(roomPost);
        }
        private string SaveImage(IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);
            var filePath = Path.Combine("wwwroot/uploads", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            return "/uploads/" + fileName;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TblRoomPost model, List<IFormFile> NewImages, List<int> ExistingImages)
        {
            if (id != model.IdRoomPost)
            {
                TempData["ErrorMessage"] = "Bài viết không tồn tại.";
                return RedirectToAction("Index");
            }

            if (model.DiaChi != "" && model.TieuDe != "" && model.MoTa != "" && model.GiaTien > 0 && model.DienTich > 0)
            {
                try
                {
                    var existingRoomPost = _context.TblRoomPosts
                        .Include(rp => rp.TblImages)
                        .FirstOrDefault(rp => rp.IdRoomPost == id);

                    if (existingRoomPost == null)
                    {
                        TempData["ErrorMessage"] = "Bài viết không tồn tại.";
                        return RedirectToAction("Index");
                    }

                    existingRoomPost.TieuDe = model.TieuDe;
                    existingRoomPost.MoTa = model.MoTa;
                    existingRoomPost.NgayDang = model.NgayDang;
                    existingRoomPost.GiaTien = model.GiaTien;
                    existingRoomPost.DienTich = model.DienTich;
                    existingRoomPost.DiaChi = model.DiaChi;
                    existingRoomPost.IdUser = model.IdUser;

                    var imagesToRemove = existingRoomPost.TblImages
                        .Where(img => !ExistingImages.Contains(img.IdImage))
                        .ToList();
                    _context.TblImages.RemoveRange(imagesToRemove);

                    foreach (var file in NewImages)
                    {
                        if (file.Length > 0)
                        {
                            var newImage = new TblImage
                            {
                                IdRoomPost = id,
                                HinhAnh = SaveImage(file)
                            };
                            existingRoomPost.TblImages.Add(newImage);
                        }
                    }

                    _context.Update(existingRoomPost);
                    _context.SaveChanges();

                    TempData["SuccessMessage"] = "Cập nhật thành công!";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Đã có lỗi xảy ra.";
                    return View(model);
                }
            }
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            var roomPost = _context.TblRoomPosts.FirstOrDefault(r => r.IdRoomPost == id);
            if (roomPost == null)
            {
                return NotFound();
            }
            return View(roomPost);
        }

        // Action POST: Thực hiện xóa
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var roomPost = _context.TblRoomPosts.FirstOrDefault(r => r.IdRoomPost == id);
            if (roomPost != null)
            {
                var img = _context.TblImages.Where(x => x.IdRoomPost == id).ToList();
                var favorite = _context.TblFavoritePosts.Where(x => x.IdRoomPost == id).ToList();
                var less = _context.TblLessees.Where(x => x.IdRoomPost == id).ToList();
                if (favorite != null)
                {
                    _context.TblFavoritePosts.RemoveRange(favorite);
                }
                if (less != null)
                {
                    _context.TblLessees.RemoveRange(less);
                }
                _context.TblImages.RemoveRange(img);
                _context.TblRoomPosts.Remove(roomPost);
                _context.SaveChanges();
            }
            TempData["SuccessMessage"] = "Xóa bài viết thành công.";
            return RedirectToAction("Index");
        }
    }
}
