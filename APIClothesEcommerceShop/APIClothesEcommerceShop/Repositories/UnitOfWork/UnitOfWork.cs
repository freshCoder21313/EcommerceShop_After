using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using APIClothesEcommerceShop.Data;
using APIClothesEcommerceShop.Repositories.Category;
using APIClothesEcommerceShop.Repositories.Comments;
using APIClothesEcommerceShop.Repositories.Reviews;
using APIClothesEcommerceShop.Repositories.WheelCoupon;
using APIClothesEcommerceShop.Services;
using APIClothesEcommerceShop.Services.CloudinaryService;

namespace APIClothesEcommerceShop.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcommerceShopContext _context;
        private readonly IGeminiAIService _ai;
        private readonly IConfiguration _configuration;
        private readonly ICloudinaryService _cloudinaryService;
        public ICategoryRepository Category { get; private set; }
        public IReviewRepository Review { get; private set; }
        public IWheelCouponRepository WheelCoupon { get; private set; }
        public ICommentRepository Comment { get; private set; }

        public UnitOfWork(EcommerceShopContext context, IGeminiAIService ai, ICloudinaryService cloudinaryService, IConfiguration configuration)
        {
            _context = context;
            _ai = ai;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
            Category = new CategoryRepository(_context);
            Review = new ReviewRepository(_context, _ai, _cloudinaryService);
            WheelCoupon = new WheelCouponRepository(_context, _configuration);
            Comment = new CommentRepository(_context);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public async Task DisposeAsync()
        {
            await _context.DisposeAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}