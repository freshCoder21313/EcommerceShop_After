using APIClothesEcommerceShop.DTO;
using APIClothesEcommerceShop.Models;
using APIClothesEcommerceShop.Repositories.Combo;
using APIClothesEcommerceShop.Repositories;
using System.Data.Common;
using System.Drawing.Text;
using static Azure.Core.HttpHeader;
using APIClothesEcommerceShop.Data;
using APIClothesEcommerceShop.DTO.Combos;
using APIClothesEcommerceShop.Repositories.DetailCombo;
using APIClothesEcommerceShop.Services.CloudinaryService; // Added for Cloudinary
using Microsoft.AspNetCore.Http; // Added for IFormFile

namespace APIClothesEcommerceShop.Services
{
    public class ComboService
    {
        private readonly IComboRepository comboRepository;
        private readonly IDetailCombo detailComboRepository;
        private readonly EcommerceShopContext db;
        private readonly ICloudinaryService _cloudinaryService; // Added for Cloudinary

        public ComboService(IComboRepository comboRepository, IDetailCombo detailComboRepository, EcommerceShopContext db, ICloudinaryService cloudinaryService)
        {
            this.comboRepository = comboRepository;
            this.detailComboRepository = detailComboRepository;
            this.db = db;
            _cloudinaryService = cloudinaryService; // Added for Cloudinary
        }
        public async Task AddCombo(ComboRequestDTO combo)
        {
            await db.Database.BeginTransactionAsync();
            try
            {
                string imageUrl = null;
                if(combo.Hinh != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(combo.Hinh, "combo-images");
                }
                var model = new Combo
                {
                    TenCombo = combo.TenCombo,
                    Hinh = imageUrl, // Store Cloudinary URL
                    SoTienGiam = combo.SoTienGiam,
                    PhanTramGiam = combo.PhanTramGiam,
                    NgayKetThuc = combo.NgayKetThuc,
                    NgayBatDau = combo.NgayBatDau,
                    SoLuong = combo.SoLuong,
                    MoTa = combo.MoTa,
                    IsActive = true,
                };
                model = await comboRepository.AddCombo(model);
                if(combo.Chitietcombos != null)
                {
                    foreach(var detail in combo.Chitietcombos)
                    {
                        var NewDetailCombo = new Chitietcombo
                        {
                            MaCombo = model.MaCombo,
                            MaSp = detail.MaSp,
                            SoLuongSP = detail.SoLuongSp,
                        };
                        await detailComboRepository.AddDetailCombo(NewDetailCombo);
                    }
                    await db.Database.CommitTransactionAsync();
                }
            }catch(Exception)
            {
               await db.Database.RollbackTransactionAsync();
               throw;
            }
        }

        public async Task EditCombo(int id, ComboRequestDTO combo)
        {
            await db.Database.BeginTransactionAsync();
            try
            {
                var findCombo = await db.Combos.FindAsync(id);
                if(findCombo == null)
                {
                    throw new Exception("Combo not found");
                }

                string imageUrl = findCombo.Hinh; // Keep existing image URL by default
                if (combo.Hinh != null)
                {
                    imageUrl = await _cloudinaryService.UploadImageAsync(combo.Hinh, "combo-images");
                }

                findCombo.TenCombo = combo.TenCombo;
                findCombo.PhanTramGiam = combo.PhanTramGiam;
                findCombo.SoTienGiam = combo.SoTienGiam;
                findCombo.MoTa = combo.MoTa;
                findCombo.IsActive = true;
                findCombo.NgayBatDau = combo.NgayBatDau;
                findCombo.NgayKetThuc = combo.NgayKetThuc;
                findCombo.Hinh = imageUrl; // Store Cloudinary URL
                findCombo.SoLuong = combo.SoLuong;
                await comboRepository.EditCombo(findCombo);
                await detailComboRepository.DeleteDetailComboByMaCombo(findCombo.MaCombo);
                foreach(var detail in combo.Chitietcombos)
                {
                    var NewCombo = new Chitietcombo
                    {
                        MaCombo = findCombo.MaCombo,
                        MaSp = detail.MaSp,
                        SoLuongSP = detail.SoLuongSp,
                    };
                    await detailComboRepository.AddDetailCombo(NewCombo);
                }   
                await db.Database.CommitTransactionAsync();
            }
            catch(Exception)
            {
                await db.Database.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
