# Detailed Configuration Guide

This document provides detailed instructions on how to configure the environment variables and services required to run the EcommerceShop project.

## Backend Configuration (`appsettings.json`)

Backend configurations are managed in the `APIClothesEcommerceShop/APIClothesEcommerceShop/appsettings.json` file. For security, it is recommended to use "User Secrets" in the development environment to store sensitive values.

### 1. Database Connection String

- **Key:** `ConnectionStrings:EcommerceShopConnect_Dot`
- **Description:** The connection string to the SQL Server database.
- **Example:** `"Server=localhost;Database=EcommerceShopDb;User Id=your_user;Password=your_password;"`

### 2. Cloudinary

Image storage and management service.

- **Keys:**
  - `CloudinarySettings:CloudName`
  - `CloudinarySettings:ApiKey`
  - `CloudinarySettings:ApiSecret`
- **How to get:** Register for an account at [Cloudinary](https://cloudinary.com/), then get the information from the dashboard.

### 3. Firebase

Used for real-time features and authentication.

- **Key:** `Firebase:ApiKey`
- **How to get:** Create a new project on the [Firebase Console](https://console.firebase.google.com/) and get the API key from the project settings.

### 4. Google Authentication

User authentication using Google accounts.

- **Keys:**
  - `Google:ClientId`
  - `Google:ClientSecret`
- **How to get:** Create an OAuth 2.0 Client ID in the [Google Cloud Console](https://console.cloud.google.com/apis/credentials).

### 5. VNPAY

Online payment gateway.

- **Keys:**
  - `VNPAY:TmnCode`
  - `VNPAY:HashSecret`
- **How to get:** Register for a business account with [VNPAY](https://vnpay.vn/) to receive the information.

### 6. MailKit (SMTP)

Email sending service.

- **Keys:**
  - `MailSettings:Mail`
  - `MailSettings:DisplayName`
  - `MailSettings:Password`
  - `MailSettings:Host`
  - `MailSettings:Port`
- **How to get:** Use the information from your email service provider (e.g., Gmail, SendGrid).

### 7. Gemini AI

Used for AI features.

- **Key:** `Gemini:ApiKey`
- **How to get:** Get the API key from [Google AI Studio](https://aistudio.google.com/).

## Frontend Configuration (`.env`)

Create a `.env` file in the root directory of the frontend (`ECOMMERCESHOPUXUI/EcommerceProject`).

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

- **Description:** The URL of the backend API.
- **Value:** `https://localhost:7217` (or the corresponding port if you change it).

### 2. Firebase

- **Description:** The keys to connect to Firebase from the frontend.
- **How to get:** Get this information from the project settings on the [Firebase Console](https://console.firebase.google.com/).

### 3. Google reCAPTCHA

- **Description:** The site key for the reCAPTCHA service.
- **How to get:** Register a new site on the [Google reCAPTCHA admin console](https://www.google.com/recaptcha/admin/).
