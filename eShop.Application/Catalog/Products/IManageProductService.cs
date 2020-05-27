using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Common;
using System.Threading.Tasks;

namespace eShop.Application.Catalog
{
    public interface IManageProductService
    {
        Task<int> Create(ProductCreateRequest request);
        Task<int> Update(ProductUpdateRequest request);
        Task<int> Delete(int productId);
        Task<bool> UpdatePrice(int productId, decimal newPrice);
        Task<bool> UpdateStock(int productId, int addedQuantity);
        Task AddViewCount(int productId);
        Task<ProductViewModel> GetById(int productId);
        Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request);
        //Task<int> AddImage(int productId, List<IFormFile> files);
        //Task<int> RemoveImage(int imageId);
        //Task<int> UpdateImage(int imageId, string caption, bool isDefault);
        //Task<List<ProductImageViewModel>> GetListImage(int productId);
    }
}
