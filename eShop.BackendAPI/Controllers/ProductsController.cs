using eShop.Application.Catalog;
using eShop.ViewModels.Catalog.ProductImages;
using eShop.ViewModels.Catalog.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace eShop.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IPublicProductService _publicProductService;
        private readonly IManageProductService _manageProductService;
        public ProductsController(IPublicProductService publicProductService, IManageProductService manageProductService)
        {
            _publicProductService = publicProductService;
            _manageProductService = manageProductService;
        }

        //https://localhost:port/products?pageIndex=1&pageSize=1&category=1
        [HttpGet("{languageId}")]
        public async Task<ActionResult> Get(string languageId, [FromQuery]GetPublicProductPagingRequest request)
        {
            var result = await _publicProductService.GetAllPaging(languageId, request);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        //https://localhost:port/product/get-paging
        [HttpGet("{id}/{languageId}")]
        public async Task<ActionResult> GetById(int id, string languageId)
        {
            var result = await _manageProductService.GetById(id, languageId);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        //https://localhost:port/products
        [HttpPost]
        public async Task<ActionResult> Create([FromForm]ProductCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var productId = await _manageProductService.Create(request);
            if (productId == 0)
            {
                return BadRequest();
            }
            var product = await _manageProductService.GetById(productId, request.LanguageId);
            //return Created(nameof(GetById), productId);
            return CreatedAtAction(nameof(GetById), new { id = productId }, product);
        }

        //https://localhost:port/products
        [HttpPut]
        public async Task<ActionResult> Update([FromBody]ProductUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var affectedResult = await _manageProductService.Update(request);
            if (affectedResult == 0)
            {
                return BadRequest();
            }

            return Ok(affectedResult);
        }
        //https://localhost:port/product/delete
        [HttpDelete("{productId}")]
        public async Task<ActionResult> Delete(int productId)
        {
            var affectedResult = await _manageProductService.Delete(productId);
            if (affectedResult == 0)
            {
                return BadRequest();
            }

            return Ok(affectedResult);
        }

        //https://localhost:port/products
        [HttpPatch("{productId}/{newPrice}")]
        public async Task<ActionResult> UpdatePrice(int productId, decimal newPrice)
        {
            var isUpdated = await _manageProductService.UpdatePrice(productId, newPrice);
            if (isUpdated)
            {
                return Ok();
            }
            return BadRequest();
        }



        #region Hande Image 
        //https://localhost:port/products
        [HttpPost("{productId}/images")]
        public async Task<ActionResult> CreateImage(int productId, [FromForm]ProductImageCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var imageId = await _manageProductService.AddImage(request);
            if (productId == 0)
            {
                return BadRequest();
            }
            var image = await _manageProductService.GetImageById(imageId);
            //return Created(nameof(GetById), productId);
            return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
        }

        //https://localhost:port/products/1/images/1
        [HttpGet("{productId}/images/{imageId}")]
        public async Task<ActionResult> GetImageById(int productId, int imageId)
        {
            var result = await _manageProductService.GetImageById(imageId);
            if (result == null)
            {
                return BadRequest();
            }
            return Ok(result);
        }

        //https://localhost:port/products
        [HttpPut("{productId}/images/{imageId}")]
        public async Task<ActionResult> UpdateImage(int productId, int imageId, [FromForm]ProductImageUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var image = await _manageProductService.UpdateImage(imageId, request);
            if (productId == 0)
            {
                return BadRequest();
            }
            //return Created(nameof(GetById), productId);
            //return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
            return Ok();
        }

        //https://localhost:port/products
        [HttpDelete("{productId}/images/{imageId}")]
        public async Task<ActionResult> RemoveImage(int productId, int imageId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var image = await _manageProductService.RemoveImage(imageId);
            if (productId == 0)
            {
                return BadRequest();
            }
            //return Created(nameof(GetById), productId);
            //return CreatedAtAction(nameof(GetImageById), new { id = imageId }, image);
            return Ok();
        }
        #endregion
    }
}
