# 🚗 LegalPark C# API  

Backend API for parking management, payment, and vehicle handling.  

---

## 📑 Table of Contents
- [About the Project](#-tentang-proyek)
- [Key Features](#-fitur-utama)
- [System Requirements](#-persyaratan-sistem)
- [Installation Steps](#-langkah-langkah-instalasi)
- [API Usage](#-penggunaan-api)
- [API Usage Workflow](#-langkah--langkah-penggunaan-api)
- [Brief Explanation](#-penjelasan-singkat)
- [A Message from the Owner](#-pesan-dari-pemilik)

---

## 📖 About the Project  
This project is a backend API built with ASP.NET Core, designed to support a digital parking management system. This API handles parking data, vehicles, transactions, and payment verification.  

---

## 🌟 Key Features 
- 🅿️ **Parking Management**: Manages parking locations and spot statuses.  
- 🚘 **Vehicle Management**: Registration and update of vehicle data.  
- 💳 **Parking Transaction Management**: Records vehicle entry/exit and transaction history.  
- ✅ **Payment Verification**: Feature for validating parking payments.  

---

## 💻 System Requirements  
To run this project, ensure you have the following installed:
- .NET SDK (version 8 or later)
- Database: Microsoft SQL Server  

---

## ⚙️ Installation Steps  

### 1️⃣ Clone Repositori  
```bash
git clone https://github.com/trieaji/legalpark-csharp-api.git
cd legalpark-csharp-api
```

2️⃣ Database Configuration

- Create a new Microsoft SQL Server database.

- Open the file appsettings.json or appsettings.Development.json.

- Adjust your database connection string:
```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=DESKTOP-UUMPA8T\\SQLEXPRESS;Database=yourdb;User ID=yourid;Password=yourpassword;TrustServerCertificate=True;"
}
```

3️⃣ Run the Project
```bash
# To restore dependencies
dotnet restore

# To run the application
dotnet run
```

---

🔌 API Usage

Use Postman or Swagger UI to test the following endpoints:

🔑 Auth

- Register: POST /api/v1/auth/register

- Verify user account: POST /api/v1/auth/verification-account

- Login: POST /api/v1/auth/login

- Generate Payment Verification Code: POST /api/v1/payment/verification/generate

🚗 [User] Vehicle Management

- Register a new vehicle: POST /user/vehicle/register

- View vehicle details by ID: GET /user/vehicle/{id}

- View all registered vehicles: GET /user/vehicles

🛠️ [Admin] Vehicle Management

- View all vehicles: GET /admin/vehicles

- View vehicle by ID: GET /admin/vehicle/{id}

🅿️ [User] Parking Transaction

- Vehicle parking entry: POST /api/v1/user/parking-transactions/entry

- Vehicle parking exit: POST /api/v1/user/parking-transactions/exit

---

📝 API Usage Workflow

🔑 Register: POST /api/v1/auth/register

✉️ Account Verification: POST /api/v1/auth/verification-account

🔑 Login: POST /api/v1/auth/login

🚘 Register Vehicle: POST /user/vehicle/register

🅿️ Parking Entry: POST /api/v1/user/parking-transactions/entry

💳 Generate Verification Code: POST /api/v1/payment/verification/generate

🅿️ Parking Exit: POST /api/v1/user/parking-transactions/exit

---

📜 Brief Explanation

After registering, the user will receive a token for account verification sent via email. Once the account is verified, the user will receive a default balance of 100K. Users can then register their vehicle and perform a parking entry. When preparing for parking exit, the user needs to generate a payment verification code first. This verification code will be sent via email, and after entering it, the user will receive a payment success notification.

---

🙋 A Message from the Owner

LegalPark is a mini-project that serves as a backend API for parking management, payment processing, vehicle data, and handling illegal parking issues.

If you are reading the code in my repository, I sincerely apologize if there are many shortcomings. Some comments are intentionally left undeleted because I need them for learning purposes.

This code runs on a website (not a native application) and is backend-only. This small project cannot yet be considered a deep project, but I am very open to anyone who wishes to develop it further together.
