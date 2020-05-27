using eShop.Application.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace eShop.BackendAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IPublicProductService _publicProductService;
        public ProductController(IPublicProductService publicProductService)
        {
            _publicProductService = publicProductService;
        }
        [HttpGet]
        public ActionResult Index()
        {
            var result =  _publicProductService.GetAll().Result;
            if(result!=null && result.Items.Count > 0 )
            {
                return Ok(result);
            }
            return NotFound();
        }
    }
}