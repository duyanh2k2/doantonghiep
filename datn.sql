CREATE DATABASE DoAnTotNghiep
GO

USE DoAnTotNghiep
CREATE TABLE tblUser (
	idUser INT IDENTITY(1,1) PRIMARY KEY,
	TaiKhoan VARCHAR(255) NOT NULL,
	MatKhau VARCHAR(255) NOT NULL,
	SDT VARCHAR(10) NOT NULL,
	HoTen NVARCHAR(255) NOT NULL,
	GT BIT NOT NULL,
	CanCuoc VARCHAR(12) NOT NULL,
	idRole INT NOT NULL
)

CREATE TABLE tblRoomPost
(
	idRoomPost INT PRIMARY KEY IDENTITY(1,1),
	TieuDe NVARCHAR(255) NOT NULL,
	MoTa NVARCHAR(1000) NOT NULL,
	NgayDang DATE NOT NULL,
	GiaTien FLOAT NOT NULL,
	DienTich FLOAT NOT NULL,
	idUser INT NOT NULL,
)
CREATE TABLE tblComment(
	idComment INT IDENTITY(1,1) PRIMARY KEY,
	NoiDung NVARCHAR(255) NOT NULL,
	idUser INT NOT NULL,
	idRoomPost INT NOT NULL
)
CREATE TABLE tblFeedBack(
	idFeedBack INT IDENTITY(1,1) PRIMARY KEY,
	NoiDung NVARCHAR(255) NOT NULL,
	idUser INT NOT NULL,
	idComment INT NOT NULL
)
CREATE TABLE tblFavoritePost(
	idFavoritePost INT IDENTITY(1,1) PRIMARY KEY,
	idUser INT NOT NULL,
	idRoomPost INT NOT NULL,
	UNIQUE(idUser,idRoomPost)
)

CREATE TABLE tblSupport(
	idSupport INT IDENTITY(1,1) PRIMARY KEY,
	TuKhoa NVARCHAR(255) NOT NULL UNIQUE,
	TraLoi NVARCHAR(1000) NOT NULL,
	idUser INT NOT NULL,
)
CREATE TABLE tblLessee(
	idLessee INT IDENTITY(1,1) PRIMARY KEY,
	idUser INT UNIQUE NOT NULL,
	idRoomPost INT NOT NULL
)
CREATE TABLE tblImage(
	idImage INT IDENTITY(1,1) PRIMARY KEY,
	idRoomPost INT NOT NULL,
	HinhAnh NVARCHAR(255) NOT NULL,
)
CREATE TABLE tblRole(
	idRole INT IDENTITY(1,1) PRIMARY KEY,
	sRole NVARCHAR(50) NOT NULL
)
ALTER TABLE dbo.tblUser ADD CONSTRAINT FK_User_Role FOREIGN KEY(idRole) REFERENCES dbo.tblRole(idRole)
GO 
ALTER TABLE dbo.tblRoomPost ADD CONSTRAINT FK_RoomPost_User FOREIGN KEY(idUser) REFERENCES dbo.tblUser(idUser)	

GO 
ALTER TABLE dbo.tblComment ADD CONSTRAINT FK_Comment_RoomPost FOREIGN KEY(idRoomPost) REFERENCES dbo.tblRoomPost(idRoomPost),	
CONSTRAINT FK_Comment_User FOREIGN KEY(idUser) REFERENCES dbo.tblUser(idUser)	
GO
ALTER TABLE dbo.tblFeedBack ADD CONSTRAINT FK_FeedBack_User FOREIGN KEY(idUser) REFERENCES dbo.tblUser(idUser),
CONSTRAINT FK_FeedBack_Comment FOREIGN KEY(idComment) REFERENCES dbo.tblComment(idComment)
GO
ALTER TABLE dbo.tblFavoritePost ADD CONSTRAINT FK_FavoritePost_User FOREIGN KEY(idUser) REFERENCES dbo.tblUser(idUser),
CONSTRAINT FK_FavoritePost_RoomPost FOREIGN KEY(idRoomPost) REFERENCES dbo.tblRoomPost(idRoomPost)
GO
ALTER TABLE dbo.tblSupport ADD CONSTRAINT FK_Support_User FOREIGN KEY(idUser) REFERENCES dbo.tblUser(idUser)
GO
ALTER TABLE dbo.tblLessee ADD CONSTRAINT FK_Lessee_User FOREIGN KEY(idUser) REFERENCES dbo.tblUser(idUser),	
CONSTRAINT FK_Lessee_RoomPost FOREIGN KEY(idRoomPost) REFERENCES dbo.tblRoomPost(idRoomPost)
GO
ALTER TABLE dbo.tblImage ADD CONSTRAINT FK_Image_RoomPost FOREIGN KEY(idRoomPost) REFERENCES dbo.tblRoomPost(idRoomPost)
GO

CREATE PROC pSearch
@Tinh NVARCHAR(30)=NULL,
@Quan NVARCHAR(30)=NULL,
@Phuong NVARCHAR(30)=NULL,
@fromArea FLOAT=NULL,
@toArea FLOAT=NULL,
@fromPrice FLOAT=NULL,
@toPrice FLOAT=NULL
AS
BEGIN
    SELECT * FROM dbo.tblRoomPost
	WHERE (@Tinh IS NULL OR DiaChi LIKE '%'+@Tinh+'%') 
	AND (@Quan IS NULL OR DiaChi LIKE '%'+@Quan+'%')
	AND (@Phuong IS NULL OR DiaChi LIKE '%'+@Phuong+'%')
	AND (@fromArea IS NULL OR (DienTich BETWEEN @fromArea AND @toArea))
	AND (@fromPrice IS NULL OR (GiaTien BETWEEN @fromPrice AND @toPrice))
END
EXEC dbo.pSearch @Tinh = N'Hà Nội',      -- nvarchar(30)
                 @Quan = N'Hoàng Mai',      -- nvarchar(30)
                 @Phuong = N'Hoàng Văn Thụ',    -- nvarchar(30)
                 @fromArea = 20,  -- float
                 @toArea = 30,    -- float
                 @fromPrice = null, -- float
                 @toPrice = null    -- float

