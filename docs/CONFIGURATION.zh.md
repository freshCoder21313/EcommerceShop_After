# 详细配置指南

本文档提供有关如何配置运行 EcommerceShop 项目所需的环境变量和服务的详细说明。

## 后端配置 (`appsettings.json`)

后端配置在 `APIClothesEcommerceShop/APIClothesEcommerceShop/appsettings.json` 文件中管理。为安全起见，建议在开发环境中使用“用户机密”来存储敏感值。

### 1. 数据库连接字符串

- **键:** `ConnectionStrings:EcommerceShopConnect_Dot`
- **描述:** SQL Server 数据库的连接字符串。
- **示例:** `"Server=localhost;Database=EcommerceShopDb;User Id=your_user;Password=your_password;"`

### 2. Cloudinary

图像存储和管理服务。

- **键:**
  - `CloudinarySettings:CloudName`
  - `CloudinarySettings:ApiKey`
  - `CloudinarySettings:ApiSecret`
- **如何获取:** 在 [Cloudinary](https://cloudinary.com/) 注册帐户，然后从仪表板获取信息。

### 3. Firebase

用于实时功能和身份验证。

- **键:** `Firebase:ApiKey`
- **如何获取:** 在 [Firebase 控制台](https://console.firebase.google.com/) 上创建一个新项目，并从项目设置中获取 API 密钥。

### 4. Google 身份验证

使用 Google 帐户进行用户身份验证。

- **键:**
  - `Google:ClientId`
  - `Google:ClientSecret`
- **如何获取:** 在 [Google Cloud Console](https://console.cloud.google.com/apis/credentials) 中创建 OAuth 2.0 客户端 ID。

### 5. VNPAY

在线支付网关。

- **键:**
  - `VNPAY:TmnCode`
  - `VNPAY:HashSecret`
- **如何获取:** 与 [VNPAY](https://vnpay.vn/) 注册企业帐户以接收信息。

### 6. MailKit (SMTP)

电子邮件发送服务。

- **键:**
  - `MailSettings:Mail`
  - `MailSettings:DisplayName`
  - `MailSettings:Password`
  - `MailSettings:Host`
  - `MailSettings:Port`
- **如何获取:** 使用您的电子邮件服务提供商（例如 Gmail、SendGrid）的信息。

### 7. Gemini AI

用于 AI 功能。

- **键:** `Gemini:ApiKey`
- **如何获取:** 从 [Google AI Studio](https://aistudio.google.com/) 获取 API 密钥。

## 前端配置 (`.env`)

在前端的根目录 (`ECOMMERCESHOPUXUI/EcommerceProject`) 中创建一个 `.env` 文件。

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

- **描述:** 后端 API 的 URL。
- **值:** `https://localhost:7217` (如果更改，则为相应的端口)。

### 2. Firebase

- **描述:** 从前端连接到 Firebase 的密钥。
- **如何获取:** 从 [Firebase 控制台](https://console.firebase.google.com/) 的项目设置中获取此信息。

### 3. Google reCAPTCHA

- **描述:** reCAPTCHA 服务的站点密钥。
- **如何获取:** 在 [Google reCAPTCHA 管理控制台](https://www.google.com/recaptcha/admin/) 上注册一个新站点。
