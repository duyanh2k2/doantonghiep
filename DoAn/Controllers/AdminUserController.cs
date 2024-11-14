using DoAn.Authen;
using DoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DoAn.Controllers
{
    [AuthenRole("3")]
    public class AdminUserController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        public AdminUserController(DoAnTotNghiepContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.TblUsers.ToList();
            return View(users);
        }
        public IActionResult Create()
        {
           var roles = _context.TblRoles.Select(u => new
           {
               Id = u.IdRole,
               Name = u.SRole,
           }).ToList(); // Lấy danh sách vai trò
            ViewBag.Roles = new SelectList(roles, "Id", "Name");
            return View();
        }
        [HttpPost]
        public IActionResult Create(TblUser user)
        {
            if (user.TaiKhoan != "" && user.MatKhau != "" && user.MatKhau.Length >= 8 && user.Sdt != "" && user.HoTen != "" && user.Gt != null && user.CanCuoc != "" && user.IdRole != 0)
            {
                var u = _context.TblUsers.FirstOrDefault(u => u.TaiKhoan.Equals(user.TaiKhoan));
                if(u == null) 
                {
                    _context.TblUsers.Add(user);
                    _context.SaveChanges();
                    TempData["SuccessMessage"] = "Người dùng đã được thêm thành công.";
                    return RedirectToAction("Index");
                }
                TempData["ErrorMessage"] = "Người dùng đã tồn tại";
            }
            ViewBag.Roles = new SelectList(_context.TblRoles, "IdRole", "SRole",user.IdRole);
            return View(user);
        }
        [HttpPost]
        public async  Task<IActionResult> Delete(int id)
        {
            var user = await _context.TblUsers.FindAsync(id);
            if (user != null)
            {
                var room = await _context.TblRoomPosts.Where(x=>x.IdUser==user.IdUser).ToListAsync();
                if (room != null)
                {
                    _context.TblRoomPosts.RemoveRange(room);
                }
                var less = await _context.TblLessees.Where(x=>x.IdUser==user.IdUser).ToListAsync();
                if (less != null)
                {
                    _context.TblLessees.RemoveRange(less);
                }
                var favorite = await _context.TblFavoritePosts.Where(x=>x.IdUser == user.IdUser).ToListAsync();
                if (favorite != null)
                {
                    _context.TblFavoritePosts.RemoveRange(favorite);
                }
                _context.TblUsers.Remove(user);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Người dùng đã được xóa thành công.";
            }
            return RedirectToAction("Index");
        }
    }
}
