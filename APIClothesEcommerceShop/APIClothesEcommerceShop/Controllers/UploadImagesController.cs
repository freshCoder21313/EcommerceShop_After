using APIClothesEcommerceShop.Services.CloudinaryService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIClothesEcommerceShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImagesController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public UploadImagesController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Không có file được tải lên." });
                }

                var imageUrl = await _cloudinaryService.UploadImageAsync(file, "products");

                return Ok(new
                {
                    Success = true,
                    ImageUrl = imageUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tải lên file.", error = ex.Message });
            }
        }
    }
}
