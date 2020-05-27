using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Common;
using System.Threading.Tasks;

namespace eShop.Application.Catalog
{
    interface IPublicProductService
    {
        Task<PagedResult<ProductViewModel>> GetAllPaging(GetPublicProductPagingRequest request);
    }
}
