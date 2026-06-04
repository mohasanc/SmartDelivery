<div align="center">

# 🚀 Smart Delivery API

### Production-ready Food Delivery Backend System

*Built with Clean Architecture · ASP.NET Core 8 · EF Core · SQL Server*

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)
![EF Core](https://img.shields.io/badge/EF_Core-8.0-512BD4?style=flat)
![SQL Server](https://img.shields.io/badge/SQL_Server-2022-CC2927?style=flat&logo=microsoftsqlserver)
![JWT](https://img.shields.io/badge/JWT-Auth-000000?style=flat&logo=jsonwebtokens)
![Swagger](https://img.shields.io/badge/Swagger-Docs-85EA2D?style=flat&logo=swagger)

</div>

---

## 📌 Overview

Smart Delivery is a **production-ready REST API** for a food delivery platform
similar to **Talabat / Uber Eats**, built from scratch using
**Clean Architecture** and industry best practices.

The system handles the full delivery lifecycle: from browsing restaurants and placing 
orders, to driver assignment and real-time status tracking.

---

## ✨ Features

| Feature | Details |
|---------|---------|
| 🔐 Authentication | JWT + Refresh Token rotation |
| 👥 Authorization | Role-based (Admin, Customer, Driver, RestaurantOwner) |
| 🏪 Restaurants | Full CRUD with search, filter, pagination |
| 🍔 Menu Management | Categories + Menu Items with availability toggle |
| 📦 Orders | Full lifecycle with State Machine |
| 🚗 Driver Assignment | Assign drivers to ready orders |
| ⭐ Rating System | Customers can rate delivered orders |
| 🔍 Search & Filter | Across restaurants, menus, and orders |
| 📄 Pagination | All list endpoints are paginated |
| 🛡️ Exception Handling | Global middleware with structured errors |
| 📝 Logging | Structured logging with Serilog |
| 💾 Caching | In-memory caching for frequently accessed data |
| 📖 Documentation | Swagger / OpenAPI 3 |

---

## 🏗️ Architecture

This project follows **Clean Architecture** with strict dependency rules:

```
┌─────────────────────────────────────────────┐
│              SmartDelivery.API              │  ← Presentation
│     Controllers · Middleware · Swagger      │
├─────────────────────────────────────────────┤
│         SmartDelivery.Infrastructure        │  ← Data & Services
│   EF Core · Repositories · JWT · Cache      │
├─────────────────────────────────────────────┤
│          SmartDelivery.Application          │  ← Business Logic
│    Services · DTOs · Validators · AutoMapper│
├─────────────────────────────────────────────┤
│            SmartDelivery.Domain             │  ← Core Entities
│      Entities · Enums · No Dependencies     │
└─────────────────────────────────────────────┘
       Dependency Direction: Inward Only ↑
```

### Patterns Used
- ✅ Repository Pattern
- ✅ Unit of Work Pattern
- ✅ Result Pattern (no exception abuse)
- ✅ SOLID Principles
- ✅ Dependency Injection
- ✅ Soft Delete

---

## 📂 Project Structure

```
SmartDelivery/
├── SmartDelivery.Domain/
│   ├── Entities/          → User, Restaurant, Order, MenuItem...
│   ├── Enums/             → OrderStatus, UserRole
│   └── Common/            → BaseEntity
│
├── SmartDelivery.Application/
│   ├── DTOs/              → Request/Response objects
│   ├── Interfaces/        → Repository & Service contracts
│   ├── Services/          → Business logic implementations
│   ├── Validators/        → FluentValidation rules
│   └── Mappings/          → AutoMapper profiles
│
├── SmartDelivery.Infrastructure/
│   ├── Data/              → AppDbContext + EF Configurations
│   ├── Repositories/      → Concrete implementations
│   ├── UnitOfWork/        → Transaction management
│   └── Services/          → JWT, Cache
│
└── SmartDelivery.API/
    ├── Controllers/       → 6 API controllers
    ├── Middleware/        → Exception + Logging
    └── Extensions/        → Swagger, DI setup
```

---

## 🛠️ Tech Stack

| Category | Technology |
|----------|-----------|
| Framework | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQL Server |
| Authentication | JWT Bearer + Refresh Tokens |
| Validation | FluentValidation 11 |
| Mapping | AutoMapper 13 |
| Logging | Serilog |
| Caching | IMemoryCache |
| Documentation | Swagger / OpenAPI 3 |
| Password Hashing | ASP.NET Identity PasswordHasher |

---

## 🗄️ Database Schema

```
Users ──────────── UserRoles ──── Roles
  │
  ├──── Restaurants ──── Categories ──── MenuItems
  │         │
  └──── Orders ──────────────────────── OrderItems
            │
          Drivers (Users with Driver role)
```

---

## 🔄 Order State Machine

```
Pending → Confirmed → Preparing → ReadyForPickup → OnTheWay → Delivered
   │            │
   └────────────┴──────────────────────────────────────────► Cancelled
```

---

## ⚡ Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/YOUR_USERNAME/SmartDelivery.git
cd SmartDelivery

# 2. Setup configuration
cp SmartDelivery.API/appsettings.Example.json SmartDelivery.API/appsettings.Development.json
```

Edit `appsettings.Development.json` with your values:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SmartDeliveryDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyMustBe32CharsMinimum!"
  }
}
```

```bash
# 3. Apply database migrations
dotnet ef database update \
  --project SmartDelivery.Infrastructure \
  --startup-project SmartDelivery.API

# 4. Run the project
dotnet run --project SmartDelivery.API
```

Open Swagger: **http://localhost:5000/swagger**

---

## 🌱 Seed Data

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@smartdelivery.com | Admin@123456 |
| Customer | customer@smartdelivery.com | Customer@123456 |
| Driver | driver@smartdelivery.com | Driver@123456 |
| RestaurantOwner | owner@smartdelivery.com | Owner@123456 |

---

## 📡 API Endpoints

<details>
<summary>🔐 Auth</summary>

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/v1/auth/register` | Register new user | Public |
| POST | `/api/v1/auth/login` | Login & get tokens | Public |
| POST | `/api/v1/auth/refresh-token` | Refresh access token | Public |
| POST | `/api/v1/auth/logout` | Logout | Any |
| PUT | `/api/v1/auth/change-password` | Change password | Any |

</details>

<details>
<summary>🏪 Restaurants</summary>

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/v1/restaurants` | List with filters | Public |
| GET | `/api/v1/restaurants/{id}` | Get details | Public |
| POST | `/api/v1/restaurants` | Create | Owner/Admin |
| PUT | `/api/v1/restaurants/{id}` | Update | Owner/Admin |
| DELETE | `/api/v1/restaurants/{id}` | Soft delete | Owner/Admin |
| PATCH | `/api/v1/restaurants/{id}/toggle-status` | Open/Close | Owner/Admin |

</details>

<details>
<summary>📦 Orders</summary>

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/v1/orders` | Place order | Customer |
| GET | `/api/v1/orders/my-orders` | My orders | Customer |
| GET | `/api/v1/orders/driver-orders` | Assigned orders | Driver |
| GET | `/api/v1/orders/{id}` | Order details | Owner |
| PATCH | `/api/v1/orders/{id}/status` | Update status | Multi-role |
| PATCH | `/api/v1/orders/{id}/assign-driver` | Assign driver | Admin |
| POST | `/api/v1/orders/{id}/rate` | Rate order | Customer |

</details>

---

## 🔮 Future Improvements

- [ ] 🐳 Docker + Docker Compose
- [ ] 📡 SignalR — Real-time order tracking
- [ ] 💳 Paymob Payment Gateway integration
- [ ] 📧 Email notifications (order confirmation)
- [ ] 📱 Push notifications
- [ ] 🧪 Unit & Integration Tests
- [ ] ☁️ Deploy to Azure / Railway
- [ ] 🔒 Rate Limiting
- [ ] 🗃️ Redis Cache (replace MemoryCache)
- [ ] 📊 Admin Dashboard

---

## 🤝 Contributing

Contributions are welcome!
Please fork the repo and create a Pull Request.

---

## 📃 License

This project is licensed under the **MIT License**.

---

<div align="center">

Made with ❤️ by **[Mohamed Hassan](https://github.com/mohasanc)**

⭐ *If you found this project useful, please give it a star!*

</div>
