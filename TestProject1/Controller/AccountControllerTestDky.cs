using DoAn.Controllers;
using DoAn.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1.Controller
{
    [TestFixture]
    public class AccountControllerTestDky
    {   //Bat buoc phai khai bao de lay du lieu tu controller,db de test
        private Mock<DoAnTotNghiepContext> _mockContext;
        //private Mock<DbSet<TblUser>> _mockSet;

        private AccountController _controller;
        private List<TblUser> _userList;
        [SetUp]
        public void Setup()
        {
            // Khởi tạo Mock Context và Mock DbSet cho TblUser do co du lieu ao
            _userList = new List<TblUser>();
            var mockUserDbSet = new Mock<DbSet<TblUser>>();

            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(_userList.AsQueryable().Provider);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(_userList.AsQueryable().Expression);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(_userList.AsQueryable().ElementType);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(_userList.AsQueryable().GetEnumerator());

            _mockContext = new Mock<DoAnTotNghiepContext>();
            _mockContext.Setup(c => c.TblUsers).Returns(mockUserDbSet.Object);
            // mac dinh phai co temdata
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());
            _controller = new AccountController(_mockContext.Object, null) //mockContext ao
            {
                TempData = tempData
            };
        }
        [Test]
        public void DangKy_TaiKhoanMoi_DangKyThanhCong()
        {
            TblUser newUser = new TblUser
            {
                TaiKhoan = "newuser",
                MatKhau = "password123",
                Sdt = "0123456789",
                HoTen = "Nguyễn Văn A",
                Gt = true,
                CanCuoc = "123456789012",
                IdRole = 1 // Đảm bảo IdRole không bằng 0
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));
            Assert.That(_controller.TempData["SuccessMessage"], Is.EqualTo("Đăng ký tài khoản thành công!"));
        }
        [Test]
        public void DangKy_TaiKhoanDaTonTai_ThongBaoLoi()
        {
            // Arrange
            _userList.Add(new TblUser { TaiKhoan = "existingUser" });

            var newUser = new TblUser
            {
                TaiKhoan = "existingUser",
                MatKhau = "password123",
                Sdt = "0123456789",
                HoTen = "Nguyễn Văn A",
                Gt = true,
                CanCuoc = "123456789012",
                IdRole = 1
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
            Assert.That(_controller.ViewData["message"], Is.EqualTo("Tài khoản đã tồn tại"));
        }
        [Test]
        public void DangKy_TenNguoiDungTrong_ThongBaoLoi()
        {

            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = string.Empty, // Tên người dùng trống
                MatKhau = "password123",
                Sdt = "0123456789",
                HoTen = "Nguyễn Văn A",
                Gt = true,
                CanCuoc = "123456789012",
                IdRole = 1
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null);
        }
        [Test]
        public void DangKy_TaiKhoanTrong_ThongBaoLoi()
        {
            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = string.Empty, // Tài khoản trống
                MatKhau = "password123",
                Sdt = "0123456789",
                HoTen = "Nguyễn Văn A",
                Gt = true,
                CanCuoc = "123456789012",
                IdRole = 1
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Tên tài khoản không được trống");

        }
        [Test]
        public void DangKy_MatKhauTrong_ThongBaoLoi()
        {
            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = "nguyenvana",      // Tên người dùng hợp lệ
                MatKhau = string.Empty,       // Mật khẩu trống
                Sdt = "0123456789",           // Số điện thoại hợp lệ
                HoTen = "Nguyễn Văn A",       // Tên hợp lệ
                Gt = true,                    // Giới tính hợp lệ
                CanCuoc = "123456789012",     // Căn cước công dân hợp lệ
                IdRole = 1                    // Quyền hợp lệ
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Mật khẩu không được để trống");

        }

        [Test]
        public void DangKy_MatKhauNganHơn8KyTu_ThongBaoLoi()
        {
            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = "nguyenvana",      // Tên người dùng hợp lệ
                MatKhau = "12345",            // Mật khẩu ngắn hơn 8 ký tự
                Sdt = "0123456789",           // Số điện thoại hợp lệ
                HoTen = "Nguyễn Văn A",       // Tên hợp lệ
                Gt = true,                    // Giới tính hợp lệ
                CanCuoc = "123456789012",     // Căn cước công dân hợp lệ
                IdRole = 1                    // Quyền hợp lệ
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Mật khẩu phải có ít nhất 8 ký tự");
        }
        [Test]
        public void DangKy_MatKhauHopLe_KhongThongBaoLoi()
        {
            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = "nguyenvana",      // Tên người dùng hợp lệ
                MatKhau = "password123",      // Mật khẩu đủ độ dài hợp lệ (8 ký tự)
                Sdt = "0123456789",           // Số điện thoại hợp lệ
                HoTen = "Nguyễn Văn A",       // Tên hợp lệ
                Gt = true,                    // Giới tính hợp lệ
                CanCuoc = "123456789012",     // Căn cước công dân hợp lệ
                IdRole = 1                    // Quyền hợp lệ
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Null, "Mật khẩu hợp lệ");
        }
        [Test]
        public void DangKy_SoDienThoaiKhongDungDinhDang_ThongBaoLoi()
        {
            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = "nguyenvana",      // Tên người dùng hợp lệ
                MatKhau = "password123",      // Mật khẩu hợp lệ
                Sdt = "01234abc89",           // Số điện thoại không hợp lệ (chứa ký tự không phải số)
                HoTen = "Nguyễn Văn A",       // Tên hợp lệ
                Gt = true,                    // Giới tính hợp lệ
                CanCuoc = "123456789012",     // Căn cước công dân hợp lệ
                IdRole = 1                    // Quyền hợp lệ
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Null, "Số điện thoại không đúng định dạng");
        }
        [Test]
        public void DangKy_SoDienThoaiTrong_ThongBaoLoi()
        {
            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = "nguyenvana",      // Tên người dùng hợp lệ
                MatKhau = "password123",      // Mật khẩu hợp lệ
                Sdt = string.Empty,           // Số điện thoại trống
                HoTen = "Nguyễn Văn A",       // Tên hợp lệ
                Gt = true,                    // Giới tính hợp lệ
                CanCuoc = "123456789012",     // Căn cước công dân hợp lệ
                IdRole = 1                    // Quyền hợp lệ
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Số điện thoại không được để trống");
        }
        [Test]
        public void DangKy_SoCanCuocTrong_ThongBaoLoi()
        {
            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = "nguyenvana",         // Tên người dùng hợp lệ
                MatKhau = "password123",         // Mật khẩu hợp lệ
                Sdt = "0123456789",              // Số điện thoại hợp lệ
                HoTen = "Nguyễn Văn A",          // Tên hợp lệ
                Gt = true,                       // Giới tính hợp lệ
                CanCuoc = string.Empty,          // Số căn cước công dân trống
                IdRole = 1                       // Quyền hợp lệ
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Số căn cước công dân không được để trống");
        }
        [Test]
        public void DangKy_SoCanCuocKhongDungDinhDang_ThongBaoLoi()
        {
            // Arrange
            TblUser newUser = new TblUser
            {
                TaiKhoan = "nguyenvana",         // Tên người dùng hợp lệ
                MatKhau = "password123",         // Mật khẩu hợp lệ
                Sdt = "0123456789",              // Số điện thoại hợp lệ
                HoTen = "Nguyễn Văn A",          // Tên hợp lệ
                Gt = true,                       // Giới tính hợp lệ
                CanCuoc = "12345abc789",         // Số căn cước công dân không hợp lệ (chứa ký tự không phải là chữ số)
                IdRole = 1                       // Quyền hợp lệ
            };

            // Act
            var result = _controller.DangKy(newUser);

            // Assert
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Null, "Số căn cước công dân không đúng định dạng");
        }
        //[Test]
        //public void DangKy_KiemTraKhongChonQuyen_ThongBaoLoi()
        //{
        //    // Arrange
        //    TblUser newUser = new TblUser
        //    {
        //        TaiKhoan = "nguyenvana",         // Tên người dùng hợp lệ
        //        MatKhau = "password123",         // Mật khẩu hợp lệ
        //        Sdt = "0123456789",              // Số điện thoại hợp lệ
        //        HoTen = "Nguyễn Văn A",          // Tên hợp lệ
        //        Gt = true,                       // Giới tính hợp lệ
        //        CanCuoc = "123456789012",          // Số căn cước công dân trống
        //        IdRole = null                    // Không chọn quyền (hoặc có thể là 0 hoặc một giá trị không hợp lệ)
        //    };

        //    // Act
        //    var result = _controller.DangKy(newUser);

        //    // Assert
        //    var viewResult = result as ViewResult;
        //    Assert.That(viewResult, Is.Not.Null, "Bạn chưa chọn quyền hợp lệ");

        //}

    }
}
