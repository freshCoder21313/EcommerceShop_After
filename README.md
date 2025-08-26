[Tiếng Việt](./README.vi.md) | [English](./README.md) | [中文](./README.zh.md)

# EcommerceShop - Modern Online Clothing Store

A comprehensive e-commerce platform for selling clothes online, built with a modern, decoupled architecture. This project features a scalable backend API and a dynamic frontend, offering an engaging shopping experience.

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)

## Table of Contents

- [Features](#🚀-features)
- [Tech Stack](#🚀-tech-stack)
- [Installation](#🛠️-installation)
- [Usage](#🎯-usage)
- [Configuration](#⚙️-configuration)
- [Architecture](#🏗️-architecture)
- [Project Structure](#📂-project-structure)
- [Authors](#👥-authors)
- [License](#📜-license)

---

## 🚀 Features

- 🛒 **Product Catalog:** Browse products by categories, view detailed descriptions, and images.
- 🔐 **Customer Authentication:** Secure login and registration using JWT and Google Authentication.
- 🛍️ **Shopping Cart & Order Management:** Add, modify, or remove products from cart, place orders, view order history, and track statuses.
- 💳 **Secure Payment Integration:** Payment processing via VNPAY.
- 📊 **Admin Dashboard:** Manage products, categories, customers, and orders.
- 💡 **Recommendation System:** Personalized product suggestions powered by ML.NET.
- 📧 **Email Service:** Automate sending order confirmations, notifications, and promo codes.
- 🌟 **Advanced Customer Features:**
  - Product comparison (with AI API for virtual try-on)
  - Spin-the-wheel coupons for discounts
  - Chatbot for support and inquiries
  - View recent browsing history

---

## 🚀 Tech Stack

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

## 🛠️ Installation

**System Requirements**:
- Windows 10/11 or Linux/macOS
- .NET 8 SDK
- Node.js (LTS version recommended)
- SQL Server or SQL Server Express/LocalDB

**Steps**:

```bash
# 1. Clone the repository
git clone <REPOSITORY_URL> # Replace with your repository URL
cd EcommerceShop

# 2. Backend Setup - API
cd APIClothesEcommerceShop/APIClothesEcommerceShop
# Configure the Database connection in `appsettings.json`
# Example:
# "EcommerceShopConnect_Dot": "Server=.;Database=EcommerceShopDb;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet restore
dotnet ef database update
dotnet run
# The API will be accessible at https://localhost:7217/swagger or http://localhost:7218/swagger

# 3. Frontend Setup - User Interface
cd ../../ECOMMERCESHOPUXUI/EcommerceProject
npm install
npm run dev
# The user interface will be accessible at http://localhost:5173
```

---

## 🎯 Usage

Once both backend and frontend services are running:

- Open your browser and go to [http://localhost:5173](http://localhost:5173)
- Use the intuitive UI to browse products, register/login, add items to the cart, and check out with VNPAY.

---

## ⚙️ Configuration

The project requires environment variables to be configured to work correctly. For details on how to configure environment variables and get API keys, please refer to the [Detailed Configuration Guide](./docs/CONFIGURATION.en.md).

---

## 🏗️ Architecture

The system follows a decoupled, client-server architecture:

- **Backend API:** Built with .NET 8 Web API, managing business logic, data storage, third-party integrations, and serving RESTful endpoints.
- **Frontend Application:** Developed with Vue.js 3, utilizing Vite as the build tool, Pinia for state management, Vue Router for navigation, and Bootstrap 5 for UI styling. It provides a fast, interactive user experience.

---

## 📂 Project Structure

```
/
├── APIClothesEcommerceShop/                # Backend API
│   ├── Controllers/                        # API endpoints
│   ├── DTO/                                # Data Transfer Objects
│   ├── Data/                               # DbContext, Migrations
│   ├── Models/                             # Database models
│   ├── Repositories/                       # Data access layer
│   └── Services/                           # Business logic
└── ECOMMERCESHOPUXUI/                        # Frontend Vue.js App
    └── EcommerceProject/
        ├── src/
        │   ├── assets/                     # Images, assets
        │   ├── components/                 # Reusable components
        │   ├── views/                      # Page views
        │   ├── router/                     # Routing configurations
        │   ├── stores/                     # State management
        │   └── services/                   # API service calls
        └── package.json
```

---


## Authors

- **Main Contributor**: Silent Stack Team

- **Contributors**:
<p align="center">
    <a href="https://github.com/dat0968/EcommerceShop/graphs/contributors">
      <img src="https://contrib.rocks/image?repo=dat0968/EcommerceShop" style="max-width: 400px;" />
    </a>
</p>


---

## 📜 License

This project is licensed under the MIT License. See [LICENSE.md](LICENSE.md) for details.