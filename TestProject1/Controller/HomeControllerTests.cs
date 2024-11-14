using DoAn.Controllers;
using DoAn.Models;
using DoAn.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1.Controller
{
    [TestFixture]
    public class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> _mockLogger;
        private DoAnTotNghiepContext _context;
        private HomeController _controller;

        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<HomeController>>();

            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;

            _context = new DoAnTotNghiepContext(options);
            _context.TblRoomPosts.AddRange(new List<TblRoomPost>
        {
             new TblRoomPost { IdRoomPost = 1, TieuDe = "Phòng trọ 1", MoTa = "Province", DiaChi = "Định Công, Hoàng Mai, Hà Nội", IdUser = 1, DienTich = 40, GiaTien = 1500000,NgayDang=DateTime.Now },

        });
            _context.TblImages.AddRange(new List<TblImage>
        {
           new TblImage { IdRoomPost = 1, HinhAnh = "image1.jpg" },

        });

            _context.SaveChanges();

            _controller = new HomeController(_mockLogger.Object, _context);
        }
        [TearDown]
        public void TearDown()
        {
            // Clean up the in-memory database
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
        [Test]

        public void Search_WithValidParameters_ReturnsCorrectResults()
        {
            // Hành động: Tìm kiếm với các tham số hợp lệ
            var result = _controller.Search("Hà Nội", "Hoàng Mai", "Định Công", "30", "50", "1000000", "2000000");

            // Xác nhận: Kết quả trả về là một ViewResult
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "Result should be a ViewResult.");

            var model = viewResult.Model as Home;
            Assert.That(model, Is.Not.Null, "Model should be of type Home.");
            Assert.That(model.roomPost.Count, Is.EqualTo(1), "There should be one matching room post.");
            Assert.That(model.image.Count, Is.EqualTo(1), "There should be one image matching the room post.");
            Assert.That(viewResult.ViewData["message"], Is.EqualTo("Phòng trọ quận Hoàng Mai phường Định Công  Giá từ: 1000000-2000000 VND, Diện tích: 30-50 m2"));
        }


        [Test]
        public void Search_WithEmptySearchCriteria_ReturnsAllResults()
        {
            // Tìm kiếm với các tham số trống hoặc null, mong đợi trả về tất cả các kết quả
            var result = _controller.Search("", "", "", "", "", "", "");

            // Xác nhận: Kết quả trả về là một ViewResult
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "The result should be a ViewResult.");

            var model = viewResult.Model as Home;
            Assert.That(model, Is.Not.Null, "The model should be of type Home.");

            // Kiểm tra xem có 1 bài đăng phòng trọ (dữ liệu mẫu) trong kết quả không
            Assert.That(model.roomPost.Count, Is.EqualTo(1), "There should be 1 room post matching the search criteria.");
            Assert.That(model.image.Count, Is.EqualTo(1), "There should be 1 image matching the search criteria.");
        }

        [Test]
        public void Search_WithInvalidPriceRange_ReturnsEmptyResults()
        {
            // Act: Perform the search with an invalid price range
            var result = _controller.Search("Hà Nội", "Hoàng Mai", "Định Công", "100", "200", "-50000", "-1000");

            // Assert: The result should be a ViewResult
            var viewResult = result as ViewResult;
            Assert.That(viewResult, Is.Not.Null, "The result should be a ViewResult.");

            // Assert: The model should be of type Home
            Assert.That(viewResult.Model, Is.InstanceOf<Home>(), "The model should be of type Home.");


            var model = viewResult.Model as Home;

            // Assert: The roomPost and image collections should be empty
            Assert.That(model.roomPost.Count, Is.EqualTo(0), "There should be no room posts matching the search criteria.");
            Assert.That(model.image.Count, Is.EqualTo(0), "There should be no images for the matching room posts.");


        }

    }

    // Add more tests to cover other scenarios, such as specific parameters combinations, edge cases, etc.
}
