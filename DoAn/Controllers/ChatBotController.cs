
using DoAn.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Controllers
{
    public class ChatBotController : Controller
    {
        private readonly DoAnTotNghiepContext _context;
        public ChatBotController(DoAnTotNghiepContext context) 
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> GetResponse([FromBody] string message)
        {
            var response = ProcessMessage(message);
            return Json(new { response });
            
        }

        private async Task<string> ProcessMessage(string message)
        {
            var sp = await _context.TblSupports.FirstOrDefaultAsync(x=>message.ToLower().Contains(x.TuKhoa.ToLower()));
            if (sp == null)
            {
                return "Bạn vui lòng liên hệ 0123456789 để giải đáp thắc mắc";
            }
            // Xử lý câu hỏi từ người dùng và tạo câu trả lời
            return $"{sp.TraLoi}";
        }
    }
}
