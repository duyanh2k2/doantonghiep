using DoAn.Controllers;
using DoAn.Models;
using DoAn.Services;
using DoAn.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestProject1.Controller
{
    [TestFixture]
    public class QuanLyControllerTest
    {
        private QuanLyController _controller;
        private Mock<DoAnTotNghiepContext> _contextMock;
        private Mock<SmsService> _smsServiceMock;
        private DefaultHttpContext _httpContext;
        private DoAnTotNghiepContext context;
        private Mock<IFormFile> _mockFile;
        [SetUp]
        public void SetUp()
        {
            _contextMock = new Mock<DoAnTotNghiepContext>();
            _smsServiceMock = new Mock<SmsService>();

            var httpContextMock = new Mock<HttpContext>();
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new TestSession(); // Tạo Session giả
            _httpContext.Session.SetInt32("IdUser", 1); // Set IdUser trong session
            var tempData = new TempDataDictionary(httpContextMock.Object, Mock.Of<ITempDataProvider>());

            _controller = new QuanLyController(_contextMock.Object, null)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = _httpContext
                },
                TempData = tempData
            };
            _mockFile = new Mock<IFormFile>();
            var content = "Fake image content"; // Nội dung giả cho file
            var fileName = "test-image.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            _mockFile.Setup(_ => _.OpenReadStream()).Returns(ms);
            _mockFile.Setup(_ => _.FileName).Returns(fileName);
            _mockFile.Setup(_ => _.ContentType).Returns("image/jpeg");
            _mockFile.Setup(_ => _.Length).Returns(ms.Length);
        }

        [Test]
        public void Index_ReturnsViewResult_WithAddRoomModel_WhenUserIdExistsInSession()
        {
            // Arrange
            var userId = 1;

            var user = new TblUser { IdUser = userId, HoTen = "Test User" };
            var users = new List<TblUser> { user }.AsQueryable();
            var userDbSetMock = new Mock<DbSet<TblUser>>();
            userDbSetMock.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(users.Provider);
            userDbSetMock.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(users.Expression);
            userDbSetMock.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            userDbSetMock.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            _contextMock.Setup(c => c.TblUsers).Returns(userDbSetMock.Object);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOf<AddRoom>(result.Model);
            var model = result.Model as AddRoom;
            Assert.AreEqual(userId, model.User.IdUser);
        }

        [Test]
        public void Index_ReturnsEmptyViewResult_WhenUserIdDoesNotExistInSession()
        {
            // Arrange
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new TestSession(); // Tạo Session giả
            _httpContext.Session.SetInt32("IdUser", 0); // Set IdUser trong session
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Model);
        }

        [Test]
        public void Index_ReturnsEmptyViewResult_WhenUserNotFoundInDatabase()
        {

            // Arrange
            var userId = 1;
            //_sessionMock.Setup(s => s.GetInt32("IdUser")).Returns(userId);

            var users = new List<TblUser>().AsQueryable(); // Database empty
            var userDbSetMock = new Mock<DbSet<TblUser>>();
            userDbSetMock.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(users.Provider);
            userDbSetMock.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(users.Expression);
            userDbSetMock.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            userDbSetMock.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());
            _contextMock.Setup(c => c.TblUsers).Returns(userDbSetMock.Object);

            // Act
            var result = _controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNull(result.Model);
        }
        [Test]
        public async Task AddRoomPost_ValidInput_RedirectsToIndexWithSuccess()
        {
            var room = new AddRoom
            {
                roomPost = new TblRoomPost
                {
                    TieuDe = "Test Title",
                    MoTa = "Test Description",
                    DiaChi = "Test Address",
                    DienTich = 20,
                    GiaTien = 1000
                }
            };
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(_ => _.FileName).Returns("testImage.jpg");
            fileMock.Setup(_ => _.Length).Returns(1024);
            fileMock.Setup(_ => _.OpenReadStream()).Returns(new MemoryStream(new byte[1024]));

            var images = new List<IFormFile> { fileMock.Object };



            var roomPostsData = new List<TblRoomPost>
            {
                new TblRoomPost
                {
                    IdRoomPost = 1,
                    TieuDe = "Test Title",
                    MoTa = "Test Description",
                    DiaChi = "Test Address"
                }
            }.AsQueryable();

            var roomPostSetMock = new Mock<DbSet<TblRoomPost>>();
            roomPostSetMock.As<IQueryable<TblRoomPost>>().Setup(m => m.Provider).Returns(roomPostsData.Provider);
            roomPostSetMock.As<IQueryable<TblRoomPost>>().Setup(m => m.Expression).Returns(roomPostsData.Expression);
            roomPostSetMock.As<IQueryable<TblRoomPost>>().Setup(m => m.ElementType).Returns(roomPostsData.ElementType);
            roomPostSetMock.As<IQueryable<TblRoomPost>>().Setup(m => m.GetEnumerator()).Returns(roomPostsData.GetEnumerator());
            _contextMock.Setup(m => m.TblRoomPosts).Returns(roomPostSetMock.Object);

            var imageSetMock = new Mock<DbSet<TblImage>>();
            _contextMock.Setup(m => m.TblImages).Returns(imageSetMock.Object);
            imageSetMock.Setup(m => m.Add(It.IsAny<TblImage>()));

            _contextMock.Setup(m => m.SaveChanges()).Returns(1);

            // Act
            var result = await _controller.addRoomPost(room, images) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.IsTrue(_controller.TempData.ContainsKey("success"));
            Assert.AreEqual("Đăng bài thành công", _controller.TempData["success"]);
        }

        [Test]
        public async Task AddRoomPost_InvalidInput_RedirectsToIndexWithError()
        {
            // Arrange
            var roomPost = new TblRoomPost(); // Không cung cấp đủ thông tin
            var addRoom = new AddRoom { roomPost = roomPost };
            var images = new List<IFormFile>(); // Không có hình ảnh

            // Act
            var result = await _controller.addRoomPost(addRoom, images) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ActionName);
            Assert.AreEqual("Đăng bài thất bại", _controller.TempData["error"]);

            _contextMock.Verify(c => c.TblRoomPosts.Add(It.IsAny<TblRoomPost>()), Times.Never);
            _contextMock.Verify(c => c.SaveChanges(), Times.Never);
        }
        [Test]
        public void UpdetInfo_WithValidUserInfo_ShouldUpdateUserAndRedirectToUserInfo()
        {
            // Arrange
            var user = new TblUser
            {
                TaiKhoan = "user123",
                Sdt = "123456789",
                HoTen = "Test User",
                CanCuoc = "1234567890",
                Gt = true,

            };

            // Mock dữ liệu người dùng
            var usersData = new List<TblUser>
    {
        new TblUser { IdUser = 1, TaiKhoan = "user123", Sdt = "123456789", HoTen = "Old Name", CanCuoc = "0000000000" }
    }.AsQueryable();

            // Mock DbSet TblUsers
            var usersMock = new Mock<DbSet<TblUser>>();
            usersMock.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(usersData.Provider);
            usersMock.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(usersData.Expression);
            usersMock.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(usersData.ElementType);
            usersMock.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(usersData.GetEnumerator());
            // Mock DbContext
            _contextMock.Setup(c => c.TblUsers).Returns(usersMock.Object);

            // Mock HttpContext.Session
            // Simulate the current user's IdUser is 1

            // Mock the Controller with the context and session
            // Act
            var result = _controller.UpdetInfo(user) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("UserInfo", result.ActionName);
            Assert.IsTrue(_controller.TempData.ContainsKey("success"));
            Assert.AreEqual("Thay đổi thông tin thành công", _controller.TempData["success"]);

            // Verify if the DbContext was updated
            _contextMock.Verify(m => m.SaveChanges(), Times.Once);
        }
        [Test]
        public void UpdetInfo_WithInvalidUserInfo_ShouldReturnError()
        {
            // Arrange
            var user = new TblUser
            {
                TaiKhoan = "user123",
                Sdt = "", // Invalid phone number
                HoTen = "Test User",
                CanCuoc = "1234567890",
                Gt = true
            };

            // Mock DbSet TblUsers and DbContext as before

            // Act
            var result = _controller.UpdetInfo(user) as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Thay đổi thông tin thất bại", _controller.TempData["error"]);
        }
        [Test]
        public void UpdateRoomPost_ValidId_ReturnsCorrectViewWithModel()
        {
            // Arrange
            var roomId = 1;
            var userId = 1;

            var roomPost = new TblRoomPost { IdRoomPost = roomId, TieuDe = "Test Room", MoTa = "Room Description", GiaTien = 1000, DienTich = 20, IdUser = userId };
            var images = new List<TblImage>
    {
        new TblImage { IdImage = 1, IdRoomPost = roomId, HinhAnh = "/uploads/image1.jpg" },
        new TblImage { IdImage = 2, IdRoomPost = roomId, HinhAnh = "/uploads/image2.jpg" }
    };
            var user = new TblUser { IdUser = userId, HoTen = "Test User", TaiKhoan = "testuser", Sdt = "123456789" };

            // Mock DbSet<TblRoomPost>
            var roomPostData = new List<TblRoomPost> { roomPost }.AsQueryable();
            var roomPostMock = new Mock<DbSet<TblRoomPost>>();
            roomPostMock.As<IQueryable<TblRoomPost>>().Setup(m => m.Provider).Returns(roomPostData.Provider);
            roomPostMock.As<IQueryable<TblRoomPost>>().Setup(m => m.Expression).Returns(roomPostData.Expression);
            roomPostMock.As<IQueryable<TblRoomPost>>().Setup(m => m.ElementType).Returns(roomPostData.ElementType);
            roomPostMock.As<IQueryable<TblRoomPost>>().Setup(m => m.GetEnumerator()).Returns(roomPostData.GetEnumerator());

            // Mock DbSet<TblImage>
            var imageData = images.AsQueryable();
            var imageMock = new Mock<DbSet<TblImage>>();
            imageMock.As<IQueryable<TblImage>>().Setup(m => m.Provider).Returns(imageData.Provider);
            imageMock.As<IQueryable<TblImage>>().Setup(m => m.Expression).Returns(imageData.Expression);
            imageMock.As<IQueryable<TblImage>>().Setup(m => m.ElementType).Returns(imageData.ElementType);
            imageMock.As<IQueryable<TblImage>>().Setup(m => m.GetEnumerator()).Returns(imageData.GetEnumerator());

            // Mock DbSet<TblUser>
            var userData = new List<TblUser> { user }.AsQueryable();
            var userMock = new Mock<DbSet<TblUser>>();
            userMock.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(userData.Provider);
            userMock.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(userData.Expression);
            userMock.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(userData.ElementType);
            userMock.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(userData.GetEnumerator());

            // Mock DbContext

            _contextMock.Setup(c => c.TblRoomPosts).Returns(roomPostMock.Object);
            _contextMock.Setup(c => c.TblImages).Returns(imageMock.Object);
            _contextMock.Setup(c => c.TblUsers).Returns(userMock.Object);

            // Act
            var result = _controller.updateRoomPost(roomId) as ViewResult;
            var model = result?.Model as updatePost;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(model);
            Assert.AreEqual("Test Room", model.roomPost.TieuDe);
            Assert.AreEqual(2, model.imageList.Count);
            Assert.AreEqual("Test User", model.User.HoTen);
            Assert.AreEqual(roomId, model.roomPost.IdRoomPost);
        }
        [Test]
        public async Task UpdateRoomPost_ValidInput_ReturnsRedirectToPostManagerWithSuccess()
        {
            // Arrange
            var roomId = 1;
            var roomPost = new TblRoomPost
            {
                IdRoomPost = roomId,
                TieuDe = "Test Room",
                MoTa = "Room Description",
                DiaChi = "Test Address",
                DienTich = 20,
                GiaTien = 1000,
                IdUser = 1
            };

            var updatedRoom = new updatePost
            {
                roomPost = new TblRoomPost
                {
                    IdRoomPost = roomId,
                    TieuDe = "Updated Room",
                    MoTa = "Updated Room Description",
                    DiaChi = "Updated Address",
                    DienTich = 30,
                    GiaTien = 1500
                }
            };

            // Mock DbSet<TblRoomPost>
            var roomPostData = new List<TblRoomPost> { roomPost }.AsQueryable();
            var roomPostMock = new Mock<DbSet<TblRoomPost>>();
            roomPostMock.As<IQueryable<TblRoomPost>>().Setup(m => m.Provider).Returns(roomPostData.Provider);
            roomPostMock.As<IQueryable<TblRoomPost>>().Setup(m => m.Expression).Returns(roomPostData.Expression);
            roomPostMock.As<IQueryable<TblRoomPost>>().Setup(m => m.ElementType).Returns(roomPostData.ElementType);
            roomPostMock.As<IQueryable<TblRoomPost>>().Setup(m => m.GetEnumerator()).Returns(roomPostData.GetEnumerator());
            roomPostMock.Setup(r => r.FindAsync(It.IsAny<object[]>())).ReturnsAsync(roomPost); // Mock the FindAsync method

            // Mock DbContext

            _contextMock.Setup(c => c.TblRoomPosts).Returns(roomPostMock.Object);

            // Mock HttpContext.Session

            // Act
            var result = await _controller.updateRoomPost(updatedRoom) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("PostManager", result.ActionName);
            Assert.AreEqual("QuanLy", result.ControllerName);
            Assert.AreEqual("Cập nhật thành công", _controller.TempData["success"]);
        }
        [Test]
        public async Task UpdateRoomPost_RoomPostNotFound_ReturnsRedirectToPostManagerWithError()
        {
            // Arrange
            var roomId = 999;  // Sử dụng Id không tồn tại trong cơ sở dữ liệu.
            var updatedRoom = new updatePost
            {
                roomPost = new TblRoomPost
                {
                    IdRoomPost = roomId,
                    TieuDe = "Updated Room",
                    MoTa = "Updated Room Description",
                    DiaChi = "Updated Address",
                    DienTich = 30,
                    GiaTien = 1500
                }
            };

            // Mock DbSet<TblRoomPost>
            var roomPostMock = new Mock<DbSet<TblRoomPost>>();
            roomPostMock.Setup(r => r.FindAsync(It.IsAny<object[]>())).ReturnsAsync((TblRoomPost)null); // Trả về null nếu không tìm thấy bài viết.

            // Mock DbContext

            _contextMock.Setup(c => c.TblRoomPosts).Returns(roomPostMock.Object);

            // Mock HttpContext.Session


            // Act
            var result = await _controller.updateRoomPost(updatedRoom) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("PostManager", result.ActionName);
            Assert.AreEqual("QuanLy", result.ControllerName);
            Assert.AreEqual("Cập nhật thất bại", _controller.TempData["error"]);
        }
        [Test]
        public async Task UpdateRoomPost_InvalidInput_ReturnsRedirectToPostManagerWithError()
        {
            // Arrange
            var updatedRoom = new updatePost
            {
                roomPost = new TblRoomPost
                {
                    IdRoomPost = 1,  // Giả lập Id hợp lệ
                    TieuDe = "",      // TieuDe rỗng => không hợp lệ
                    MoTa = "Updated Room Description",
                    DiaChi = "Updated Address",
                    DienTich = 30,
                    GiaTien = 1500
                }
            };

            // Mock DbSet<TblRoomPost>
            var roomPostMock = new Mock<DbSet<TblRoomPost>>();
            roomPostMock.Setup(r => r.FindAsync(It.IsAny<object[]>())).ReturnsAsync(new TblRoomPost
            {
                IdRoomPost = 1,
                TieuDe = "Old Room",
                MoTa = "Old Description",
                DiaChi = "Old Address",
                DienTich = 25,
                GiaTien = 1000,
                IdUser = 1
            }); // Giả lập tìm thấy bài viết cũ.

            // Mock DbContext

            _contextMock.Setup(c => c.TblRoomPosts).Returns(roomPostMock.Object);

            // Mock HttpContext.Session


            // Act
            var result = await _controller.updateRoomPost(updatedRoom) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("PostManager", result.ActionName);
            Assert.AreEqual("QuanLy", result.ControllerName);
            Assert.AreEqual("Cập nhật thất bại", _controller.TempData["error"]);
        }
        [Test]
        public void LoadRoomPost_ValidUser_ReturnsSuccess()
        {
            // Arrange


            // Mocking a list of TblRoomPosts for the given user
            var roomPosts = new List<TblRoomPost>
            {
                new TblRoomPost { IdRoomPost = 1, TieuDe = "Room 1", DiaChi = "Address 1",IdUser=1 },
                new TblRoomPost { IdRoomPost = 2, TieuDe = "Room 2", DiaChi = "Address 2",IdUser=1 }
            };

            var roomPostDbSetMock = new Mock<DbSet<TblRoomPost>>();
            roomPostDbSetMock.As<IQueryable<TblRoomPost>>()
                .Setup(m => m.Provider).Returns(roomPosts.AsQueryable().Provider);
            roomPostDbSetMock.As<IQueryable<TblRoomPost>>()
                .Setup(m => m.Expression).Returns(roomPosts.AsQueryable().Expression);
            roomPostDbSetMock.As<IQueryable<TblRoomPost>>()
                .Setup(m => m.ElementType).Returns(roomPosts.AsQueryable().ElementType);
            roomPostDbSetMock.As<IQueryable<TblRoomPost>>()
                .Setup(m => m.GetEnumerator()).Returns(roomPosts.GetEnumerator());

            _contextMock.Setup(c => c.TblRoomPosts).Returns(roomPostDbSetMock.Object);

            // Act
            var result = _controller.loadRoomPost() as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = JsonConvert.SerializeObject(result.Value);
            // Lấy kết quả từ JsonResult và ép kiểu thành dynamic
            dynamic response = JsonConvert.DeserializeObject(json);
            Assert.AreEqual(200, (int)response.code);
            Assert.AreEqual("Lấy dữ liệu thành công", response.msg.ToString());
            Assert.AreEqual(2, response.r.Count);
        }

        [Test]
        public void LoadRoomPost_InvalidUser_ReturnsFailure()
        {
            // Arrange
            _httpContext.Session.SetInt32("IdUser", 0);

            // Act
            var result = _controller.loadRoomPost() as JsonResult;

            // Assert
            Assert.IsNotNull(result);
            var json = JsonConvert.SerializeObject(result.Value);
            // Lấy kết quả từ JsonResult và ép kiểu thành dynamic
            dynamic response = JsonConvert.DeserializeObject(json);
            Assert.AreEqual(400, (int)response.code);
            Assert.AreEqual("Lấy dữ liệu thất bại", response.msg.ToString());
        }
        [Test]
        public async Task RemoveRoomPost_RoomExistsAndHasNoLessee_ReturnsSuccess()
        {
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase1")
            .Options;
            context = new DoAnTotNghiepContext(options);
            _controller = new QuanLyController(context, null);

            // Thêm dữ liệu vào cơ sở dữ liệu InMemory
            context.TblRoomPosts.Add(new TblRoomPost { IdRoomPost = 1, MoTa = "1233", TieuDe = "1314", DiaChi = "Test Address" });
            context.SaveChanges();
            var result = await _controller.removeRoomPost(1);

            // Kiểm tra kết quả trả về
            var jsonResult = result as JsonResult;
            Assert.AreEqual(200, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Xóa bài đăng thành công", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));
        }
        [Test]
        public async Task RemoveRoomPost_RoomPostExistsButHasLessee_ReturnsError()
        {
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase1")
               .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);

            // Thêm dữ liệu giả lập vào cơ sở dữ liệu InMemory
            context.TblRoomPosts.Add(new TblRoomPost { IdRoomPost = 1, MoTa = "1233", TieuDe = "1314", DiaChi = "Test Address" });
            context.SaveChanges();
            // Thêm dữ liệu giả lập cho TblLessees (giả sử có người thuê)
            context.TblLessees.Add(new TblLessee { IdRoomPost = 1, IdLessee = 1 });
            context.SaveChanges();

            // Thực hiện gọi phương thức removeRoomPost với RoomPost có người thuê
            var result = await _controller.removeRoomPost(1);

            // Kiểm tra kết quả trả về
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(202, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Vui lòng xóa người thuê", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));
        }
        [Test]
        public void AddLess_RoomPostAndUserValid_AddsLesseeSuccessfully()
        {

            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);

            // Thêm dữ liệu giả lập vào cơ sở dữ liệu InMemory nếu cần thiết
            context.TblRoomPosts.Add(new TblRoomPost { IdRoomPost = 1, MoTa = "1233", TieuDe = "1314", DiaChi = "Test Address" });
            context.TblUsers.Add(new TblUser { IdUser = 1, HoTen = "Test User", TaiKhoan = "testuser", MatKhau = "password", CanCuoc = "121314123", Gt = true, Sdt = "0398759537" });
            context.SaveChanges();
            // Gọi phương thức addLess với user và room post hợp lệ
            var result = _controller.addLess(1, 1); // IdUser = 1 và IdRoomPost = 1

            // Kiểm tra kết quả trả về
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            // Kiểm tra mã trả về và thông báo
            Assert.AreEqual(200, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Thêm người thuê thành công", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));

            // Kiểm tra dữ liệu đã được thêm vào TblLessees
            var lessee = context.TblLessees.FirstOrDefault(x => x.IdUser == 1 && x.IdRoomPost == 1);
            Assert.IsNotNull(lessee);
            Assert.AreEqual(1, lessee.IdUser);
            Assert.AreEqual(1, lessee.IdRoomPost);
        }

        [Test]
        public void AddLess_RoomPostAlreadyHasLessee_ReturnsError()
        {
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);

            // Thêm dữ liệu giả lập vào cơ sở dữ liệu InMemory nếu cần thiết

            // Thêm một người thuê giả lập vào TblLessees
            context.TblLessees.Add(new TblLessee { IdUser = 2, IdRoomPost = 2 });
            context.SaveChanges();

            // Gọi phương thức addLess với user và room post đã có người thuê
            var result = _controller.addLess(2, 2); // IdUser = 1 và IdRoomPost = 1

            // Kiểm tra kết quả trả về
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            // Kiểm tra mã trả về và thông báo lỗi
            Assert.AreEqual(201, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Phòng này đã có người thuê", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));
        }

        [Test]
        public void AddLess_InvalidUserOrRoomPost_ReturnsError()
        {
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase2")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);

            // Gọi phương thức addLess với IdUser và IdRoomPost không hợp lệ
            var result = _controller.addLess(0, 0); // IdUser = 0 và IdRoomPost = 0 (không hợp lệ)

            // Kiểm tra kết quả trả về
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            // Kiểm tra mã trả về và thông báo lỗi
            Assert.AreEqual(201, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Phòng này đã có người thuê", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));
        }

        [Test]
        public async Task AddLess_ThrowsException_ReturnsErrorMessage()
        {
            // Làm cho phương thức throw exception bằng cách giả lập lỗi
            var invalidContext = new Mock<DoAnTotNghiepContext>().Object;
            var controllerWithInvalidContext = new QuanLyController(invalidContext, null);

            // Gọi phương thức addLess với IdUser và IdRoomPost hợp lệ nhưng context không hợp lệ
            var result = controllerWithInvalidContext.addLess(1, 1);

            // Kiểm tra kết quả trả về là lỗi
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(500, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
        }
        [Test]
        public async Task RemoveLess_ValidOtp_RemovesLessee()
        {
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);
            var lessee = new TblLessee { IdLessee = 1, IdUser = 1, IdRoomPost = 1 };
            context.TblLessees.Add(lessee);
            context.SaveChanges();
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new TestSession(); // Tạo Session giả
            _httpContext.Session.SetString("Otp", "123456");
            _httpContext.Session.SetString("OtpTime", DateTime.Now.AddMinutes(1).ToString());// Set IdUser trong session
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Arrange
            int idLess = 1;
            string validOtp = "123456";

            // Giả lập OTP và thời gian hết hạn trong Session


            // Act
            var result = await _controller.removeLess(idLess, validOtp);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.AreEqual(200, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Xóa người thuê thành công", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));
        }
        [Test]
        public async Task RemoveLess_InvalidOtp_ReturnsError()
        {
            // Arrange
            int idLess = 1;
            string invalidOtp = "654321";

            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new TestSession(); // Tạo Session giả
            _httpContext.Session.SetString("Otp", "123456");
            _httpContext.Session.SetString("OtpTime", DateTime.Now.AddMinutes(1).ToString());// Set IdUser trong session
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = await _controller.removeLess(idLess, invalidOtp);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(201, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Mã xác nhận không đúng hoặc đã hết hạn", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));

        }
        [Test]
        public async Task RemoveLess_ExpiredOtp_ReturnsError()
        {
            // Arrange
            int idLess = 1;
            string invalidOtp = "123456";

            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new TestSession(); // Tạo Session giả
            _httpContext.Session.SetString("Otp", "123456");
            _httpContext.Session.SetString("OtpTime", DateTime.Now.AddMinutes(-1).ToString());// Set IdUser trong session
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = await _controller.removeLess(idLess, invalidOtp);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            Assert.AreEqual(201, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Mã xác nhận không đúng hoặc đã hết hạn", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));

        }
        [Test]
        public async Task RemoveLess_InvalidId_ReturnsError()
        {
            // Arrange
            int invalidId = 0;
            string validOtp = "123456";

            // Giả lập OTP và thời gian hết hạn trong Session
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);
            _httpContext = new DefaultHttpContext();
            _httpContext.Session = new TestSession(); // Tạo Session giả
            _httpContext.Session.SetString("Otp", "123456");
            _httpContext.Session.SetString("OtpTime", DateTime.Now.AddMinutes(-1).ToString());// Set IdUser trong session
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext
            };

            // Act
            var result = await _controller.removeLess(invalidId, validOtp);

            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);

            Assert.AreEqual(202, jsonResult?.Value?.GetType().GetProperty("code")?.GetValue(jsonResult.Value));
            Assert.AreEqual("Xóa người thuê thất bại", jsonResult?.Value?.GetType().GetProperty("msg")?.GetValue(jsonResult.Value));

        }
        [Test]
        public async Task UploadSingleImage_ValidFile_ReturnsSuccess()
        {
            // Arrange

            // Khởi tạo DbContext với cấu hình InMemory

            int roomId = 1;  // ID của phòng trọ mà bạn muốn upload ảnh
            var file = _mockFile.Object;
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);
            // Act
            var result = await _controller.UploadSingleImage(file, roomId);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var json = JsonConvert.SerializeObject(jsonResult.Value);
            // Lấy kết quả từ JsonResult và ép kiểu thành dynamic
            dynamic response = JsonConvert.DeserializeObject(json);
            Assert.AreEqual(true, (bool)response.success);
            Assert.AreEqual($"/uploads/test-image.jpg", response.imagePath.ToString());

            // Kiểm tra xem ảnh đã được lưu vào cơ sở dữ liệ
            var savedImage = await context.TblImages.FindAsync((int)response.idImage);
            Assert.IsNotNull(savedImage);
            Assert.AreEqual($"/uploads/test-image.jpg", savedImage.HinhAnh);
        }
        [Test]
        public async Task UploadSingleImage_NoFile_ReturnsError()
        {
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);
            // Arrange
            IFormFile file = null;
            int roomId = 1;

            // Act
            var result = await _controller.UploadSingleImage(file, roomId);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var json = JsonConvert.SerializeObject(jsonResult.Value);
            // Lấy kết quả từ JsonResult và ép kiểu thành dynamic
            dynamic response = JsonConvert.DeserializeObject(json);
            Assert.AreEqual(false, (bool)response.success);
            Assert.AreEqual("No image uploaded.", response.message.ToString());
        }
        [Test]
        public async Task DeleteImage_InvalidId_ReturnsError()
        {
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
               .UseInMemoryDatabase(databaseName: "TestDatabase3")
               .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);
            // Arrange
            int invalidId = -1;

            // Act
            var result = await _controller.DeleteImage(invalidId);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var json = JsonConvert.SerializeObject(jsonResult.Value);
            // Lấy kết quả từ JsonResult và ép kiểu thành dynamic
            dynamic response = JsonConvert.DeserializeObject(json);
            Assert.AreEqual(false, (bool)response.success);
            Assert.AreEqual("Hình ảnh không tồn tại", response.message.ToString());
        }

        [Test]
        public async Task DeleteImage_Success_ReturnsSuccess()
        {
            // Arrange
            int imageId = 1;

            // Set up mock data
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            _controller = new QuanLyController(context, null);
            context.TblImages.Add(new TblImage { IdImage = imageId, HinhAnh = "/uploads/test.jpg", IdRoomPost = 1 });
            await context.SaveChangesAsync();

            // Create a new controller instanc

            // Act
            var result = await _controller.DeleteImage(imageId);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var json = JsonConvert.SerializeObject(jsonResult.Value);
            // Lấy kết quả từ JsonResult và ép kiểu thành dynamic
            dynamic response = JsonConvert.DeserializeObject(json);
            Assert.AreEqual(true, (bool)response.success);

            // Verify that the image was deleted
            var deletedImage = await context.TblImages.FindAsync(imageId);
            Assert.IsNull(deletedImage);
        }

        [Test]
        public async Task DeleteImage_ErrorDeletingImage_ReturnsError()
        {
            // Arrange
            int imageId = 1;

            // Create an empty in-memory database context
            var options = new DbContextOptionsBuilder<DoAnTotNghiepContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase3")
                .Options;

            // Khởi tạo DbContext với cấu hình InMemory
            context = new DoAnTotNghiepContext(options);

            // Khởi tạo controller với DbContext đã cấu hình
            var controller = new QuanLyController(context, null);

            // Mock exception for the SaveChangesAsync method
            var exceptionMessage = "Error deleting image";
            context.Database.EnsureCreated(); // Ensures the in-memory database is created.

            // Simulate an exception in saving changes
            var mock = new Mock<DoAnTotNghiepContext>(options);
            mock.Setup(db => db.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await controller.DeleteImage(imageId);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.IsNotNull(jsonResult);
            var json = JsonConvert.SerializeObject(jsonResult.Value);
            // Lấy kết quả từ JsonResult và ép kiểu thành dynamic
            dynamic response = JsonConvert.DeserializeObject(json);
            Assert.AreEqual(false, (bool)response.success);
        }

    }
}

public class TestSession : ISession
{
    private Dictionary<string, byte[]> _sessionStorage = new Dictionary<string, byte[]>();

    public IEnumerable<string> Keys => _sessionStorage.Keys;

    public void Clear() => _sessionStorage.Clear();

    public bool TryGetValue(string key, out byte[] value) => _sessionStorage.TryGetValue(key, out value);

    public void Set(string key, byte[] value) => _sessionStorage[key] = value;

    public void Remove(string key) => _sessionStorage.Remove(key);

    public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public bool IsAvailable => true;

    public string Id => Guid.NewGuid().ToString();
}
