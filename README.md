# 🚗 LegalPark C# API  

Backend API untuk manajemen parkir, pembayaran, dan kendaraan.  

---

## 📑 Daftar Isi
- [Tentang Proyek](#-tentang-proyek)
- [Fitur Utama](#-fitur-utama)
- [Persyaratan Sistem](#-persyaratan-sistem)
- [Langkah-langkah Instalasi](#-langkah-langkah-instalasi)
- [Penggunaan API](#-penggunaan-api)
- [Langkah-langkah Penggunaan API](#-langkah--langkah-penggunaan-api)
- [Penjelasan Singkat](#-penjelasan-singkat)
- [Pesan dari Pemilik](#-pesan-dari-pemilik)

---

## 📖 Tentang Proyek  
Proyek ini adalah backend API yang dibangun dengan **ASP.NET Core**, dirancang untuk mendukung sistem manajemen parkir digital.  
API ini mengelola data parkir, kendaraan, transaksi, dan verifikasi pembayaran.  

---

## 🌟 Fitur Utama  
- 🅿️ **Manajemen Parkir**: Mengelola lokasi dan status tempat parkir.  
- 🚘 **Manajemen Kendaraan**: Pendaftaran dan pembaruan data kendaraan.  
- 💳 **Manajemen Transaksi Parkir**: Pencatatan masuk/keluar kendaraan dan riwayat transaksi.  
- ✅ **Verifikasi Pembayaran**: Fitur untuk memvalidasi pembayaran parkir.  

---

## 💻 Persyaratan Sistem  
Untuk menjalankan proyek ini, pastikan Anda telah menginstal:
- .NET SDK (versi 8 atau yang lebih baru)
- Database: Microsoft SQL Server  

---

## ⚙️ Langkah-langkah Instalasi  

### 1️⃣ Clone Repositori  
```bash
git clone https://github.com/trieaji/legalpark-csharp-api.git
cd legalpark-csharp-api
```

2️⃣ Konfigurasi Database

- Buat database Microsoft SQL Server baru.

- Buka file appsettings.json atau appsettings.Development.json.

- Sesuaikan string koneksi database Anda:
```bash
"ConnectionStrings": {
  "DefaultConnection": "Server=DESKTOP-UUMPA8T\\SQLEXPRESS;Database=yourdb;User ID=yourid;Password=yourpassword;TrustServerCertificate=True;"
}
```

3️⃣ Jalankan Proyek
```bash
# Untuk memulihkan dependensi
dotnet restore

# Untuk menjalankan aplikasi
dotnet run
```

---

🔌 Penggunaan API

Gunakan Postman atau Swagger UI untuk mencoba endpoint berikut:

🔑 Auth

- Register: POST /api/v1/auth/register

- Verifikasi Akun: POST /api/v1/auth/verification-account

- Login: POST /api/v1/auth/login

- Verifikasi Pembayaran: POST /api/v1/payment/verification/generate

🚗 [User] Kendaraan

- Daftar Kendaraan: POST /user/vehicle/register

- Lihat Detail Kendaraan: GET /user/vehicle/{id}

- Lihat Semua Kendaraan: GET /user/vehicles

🛠️ [Admin] Kendaraan

- Lihat Semua Kendaraan: GET /admin/vehicles

- Lihat Berdasarkan ID: GET /admin/vehicle/{id}

🅿️ [User] Parking Transaction

- Parkir Masuk: POST /api/v1/user/parking-transactions/entry

- Parkir Keluar: POST /api/v1/user/parking-transactions/exit

---

📝 Langkah - langkah Penggunaan API

🔑 Register: POST /api/v1/auth/register

✉️ Verifikasi Akun: POST /api/v1/auth/verification-account

🔑 Login: POST /api/v1/auth/login

🚘 Daftarkan Kendaraan: POST /user/vehicle/register

🅿️ Parkir Masuk: POST /api/v1/user/parking-transactions/entry

💳 Generate Kode Verifikasi: POST /api/v1/payment/verification/generate

🅿️ Parkir Keluar: POST /api/v1/user/parking-transactions/exit

---

📜 Penjelasan Singkat

Setelah melakukan register, user akan mendapatkan token untuk verifikasi akun yang dikirim melalui email.

Saat akun sudah diverifikasi, user akan mendapatkan saldo default senilai 100K.

User dapat mendaftarkan kendaraan dan melakukan parkir masuk.

Saat hendak parkir keluar, user perlu melakukan verifikasi pembayaran terlebih dahulu.

Kode verifikasi akan dikirimkan via email, dan setelah memasukkannya, user akan menerima notifikasi pembayaran berhasil.

---

🙋 Pesan dari Pemilik

LegalPark adalah sebuah mini project yang berguna sebagai backend API untuk manajemen parkir, pembayaran, kendaraan, dan permasalahan parkir liar.

Mungkin jika kamu membaca codingan yang ada di repo saya mohon maaf sekali jika banyak kurangnya, ada comment yang sengaja tidak aku hapus karena aku perlu untuk belajar.

Kode ini berjalan di website (bukan aplikasi) dan hanya sisi backend saja.
Project kecil ini belum bisa dibilang sebagai project deep. Tapi saya sangat terbuka jika ada yang ingin mengembangkan bersama.
