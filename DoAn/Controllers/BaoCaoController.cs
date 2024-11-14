using DoAn.Authen;
using DoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers
{
    [AuthenRole("2")]
    [AuthenOtp]
    public class BaoCaoController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        public BaoCaoController(DoAnTotNghiepContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var us = HttpContext.Session.GetInt32("IdUser") ?? 0;
            if(us > 0)
            {
                var room = _context.TblRoomPosts.Where(x=>x.IdUser == us).Count();
                var less = _context.TblRoomPosts.Where(x=>x.TblLessees.Any() && x.IdUser == us).Count();
                var roomStats = new
                {
                    TotalRooms = room,
                    RentedRooms = less,
                    AvailableRooms = ((room-less)<0)?0: (room - less)
                };
                var lesseeStats = new
                {
                    TotalLessees = less,
                };
                var lesseeList = _context.TblLessees.Include(x=>x.IdUserNavigation).Include(c=>c.IdRoomPostNavigation).Where(u=>u.IdRoomPostNavigation.IdUser==us).ToList();
            

                // Truyền dữ liệu qua View
                ViewBag.RoomStats = roomStats;
                ViewBag.LesseeStats = lesseeStats;
                return View(lesseeList);
            }
            return View();
        }
    }
}
