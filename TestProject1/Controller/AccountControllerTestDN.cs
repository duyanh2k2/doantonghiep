using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using System.Linq;
using DoAn.Models;
using DoAn.Controllers;
using Microsoft.EntityFrameworkCore;
using System;
using DoAn.Services;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;

namespace TestProject1.Controller
{
    [TestFixture]
    public class AccountControllerTestDN
    {   //Bat buoc phai khai bao de lay du lieu tu controller,db de test
        private Mock<DoAnTotNghiepContext> _mockContext;
        //private Mock<DbSet<TblUser>> _mockSet;
        private Mock<SmsService> mockSmsService;

        private AccountController _controller;
        private List<TblUser> _userList;
        [SetUp]
        public void Setup()
        {
            // Khởi tạo dữ liệu người dùng giả lập trong cơ sở dữ liệu
            _userList = new List<TblUser>
    {
        new TblUser { TaiKhoan = "MaiAnh", MatKhau = "Password123", IdRole = 1, HoTen = "Nguyễn Văn A", IdUser = 1 }
    };

            var mockUserDbSet = new Mock<DbSet<TblUser>>();

            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(_userList.AsQueryable().Provider);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(_userList.AsQueryable().Expression);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(_userList.AsQueryable().ElementType);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(_userList.AsQueryable().GetEnumerator());

            _mockContext = new Mock<DoAnTotNghiepContext>();
            _mockContext.Setup(c => c.TblUsers).Returns(mockUserDbSet.Object);

            // Khởi tạo HttpContext với Session và TempData
            var httpContext = new DefaultHttpContext();
            var mockSession = new Mock<ISession>();
            httpContext.Session = mockSession.Object;  // Mock session here
            var tempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Twilio:AccountSid"]).Returns("ACa20f3ac0288fdb5d4ac3a2bf6b856041");
            mockConfiguration.Setup(c => c["Twilio:AuthToken"]).Returns("c7dcdb41f932d4bf76c0ec47c08672e2");
            mockConfiguration.Setup(c => c["Twilio:FromNumber"]).Returns("+15186213808");

            mockSmsService = new Mock<SmsService>(mockConfiguration.Object) { CallBase = true };
            // Khởi tạo Controller và gán TempData, HttpContext
            _controller = new AccountController(_mockContext.Object, mockSmsService.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }

        [Test]
        public void DangNhapThanhCong()
        {
            var loginModel = new TblUser
            {
                TaiKhoan = "MaiAnh",           // Tên đăng nhập hợp lệ
                MatKhau = "Password123"      // Mật khẩu hợp lệ
            };

            // Act
            var result = _controller.Index(loginModel) as RedirectToActionResult;

            // Assert
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null, "");
            Assert.That(redirectResult.ActionName, Is.EqualTo("SendOtp"), "");
            

        }
        [Test]
        public void DangNhapSaiMatKhau()
        {
            var loginModel = new TblUser
            {
                TaiKhoan = "MaiAnh",
                MatKhau = "Anh12345@"
            };
            var result = _controller.Index(loginModel) as ViewResult;
            Assert.That(result, Is.Not.Null, "");

            Assert.That(result.TempData["error"], Is.EqualTo("Tài khoản mật khẩu không chính xác"), "Thông báo lỗi không đúng");
        }
        [Test]
        public void DangNhapMatKhauTrong()
        {
            // Arrange
            var loginModel = new TblUser
            {
                TaiKhoan = "MaiAnh",
                MatKhau = string.Empty     // Mật khẩu trống
            };

            // Act
            var result = _controller.Index(loginModel) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Tai khoan hoac mat khau khong duoc trong");

        }

        [Test]
        public void DangNhapMatKhauNganHơn8KyTu()
        {
            // Arrange
            var loginModel = new TblUser
            {
                TaiKhoan = "MaiAnh",      // Tên người dùng hợp lệ
                MatKhau = "Anh123",            // Mật khẩu ngắn hơn 8 ký tự

            };

            // Act
            var result = _controller.Index(loginModel) as ViewResult;
            // Assert
            var viewResult = result as ViewResult;
            Assert.That(result, Is.Not.Null, "Mật khẩu phải có ít nhất 8 ký tự");
        }
        [Test]
        public async Task RecoverPass_ValidUser_ReturnsRedirectToAction()
        {
            // Arrange

            // Act
            var result = await _controller.RecoverPass("MaiAnh") as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            mockSmsService.Verify(s => s.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
