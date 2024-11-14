using DoAn.Authen;
using DoAn.Models;
using DoAn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers
{
    public class DeltailController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        public DeltailController(DoAnTotNghiepContext context)
        {
            _context = context;
        }
        public IActionResult Index(int id)
        {
            if(id > 0)
            {
                var p = _context.TblRoomPosts.Where(u=>u.IdRoomPost == id).FirstOrDefault();
                if(p != null)
                {
                    List<TblImage> i = _context.TblImages.Where(u=>u.IdRoomPost == id).ToList();
                    var u = _context.TblUsers.Where(u=>u.IdUser == p.IdUser).FirstOrDefault();
                    var detail = new deltail
                    {
                        post = p,
                        image = i,
                        user = u,
                    };
                    return View(detail);
                }
            }
            return RedirectToAction("Index","Home");
        }
        [HttpGet]
        public JsonResult LoadComment(int idRoomPost)
        {
            if(idRoomPost > 0)
            {
                var c = _context.TblComments.Where(c=>c.IdRoomPost==idRoomPost).ToList();   
                return Json(new { code = 200, c = c, msg = "Lấy dữ liệu thành công" });
            }
            return Json(new { code = 500, msg = "Lấy dữ liệu thất bại"});
        }
        [HttpGet]
        public JsonResult LoadUser()
        {
            try
            {
                var u = _context.TblUsers.Select(p=>new {p.IdUser,p.HoTen}).ToList();
                return Json(new { code = 200, u = u, msg = "Lấy dữ liệu thành công" });
            }
            catch { return Json(new { code = 500, msg = "Lấy dữ liệu thất bại" }); }
        }
        [HttpGet]
        public JsonResult LoadFeedBack()
        {
            try
            {
                var f = _context.TblFeedBacks.ToList();
                return Json(new { code = 200, f = f, msg = "Lấy dữ liệu thành công" });
            }
            catch { return Json(new { code = 500, msg = "Lấy dữ liệu thất bại" }); }
        }
        [AuthenLogin]
        [HttpPost]
        public IActionResult addComment (string comment,int idRoomPost)
        {
            if(comment !="" && idRoomPost>0)
            {
                int id = HttpContext.Session.GetInt32("IdUser")??0;
                if (id > 0)
                {
                    var c = new TblComment
                    {
                        NoiDung = comment,
                        IdRoomPost = idRoomPost,
                        IdUser = id,

                    };
                    _context.TblComments.Add(c);
                    _context.SaveChanges();
                    return Json(new { code = 200, msg = "Thành công" });
                }
                else
                {
                    return Json(new { code = 500, msg = "Vui lòng đăng nhập" });
                }
            }
            return Json(new { code = 501,msg="Thất bại" });
        }
        [AuthenLogin]
        [HttpPost]
        public IActionResult addFeedBack(string feedback, int idComment)
        {
            if (feedback != "" && idComment > 0)
            {
                int id = HttpContext.Session.GetInt32("IdUser") ?? 0;
                if (id > 0)
                {
                    var c = new TblFeedBack
                    {
                        NoiDung = feedback,
                        IdComment = idComment,
                        IdUser = id,

                    };
                    _context.TblFeedBacks.Add(c);
                    _context.SaveChanges();
                    return Json(new { code = 200, msg = "Thành công" });
                }
                else
                {
                    return Json(new { code = 500, msg = "Vui lòng đăng nhập" });
                }
            }
            return Json(new { code = 501, msg = "Thất bại" });
        }
        [AuthenLogin]
        [HttpPost]
        public IActionResult removeComment(int idComment)
        {
            if (idComment > 0)
            {  
                var comment = _context.TblComments.Where(c=>c.IdComment == idComment).FirstOrDefault();
                var feed= _context.TblFeedBacks.Where(f=>f.IdComment == idComment).ToList();
                _context.TblFeedBacks.RemoveRange(feed);
                _context.SaveChanges();
                _context.TblComments.Remove(comment);
                _context.SaveChanges(true);
                return Json(new { code = 200, msg = "Xóa thành công" });
            }
            return Json(new { code = 501, msg = "Xóa bình thất bại" });
        }
    }
}
