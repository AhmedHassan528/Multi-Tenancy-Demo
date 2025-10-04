# 🏬 Multi-Tenancy E-Commerce API

## 📖 Overview
This project is a **multi-tenant e-commerce platform** built using **ASP.NET Core Web API** and **SQL Server**.  
Each tenant (e.g., LinkedIn, Twitter, Microsoft) has its own **isolated database** for scalability, security, and data privacy.  
The system supports user authentication, product management, categories, brands, wishlists, and addresses — all separated per tenant.

---

## 🌐 Postman Workspace
You can explore and test all APIs online using the public Postman workspace:  
👉 [**Open API on Postman**](https://www.postman.com/avionics-geologist-69968294/ecom/request/ptutkc0/create-product?tab=auth)

---

## ⚙️ Technology Stack
- **Backend:** ASP.NET Core Web API (.NET 8)
- **Database:** SQL Server (Separate DB per Tenant)
- **Authentication:** JWT + Role-based Authorization
- **Storage:** Local file system for images
- **Testing:** Postman Collection (`Multi Tenancy-linkedIn.postman_collection.json`)

---

## 🏗️ Architecture
The project uses a **hybrid multi-tenancy design**:

| Component | Description |
|------------|-------------|
| **Central Database** | Stores tenant metadata, connection strings, and subscription details. |
| **Tenant Databases** | Each tenant (LinkedIn, Twitter, etc.) has its own schema and data. |
| **Dynamic Connection Resolver** | Automatically switches the database connection based on the `tenant` header. |
| **Shared Backend** | One backend serves all tenants with isolated data access. |

---

## 🚀 Getting Started

### 1️⃣ Clone Repository
```bash
git clone https://github.com/AhmedHassan528/Multi-Tenancy-Demo.git
cd multi-tenancy-api
```

### 2️⃣ Configure `appsettings.json`
Edit the root `appsettings.json` file to set up your connection strings and tenant info:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MainTenantsDb;Trusted_Connection=True;"
  },
  "JwtSettings": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "http://localhost:5000",
    "Audience": "http://localhost:5000"
  }
}
```

---

## 🧱 Tenant Settings Configuration (`appsettings.json`)

The **TenantSettings** section defines all tenants, their connection strings, business domains, and chatbot configurations.  
Make sure to edit this section carefully to match your environment.

```json
"TenantSettings": {
    "Defaults": {
        "DBProvider": "mssql",
        "ConnectionString": "Server=AhmedHassan;Database=TenantSharedDB;Encrypt=False;Trusted_Connection=True;MultipleActiveResultSets=true"
    },
    "Tenants": [
        {
            "Name": "ToyTrove",
            "BusnissDomainTopic": "ToyTrove",
            "ChatRequestSystemMessage": "You are ToyTrove AI Assistant, a knowledgeable and friendly chatbot for ToyTrove, a store specializing exclusively in toys and related services...",
            "TId": "ToyTrove",
            "ConnectionString": "Server=AhmedHassan;Database=TenantToyTroveDB;Encrypt=False;Trusted_Connection=True;MultipleActiveResultSets=true",
            "paymentGateway": "stripe",
            "StripeSecretKey": "sk_test_5",
            "StripePublishableKey": "pk_test_5"
        },
        {
            "Name": "twitter",
            "BusnissDomainTopic": "Hardware Pc Store",
            "ChatRequestSystemMessage": "You are HardwareHub AI Assistant, a knowledgeable and friendly chatbot for HardwareHub...",
            "TId": "twitter",
            "StripeSecretKey": "sk_test_5",
            "StripePublishableKey": "pk_test_5"
        },
        {
            "Name": "linkedIn",
            "BusnissDomainTopic": "supermarket and market products",
            "ChatRequestSystemMessage": "You are FreshCart AI Assistant, a knowledgeable and friendly chatbot for FreshCart...",
            "TId": "linkedIn",
            "paymentGateway": "stripe",
            "StripeSecretKey": "sk_test_5",
            "StripePublishableKey": "pk_test_5"
        }
    ]
},
```

### 🧩 Notes:
- Each tenant has its own **database connection string**.
- The chatbot system is configured individually per tenant.
- Always ensure `"DBProvider"` matches your backend setup (e.g., `"mssql"`).
- To add a new tenant, just append a new object inside the `"Tenants"` array and restart the application.

---

## 🧠 Running the Project

```bash
dotnet restore
dotnet build
dotnet run
```

The API will start at:
```
http://localhost:5000
https://localhost:5001
```

You can test all endpoints using the included **Postman collection**:
> `Multi Tenancy-linkedIn.postman_collection.json`

---

## 🔐 Tenant Handling

### ⚠️ REQUIRED HEADER
Every API request **must include** a tenant header — this determines which database will be used.

```http
tenant: linkedIn
```

Without this header, **the API will reject the request**.

---

## 🧾 API Documentation

### 🔸 Authentication
| Action | Method | Endpoint |
|---------|--------|-----------|
| Register | `POST` | `/api/auth/Register` |
| Login | `POST` | `/api/auth/Login` |
| Forgot Password | `POST` | `/api/auth/ForgotPassword` |
| Forgot Password Confirmation | `POST` | `/api/auth/ForgotPasswordConfermation` |
| Confirm Email | `POST` | `/api/Auth/ConfirmEmail?UserId={id}&Token={token}` |
| Get All Users | `GET` | `/api/Auth/GetAllUsersAsync` |
| Add Role to User | `POST` | `/api/auth/AddRoleToUser` |

---

### 🔸 Products
| Action | Method | Endpoint |
|---------|--------|-----------|
| Create Product | `POST` | `/api/Products` |
| Update Product | `PUT` | `/api/Products/{id}` |
| Delete Product | `DELETE` | `/api/Products/{id}` |
| Get Product | `GET` | `/api/Products/{id}` |
| Get All Products | `GET` | `/api/Products` |
| Admin Get All Products | `GET` | `/api/Products/AdminGetAllAsync` |

---

### 🔸 Categories
| Action | Method | Endpoint |
|---------|--------|-----------|
| Create Category | `POST` | `/api/Category` |
| Update Category | `PUT` | `/api/Category/{id}` |
| Delete Category | `DELETE` | `/api/Category/{id}` |
| Get All Categories | `GET` | `/api/Category` |
| Get Category by ID | `GET` | `/api/Category/{id}` |

---

### 🔸 Brands
| Action | Method | Endpoint |
|---------|--------|-----------|
| Create Brand | `POST` | `/api/Brand` |
| Update Brand | `PUT` | `/api/Brand/{id}` |
| Delete Brand | `DELETE` | `/api/Brand/{id}` |
| Get All Brands | `GET` | `/api/Brand` |
| Get Brand by ID | `GET` | `/api/Brand/{id}` |

---

### 🔸 WishList
| Action | Method | Endpoint |
|---------|--------|-----------|
| Add to Wishlist | `POST` | `/api/WishList/add/{productId}` |
| Get Wishlist | `GET` | `/api/WishList` |
| Get Wishlist Products | `GET` | `/api/WishList/Product` |
| Delete Wishlist Item | `DELETE` | `/api/WishList/{id}` |
| Clear Wishlist | `DELETE` | `/api/WishList` |

---

### 🔸 User Addresses
| Action | Method | Endpoint |
|---------|--------|-----------|
| Add Address | `POST` | `/api/Address` |
| Get Addresses | `GET` | `/api/Address` |
| Get Address by ID | `GET` | `/api/Address/GetAddressByID/{id}` |
| Delete Address | `DELETE` | `/api/Address/{id}` |

---

## 🧰 Postman Environment Variable
Before running any request, set the base URL variable in Postman:

```
MlultiTenancy = https://localhost:5001
```

You can import the collection file:
> `Multi Tenancy-linkedIn.postman_collection.json`

Then update the `tenant` header value (e.g. `linkedIn`, `twitter`, `ToyTrove`) as needed.

---

## 🧱 Adding a New Tenant
1. Open `appsettings.json`
2. Add a new entry inside `"Tenants"` array under `"TenantSettings"`.
3. Create the database for that tenant.
4. Restart the API and test using the new tenant header.

---

## 🧑‍💻 Author
**Ahmed Hassan**  
Microsoft Student Ambassador 🌍  
**Specialization:** Software Engineering / Full-Stack Developer  
📧 [ahmed.hassan52852@gmail.com]  
🔗 [LinkedIn Profile](https://www.linkedin.com/in/ahmed-hassan-564b95229/)

---

## 📜 License
This project is open source and available under the [MIT License](LICENSE).
