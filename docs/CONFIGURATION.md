# Hướng dẫn Cấu hình Chi tiết

Tài liệu này cung cấp hướng dẫn chi tiết về cách cấu hình các biến môi trường và dịch vụ cần thiết để chạy dự án EcommerceShop.

## Cấu hình Backend (`appsettings.json`)

Các cấu hình cho backend được quản lý trong file `APIClothesEcommerceShop/APIClothesEcommerceShop/appsettings.json`. Để bảo mật, bạn nên sử dụng "User Secrets" trong môi trường phát triển để lưu các giá trị nhạy cảm.

### 1. Chuỗi kết nối Database

- **Key:** `ConnectionStrings:EcommerceShopConnect_Dot`
- **Mô tả:** Chuỗi kết nối đến cơ sở dữ liệu SQL Server.
- **Ví dụ:** `"Server=localhost;Database=EcommerceShopDb;User Id=your_user;Password=your_password;"`

### 2. Cloudinary

Dịch vụ lưu trữ và quản lý hình ảnh.

- **Keys:**
  - `CloudinarySettings:CloudName`
  - `CloudinarySettings:ApiKey`
  - `CloudinarySettings:ApiSecret`
- **Cách lấy:** Đăng ký tài khoản tại [Cloudinary](https://cloudinary.com/), sau đó lấy thông tin từ dashboard.

### 3. Firebase

Sử dụng cho các tính năng real-time và xác thực.

- **Key:** `Firebase:ApiKey`
- **Cách lấy:** Tạo một dự án mới trên [Firebase Console](https://console.firebase.google.com/) và lấy API key từ cài đặt dự án.

### 4. Google Authentication

Xác thực người dùng bằng tài khoản Google.

- **Keys:**
  - `Google:ClientId`
  - `Google:ClientSecret`
- **Cách lấy:** Tạo một OAuth 2.0 Client ID trong [Google Cloud Console](https://console.cloud.google.com/apis/credentials).

### 5. VNPAY

Cổng thanh toán trực tuyến.

- **Keys:**
  - `VNPAY:TmnCode`
  - `VNPAY:HashSecret`
- **Cách lấy:** Đăng ký tài khoản doanh nghiệp với [VNPAY](https://vnpay.vn/) để nhận thông tin.

### 6. MailKit (SMTP)

Dịch vụ gửi email.

- **Keys:**
  - `MailSettings:Mail`
  - `MailSettings:DisplayName`
  - `MailSettings:Password`
  - `MailSettings:Host`
  - `MailSettings:Port`
- **Cách lấy:** Sử dụng thông tin từ nhà cung cấp dịch vụ email của bạn (ví dụ: Gmail, SendGrid).

### 7. Gemini AI

Sử dụng cho các tính năng AI.

- **Key:** `Gemini:ApiKey`
- **Cách lấy:** Lấy API key từ [Google AI Studio](https://aistudio.google.com/).

## Cấu hình Frontend (`.env`)

Tạo một file `.env` trong thư mục gốc của frontend (`ECOMMERCESHOPUXUI/EcommerceProject`).

```
VITE_API_URL=https://localhost:7217

VITE_FIREBASE_API_KEY=YOUR_FIREBASE_API_KEY_HERE
VITE_FIREBASE_AUTH_DOMAIN=YOUR_FIREBASE_AUTH_DOMAIN_HERE
VITE_FIREBASE_PROJECT_ID=YOUR_FIREBASE_PROJECT_ID_HERE
VITE_FIREBASE_STORAGE_BUCKET=YOUR_FIREBASE_STORAGE_BUCKET_HERE
VITE_FIREBASE_MESSAGING_SENDER_ID=YOUR_FIREBASE_MESSAGING_SENDER_ID_HERE
VITE_FIREBASE_APP_ID=YOUR_FIREBASE_APP_ID_HERE

VITE_RECAPTCHA_SITE_KEY=YOUR_RECAPTCHA_SITE_KEY_HERE
```

### 1. VITE_API_URL

- **Mô tả:** URL của backend API.
- **Giá trị:** `https://localhost:7217` (hoặc port tương ứng nếu bạn thay đổi).

### 2. Firebase

- **Mô tả:** Các keys để kết nối với Firebase từ frontend.
- **Cách lấy:** Lấy thông tin này từ cài đặt dự án trên [Firebase Console](https://console.firebase.google.com/).

### 3. Google reCAPTCHA

- **Mô tả:** Site key cho dịch vụ reCAPTCHA.
- **Cách lấy:** Đăng ký một site mới trên [Google reCAPTCHA admin console](https://www.google.com/recaptcha/admin/).
