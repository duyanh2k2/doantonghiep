using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DoAn.Controllers;
using DoAn.Models;
using DoAn.ViewModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Http;
using Moq.EntityFrameworkCore;

namespace TestProject1.Controller
{
    [TestFixture]
    public class RoomControllerTests
    {
        private Mock<DoAnTotNghiepContext> _mockContext;
        private RoomController _controller;
        private List<TblFavoritePost> _favoritePosts;
        private List<TblImage> _images;
        private Mock<ISession> _mockSession;

        [SetUp]
        public void Setup()
        {
            // Khởi tạo dữ liệu mẫu cho các bài đăng yêu thích và hình ảnh
            _favoritePosts = new List<TblFavoritePost>
            {
                new TblFavoritePost { IdFavoritePost = 1, IdUser = 1, IdRoomPost = 101 },
                new TblFavoritePost { IdFavoritePost = 2, IdUser = 1, IdRoomPost = 102 }
            };

            _images = new List<TblImage>
            {
                new TblImage { IdImage = 1, HinhAnh = "image1.jpg" },
                new TblImage { IdImage = 2, HinhAnh = "image2.jpg" }
            };
            var favoritePosts = new List<TblFavoritePost>
              {
                           new TblFavoritePost { IdUser = 1, IdRoomPost = 101 }  // Bài đăng yêu thích đã tồn tại
               };



            // Mock DbSet cho TblFavoritePost
            var mockFavoritePostSet = new Mock<DbSet<TblFavoritePost>>();
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.Provider).Returns(_favoritePosts.AsQueryable().Provider);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.Expression).Returns(_favoritePosts.AsQueryable().Expression);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.ElementType).Returns(_favoritePosts.AsQueryable().ElementType);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.GetEnumerator()).Returns(_favoritePosts.GetEnumerator());

            // Mock DbSet cho TblImage
            var mockImageSet = new Mock<DbSet<TblImage>>();
            mockImageSet.As<IQueryable<TblImage>>()
                .Setup(m => m.Provider).Returns(_images.AsQueryable().Provider);
            mockImageSet.As<IQueryable<TblImage>>()
                .Setup(m => m.Expression).Returns(_images.AsQueryable().Expression);
            mockImageSet.As<IQueryable<TblImage>>()
                .Setup(m => m.ElementType).Returns(_images.AsQueryable().ElementType);
            mockImageSet.As<IQueryable<TblImage>>()
                .Setup(m => m.GetEnumerator()).Returns(_images.GetEnumerator());

            // Mock DoAnTotNghiepContext và trả về DbSet
            _mockContext = new Mock<DoAnTotNghiepContext>();
            _mockContext.Setup(c => c.TblFavoritePosts).Returns(mockFavoritePostSet.Object);
            _mockContext.Setup(c => c.TblImages).Returns(mockImageSet.Object);

            // Mock HttpContext.Session
            _mockSession = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _mockSession.Object;

            // Giả lập phương thức TryGetValue của ISession để trả về giá trị IdUser
            _mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny)).Returns(true)
                .Callback((string key, out byte[] value) =>
                {
                    // Giả lập trả về IdUser là 1
                    value = BitConverter.GetBytes(1);
                });
            var tempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>());



            // Khởi tạo controller
            _controller = new RoomController(_mockContext.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext,

                }
            };


        }

        [Test]
        public void Index_ShouldRedirectToLogin_WhenUserIsNotLoggedIn()
        {
            // Arrange: Giả lập trường hợp người dùng chưa đăng nhập
            _mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny)).Returns(false); // Không có IdUser trong session

            // Act: Gọi phương thức Index
            var result = _controller.Index() as RedirectToActionResult;

            // Assert: Kiểm tra người dùng được chuyển hướng đến trang login
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ActionName, Is.EqualTo("Index"));
            Assert.That(result.ControllerName, Is.EqualTo("Home"));
        }


        [Test]
        public void Index_ShouldReturnView_WhenUserIsLoggedInButHasNoFavorites()
        {
            // Arrange: Giả lập người dùng đã đăng nhập nhưng không có bài đăng yêu thích
            _mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny)).Returns(true)
                .Callback((string key, out byte[] value) =>
                {
                    // Giả lập trả về IdUser là 1
                    value = BitConverter.GetBytes(1);
                });

            // Giả lập dữ liệu bài đăng yêu thích rỗng
            var mockFavoritePostSet = new Mock<DbSet<TblFavoritePost>>();
            var favoritePosts = new List<TblFavoritePost>(); // Rỗng (người dùng không có bài đăng yêu thích)

            // Giả lập DbSet cho TblFavoritePosts
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.Provider).Returns(favoritePosts.AsQueryable().Provider);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.Expression).Returns(favoritePosts.AsQueryable().Expression);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.ElementType).Returns(favoritePosts.AsQueryable().ElementType);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.GetEnumerator()).Returns(favoritePosts.GetEnumerator());

            // Giả lập dữ liệu hình ảnh (2 hình ảnh)
            var mockImageSet = new Mock<DbSet<TblImage>>();
            var images = new List<TblImage>
    {
        new TblImage { IdImage = 1, IdRoomPost = 101, HinhAnh = "image1.jpg" },
        new TblImage { IdImage = 2, IdRoomPost = 102, HinhAnh = "image2.jpg" }
    };
            mockImageSet.As<IQueryable<TblImage>>()
                .Setup(m => m.Provider).Returns(images.AsQueryable().Provider);
            mockImageSet.As<IQueryable<TblImage>>()
                .Setup(m => m.Expression).Returns(images.AsQueryable().Expression);
            mockImageSet.As<IQueryable<TblImage>>()
                .Setup(m => m.ElementType).Returns(images.AsQueryable().ElementType);
            mockImageSet.As<IQueryable<TblImage>>()
                .Setup(m => m.GetEnumerator()).Returns(images.GetEnumerator());

            // Mock DoAnTotNghiepContext và trả về DbSet đã giả lập
            _mockContext.Setup(c => c.TblFavoritePosts).Returns(mockFavoritePostSet.Object);
            _mockContext.Setup(c => c.TblImages).Returns(mockImageSet.Object);

            // Mock HttpContext.Session
            var httpContext = new DefaultHttpContext();
            httpContext.Session = _mockSession.Object;

            // Khởi tạo controller và gán HttpContext
            _controller = new RoomController(_mockContext.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                }
            };

            // Act: Gọi phương thức Index
            var result = _controller.Index() as ViewResult;
            var model = result?.Model as FarvoriteRoom;

            // Assert: Kiểm tra model và view
            Assert.That(result, Is.Not.Null);
            Assert.That(model, Is.Not.Null, "Model không thể null");
            Assert.That(model.FavoritePost.Count, Is.EqualTo(0), "Số bài đăng yêu thích không đúng (mong đợi 0)");
            Assert.That(model.images.Count, Is.EqualTo(2), "Số hình ảnh không đúng (mong đợi 2)");
        }



        [Test]
        public void DeleteFavorite_ShouldDeleteFavoritePost_WhenUserHasPermission()
        {
            // Arrange: Giả lập trường hợp người dùng đã đăng nhập và có quyền xóa bài đăng yêu thích
            int favoritePostId = 1;  // Id của bài đăng yêu thích cần xóa
            int userId = 1;  // Id người dùng đã đăng nhập

            // Giả lập dữ liệu bài đăng yêu thích
            var fakeFavoritePost = new TblFavoritePost { IdFavoritePost = favoritePostId, IdUser = userId };
            var fakeFavoritePostList = new List<TblFavoritePost> { fakeFavoritePost }.AsQueryable();

            // Giả lập DbSet<TblFavoritePost> trả về danh sách bài đăng yêu thích
            var mockFavoritePostSet = new Mock<DbSet<TblFavoritePost>>();
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.Provider).Returns(fakeFavoritePostList.Provider);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.Expression).Returns(fakeFavoritePostList.Expression);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.ElementType).Returns(fakeFavoritePostList.ElementType);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.GetEnumerator()).Returns(fakeFavoritePostList.GetEnumerator());

            _mockContext.Setup(c => c.TblFavoritePosts).Returns(mockFavoritePostSet.Object);

            // Mock ISession để giả lập người dùng đã đăng nhập với Id = 1
            var mockSession = new Mock<ISession>();
            byte[] userIdBytes = BitConverter.GetBytes(userId);
            mockSession.Setup(s => s.TryGetValue("IdUser", out userIdBytes)).Returns(true); // Trả về giá trị IdUser trong session

            // Mock TempData (IDictionary<string, object>)
            var mockTempData = new Mock<ITempDataDictionary>();

            // Đảm bảo TempData được gán đúng giá trị trong controller
            _controller.TempData = mockTempData.Object;

            // Giả lập controller context với TempData và Session
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { Session = mockSession.Object }
            };

            // Act: Gọi phương thức DeleteFavorite với Id = 1
            var result = _controller.DeleteFarvorite(favoritePostId);  // Truyền trực tiếp Id = 1

            // Assert: Kiểm tra RedirectToActionResult
            var redirectResult = result as RedirectToActionResult;
            Assert.That(redirectResult, Is.Not.Null);  // Kiểm tra xem có phải là RedirectToActionResult
            Assert.That(redirectResult.ActionName, Is.EqualTo("Index"));  // Kiểm tra hành động redirect đúng là Index

            // Kiểm tra TempData["success"] có giá trị "Xóa bài đăng yêu thích thành công"
            mockTempData.VerifySet(td => td["success"] = "Xóa bài đăng yêu thích thành công", Times.Once);
        }


        [Test]
        public void DeleteFarvorite_ShouldRedirectToIndex_WhenFavoritePostIsDeletedSuccessfully()
        {
            // Arrange: Giả lập bài đăng yêu thích hợp lệ và người dùng là chủ sở hữu
            var fakeFavoritePost = new TblFavoritePost { IdFavoritePost = 1, IdUser = 1 }; // Bài đăng yêu thích của người dùng
            var fakeFavoritePostList = new List<TblFavoritePost> { fakeFavoritePost }.AsQueryable();

            var mockFavoritePostSet = new Mock<DbSet<TblFavoritePost>>();
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.Provider).Returns(fakeFavoritePostList.Provider);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.Expression).Returns(fakeFavoritePostList.Expression);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.ElementType).Returns(fakeFavoritePostList.ElementType);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>()
                .Setup(m => m.GetEnumerator()).Returns(fakeFavoritePostList.GetEnumerator());

            _mockContext.Setup(c => c.TblFavoritePosts).Returns(mockFavoritePostSet.Object);

            // Mock session để giả lập người dùng đã đăng nhập với Id = 1
            _mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny))
                .Returns((string key, out byte[] value) =>
                {
                    value = BitConverter.GetBytes(1); // Người dùng có Id = 1
                    return true;
                });

            // Mock TempData
            var mockTempData = new Mock<ITempDataDictionary>();
            _controller.TempData = mockTempData.Object;

            // Act: Gọi phương thức DeleteFarvorite
            var result = _controller.DeleteFarvorite(1) as RedirectToActionResult;

            // Assert: Kiểm tra hành động redirect và thông báo thành công
            Assert.That(result, Is.Not.Null, "RedirectToActionResult không hợp lệ");
            Assert.That(result.ActionName, Is.EqualTo("Index"), "Hành động không đúng");

        }


        [Test]
        public void AddFavorite_ShouldReturnCode201()
        {
            // Arrange: Giả lập IdUser và DbContext
            var mockSession = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession.Object;

            // Giả lập session với IdUser = 1
            mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny)).Returns(true)
                .Callback((string key, out byte[] value) =>
                {
                    value = BitConverter.GetBytes(1);  // Giả lập người dùng có Id = 1
                });

            // Giả lập DbContext - bài đăng chưa yêu thích
            var mockFavoritePostSet = new Mock<DbSet<TblFavoritePost>>();
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>().Setup(m => m.Provider).Returns(new List<TblFavoritePost>().AsQueryable().Provider);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>().Setup(m => m.Expression).Returns(new List<TblFavoritePost>().AsQueryable().Expression);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>().Setup(m => m.ElementType).Returns(new List<TblFavoritePost>().AsQueryable().ElementType);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>().Setup(m => m.GetEnumerator()).Returns(new List<TblFavoritePost>().AsQueryable().GetEnumerator());

            // Giả lập DbContext trả về DbSet cho TblFavoritePosts
            var mockContext = new Mock<DoAnTotNghiepContext>();
            mockContext.Setup(c => c.TblFavoritePosts).Returns(mockFavoritePostSet.Object);

            // Mock SaveChanges
            mockContext.Setup(m => m.SaveChanges()).Returns(1);  // Trả về số lượng bản ghi bị ảnh hưởng

            // Tạo controller với mock context
            var controller = new RoomController(mockContext.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Act: Gọi phương thức addFavorite với id bài đăng chưa yêu thích
            var result = controller.addFavorite(101) as JsonResult;

            // Assert: Kiểm tra kết quả trả về không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra dữ liệu trả về là đúng
            var jsonResponse = result.Value as FavoriteResponse;
            Assert.That(jsonResponse, Is.Not.Null, "Response data is null");

            // Kiểm tra mã phản hồi là 201 khi bài đăng chưa yêu thích
            Assert.That(jsonResponse.Code, Is.EqualTo(201), "Mã phản hồi không đúng");
            Assert.That(jsonResponse.Msg, Is.EqualTo("Lưu bài đăng thành công."), "Thông báo không đúng");

            // Kiểm tra rằng SaveChanges được gọi vì bài đăng đã được thêm vào
            mockContext.Verify(m => m.SaveChanges(), Times.Once(), "SaveChanges chưa được gọi đúng số lần");
        }

        [Test]
        public void AddFavorite_ShouldReturnCode200_WhenPostAlreadyFavorited()
        {
            // Arrange: Giả lập IdUser và DbContext
            var mockSession = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession.Object;
            byte[] value = BitConverter.GetBytes(1);
            // Giả lập session với IdUser = 1
            mockSession.Setup(s => s.TryGetValue("IdUser", out value)).Returns(true);
            Console.Write(value[0]);
            // Giả lập DbContext với bài đăng đã được yêu thích
            var favoritePost = new TblFavoritePost { IdUser = value[0], IdRoomPost = 101 };
            var data = new List<TblFavoritePost> { favoritePost }.AsQueryable();

            var mockFavoritePostSet = new Mock<DbSet<TblFavoritePost>>();
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>().Setup(m => m.Provider).Returns(data.Provider);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>().Setup(m => m.Expression).Returns(data.Expression);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockFavoritePostSet.As<IQueryable<TblFavoritePost>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            var mockContext = new Mock<DoAnTotNghiepContext>();
            mockContext.Setup(c => c.TblFavoritePosts).Returns(mockFavoritePostSet.Object);



            // Tạo controller với mock context
            var controller = new RoomController(mockContext.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Act: Gọi phương thức addFavorite với id bài đăng đã được yêu thích
            var result = controller.addFavorite(101) as JsonResult;

            // Assert: Kiểm tra kết quả trả về không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra dữ liệu trả về là đúng
            var jsonResponse = result.Value as FavoriteResponse;
            Assert.That(jsonResponse, Is.Not.Null, "Response data is null");

            // Kiểm tra mã phản hồi là 200 khi bài đăng đã được yêu thích
            Assert.That(jsonResponse.Code, Is.EqualTo(200), "Mã phản hồi không đúng");
            Assert.That(jsonResponse.Msg, Is.EqualTo("Bài đăng này đã được lưu."), "Thông báo không đúng");
        }

        [Test]
        public void AddFavorite_ShouldReturnCode400_WhenUserOrPostIdIsInvalid()
        {
            // Arrange: Giả lập session không chứa IdUser
            var mockSession = new Mock<ISession>();
            var httpContext = new DefaultHttpContext();
            httpContext.Session = mockSession.Object;

            // Giả lập không có IdUser trong session
            mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny)).Returns(false);

            var mockContext = new Mock<DoAnTotNghiepContext>();

            // Tạo controller với mock context
            var controller = new RoomController(mockContext.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };

            // Act: Gọi phương thức addFavorite với id bài đăng không hợp lệ (0)
            var result = controller.addFavorite(0) as JsonResult;

            // Assert: Kiểm tra kết quả trả về không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra dữ liệu trả về là đúng
            var jsonResponse = result.Value as FavoriteResponse;
            Assert.That(jsonResponse, Is.Not.Null, "Response data is null");

            // Kiểm tra mã phản hồi là 400 khi IdUser hoặc id bài đăng không hợp lệ
            Assert.That(jsonResponse.Code, Is.EqualTo(400), "Mã phản hồi không đúng");
            Assert.That(jsonResponse.Msg, Is.EqualTo("Lưu bài đăng thất bại."), "Thông báo không đúng");
        }





    }
}
