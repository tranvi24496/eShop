using eShop.Data.EF;
using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Common;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eShop.Application.Catalog
{
    public class PublicProductService : IPublicProductService
    {
        private readonly EShopDbContext _context;
        public PublicProductService(EShopDbContext context)
        {
            _context = context;
        }

        //public async Task<PagedResult<ProductViewModel>> GetAll()
        //{
        //    var query = from p in _context.Products
        //                join pt in _context.ProductTranslations on p.Id equals pt.ProductId
        //                join pic in _context.ProductInCategories on p.Id equals pic.ProductId
        //                join c in _context.Categories on pic.CategoryId equals c.Id
        //                select new { p, pt, pic };
        //    var totalRecord = await query.CountAsync();
        //    var data = await query.Select(x => new ProductViewModel()
        //        {
        //            Id = x.p.Id,
        //            Price = x.p.Price,
        //            OriginalPrice = x.p.OriginalPrice,
        //            Stock = x.p.Stock,
        //            ViewCount = x.p.ViewCount,
        //            DateCreated = x.p.DateCreated,
        //            Name = x.pt.Name,
        //            Description = x.pt.Description,
        //            Details = x.pt.Details,
        //            SeoDescription = x.pt.SeoDescription,
        //            SeoTitle = x.pt.SeoTitle,
        //            SeoAlias = x.pt.SeoAlias,
        //            LanguageId = x.pt.LanguageId
        //        }).ToListAsync(); ;

        //    // 4. Result
        //    var pageResult = new PagedResult<ProductViewModel>()
        //    {
        //        TotalRecord = totalRecord,
        //        Items = data
        //    };
        //    return pageResult;
        //}

        public async Task<PagedResult<ProductViewModel>> GetAllPaging(string languageId, GetPublicProductPagingRequest request)
        {
            // 1. Query
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        where pt.LanguageId == languageId
                        select new { p, pt, pic };
            // 2. Filter
            if (request.CategoryId.HasValue && request.CategoryId.Value > 0)
            {
                query = query.Where(x => x.pic.CategoryId == request.CategoryId);
            }
            // 3. Paging
            var totalRecord = await query.CountAsync();
            var data = await query.Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new ProductViewModel()
                {
                    Id = x.p.Id,
                    Price = x.p.Price,
                    OriginalPrice = x.p.OriginalPrice,
                    Stock = x.p.Stock,
                    ViewCount = x.p.ViewCount,
                    DateCreated = x.p.DateCreated,
                    Name = x.pt.Name,
                    Description = x.pt.Description,
                    Details = x.pt.Details,
                    SeoDescription = x.pt.SeoDescription,
                    SeoTitle = x.pt.SeoTitle,
                    SeoAlias = x.pt.SeoAlias,
                    LanguageId = x.pt.LanguageId
                }).ToListAsync(); ;

            // 4. Result
            var pageResult = new PagedResult<ProductViewModel>()
            {
                TotalRecord = totalRecord,
                Items = data
            };
            return pageResult;
        }

    }
}
