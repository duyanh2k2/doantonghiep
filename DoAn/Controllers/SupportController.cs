using DoAn.Authen;
using DoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers
{
    [AuthenRole("3")]
    public class SupportController : Controller
    {
        private readonly DoAnTotNghiepContext _context;

        public SupportController(DoAnTotNghiepContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var supports = await _context.TblSupports.ToListAsync();
            return View(supports);
        }
        [HttpGet]
        public async Task<IActionResult> GetSupport(int id)
        {
            var support = await _context.TblSupports.FindAsync(id);
            if (support == null)
            {
                return NotFound();
            }
            return Json(new { idSupport = support.IdSupport, tuKhoa = support.TuKhoa, traLoi = support.TraLoi });
        }
        [HttpPost]
        public async Task<IActionResult> Create(string TuKhoa, string TraLoi)
        {
            var idUser = HttpContext.Session.GetInt32("IdUser") ?? 0;
            if (TuKhoa!="" && TraLoi!=""&&idUser>0)
            {
                var support = new TblSupport
                {
                    TuKhoa = TuKhoa,
                    TraLoi = TraLoi,
                    IdUser = idUser
                };
                _context.Add(support);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm thành công";
                return RedirectToAction(nameof(Index));
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(string TuKhoa, string TraLoi , int IdSupport)
        {
            var idUser = HttpContext.Session.GetInt32("IdUser") ?? 0;
            if (TuKhoa != "" && TraLoi != "" && idUser > 0 && IdSupport>0)
            {
                var sup = await _context.TblSupports.FindAsync(IdSupport);
                if(sup != null)
                {
                    sup.TraLoi= TraLoi;
                    sup.IdUser= idUser;
                    sup.TuKhoa = TuKhoa;
                    _context.Update(sup);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật thành công";
                    return RedirectToAction(nameof(Index));
                }
            }
            TempData["ErrorMessage"] = "Có lỗi xảy ra vui lòng thử lại";
            return View();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var support = await _context.TblSupports.FindAsync(id);
            if (support == null)
            {
                return NotFound();
            }

            _context.TblSupports.Remove(support);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
