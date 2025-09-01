using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using APIClothesEcommerceShop.Data;
using APIClothesEcommerceShop.Models;
using APIClothesEcommerceShop.Services.CloudinaryService; // Added for Cloudinary
using Microsoft.AspNetCore.Hosting; // Added for IWebHostEnvironment
using Microsoft.AspNetCore.Http; // Added for FormFile
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;

namespace APIClothesEcommerceShop.Repositories.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly EcommerceShopContext _db;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;

        public DbInitializer(EcommerceShopContext db, ICloudinaryService cloudinaryService, IWebHostEnvironment webHostEnvironment, IConfiguration configuration)
        {
            _db = db;
            _cloudinaryService = cloudinaryService;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public void InitializeDb()
        {
            // _db.Database.EnsureCreated();
            // InitTestAccount();
#if DEBUG
            // TestCloudinaryMigrationAsync().Wait(); // Call the new test method
#endif
            MigrateImagesToCloudinary().Wait(); // Call the new migration method

            if (_configuration.GetValue<bool>("MongoDbSettings:EnableMigration"))
            {
                MigrateToMongoDbAsync().Wait();
            }
            // InitTestOrder(3);
            // CreateOrderForTest("customer.demo@email.com", 14000000);
            // UpdateStreakForTest("customer.demo@email.com", 7);
        }

        private async Task MigrateToMongoDbAsync()
        {
            Console.WriteLine("Starting data migration to MongoDB with NoSQL-first approach...");

            var connectionString = _configuration["MongoDbSettings:ConnectionString"];
            var databaseName = _configuration["MongoDbSettings:DatabaseName"];

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
            {
                Console.WriteLine("MongoDB settings not found in appsettings.json. Aborting migration.");
                return;
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            // --- Products ---
            Console.WriteLine("Migrating Products...");
            var productsToMigrate = (await _db.Sanphams
                .Include(s => s.Chitietsanphams)
                    .ThenInclude(ctsp => ctsp.Hinhanhs)
                .Include(s => s.DanhGias)
                .Include(s => s.Chitietdanhmucs)
                    .ThenInclude(ctdm => ctdm.MaDanhMucConNavigation)
                .ToListAsync())
                .Select(s => new
                {
                    s.MaSp,
                    TenSp = s.TenSanPham,
                    s.MoTa,
                    s.IsActive,
                    Details = s.Chitietsanphams.Select(ctsp => new
                    {
                        ctsp.MaCtsp,
                        Size = ctsp.KichThuoc,
                        Mau = ctsp.MauSac,
                        SoLuong = ctsp.SoLuongTon,
                        ctsp.DonGia,
                        Images = ctsp.Hinhanhs.Select(h => h.TenHinhAnh).ToList()
                    }).ToList(),
                    Reviews = s.DanhGias.Select(dg => new
                    {
                        Id = dg.Id,
                        dg.MaKh,
                        dg.NoiDung,
                        dg.SoSao,
                        dg.NgayDanhGia,
                        dg.TenCacHinhAnh
                    }).ToList(),
                    Categories = s.Chitietdanhmucs.Select(ctdm => new
                    {
                        Id = ctdm.MaDanhMucCon,
                        Name = ctdm.MaDanhMucConNavigation?.TenDanhMucCon
                    }).ToList()
                }).ToList();
            await MigrateCollectionAsync(database, "Products", productsToMigrate);

            // --- Categories ---
            Console.WriteLine("Migrating Categories...");
            var categoriesToMigrate = (await _db.Danhmucchas
                .Include(dmc => dmc.Chitietdanhmucs)
                    .ThenInclude(ctdm => ctdm.MaDanhMucConNavigation)
                .ToListAsync())
                .Select(dmc => new
                {
                    MaDanhMucCha = dmc.MaDanhMucCha,
                    TenDanhMucCha = dmc.TenDanhMucCha,
                    SubCategories = dmc.Chitietdanhmucs.Select(ctdm => new
                    {
                        Id = ctdm.MaDanhMucCon,
                        Name = ctdm.MaDanhMucConNavigation.TenDanhMucCon
                    }).Distinct().ToList()
                }).ToList();
            await MigrateCollectionAsync(database, "Categories", categoriesToMigrate);

            // --- Customers ---
            Console.WriteLine("Migrating Customers...");
            var customersToMigrate = (await _db.Khachhangs
                .Include(kh => kh.Diachis)
                .Include(kh => kh.Sanphamyeuthichs)
                .Include(kh => kh.LichSuXems)
                .Include(kh => kh.Giohangs)
                    .ThenInclude(gh => gh.MaCtspNavigation)
                        .ThenInclude(ctsp => ctsp.MaSpNavigation)
                .ToListAsync())
                .Select(kh => new
                {
                    kh.MaKh,
                    kh.HoTen,
                    kh.TenTaiKhoan,
                    kh.Email,
                    kh.NgayTao,
                    kh.IsActive,
                    kh.TinhTrang,
                    kh.Sdt,
                    kh.Cccd,
                    kh.NgaySinh,
                    kh.GioiTinh,
                    kh.HinhDaiDien,
                    Addresses = kh.Diachis.Select(dc => new
                    {
                        DiaChiChiTiet = dc.diachichitiet,
                        dc.XaPhuong,
                        dc.QuanHuyen,
                        dc.Tinh,
                        MacDinh = dc.MacDinh
                    }).ToList(),
                    FavoriteProducts = kh.Sanphamyeuthichs.Select(spyt => spyt.MaSp).ToList(),
                    ViewHistory = kh.LichSuXems.Select(lsx => lsx.MaSp).ToList(),
                    ShoppingCart = kh.Giohangs.Select(gh => new
                    {
                        gh.MaCtsp,
                        gh.SoLuong,
                        Product = gh.MaCtspNavigation != null ? new
                        {
                            TenSp = gh.MaCtspNavigation.MaSpNavigation?.TenSanPham,
                            Size = gh.MaCtspNavigation.KichThuoc,
                            Mau = gh.MaCtspNavigation.MauSac,
                            gh.MaCtspNavigation.DonGia
                        } : null
                    }).ToList()
                }).ToList();
            await MigrateCollectionAsync(database, "Customers", customersToMigrate);

            // --- Orders ---
            Console.WriteLine("Migrating Orders...");
            var ordersToMigrate = (await _db.Hoadons
                .Include(hd => hd.Cthoadons)
                    .ThenInclude(cthd => cthd.MaCtspNavigation)
                        .ThenInclude(ctsp => ctsp.MaSpNavigation)
                .Include(hd => hd.Chitietcombohoadons)
                    .ThenInclude(ctcbd => ctcbd.MaComboNavigation)
                .ToListAsync())
                .Select(hd => new
                {
                    hd.MaHd,
                    hd.MaKh,
                    hd.NgayTao,
                    hd.DiaChiNhanHang,
                    hd.HoTen,
                    hd.Sdt,
                    hd.PhiVanChuyen,
                    hd.TienGoc,
                    hd.HinhThucTt,
                    hd.TinhTrang,
                    hd.IsActive,
                    Details = hd.Cthoadons.Select(cthd => new
                    {
                        cthd.MaCtsp,
                        cthd.SoLuong,
                        cthd.Gia,
                        Product = cthd.MaCtspNavigation != null ? new
                        {
                            TenSp = cthd.MaCtspNavigation.MaSpNavigation?.TenSanPham,
                            Size = cthd.MaCtspNavigation.KichThuoc,
                            Mau = cthd.MaCtspNavigation.MauSac
                        } : null
                    }).ToList(),
                    ComboDetails = hd.Chitietcombohoadons.Select(ctcbd => new
                    {
                        ctcbd.MaCombo,
                        ctcbd.SoLuong,
                        ctcbd.DonGia,
                        Combo = ctcbd.MaComboNavigation != null ? new { ctcbd.MaComboNavigation.TenCombo } : null
                    }).ToList()
                }).ToList();
            await MigrateCollectionAsync(database, "Orders", ordersToMigrate);

            // --- Other collections that can remain separate ---
            Console.WriteLine("Migrating Staffs, Roles, Coupons, Combos...");
            await MigrateCollectionAsync(database, "Staffs", (await _db.Nhanviens.ToListAsync()).Select(s => new { s.MaNv, s.HoTen, s.Email, s.TenTaiKhoan, s.IsActive }).ToList());
            await MigrateCollectionAsync(database, "Roles", (await _db.Chucvus.ToListAsync()).Select(r => new { r.MaChucVu, r.TenChucVu, r.IsActive }).ToList());
            await MigrateCollectionAsync(database, "Coupons", (await _db.Macoupons.ToListAsync()).Select(c => new { c.MaCode, c.MoTa, c.PhanTramGiam, c.SoTienGiam, c.DonHangToiThieu, c.SoLuong, c.NgayBatDau, c.NgayKetThuc, c.TrangThai }).ToList());

            // Combos (with circular reference fix)
            var combosToMigrate = (await _db.Combos
                .Include(c => c.Chitietcombos)
                    .ThenInclude(ct => ct.MaSpNavigation)
                .ToListAsync())
                .Select(c => new
                {
                    c.MaCombo,
                    c.TenCombo,
                    c.Hinh,
                    c.SoLuong,
                    c.MoTa,
                    c.IsActive,
                    Chitietcombos = c.Chitietcombos.Select(ct => new
                    {
                        ct.MaSp,
                        ct.SoLuongSP,
                        TenSp = ct.MaSpNavigation.TenSanPham
                    }).ToList()
                }).ToList();
            await MigrateCollectionAsync(database, "Combos", combosToMigrate);

            Console.WriteLine("Data migration to MongoDB completed with a NoSQL-first schema.");
        }

        private async Task MigrateCollectionAsync<T>(IMongoDatabase database, string collectionName, List<T> data) where T : class
        {
            try
            {
                // Check if collection already has data
                var collectionExists = await database.ListCollectionNames().ToListAsync().ContinueWith(t => t.Result.Contains(collectionName));
                if (collectionExists)
                {
                    var collectionForCount = database.GetCollection<BsonDocument>(collectionName);
                    if (await collectionForCount.CountDocumentsAsync(new BsonDocument()) > 0)
                    {
                        Console.WriteLine($"Collection '{collectionName}' already contains data. Skipping migration for this collection.");
                        return;
                    }
                }


                if (data == null || !data.Any())
                {
                    Console.WriteLine($"No data found for '{collectionName}'. Skipping.");
                    return;
                }

                var collection = database.GetCollection<BsonDocument>(collectionName);
                var bsonDocuments = data.Select(item => item.ToBsonDocument()).ToList();
                await collection.InsertManyAsync(bsonDocuments);
                Console.WriteLine($"Successfully migrated {bsonDocuments.Count} documents to '{collectionName}' collection.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during migration of '{collectionName}': {ex.Message}");
                // Optional: Log the full exception
            }
        }

        private void InitCombo(int numCreate, int? idOrder = null)
        {
            var sanphams = _db.Sanphams.ToList();
            var chitietsanphams = _db.Chitietsanphams.ToList();
            idOrder = idOrder ?? _db.Hoadons.First().MaHd;

            var random = new Random();

            for (int i = 1; i <= numCreate; i++)
            {
                // Tạo combo mới
                var combo = new APIClothesEcommerceShop.Models.Combo
                {
                    TenCombo = $"Combo {i}",
                    Hinh = null,
                    SoLuong = random.Next(5, 20),
                    MoTa = $"Mô tả cho combo {i}",
                    IsActive = true
                };

                // Chọn ngẫu nhiên 2-4 chi tiết sản phẩm cho combo này
                var ctspInCombo = chitietsanphams.OrderBy(x => random.Next()).Take(random.Next(2, 5)).ToList();

                int tongGia = 0;
                foreach (var ctsp in ctspInCombo)
                {
                    int soLuong = random.Next(1, 5);
                    int donGia = ctsp.DonGia;

                    combo.Chitietcombohoadons.Add(new Chitietcombohoadon
                    {
                        MaHd = idOrder.Value,
                        MaCtsp = ctsp.MaCtsp,
                        SoLuong = soLuong,
                        DonGia = donGia
                    });

                    tongGia += donGia * soLuong;
                }

                // Gán giá combo là tổng giá các sản phẩm giảm 10%
                //combo.GiaCombo = (int)(tongGia * 0.9);

                _db.Combos.Add(combo);
            }

            _db.SaveChanges();
        }

        private void InitTestAccount()
        {
            // Kiểm tra nếu đã có tài khoản mẫu thì không tạo lại
            if (!_db.Khachhangs.Any(kh => kh.Email == "customer.demo@email.com"))
            {
                var customer = new Khachhang
                {
                    HoTen = "Khách Hàng Demo",
                    TenTaiKhoan = "customer.demo",
                    Email = "customer.demo@email.com",
                    MatKhau = new HashPassword.PasswordHasher().HashPassword("CustomerDemo@123"),
                    NgayTao = DateTime.Now,
                    IsActive = true,
                    TinhTrang = "Đang hoạt động",
                    Sdt = "0900000001",
                    DiaChi = "123 Đường Demo, Quận 1, TP.HCM",
                    Cccd = "123456789012",
                    NgaySinh = DateOnly.FromDateTime(DateTime.Now.AddYears(-20)),
                    GioiTinh = "Nam",
                    HinhDaiDien = null
                };
                _db.Khachhangs.Add(customer);
            }

            if (!_db.Nhanviens.Any(nv => nv.Email == "staff6real.demo@email.com"))
            {
                // Lấy mã chức vụ đầu tiên hoặc tạo mới nếu chưa có
                var chucVu = _db.Chucvus.Skip(1).FirstOrDefault() ?? new Chucvu { TenChucVu = "Nhân viên" };
                if (chucVu.MaChucVu == 0)
                {
                    _db.Chucvus.Add(chucVu);
                    _db.SaveChanges();
                }

                var staff = new Nhanvien
                {
                    HoTen = "Nhân Viên Demo Real",
                    TenTaiKhoan = "staff6real.demo",
                    Email = "staff6real.demo@email.com",
                    MatKhau = "staff6realDemo@123", // Nhân viên không mã hóa mật khẩu như AccountRepository
                    NgayVaoLam = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true,
                    TinhTrang = "Đang hoạt động",
                    Sdt = "0900060002",
                    DiaChi = "666 Đường Demo, Quận 6, TP.Bình Hòa",
                    Cccd = "423456689012",
                    NgaySinh = DateOnly.FromDateTime(DateTime.Now.AddYears(-26)),
                    GioiTinh = "Nữ",
                    MaChucVu = chucVu.MaChucVu
                };
                _db.Nhanviens.Add(staff);
            }

            if (!_db.Nhanviens.Any(nv => nv.Email == "staff.demo@email.com"))
            {
                // Lấy mã chức vụ đầu tiên hoặc tạo mới nếu chưa có
                var chucVu = _db.Chucvus.FirstOrDefault() ?? new Chucvu { TenChucVu = "Nhân viên" };
                if (chucVu.MaChucVu == 0)
                {
                    _db.Chucvus.Add(chucVu);
                    _db.SaveChanges();
                }

                var staff = new Nhanvien
                {
                    HoTen = "Nhân Viên Demo",
                    TenTaiKhoan = "staff.demo",
                    Email = "staff.demo@email.com",
                    MatKhau = "StaffDemo@123", // Nhân viên không mã hóa mật khẩu như AccountRepository
                    NgayVaoLam = DateOnly.FromDateTime(DateTime.Now),
                    IsActive = true,
                    TinhTrang = "Đang hoạt động",
                    Sdt = "0900000002",
                    DiaChi = "143 Đường Demo, Quận 1, TP.HCM",
                    Cccd = "423456789012",
                    NgaySinh = DateOnly.FromDateTime(DateTime.Now.AddYears(-21)),
                    GioiTinh = "Nam",
                    MaChucVu = chucVu.MaChucVu
                };
                _db.Nhanviens.Add(staff);
            }
            else
            {
                var staff = _db.Nhanviens.FirstOrDefault(nv => nv.Email == "staff.demo@email.com");
                staff.MatKhau = new HashPassword.PasswordHasher().HashPassword("StaffDemo@123");
                // staff.MatKhau = "StaffDemo@123";
                _db.Nhanviens.Update(staff);
            }

            _db.SaveChanges();
        }

        private void InitTestOrder(int numCreate, string username = "customer.demo", int? totalAmount = null)
        {
            var khachHang = _db.Khachhangs.FirstOrDefault(kh => kh.TenTaiKhoan == username || kh.Email == username);
            if (khachHang == null) return;

            var sanphams = _db.Sanphams.ToList();
            var chitietsanphams = _db.Chitietsanphams.ToList();
            var random = new Random();

            for (int i = 1; i <= numCreate; i++)
            {
                // Tạo hóa đơn mới
                var hoadon = new Hoadon
                {
                    MaKh = khachHang.MaKh,
                    NgayTao = DateTime.Now,
                    DiaChiNhanHang = "123 Đường Demo, Quận 1, TP.HCM",
                    HoTen = khachHang.HoTen,
                    Sdt = khachHang.Sdt ?? "0900000001",
                    PhiVanChuyen = 20000,
                    TienGoc = 0, // Sẽ tính sau
                    HinhThucTt = "VnPay",
                    TinhTrang = "Đã nhận",
                    IsActive = true
                };

                int tongTien = 0;

                if (totalAmount.HasValue)
                {
                    tongTien = totalAmount.Value;
                }
                else
                {
                    // Chọn ngẫu nhiên 1-3 sản phẩm cho hóa đơn này
                    var ctspInOrder = chitietsanphams.Take(random.Next(1, 4)).ToList();
                    List<Cthoadon> cthdInOrder = new();
                    foreach (var ctsp in ctspInOrder)
                    {
                        int soLuong = random.Next(1, 5);
                        int donGia = ctsp.DonGia;

                        Cthoadon newCtHd = (new Cthoadon
                        {
                            MaCtsp = ctsp.MaCtsp,
                            SoLuong = soLuong,
                            Gia = donGia
                        });

                        tongTien += donGia * soLuong;

                        cthdInOrder.Add(newCtHd);
                    }

                    var ctcbos = _db.Combos
                        .Take(random.Next(1, 3))
                        .ToList();
                    List<Chitietcombohoadon> ctcboInOrders = new();
                    foreach (var combo in ctcbos)
                    {
                        int soLuong = random.Next(1, 3);
                        int donGia =
                         ((int?)combo?.Chitietcombos.Sum(ctbo => ctbo.SoLuongSP
                         * ctbo.MaSpNavigation.Chitietsanphams.Average(ctsp => ctsp.DonGia)) ?? 0); // ? Maybe not fine

                        Chitietcombohoadon ctcbo = (new Chitietcombohoadon
                        {
                            MaCtsp = chitietsanphams.FirstOrDefault()?.MaCtsp ?? throw new Exception("Cannot find product detail"), // Combo không có mã chi tiết sản phẩm
                            SoLuong = soLuong,
                            DonGia = donGia,
                            MaCombo = combo.MaCombo // Gán ID của combo
                        });

                        tongTien += donGia * soLuong;
                        ctcboInOrders.Add(ctcbo);
                    }
                    _db.AddRange(cthdInOrder);
                    _db.AddRange(ctcboInOrders);
                }

                // Gán tổng tiền là tổng giá các sản phẩm giảm 10%
                hoadon.TienGoc = (tongTien);

                _db.Hoadons.Add(hoadon);
                _db.SaveChanges();

                Console.WriteLine($">>> Đã tạo hóa đơn {hoadon.MaHd} cho khách hàng {khachHang.HoTen} với tổng tiền {hoadon.TienGoc} VNĐ");
            }

        }

        public void CreateOrderForTest(string email, int? totalAmount = null)
        {
            InitTestOrder(1, email, totalAmount);
        }

        public void UpdateStreakForTest(string email, int streak)
        {
            var customer = _db.Khachhangs.FirstOrDefault(kh => kh.Email == email);
            if (customer != null)
            {
                customer.Streak = streak;
                _db.SaveChanges();
            }
        }

        private async Task MigrateImagesToCloudinary()
        {
            Console.WriteLine("Starting image migration to Cloudinary...");

            // Migrate Khachhang images
            var customers = await _db.Khachhangs.Where(c => c.HinhDaiDien != null && !c.HinhDaiDien.StartsWith("http")).ToListAsync();
            foreach (var customer in customers)
            {
                var newUrl = await MigrateImage(customer.HinhDaiDien, "customer-profiles");
                if (newUrl != null)
                {
                    customer.HinhDaiDien = newUrl;
                }
            }

            // Migrate Nhanvien images
            var staffs = await _db.Nhanviens.Where(n => n.HinhDaiDien != null && !n.HinhDaiDien.StartsWith("http")).ToListAsync();
            foreach (var staff in staffs)
            {
                if (string.IsNullOrEmpty(staff.HinhDaiDien)) continue;
                var newUrl = await MigrateImage(staff.HinhDaiDien, "staff-profiles");
                if (newUrl != null)
                {
                    staff.HinhDaiDien = newUrl;
                }
            }

            // Migrate DanhGia images
            var reviews = await _db.DanhGias.Where(r => r.TenCacHinhAnh != null && !r.TenCacHinhAnh.StartsWith("http")).ToListAsync();
            foreach (var review in reviews)
            {
                // Assuming TenCacHinhAnh stores comma-separated local paths
                if (string.IsNullOrEmpty(review.TenCacHinhAnh)) continue;
                var localPaths = review.TenCacHinhAnh.Split(',', StringSplitOptions.RemoveEmptyEntries);
                List<string> cloudinaryUrls = new List<string>();
                foreach (var localPath in localPaths)
                {
                    var imageUrl = await MigrateImage(localPath.Trim(), "review-images");
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        cloudinaryUrls.Add(imageUrl);
                    }
                }
                if (cloudinaryUrls.Any())
                {
                    review.TenCacHinhAnh = string.Join(",", cloudinaryUrls);
                }
            }

            // Migrate Combo images
            var combos = await _db.Combos.Where(c => c.Hinh != null && !c.Hinh.StartsWith("http")).ToListAsync();
            foreach (var combo in combos)
            {
                if (string.IsNullOrEmpty(combo.Hinh)) continue;
                var newUrl = await MigrateImage(combo.Hinh, "combo-images");
                if (newUrl != null)
                {
                    combo.Hinh = newUrl;
                }
            }

            // Migrate Hinhanh images
            var images = await _db.Hinhanhs.Where(h => h.TenHinhAnh != null && !h.TenHinhAnh.StartsWith("http")).ToListAsync();
            foreach (var image in images)
            {
                if (string.IsNullOrEmpty(image.TenHinhAnh)) continue;
                var newUrl = await MigrateImage(image.TenHinhAnh, "product-detail-images");
                if (newUrl != null)
                {
                    image.TenHinhAnh = newUrl;
                }
            }

            await _db.SaveChangesAsync();
            Console.WriteLine("Image migration to Cloudinary completed.");
        }

        private async Task<string?> MigrateImage(string imagePath, string cloudinaryTag)
        {
            if (string.IsNullOrEmpty(imagePath) || imagePath.StartsWith("http"))
            {
                return imagePath; // Already migrated or no image
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Construct full local path
            // Assuming local paths are relative to wwwroot, e.g., /AnhKhachHang/image.jpg

            string fileName = Path.GetFileName(imagePath); // chỉ lấy tên file

            // Tìm tất cả file trùng tên trong wwwroot (kể cả thư mục con)
            string[] matches = Directory.GetFiles(wwwRootPath, fileName, SearchOption.AllDirectories);

            string? fullLocalPath = matches.FirstOrDefault(); // lấy file đầu tiên tìm thấy

            if (!File.Exists(fullLocalPath))
            {
                Console.WriteLine($"Warning: Local image file not found: {fullLocalPath}");
                return null; // File not found, cannot migrate
            }

            try
            {
                using (var stream = new FileStream(fullLocalPath, FileMode.Open))
                {
                    // Create a dummy IFormFile from the local file stream
                    var formFile = new FormFile(stream, 0, stream.Length, "file", Path.GetFileName(fullLocalPath));
                    var cloudinaryUrl = await _cloudinaryService.UploadImageAsync(formFile, cloudinaryTag);

                    // Optional: Delete local file after successful upload
                    // File.Delete(fullLocalPath);
                    Console.WriteLine($"Migrated {imagePath} to {cloudinaryUrl}");
                    return cloudinaryUrl;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error migrating image {imagePath}: {ex.Message}");
                return null;
            }
        }

#if DEBUG
        private async Task TestCloudinaryMigrationAsync()
        {
            Console.WriteLine("[TEST MODE] Starting Cloudinary migration test for a single image from Hinhanh table...");

            // Find one image from Hinhanh that has a local image URL
            var image = await _db.Hinhanhs
                .Where(h => !string.IsNullOrEmpty(h.TenHinhAnh) && !h.TenHinhAnh.StartsWith("http"))
                .FirstOrDefaultAsync();

            if (image == null)
            {
                Console.WriteLine("[TEST MODE] No local images found in Hinhanh table to test.");
                return;
            }

            Console.WriteLine($"[TEST MODE] Found image to test. ID: {image.MaHinhAnh}, Current Image URL: {image.TenHinhAnh}");

            var originalUrl = image.TenHinhAnh;
            var newUrl = await MigrateImage(originalUrl, "product-images");

            if (newUrl != null && newUrl != originalUrl)
            {
                Console.WriteLine($"[TEST MODE] SUCCESS: Image migrated successfully.");
                Console.WriteLine($"[TEST MODE] >> Old URL: {originalUrl}");
                Console.WriteLine($"[TEST MODE] >> New URL: {newUrl}");

                // Update the URL in the entity
                image.TenHinhAnh = newUrl;

                // Save changes to the database
                await _db.SaveChangesAsync();
                Console.WriteLine($"[TEST MODE] Database updated for Hinhanh ID: {image.MaHinhAnh}.");
            }
            else if (newUrl == originalUrl)
            {
                Console.WriteLine("[TEST MODE] SKIPPED: The image URL was already a remote URL or empty.");
            }
            else
            {
                Console.WriteLine($"[TEST MODE] FAILED: Image migration failed for Hinhanh ID: {image.MaHinhAnh}. Check previous logs for errors.");
            }

            Console.WriteLine("[TEST MODE] Cloudinary migration test finished.");
        }
#endif
    }
}
