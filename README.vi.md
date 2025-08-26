[Tiếng Việt](./README.vi.md) | [English](./README.md) | [中文](./README.zh.md)

# EcommerceShop - Cửa Hàng Quần Áo Trực Tuyến Hiện Đại

Nền tảng thương mại điện tử toàn diện dành cho việc bán quần áo trực tuyến, xây dựng theo kiến trúc hiện đại, tách rời. Dự án có API phần backend mở rộng và giao diện người dùng linh hoạt, mang đến trải nghiệm mua sắm hấp dẫn.

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)

## Mục Lục

- [Tính Năng](#🚀-tính-năng)
- [Công nghệ sử dụng](#🚀-công-nghệ-sử-dụng)
- [Cài Đặt](#🛠️-cài-đặt)
- [Sử Dụng](#🎯-sử-dụng)
- [Cấu Hình](#⚙️-cấu-hình)
- [Kiến Trúc](#🏗️-kiến-trúc)
- [Cấu Trúc Dự Án](#📂-cấu-trúc-dự-án)
- [Tác Giả](#👥-tác-giả)
- [Giấy Phép](#📜-giấy-phép)

---

## 🚀 Tính Năng

- 🛒 **Thư Viện Sản Phẩm:** Duyệt sản phẩm theo danh mục, xem chi tiết mô tả, hình ảnh rõ nét.
- 🔐 **Xác Thực Khách Hàng:** Đăng nhập, đăng ký an toàn qua JWT và xác thực Google.
- 🛍️ **Giỏ Hàng & Quản Lý Đơn Hàng:** Thêm, sửa, xóa sản phẩm, đặt hàng, xem lịch sử mua, theo dõi trạng thái.
- 💳 **Thanh Toán An Toàn:** Tích hợp thanh toán qua VNPAY.
- 📊 **Bảng Điều Khiển Quản Trị:** Quản lý sản phẩm, danh mục, khách hàng, đơn hàng.
- 💡 **Hệ Thống Gợi Ý:** Đề xuất sản phẩm cá nhân hoá dựa trên ML.NET.
- 📧 **Dịch Vụ Gửi Email:** Tự động gửi xác nhận đơn hàng, thông báo, mã giảm giá.
- 🌟 **Các Tính Năng Nâng Cao Khác:**
  - So sánh sản phẩm (có API AI thử thử ảo)
  - Vòng quay may mắn giảm giá
  - Chatbot hỗ trợ và tư vấn khách hàng
  - Xem lịch sử duyệt gần nhất

---

## 🚀 Công nghệ sử dụng

### Backend
- **Framework:** ASP.NET Core 8.0
- **Database:** SQL Server
- **Authentication:** JWT Bearer, Google Authentication
- **Cloud Services:** Cloudinary, Firebase (Admin, Database), Google Cloud Storage
- **Email:** MailKit
- **Caching:** Redis
- **Machine Learning:** Microsoft.ML
- **API Documentation:** Swashbuckle (Swagger)
- **Payment:** VNPAY

### Frontend
- **Framework:** Vue.js 3
- **Build Tool:** Vite
- **State Management:** Pinia
- **Routing:** Vue Router
- **UI Frameworks/Libraries:** Bootstrap 5, jQuery, SweetAlert2, Swiper
- **HTTP Client:** Axios
- **Charts:** Chart.js

---

## 🛠️ Cài Đặt

**Yêu cầu hệ thống**:
- Windows 10/11 hoặc Linux/macOS
- .NET 8 SDK
- Node.js (phiên bản LTS khuyên dùng)
- SQL Server hoặc SQL Server Express/LocalDB

**Các bước thực hiện**:

```bash
# 1. Clone kho chứa mã nguồn
git clone <URL_REPOSITORY> # Thay thế bằng URL repo của bạn
cd EcommerceShop

# 2. Cài đặt Backend - API
cd APIClothesEcommerceShop/APIClothesEcommerceShop
# Cấu hình kết nối Database trong `appsettings.json`
# Ví dụ:
# "EcommerceShopConnect_Dot": "Server=.;Database=EcommerceShopDb;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet restore
dotnet ef database update
dotnet run
# API sẽ có thể truy cập tại https://localhost:7217/swagger hoặc http://localhost:7218/swagger

# 3. Cài đặt Frontend - Giao diện người dùng
cd ../../ECOMMERCESHOPUXUI/EcommerceProject
npm install
npm run dev
# Giao diện người dùng sẽ có thể truy cập tại http://localhost:5173
```

---

## 🎯 Sử Dụng

Sau khi cả hai dịch vụ backend và frontend đều hoạt động:

- Mở trình duyệt truy cập [http://localhost:5173](http://localhost:5173)
- Sử dụng giao diện thân thiện để duyệt sản phẩm, đăng ký/đăng nhập, thêm sản phẩm vào giỏ, thanh toán qua VNPAY.

---

## ⚙️ Cấu Hình

Dự án yêu cầu cấu hình các biến môi trường để hoạt động chính xác. Để biết chi tiết về cách cấu hình biến môi trường và lấy API keys, vui lòng tham khảo [Hướng dẫn Cấu hình chi tiết](./docs/CONFIGURATION.md).


### Backend (`appsettings.json`)
Cần cập nhật các thông tin nhạy cảm trong `appsettings.json` hoặc sử dụng User Secrets cho môi trường phát triển.
- `ConnectionStrings:EcommerceShopConnect_Dot`: Chuỗi kết nối đến SQL Server.
- `CloudinarySettings`: API keys cho dịch vụ Cloudinary.
- `Firebase`: API key cho Firebase.
- `Google`: Client ID và Client Secret cho Google Authentication.
- `VNPAY`: TmnCode và HashSecret cho cổng thanh toán VNPAY.
- `MailSettings`: Cấu hình SMTP để gửi email.
- `Gemini:ApiKey`: API key cho Gemini AI.

### Frontend (`.env`)
Tạo file `.env` trong thư mục `ECOMMERCESHOPUXUI/EcommerceProject` và thêm các biến sau:
- `VITE_API_URL`: URL của backend API (ví dụ: `https://localhost:7217`).
- `VITE_FIREBASE_API_KEY`: API key cho Firebase.
- `VITE_FIREBASE_AUTH_DOMAIN`: Domain xác thực của Firebase.
- `VITE_FIREBASE_PROJECT_ID`: Project ID của Firebase.
- `VITE_FIREBASE_STORAGE_BUCKET`: Storage Bucket của Firebase.
- `VITE_FIREBASE_MESSAGING_SENDER_ID`: Sender ID của Firebase Messaging.
- `VITE_FIREBASE_APP_ID`: App ID của Firebase.
- `VITE_RECAPTCHA_SITE_KEY`: Site key cho Google reCAPTCHA.

---

## 🏗️ Kiến Trúc Hệ Thống

Hệ thống theo kiến trúc tách biệt, client-server:

- **API Backend:** Viết bằng .NET 8 Web API, quản lý logic nghiệp vụ, dữ liệu, tích hợp dịch vụ thứ ba và cung cấp API RESTful.
- **Giao diện Frontend:** Phát triển bằng Vue.js 3, dùng Vite làm công cụ build, Pinia quản lý trạng thái, Vue Router điều hướng, Bootstrap 5 UI. Tạo trải nghiệm tương tác nhanh, mượt mà.

---

## 📂 Cấu Trúc Dự Án

```
/
├── APIClothesEcommerceShop/                # API Backend
│   ├── Controllers/                        # Các điểm cuối API
│   ├── DTO/                                # Data Transfer Objects
│   ├── Data/                               # DbContext, Migration
│   ├── Models/                             # Mô hình dữ liệu
│   ├── Repositories/                       # Lớp tiếp cận dữ liệu
│   └── Services/                           # Logic nghiệp vụ
└── ECOMMERCESHOPUXUI/                        # Frontend Vue.js
    └── EcommerceProject/
        ├── src/
        │   ├── assets/                     # Hình ảnh, tài nguyên
        │   ├── components/                 # Components dùng chung
        │   ├── views/                      # Các trang view
        │   ├── router/                     # Cấu hình định tuyến
        │   ├── stores/                     # Quản lý trạng thái
        │   └── services/                   # Call API
        └── package.json
```

---


## Tác giả

- **Nhóm đóng góp chính**: Silent Stack Team

- **Người đóng góp**:
<p align="center">
    <a href="https://github.com/dat0968/EcommerceShop/graphs/contributors">
      <img src="https://contrib.rocks/image?repo=dat0968/EcommerceShop" style="max-width: 400px;" />
    </a>
</p>


---

## 📜 Giấy Phép

Dự án được cấp phép theo Giấy phép MIT. Xem chi tiết tại [LICENSE.md](LICENSE.md).