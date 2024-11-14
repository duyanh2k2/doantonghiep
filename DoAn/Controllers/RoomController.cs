using DoAn.Authen;
using DoAn.Models;
using DoAn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers
{
    [AuthenRole("1")]
    [AuthenOtp]
    public class RoomController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        public RoomController(DoAnTotNghiepContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var idUser = HttpContext.Session.GetInt32("IdUser")??0;
            if (idUser > 0)
            {
                var FavoriteRoom = _context.TblFavoritePosts
                                   .Include(f => f.IdRoomPostNavigation) // Lấy thông tin bài đăng liên quan
                                   .Where(f => f.IdUser == idUser) // Lọc theo IdUser
                                   .ToList();
                var img = _context.TblImages.ToList();
                var f = new FarvoriteRoom
                {
                    FavoritePost = FavoriteRoom,
                    images = img,
                };
                return View(f);
            }
            return RedirectToAction("Index","Home");
        }
        public IActionResult DeleteFarvorite(int id)
        {
            if(id > 0)
            {
                var f = _context.TblFavoritePosts.Where(f => f.IdFavoritePost == id).FirstOrDefault();
                if (f != null)
                {
                    var u = HttpContext.Session.GetInt32("IdUser") ?? 0;
                    if (u > 0 && f.IdUser==u)
                    {
                        _context.TblFavoritePosts.Remove(f);
                        _context.SaveChanges();
                        TempData["success"] = "Xóa bài đăng yêu thích thành công";
                        return RedirectToAction("Index");
                    }
                }
            }
            TempData["error"] = "Xóa bài yêu thích thất bại";
            return RedirectToAction("Index");
        }
        [HttpPost]
        public JsonResult addFavorite(int id)
        {
            var u = HttpContext.Session.GetInt32("IdUser") ?? 0;
            if(u > 0 && id >0) 
            {
                var f = _context.TblFavoritePosts.Where(f=>f.IdUser ==u && f.IdRoomPost== id).FirstOrDefault();
                if(f != null)
                {
                    return Json(new { code = 201, msg = "Bài đăng này đã được lưu." });
                }
                else
                {
                    var fa = new TblFavoritePost
                    {
                        IdRoomPost = id,
                        IdUser = u,
                    };
                    _context.TblFavoritePosts.Add(fa);
                    _context.SaveChanges();
                    return Json(new { code = 200, msg = "Lưu bài đăng thành công." });
                }
            }
            return Json(new { code = 400, msg = "Lưu bài đăng thất bại." });
        }
    }
}
