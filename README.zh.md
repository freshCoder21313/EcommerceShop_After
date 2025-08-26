[Tiếng Việt](./README.vi.md) | [English](./README.md) | [中文](./README.zh.md)

# EcommerceShop - 现代在线服装店铺

一个全面的电子商务平台，专为在线销售服装而设计，采用现代化、解耦的架构。该项目具有可扩展的后端API和动态前端，带来极佳的购物体验。

![Build Status](https://img.shields.io/badge/build-passing-brightgreen)
![License](https://img.shields.io/badge/license-MIT-blue)

## 目录

- [功能特点](#🚀-功能特点)
- [技术栈](#🚀-技术栈)
- [安装指南](#🛠️-安装指南)
- [使用说明](#🎯-使用说明)
- [配置方式](#⚙️-配置方式)
- [架构设计](#🏗️-架构设计)
- [项目结构](#📂-项目结构)
- [作者信息](#👥-作者信息)
- [许可证](#📜-许可证)

---

## 🚀 功能特点

- 🛒 **商品目录：** 按类别浏览商品，查看详细描述和图片
- 🔐 **客户验证：** 通过JWT和Google验证实现安全登录和注册
- 🛍️ **购物车和订单管理：** 添加/修改/删除商品，提交订单，查看订单历史和状态
- 💳 **安全支付集成：** 通过VNPAY进行支付
- 📊 **后台管理面板：** 管理商品、类别、客户和订单
- 💡 **推荐系统：** 利用ML.NET提供个性化商品推荐
- 📧 **邮件服务：** 自动发送订单确认、通知和优惠码
- 🌟 **高级客户功能：** 
  - 商品对比（结合AI API实现虚拟试穿）
  - 转盘抽奖优惠券
  - 聊天机器人支持
  - 查看近期浏览历史

---

## 🚀 技术栈

### 后端
- **框架:** ASP.NET Core 8.0
- **数据库:** SQL Server
- **认证:** JWT Bearer, Google Authentication
- **云服务:** Cloudinary, Firebase (Admin, Database), Google Cloud Storage
- **电子邮件:** MailKit
- **缓存:** Redis
- **机器学习:** Microsoft.ML
- **API文档:** Swashbuckle (Swagger)
- **支付:** VNPAY

### 前端
- **框架:** Vue.js 3
- **构建工具:** Vite
- **状态管理:** Pinia
- **路由:** Vue Router
- **UI框架/库:** Bootstrap 5, jQuery, SweetAlert2, Swiper
- **HTTP客户端:** Axios
- **图表:** Chart.js

---

## 🛠️ 安装指南

**系统需求：**
- Windows 10/11 或 Linux/macOS
- .NET 8 SDK
- Node.js（建议使用LTS版本）
- SQL Server或SQL Server Express/LocalDB

**操作步骤：**

```bash
# 1. 克隆仓库
git clone <仓库地址> # 替换为你的仓库链接
cd EcommerceShop

# 2. 后端设置 - API
cd APIClothesEcommerceShop/APIClothesEcommerceShop
# 在 `appsettings.json` 中配置数据库连接
# 示例:
# "EcommerceShopConnect_Dot": "Server=.;Database=EcommerceShopDb;Trusted_Connection=True;TrustServerCertificate=True;"
dotnet restore
dotnet ef database update
dotnet run
# API将通过 https://localhost:7217/swagger 或 http://localhost:7218/swagger 访问

# 3. 前端设置 - 用户界面
cd ../../ECOMMERCESHOPUXUI/EcommerceProject
npm install
npm run dev
# 用户界面将通过 http://localhost:5173 访问
```

---

## 🎯 使用说明

当后端和前端服务都在运行时:

- 打开浏览器并转到 [http://localhost:5173](http://localhost:5173)
- 使用直观的用户界面浏览产品、注册/登录、将商品添加到购物车并使用VNPAY结账。

---

## ⚙️ 配置方式

项目需要配置环境变量才能正常工作。有关如何配置环境变量和获取API密钥的详细信息，请参阅[详细配置指南](./docs/CONFIGURATION.zh.md)。

---

## 🏗️ 系统架构

系统采用客户端-服务器解耦架构：

- **后端API：** 使用.NET 8 Web API开发，处理业务逻辑、数据存储、第三方服务集成及提供RESTful接口
- **前端应用：** 基于Vue.js 3，使用Vite作为构建工具，Pinia管理状态，Vue Router进行路由配置，Bootstrap 5提供UI样式，确保用户操作流畅快速。

---

## 📂 项目结构

```
/
├── APIClothesEcommerceShop/                # 后端API
│   ├── Controllers/                        # API控制器
│   ├── DTO/                                # 数据传输对象
│   ├── Data/                               # 数据库上下文、迁移
│   ├── Models/                             # 数据模型
│   ├── Repositories/                       #数据访问层
│   └── Services/                           # 业务逻辑
└── ECOMMERCESHOPUXUI/                        # 前端Vue.js应用
    └── EcommerceProject/
        ├── src/
        │   ├── assets/                     # 图片资源
        │   ├── components/                 # 可复用组件
        │   ├── views/                      # 页面视图
        │   ├── router/                     # 路由配置
        │   ├── stores/                     # 状态管理
        │   └── services/                   # API调用
        └── package.json
```

---

## 作者

- **主要贡献者**：Silent Stack团队

- **贡献者**：
<p align="center">
    <a href="https://github.com/dat0968/EcommerceShop/graphs/contributors">
      <img src="https://contrib.rocks/image?repo=dat0968/EcommerceShop" style="max-width: 400px;" />
    </a>
</p>


---

## 📜 许可证

本项目基于MIT许可证授权，详见 [LICENSE.md](LICENSE.md)。
