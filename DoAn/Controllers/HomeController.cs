using DoAn.Models;
using DoAn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DoAn.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DoAnTotNghiepContext _context;

        public HomeController(ILogger<HomeController> logger, DoAnTotNghiepContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            List<TblRoomPost> posts = _context.TblRoomPosts.ToList();
            List<TblImage> images = _context.TblImages.ToList();
            var homepage = new Home
            {
                roomPost = posts,
                image = images
            };
            return View(homepage);
        }
        public IActionResult Search(string? province, string? district, string? ward, string? fromDienTich, string? toDienTich, string? fromGia, string? toGia)
        {
            float? fromArea,toArea, fromPrice, toPrice;
            province= string.IsNullOrWhiteSpace(province) ? null: province;
            district = string.IsNullOrWhiteSpace(district) ? null : district;
            ward = string.IsNullOrWhiteSpace(ward) ? null : ward;
            fromArea = string.IsNullOrWhiteSpace(fromDienTich) ? null : float.Parse(fromDienTich);
            toArea = string.IsNullOrWhiteSpace(toDienTich) ? null : float.Parse(toDienTich);
            fromPrice = string.IsNullOrWhiteSpace(fromGia) ? null : float.Parse(fromGia);
            toPrice = string.IsNullOrWhiteSpace(toGia) ? null : float.Parse(toGia);
            var post = _context.TblRoomPosts.FromSqlInterpolated($"EXEC dbo.pSearch @Tinh = {province}, @Quan= {district}, @Phuong = {ward}, @fromArea = {fromArea}, @toArea = {toArea}").ToList();
            List<TblImage> images = _context.TblImages.ToList();
            var searchPage = new Home
            {
                roomPost = post,
                image = images,
            };
            ViewBag.message = "Phòng trọ";
            if (district != null)
            {
                ViewBag.message+= $" quận {district} ";

            }
            if (ward != null)
            {
                ViewBag.message += $"phường {ward} ";
            }
            if(fromPrice != null && toPrice != null)
            {
                ViewBag.message += $" Giá từ: {fromPrice}-{toPrice} VND,";
            }
            if (fromArea != null && toArea != null)
            {
                ViewBag.message += $" Diện tích: {fromArea}-{toArea} m2";
            }
            return View(searchPage);
      
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}