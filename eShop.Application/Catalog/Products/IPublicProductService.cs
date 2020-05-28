using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Common;
using System.Threading.Tasks;

namespace eShop.Application.Catalog
{
    public interface IPublicProductService
    {
        Task<PagedResult<ProductViewModel>> GetAllPaging(string languageId,GetPublicProductPagingRequest request);
        //Task<PagedResult<ProductViewModel>> GetAll();
    }
}
