using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Catalog.Products.Public;
using eShop.ViewModels.Common;
using System.Threading.Tasks;

namespace eShop.Application.Catalog
{
    interface IPublicProductService
    {
        Task<PagedResult<ProductViewModel>> GetAllPaging(GetProductPagingRequest request);
    }
}
