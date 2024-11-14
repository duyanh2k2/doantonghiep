using DoAn.Controllers;
using DoAn.Models;
using DoAn.ViewModel;
using Microsoft.AspNetCore.Http;
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
    public class DeltailControllerTests
    {
        private Mock<DoAnTotNghiepContext> _mockContext;
        private DeltailController _controller;
        private List<TblRoomPost> _roomPostList;
        private List<TblUser> _userList;
        private List<TblImage> _imageList;
        private List<TblComment> _commentList;
        private List<TblFeedBack> _feedBackList;

        [SetUp]
        public void Setup()
        {
            // Khởi tạo dữ liệu giả lập cho các bảng liên quan đến RoomPost, User, Comment, FeedBack và Image
            _roomPostList = new List<TblRoomPost>
            {
                new TblRoomPost { IdRoomPost = 1, TieuDe = "Phòng trọ đẹp", MoTa = "Mô tả phòng trọ", GiaTien = 1500, DienTich = 20, IdUser = 1, DiaChi = "Hà Nội", NgayDang = DateTime.Now },
                new TblRoomPost { IdRoomPost = 2, TieuDe = "Căn hộ cao cấp", MoTa = "Mô tả căn hộ", GiaTien = 3000, DienTich = 50, IdUser = 2, DiaChi = "Hồ Chí Minh", NgayDang = DateTime.Now }
            };
            _userList = new List<TblUser>
            {
                new TblUser { IdUser = 1, HoTen = "Nguyễn Văn A" },
                new TblUser { IdUser = 2, HoTen = "Nguyễn Văn B" }
            };
            _imageList = new List<TblImage>
            {
                new TblImage { IdImage = 1, IdRoomPost = 1, HinhAnh = "image1.jpg" },
                new TblImage { IdImage = 2, IdRoomPost = 2, HinhAnh = "image2.jpg" }
            };
            // Mock các DbSet
            var mockRoomPostDbSet = new Mock<DbSet<TblRoomPost>>();
            mockRoomPostDbSet.As<IQueryable<TblRoomPost>>().Setup(m => m.Provider).Returns(_roomPostList.AsQueryable().Provider);
            mockRoomPostDbSet.As<IQueryable<TblRoomPost>>().Setup(m => m.Expression).Returns(_roomPostList.AsQueryable().Expression);
            mockRoomPostDbSet.As<IQueryable<TblRoomPost>>().Setup(m => m.ElementType).Returns(_roomPostList.AsQueryable().ElementType);
            mockRoomPostDbSet.As<IQueryable<TblRoomPost>>().Setup(m => m.GetEnumerator()).Returns(_roomPostList.AsQueryable().GetEnumerator());

            var mockUserDbSet = new Mock<DbSet<TblUser>>();
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(_userList.AsQueryable().Provider);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(_userList.AsQueryable().Expression);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(_userList.AsQueryable().ElementType);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(_userList.AsQueryable().GetEnumerator());

            var mockImageDbSet = new Mock<DbSet<TblImage>>();
            mockImageDbSet.As<IQueryable<TblImage>>().Setup(m => m.Provider).Returns(_imageList.AsQueryable().Provider);
            mockImageDbSet.As<IQueryable<TblImage>>().Setup(m => m.Expression).Returns(_imageList.AsQueryable().Expression);
            mockImageDbSet.As<IQueryable<TblImage>>().Setup(m => m.ElementType).Returns(_imageList.AsQueryable().ElementType);
            mockImageDbSet.As<IQueryable<TblImage>>().Setup(m => m.GetEnumerator()).Returns(_imageList.AsQueryable().GetEnumerator());

            var mockComments = new List<TblComment>
            {
                new TblComment { IdRoomPost = 1, NoiDung = "Bình luận 1" },
                new TblComment { IdRoomPost = 1, NoiDung = "Bình luận 2" }
            };

            var mockCommentDbSet = new Mock<DbSet<TblComment>>();
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Provider).Returns(mockComments.AsQueryable().Provider);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Expression).Returns(mockComments.AsQueryable().Expression);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.ElementType).Returns(mockComments.AsQueryable().ElementType);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.GetEnumerator()).Returns(mockComments.GetEnumerator());

            _mockContext = new Mock<DoAnTotNghiepContext>();
            _mockContext.Setup(c => c.TblComments).Returns(mockCommentDbSet.Object);

            _controller = new DeltailController(_mockContext.Object);

            _mockContext = new Mock<DoAnTotNghiepContext>();
            _mockContext.Setup(c => c.TblComments).Returns(mockCommentDbSet.Object);
            _controller = new DeltailController(_mockContext.Object);

            // Mock DbContext
            _mockContext = new Mock<DoAnTotNghiepContext>();
            _mockContext.Setup(c => c.TblRoomPosts).Returns(mockRoomPostDbSet.Object);
            _mockContext.Setup(c => c.TblUsers).Returns(mockUserDbSet.Object);
            _mockContext.Setup(c => c.TblImages).Returns(mockImageDbSet.Object);
            _mockContext.Setup(c => c.TblComments).Returns(mockCommentDbSet.Object);


            // Khởi tạo Controller
            _controller = new DeltailController(_mockContext.Object);
        }

        [Test]
        public void Index_ReturnsView_WhenPostExists()
        {
            // Arrange
            int id = 1;

            // Act
            var result = _controller.Index(id) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var model = result.Model as deltail;
            Assert.That(model, Is.Not.Null, "Model không thể null");
            Assert.That(result is ViewResult, Is.True, "Kết quả phải là ViewResult");

            Assert.That(model.post.TieuDe, Is.EqualTo("Phòng trọ đẹp"), "Tiêu đề không khớp");
            Assert.That(model.image.Count, Is.EqualTo(1), "Số lượng hình ảnh không khớp");
            Assert.That(model.user.HoTen, Is.EqualTo("Nguyễn Văn A"), "Họ tên người dùng không khớp");
        }

        [Test]
        public void Index_Redirects_WhenPostNotFound()
        {
            // Arrange
            int id = 999; // Không có bài đăng với id này

            // Act
            var result = _controller.Index(id) as RedirectToActionResult;

            // Assert
            Assert.That(result, Is.Not.Null, "Kết quả trả về không phải RedirectToActionResult");
            Assert.That(result.ControllerName, Is.EqualTo("Home"), "Controller name không đúng");
            Assert.That(result.ActionName, Is.EqualTo("Index"), "Action name không đúng");
        }

        [Test]
        public void LoadComment_ReturnsComments_WhenValidId()
        {
            int idRoomPost = 1;

            List<TblComment> mockComments = new List<TblComment>
{
    new TblComment { IdRoomPost = 2, NoiDung = "Bình luận 1" },
    new TblComment { IdRoomPost = 1, NoiDung = "Bình luận 2" }
};

            Mock<DoAnTotNghiepContext> mockContext = new Mock<DoAnTotNghiepContext>();

            Mock<DbSet<TblComment>> mockCommentDbSet = new Mock<DbSet<TblComment>>();

            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Provider).Returns(mockComments.AsQueryable().Provider);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Expression).Returns(mockComments.AsQueryable().Expression);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.ElementType).Returns(mockComments.AsQueryable().ElementType);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.GetEnumerator()).Returns(mockComments.GetEnumerator());

            mockContext.Setup(c => c.TblComments).Returns(mockCommentDbSet.Object);

            _controller = new DeltailController(mockContext.Object);


            JsonResult result = _controller.LoadComment(idRoomPost);
            Console.WriteLine(result.Value);
            Assert.That(result.Value, Is.InstanceOf<CommentResponse>(), "Expected result.Value to be of type 'CommentResponse'");

            // Chuyển kết quả thành CommentResponse
            var response = result.Value as CommentResponse;
            Assert.That(response, Is.Not.Null, "Response is not of expected type");

            // Kiểm tra code
            Assert.That(response.code, Is.EqualTo(200), "Response code is not 200");

            // Kiểm tra comments
            Assert.That(response.comments, Is.Not.Null, "'comments' is not a valid list");
            Assert.That(response.comments.Count, Is.EqualTo(2), "Number of comments is not correct");
            Assert.That(response.comments[0].NoiDung, Is.EqualTo("Bình luận 1"));
            Assert.That(response.comments[1].NoiDung, Is.EqualTo("Bình luận 2"));
        }

        [Test]
        public void LoadComment_ReturnsError_WhenInvalidId()
        {
            // Arrange
            int idRoomPost = 0; // ID không hợp lệ

            // Act
            var result = _controller.LoadComment(idRoomPost) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var jsonData = result.Value as CommentResponse;

            // Kiểm tra mã phản hồi và thông báo
            Assert.That(jsonData.code, Is.EqualTo(500));  // Kiểm tra mã lỗi
            Assert.That(jsonData.msg, Is.EqualTo("Bài đăng không tồn tại"));  // Kiểm tra thông báo lỗi
        }

        [Test]
        public void LoadComment_ReturnsError_WhenNoCommentsFound()
        {
            // Arrange: Giả lập không có bình luận
            var mockCommentDbSet = new Mock<DbSet<TblComment>>();
            var emptyCommentList = new List<TblComment>(); // Không có bình luận

            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Provider).Returns(emptyCommentList.AsQueryable().Provider);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Expression).Returns(emptyCommentList.AsQueryable().Expression);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.ElementType).Returns(emptyCommentList.AsQueryable().ElementType);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.GetEnumerator()).Returns(emptyCommentList.AsQueryable().GetEnumerator());

            _mockContext.Setup(c => c.TblComments).Returns(mockCommentDbSet.Object);
            _controller = new DeltailController(_mockContext.Object);

            int idRoomPost = 1; // ID hợp lệ nhưng không có bình luận

            // Act
            var result = _controller.LoadComment(idRoomPost) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var response = result.Value as CommentResponse;
            Assert.That(response, Is.Not.Null, "Response không phải CommentResponse");

            // Kiểm tra mã phản hồi và thông báo
            Assert.That(response.code, Is.EqualTo(404), "Mã phản hồi không đúng");
            Assert.That(response.msg, Is.EqualTo("Không tìm thấy bình luận"), "Thông báo không đúng");
        }


        [Test]
        public void LoadUser_ReturnsUsers_WhenDataIsAvailable()
        {
            // Arrange
            var mockContext = new Mock<DoAnTotNghiepContext>();

            var userList = new List<TblUser>
    {
        new TblUser { IdUser = 1, HoTen = "Nguyễn Văn A" },
        new TblUser { IdUser = 2, HoTen = "Nguyễn Văn B" }
    };

            var mockUserDbSet = new Mock<DbSet<TblUser>>();
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.Provider).Returns(userList.AsQueryable().Provider);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.Expression).Returns(userList.AsQueryable().Expression);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.ElementType).Returns(userList.AsQueryable().ElementType);
            mockUserDbSet.As<IQueryable<TblUser>>().Setup(m => m.GetEnumerator()).Returns(userList.AsQueryable().GetEnumerator());

            mockContext.Setup(c => c.TblUsers).Returns(mockUserDbSet.Object);

            var controller = new DeltailController(mockContext.Object);

            // Act
            var result = controller.LoadUser() as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);

            var jsonData = result.Value as LoadUserResponse;
            Assert.That(jsonData, Is.Not.Null);

            Assert.That(jsonData.code, Is.EqualTo(200));
            Assert.That(jsonData.msg, Is.EqualTo("Lấy dữ liệu thành công"));
            Assert.That(jsonData.u.Count, Is.EqualTo(2));
            Assert.That(jsonData.u[0].HoTen, Is.EqualTo("Nguyễn Văn A"));
            Assert.That(jsonData.u[1].HoTen, Is.EqualTo("Nguyễn Văn B"));
        }

        [Test]
        public void LoadUser_ReturnsError_WhenExceptionOccurs()
        {
            // Arrange
            var mockContext = new Mock<DoAnTotNghiepContext>();

            // Giả lập lỗi khi truy cập vào TblUsers
            var mockUserDbSet = new Mock<DbSet<TblUser>>();
            mockUserDbSet.As<IQueryable<TblUser>>()
                .Setup(m => m.Provider)
                .Throws(new Exception("Database error")); // Ném ngoại lệ khi truy vấn TblUsers

            mockContext.Setup(c => c.TblUsers).Returns(mockUserDbSet.Object);

            var controller = new DeltailController(mockContext.Object);

            // Act
            var result = controller.LoadUser() as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);

            // Kiểm tra kết quả trả về có phải là UserResponse
            var jsonData = result.Value as LoadUserResponse;
            Assert.That(jsonData, Is.Not.Null);

            // Kiểm tra mã phản hồi
            Assert.That(jsonData.code, Is.EqualTo(500), "Mã phản hồi không đúng");

            // Kiểm tra thông báo
            Assert.That(jsonData.msg, Is.EqualTo("Lấy dữ liệu thất bại"), "Thông báo không đúng");
        }

    //    [Test]
    //    public void LoadFeedBack_ReturnsData_WhenNoErrorOccurs()
    //    {
    //        // Arrange: Giả lập dữ liệu phản hồi
    //        var mockFeedBackList = new List<TblFeedBack>
    //{
    //    new TblFeedBack { IdFeedBack = 1, NoiDung = "Phản hồi 1", IdComment = 1, IdUser = 1, IdCommentNavigation = new TblComment { IdRoomPost = 1 } },
    //    new TblFeedBack { IdFeedBack = 2, NoiDung = "Phản hồi 2", IdComment = 2, IdUser = 2, IdCommentNavigation = new TblComment { IdRoomPost = 1 } }
    //};

    //        var mockFeedBackDbSet = new Mock<DbSet<TblFeedBack>>();
    //        mockFeedBackDbSet.As<IQueryable<TblFeedBack>>()
    //            .Setup(m => m.Provider).Returns(mockFeedBackList.AsQueryable().Provider);
    //        mockFeedBackDbSet.As<IQueryable<TblFeedBack>>()
    //            .Setup(m => m.Expression).Returns(mockFeedBackList.AsQueryable().Expression);
    //        mockFeedBackDbSet.As<IQueryable<TblFeedBack>>()
    //            .Setup(m => m.ElementType).Returns(mockFeedBackList.AsQueryable().ElementType);
    //        mockFeedBackDbSet.As<IQueryable<TblFeedBack>>()
    //            .Setup(m => m.GetEnumerator()).Returns(mockFeedBackList.AsQueryable().GetEnumerator());

    //        // Mock DoAnTotNghiepContext
    //        var mockContext = new Mock<DoAnTotNghiepContext>();
    //        mockContext.Setup(c => c.TblFeedBacks).Returns(mockFeedBackDbSet.Object);

    //        var controller = new DeltailController(mockContext.Object);

    //        // Act: Gọi phương thức LoadFeedBack
    //        var result = controller.LoadFeedBack(1) as JsonResult;

    //        // Assert: Kiểm tra kết quả trả về không phải null
    //        Assert.That(result, Is.Not.Null, "JsonResult is null");

    //        var jsonData = result.Value as FeedBackResponse;
    //        Assert.That(jsonData, Is.Not.Null, "Dữ liệu trả về không hợp lệ");

    //        // Kiểm tra mã phản hồi và thông báo
    //        Assert.That(jsonData.code, Is.EqualTo(200), "Mã phản hồi không đúng");
    //        Assert.That(jsonData.msg, Is.EqualTo("Lấy dữ liệu thành công"), "Thông báo không đúng");

    //        // Kiểm tra danh sách phản hồi
    //        Assert.That(jsonData.feedbacks, Is.Not.Null, "Dữ liệu phản hồi không có");
    //        Assert.That(jsonData.feedbacks.Count, Is.GreaterThan(0), "Số lượng phản hồi không đúng");
    //    }
    //    [Test]
    //    public void LoadFeedBack_ReturnsError_WhenPostNotFound()
    //    {
    //        // Arrange: Giả lập không có dữ liệu phản hồi cho bài đăng
    //        var mockFeedBackList = new List<TblFeedBack>(); // Không có dữ liệu phản hồi trong cơ sở dữ liệu

    //        var mockFeedBackDbSet = new Mock<DbSet<TblFeedBack>>();
    //        mockFeedBackDbSet.As<IQueryable<TblFeedBack>>()
    //            .Setup(m => m.Provider).Returns(mockFeedBackList.AsQueryable().Provider);
    //        mockFeedBackDbSet.As<IQueryable<TblFeedBack>>()
    //            .Setup(m => m.Expression).Returns(mockFeedBackList.AsQueryable().Expression);
    //        mockFeedBackDbSet.As<IQueryable<TblFeedBack>>()
    //            .Setup(m => m.ElementType).Returns(mockFeedBackList.AsQueryable().ElementType);
    //        mockFeedBackDbSet.As<IQueryable<TblFeedBack>>()
    //            .Setup(m => m.GetEnumerator()).Returns(mockFeedBackList.AsQueryable().GetEnumerator());

    //        // Mock DoAnTotNghiepContext
    //        var mockContext = new Mock<DoAnTotNghiepContext>();
    //        mockContext.Setup(c => c.TblFeedBacks).Returns(mockFeedBackDbSet.Object);

    //        var controller = new DeltailController(mockContext.Object);

    //        // Act: Gọi phương thức LoadFeedBack với một ID không tồn tại
    //        var result = controller.LoadFeedBack(999) as JsonResult; // Giả sử ID 999 không tồn tại

    //        // Assert: Kiểm tra kết quả trả về không phải null
    //        Assert.That(result, Is.Not.Null, "JsonResult is null");

    //        var jsonData = result.Value as FeedBackResponse;
    //        Assert.That(jsonData, Is.Not.Null, "Dữ liệu trả về không hợp lệ");

    //        // Kiểm tra mã phản hồi và thông báo lỗi
    //        Assert.That(jsonData.code, Is.EqualTo(404), "Mã phản hồi không đúng");  // Kiểm tra mã 404
    //        Assert.That(jsonData.msg, Is.EqualTo("Không tìm thấy phản hồi"), "Thông báo không đúng");  // Kiểm tra thông báo chính xác

    //        // Kiểm tra danh sách phản hồi
    //        Assert.That(jsonData.feedbacks, Is.Null, "Dữ liệu phản hồi không nên có");
    //    }
        [Test]
        public void AddComment_ReturnsError_WhenUserNotLoggedIn()
        {
            // Arrange: Giả lập session không có IdUser
            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny))
                       .Returns(false);  // Giả lập không có giá trị cho "IdUser"

            var mockContext = new Mock<DoAnTotNghiepContext>();
            var controller = new DeltailController(mockContext.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext { Session = mockSession.Object }
                }
            };

            // Act: Gọi phương thức addComment
            var result = controller.addComment("Đây là một bình luận", 1) as JsonResult;

            // Assert: Kiểm tra JsonResult không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra rằng Value của JsonResult không phải là null
            Assert.That(result.Value, Is.Not.Null, "Dữ liệu trả về từ JsonResult là null");


            var jsonData = result.Value as addCommentResponse;
            Assert.That(jsonData, Is.Not.Null, "Dữ liệu trả về không hợp lệ");

            // Kiểm tra mã phản hồi và thông báo
            Assert.That(jsonData.code, Is.EqualTo(500), "Mã phản hồi không đúng");
            Assert.That(jsonData.msg, Is.EqualTo("Vui lòng đăng nhập"), "Thông báo không đúng");
        }

        [Test]
        public void AddComment_ReturnsSuccess_WhenUserLoggedIn()
        {
            // Arrange: Giả lập session có IdUser hợp lệ
            var mockSession = new Mock<ISession>();
            byte[] userId = BitConverter.GetBytes(1);  // Giả lập IdUser có giá trị là 1
            mockSession.Setup(s => s.TryGetValue("IdUser", out userId)).Returns(true);  // Giả lập có giá trị IdUser trong session

            var mockContext = new Mock<DoAnTotNghiepContext>();

            // Giả lập DbSet<TblComment> trong mockContext nếu cần
            var mockDbSet = new Mock<DbSet<TblComment>>();
            mockContext.Setup(c => c.TblComments).Returns(mockDbSet.Object);

            var controller = new DeltailController(mockContext.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext { Session = mockSession.Object }
                }
            };


            // Act: Gọi phương thức addComment
            var result = controller.addComment("Đây là một bình luận", 1) as JsonResult;

            // Assert: Kiểm tra JsonResult không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra rằng Value của JsonResult không phải là null
            Assert.That(result.Value, Is.Not.Null, "Dữ liệu trả về từ JsonResult là null");

            // Ép kiểu JsonResult.Value thành AddCommentResponse
            var jsonData = result.Value as addCommentResponse;
            Assert.That(jsonData, Is.Not.Null, "Dữ liệu trả về không hợp lệ");

            // Kiểm tra mã phản hồi và thông báo thành công
            Assert.That(jsonData.code, Is.EqualTo(200), "Mã phản hồi không đúng");
            Assert.That(jsonData.msg, Is.EqualTo("Thành công"), "Thông báo không đúng");

            // Kiểm tra danh sách bình luận
            Assert.That(jsonData.comments, Is.Not.Null, "Dữ liệu bình luận không có");
            Assert.That(jsonData.comments.Count, Is.GreaterThan(0), "Không có bình luận trong dữ liệu trả về");
        }
        [Test]
        public void AddComment_ReturnsError_WhenInputIsInvalid()
        {
            // Arrange: Giả lập session với IdUser hợp lệ
            var mockSession = new Mock<ISession>();
            byte[] userId = BitConverter.GetBytes(1);  // Giả lập IdUser hợp lệ
            mockSession.Setup(s => s.TryGetValue("IdUser", out userId)).Returns(true);  // Giả lập có giá trị IdUser trong session

            var mockContext = new Mock<DoAnTotNghiepContext>();

            var controller = new DeltailController(mockContext.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext { Session = mockSession.Object }
                }
            };

            // Act: Gọi phương thức addComment với comment trống
            var result = controller.addComment("", 1) as JsonResult;

            // Assert: Kiểm tra JsonResult không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra giá trị trả về có đúng mã lỗi và thông báo
            var jsonData = result.Value as addCommentResponse;
            Assert.That(jsonData, Is.Not.Null, "Dữ liệu trả về không hợp lệ");
            Assert.That(jsonData.code, Is.EqualTo(501), "Mã phản hồi không đúng");
            Assert.That(jsonData.msg, Is.EqualTo("Thất bại"), "Thông báo không đúng");
        }
        [Test]
        public void AddFeedBack_ReturnsSuccess_WhenInputIsValid()
        {
            // Arrange: Giả lập session có IdUser hợp lệ
            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny))
                       .Returns((string key, out byte[] value) =>
                       {
                           if (key == "IdUser")
                           {
                               value = BitConverter.GetBytes(1);  // Giả lập IdUser là 1 (hợp lệ)
                               return true;  // Trả về true nếu tìm thấy giá trị
                           }
                           value = null;
                           return false;
                       });

            // Giả lập DbSet TblFeedBacks trong DoAnTotNghiepContext
            var mockFeedBackDbSet = new Mock<DbSet<TblFeedBack>>();
            var mockContext = new Mock<DoAnTotNghiepContext>();
            mockContext.Setup(c => c.TblFeedBacks).Returns(mockFeedBackDbSet.Object);

            var controller = new DeltailController(mockContext.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext { Session = mockSession.Object }
                }
            };

            // Act: Gọi phương thức addFeedBack với dữ liệu hợp lệ
            var result = controller.addFeedBack("Đây là một phản hồi", 1) as JsonResult;

            // Assert: Kiểm tra JsonResult không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra rằng Value của JsonResult không phải là null
            Assert.That(result.Value, Is.Not.Null, "Dữ liệu trả về từ JsonResult là null");

            // Ép kiểu JsonResult.Value thành FeedBackResponse
            var jsonData = result.Value as addFeedBackResponse;
            Assert.That(jsonData, Is.Not.Null, "Dữ liệu trả về không hợp lệ");

            // Kiểm tra mã phản hồi và thông báo thành công
            Assert.That(jsonData.code, Is.EqualTo(200), "Mã phản hồi không đúng");
            Assert.That(jsonData.msg, Is.EqualTo("Thành công"), "Thông báo không đúng");
        }

        [Test]
        public void AddFeedBack_ReturnsError_WhenUserNotLoggedIn()
        {
            // Arrange: Giả lập session không có giá trị IdUser (người dùng chưa đăng nhập)
            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.TryGetValue("IdUser", out It.Ref<byte[]>.IsAny))
                       .Returns(false);  // Giả lập không tìm thấy IdUser trong session

            // Giả lập DbSet TblFeedBacks trong DoAnTotNghiepContext
            var mockFeedBackDbSet = new Mock<DbSet<TblFeedBack>>();
            var mockContext = new Mock<DoAnTotNghiepContext>();
            mockContext.Setup(c => c.TblFeedBacks).Returns(mockFeedBackDbSet.Object);

            var controller = new DeltailController(mockContext.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext { Session = mockSession.Object }
                }
            };

            // Act: Gọi phương thức addFeedBack với dữ liệu hợp lệ (feedback và idComment hợp lệ)
            var result = controller.addFeedBack("Đây là một phản hồi", 1) as JsonResult;

            // Assert: Kiểm tra JsonResult không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra rằng Value của JsonResult không phải là null
            Assert.That(result.Value, Is.Not.Null, "Dữ liệu trả về từ JsonResult là null");

            // Ép kiểu JsonResult.Value thành FeedBackResponse
            var jsonData = result.Value as addFeedBackResponse;
            Assert.That(jsonData, Is.Not.Null, "Dữ liệu trả về không hợp lệ");

            // Kiểm tra mã phản hồi và thông báo lỗi (vì người dùng chưa đăng nhập)
            Assert.That(jsonData.code, Is.EqualTo(500), "Mã phản hồi không đúng");
            Assert.That(jsonData.msg, Is.EqualTo("Vui lòng đăng nhập"), "Thông báo không đúng");
        }

        [Test]
        public void AddFeedBack_ReturnsError_WhenInputIsInvalid()
        {
            // Arrange: Giả lập không có IdUser trong session
            var mockSession = new Mock<ISession>();
            mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out It.Ref<byte[]>.IsAny))
                       .Returns(false); // Giả lập người dùng chưa đăng nhập

            var mockContext = new Mock<DoAnTotNghiepContext>();
            var controller = new DeltailController(mockContext.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext { Session = mockSession.Object }
                }
            };

            // Act: Gọi phương thức addFeedBack với dữ liệu không hợp lệ (feedback trống và idComment = 0)
            var result = controller.addFeedBack("", 0) as JsonResult;

            // Assert: Kiểm tra JsonResult không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra giá trị trả về không phải null
            Assert.That(result.Value, Is.Not.Null, "Dữ liệu trả về là null");

            // Ép kiểu JsonResult.Value thành đối tượng trả về và kiểm tra
            var jsonData = result.Value as addFeedBackResponse;
            Assert.That(jsonData, Is.Not.Null, "Dữ liệu trả về không hợp lệ");

            // Kiểm tra mã phản hồi là 501 (đầu vào không hợp lệ)
            Assert.That(jsonData.code, Is.EqualTo(501), "Mã phản hồi không đúng");

            // Kiểm tra thông báo là "Thất bại"
            Assert.That(jsonData.msg, Is.EqualTo("Thất bại"), "Thông báo không đúng");
        }

        [Test]
        public void RemoveComment_ReturnsSuccess_WhenCommentExists()
        {
            // Arrange: Tạo đối tượng giả lập TblComment và TblFeedBack
            var commentId = 1;
            var comment = new TblComment { IdComment = commentId, NoiDung = "Comment to be removed" };
            var feedback = new TblFeedBack { IdFeedBack = 1, IdComment = commentId, NoiDung = "Feedback related to comment" };

            // Tạo DbSet giả lập cho TblComment
            var mockCommentDbSet = new Mock<DbSet<TblComment>>();
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Provider).Returns(new List<TblComment> { comment }.AsQueryable().Provider);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Expression).Returns(new List<TblComment> { comment }.AsQueryable().Expression);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.ElementType).Returns(new List<TblComment> { comment }.AsQueryable().ElementType);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.GetEnumerator()).Returns(new List<TblComment> { comment }.AsQueryable().GetEnumerator());

            // Tạo DbSet giả lập cho TblFeedBack
            var mockFeedBackDbSet = new Mock<DbSet<TblFeedBack>>();
            mockFeedBackDbSet.As<IQueryable<TblFeedBack>>().Setup(m => m.Provider).Returns(new List<TblFeedBack> { feedback }.AsQueryable().Provider);
            mockFeedBackDbSet.As<IQueryable<TblFeedBack>>().Setup(m => m.Expression).Returns(new List<TblFeedBack> { feedback }.AsQueryable().Expression);
            mockFeedBackDbSet.As<IQueryable<TblFeedBack>>().Setup(m => m.ElementType).Returns(new List<TblFeedBack> { feedback }.AsQueryable().ElementType);
            mockFeedBackDbSet.As<IQueryable<TblFeedBack>>().Setup(m => m.GetEnumerator()).Returns(new List<TblFeedBack> { feedback }.AsQueryable().GetEnumerator());

            // Mock DoAnTotNghiepContext
            var mockContext = new Mock<DoAnTotNghiepContext>();
            mockContext.Setup(c => c.TblComments).Returns(mockCommentDbSet.Object);
            mockContext.Setup(c => c.TblFeedBacks).Returns(mockFeedBackDbSet.Object);

            // Mock SaveChanges
            mockContext.Setup(m => m.SaveChanges()).Returns(1);  // Trả về số lượng bản ghi bị ảnh hưởng

            // Tạo controller với mock context
            var controller = new DeltailController(mockContext.Object);

            // Act: Gọi phương thức RemoveComment
            var result = controller.removeComment(commentId) as JsonResult;

            // Assert: Kiểm tra kết quả trả về không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra dữ liệu trả về là đúng kiểu RemoveCommentResponse
            Assert.That(result.Value, Is.InstanceOf<RemoveCommentResponse>(), "Dữ liệu trả về không phải RemoveCommentResponse");

            // Kiểm tra mã phản hồi và thông báo
            var jsonData = result.Value as RemoveCommentResponse;
            Assert.That(jsonData.code, Is.EqualTo(200), "Mã phản hồi không đúng");
            Assert.That(jsonData.msg, Is.EqualTo("Xóa bình luận thành công"), "Thông báo không đúng");

            // Kiểm tra rằng SaveChanges được gọi đúng số lần
            mockContext.Verify(m => m.SaveChanges(), Times.Once(), "SaveChanges chưa được gọi đúng số lần");
        }

        [Test]
        public void RemoveComment_ReturnsError_WhenCommentDoesNotExist()
        {
            // Arrange: Tạo DbSet giả lập cho TblComment không có bất kỳ bản ghi nào (không có bình luận)
            var commentId = 1; // ID bình luận không tồn tại trong cơ sở dữ liệu giả lập
            var commentList = new List<TblComment>().AsQueryable();  // Danh sách bình luận trống, không có bình luận với ID = commentId

            // Giả lập DbSet cho TblComment không có phần tử nào
            var mockCommentDbSet = new Mock<DbSet<TblComment>>();
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Provider).Returns(commentList.Provider);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.Expression).Returns(commentList.Expression);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.ElementType).Returns(commentList.ElementType);
            mockCommentDbSet.As<IQueryable<TblComment>>().Setup(m => m.GetEnumerator()).Returns(commentList.GetEnumerator());

            // Giả lập DbSet cho TblFeedBack (không có feedback nào liên quan)
            var mockFeedBackDbSet = new Mock<DbSet<TblFeedBack>>();

            // Mock DoAnTotNghiepContext
            var mockContext = new Mock<DoAnTotNghiepContext>();
            mockContext.Setup(c => c.TblComments).Returns(mockCommentDbSet.Object);
            mockContext.Setup(c => c.TblFeedBacks).Returns(mockFeedBackDbSet.Object);

            // Mock SaveChanges không thay đổi gì vì không có bình luận để xóa
            mockContext.Setup(m => m.SaveChanges()).Returns(0);  // Không có bản ghi nào được thay đổi

            // Tạo controller với mock context
            var controller = new DeltailController(mockContext.Object);

            // Act: Gọi phương thức RemoveComment với ID không hợp lệ
            var result = controller.removeComment(commentId) as JsonResult;

            // Assert: Kiểm tra kết quả trả về không phải null
            Assert.That(result, Is.Not.Null, "JsonResult is null");

            // Kiểm tra dữ liệu trả về là đúng kiểu RemoveCommentResponse
            Assert.That(result.Value, Is.InstanceOf<RemoveCommentResponse>(), "Dữ liệu trả về không phải RemoveCommentResponse");

            // Kiểm tra mã phản hồi và thông báo lỗi
            var jsonData = result.Value as RemoveCommentResponse;
            Assert.That(jsonData.code, Is.EqualTo(501), "Mã phản hồi không đúng");
            Assert.That(jsonData.msg, Is.EqualTo("Xóa bình luận thất bại"), "Thông báo không đúng");

            // Kiểm tra rằng SaveChanges không được gọi vì không có bình luận nào để xóa
            mockContext.Verify(m => m.SaveChanges(), Times.Never, "SaveChanges() không nên được gọi khi không tìm thấy bình luận");
        }



















    }
}
