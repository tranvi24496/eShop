using eShop.Data.EF;
using eShop.Data.Entities;
using eShop.Utilities;
using eShop.ViewModels.Catalog.ProductImages;
using eShop.ViewModels.Catalog.Products;
using eShop.ViewModels.Common;
using eShopSolution.Application.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace eShop.Application.Catalog
{
    public class ManageProductService : IManageProductService
    {
        private readonly EShopDbContext _context;
        private readonly IStorageService _storageService;
        public ManageProductService(EShopDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        public async Task<int> AddImage(ProductImageCreateRequest request)
        {
            var productImage = new ProductImage()
            {
                DateCreated = DateTime.Now,
                IsDefault = request.IsDefault,
                Caption = request.Caption,
                SortOrder = request.SortOrder
            };
            if (request.ImageFile == null) return -1;
            if (request.ImageFile != null)
            {
                productImage.ImagePath = await SaveFile(request.ImageFile);
                productImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Add(productImage);
            await _context.SaveChangesAsync();
            return productImage.Id;
        }

        public async Task AddViewCount(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Can't find product : {productId}");
            product.ViewCount += 1;
            await _context.SaveChangesAsync();
        }

        public async Task<int> Create(ProductCreateRequest request)
        {
            var product = new Product()
            {
                Price = request.Price,
                OriginalPrice = request.OriginalPrice,
                Stock = request.Stock,
                ViewCount = 0,
                DateCreated = DateTime.Now,
                ProductTranslations = new List<ProductTranslation>()
                {
                    new ProductTranslation()
                    {
                        Name = request.Name,
                        Description = request.Description,
                        Details = request.Description,
                        SeoDescription = request.SeoDescription,
                        SeoTitle = request.SeoTitle,
                        SeoAlias= request.SeoAlias,
                        LanguageId = request.LanguageId
                    }
                }
            };
            //Save Image
            if (request.ThumbnailImage != null)
            {
                product.ProductImages = new List<ProductImage>() {
                    new ProductImage()
                    {
                        Caption = "Thumbnail image",
                        DateCreated = DateTime.Now,
                        FileSize = request.ThumbnailImage.Length,
                        ImagePath = await this.SaveFile(request.ThumbnailImage),
                        IsDefault = true,
                        SortOrder = 1
                    }
                };
            }
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product.Id;
        }

        public async Task<int> Delete(int productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Can't find a product with id: {productId}");
            var images = _context.ProductImages.Where(c => c.ProductId == productId);
            foreach (var item in images)
            {
                await _storageService.DeleteFileAsync(item.ImagePath);
            }
            _context.Products.Remove(product);
            return await _context.SaveChangesAsync();
        }

        public async Task<PagedResult<ProductViewModel>> GetAllPaging(GetManageProductPagingRequest request)
        {
            // 1. Query
            var query = from p in _context.Products
                        join pt in _context.ProductTranslations on p.Id equals pt.ProductId
                        join pic in _context.ProductInCategories on p.Id equals pic.ProductId
                        join c in _context.Categories on pic.CategoryId equals c.Id
                        select new { p, pt, pic };
            // 2. Filter
            if (!string.IsNullOrEmpty(request.keyword))
            {
                query = query.Where(x => x.pt.Name.Contains(request.keyword));
            }
            if (request.CategoryIds.Count > 0)
            {
                query = query.Where(x => request.CategoryIds.Contains(x.pic.CategoryId));
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

        public async Task<ProductViewModel> GetById(int productId, string languageId)
        {
            var product = await _context.Products.FindAsync(productId);
            var languageProduct = await _context.ProductTranslations.FirstOrDefaultAsync(c => c.ProductId == productId && c.LanguageId == languageId);
            var data = new ProductViewModel()
            {
                Id = product.Id,
                Price = product.Price,
                OriginalPrice = product.OriginalPrice,
                Stock = product.Stock,
                ViewCount = product.ViewCount,
                DateCreated = product.DateCreated,
                Name = languageProduct.Name,
                Description = languageProduct.Description,
                Details = languageProduct.Details,
                SeoDescription = languageProduct.SeoDescription,
                SeoTitle = languageProduct.SeoTitle,
                SeoAlias = languageProduct.SeoAlias,
                LanguageId = languageProduct.LanguageId
            };

            // 4. Result
            return data;
        }

        public async Task<ProductImageViewModel> GetImageById(int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            if (image == null)
            {
                throw new EShopException("Can't find Image with Id : {imageId}");
            }
            var viewModel = new ProductImageViewModel()
            {
                Id = image.Id,
                ProductId = image.ProductId,
                DateCreated = image.DateCreated,
                IsDefault = image.IsDefault,
                Caption = image.Caption,
                SortOrder = image.SortOrder,
                FileSize = image.FileSize,
                ImagePath = image.ImagePath
            };
            return viewModel;
        }

        public async Task<List<ProductImageViewModel>> GetListImages(int productId)
        {
            var images = await _context.ProductImages.Where(c => c.ProductId == productId)
                .Select(c => new ProductImageViewModel()
                {
                    Id = c.Id,
                    ProductId = c.ProductId,
                    DateCreated = c.DateCreated,
                    IsDefault = c.IsDefault,
                    Caption = c.Caption,
                    SortOrder = c.SortOrder,
                    FileSize = c.FileSize,
                    ImagePath = c.ImagePath
                }).ToListAsync();
            return images;
        }

        public async Task<int> RemoveImage( int imageId)
        {
            var image = await _context.ProductImages.FindAsync(imageId);
            _context.ProductImages.Remove(image);
            //Delete File in storage
            await _storageService.DeleteFileAsync(image.ImagePath);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> Update(ProductUpdateRequest request)
        {
            var product = await _context.Products.FindAsync(request.Id);
            var productTranslation = await _context.ProductTranslations.FirstOrDefaultAsync(x => x.ProductId == product.Id
            && x.LanguageId == request.LanguageId);
            if (product == null) throw new EShopException($"Can't find a product with id: {request.Id}");
            productTranslation.Name = request.Name;
            productTranslation.Description = request.Description;
            productTranslation.Details = request.Details;
            productTranslation.SeoDescription = request.SeoDescription;
            productTranslation.SeoTitle = request.SeoTitle;
            productTranslation.SeoAlias = request.SeoAlias;

            //Save Image
            if (request.ThumbnailImage != null)
            {
                var thumbnailImage = _context.ProductImages.FirstOrDefault(x => x.IsDefault == true && x.ProductId == request.Id);
                if (thumbnailImage != null)
                {
                    thumbnailImage.FileSize = request.ThumbnailImage.Length;
                    //Remove file in storage
                    await _storageService.DeleteFileAsync(thumbnailImage.ImagePath);
                    thumbnailImage.ImagePath = await this.SaveFile(request.ThumbnailImage);
                    _context.ProductImages.Update(thumbnailImage);
                }
            }
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateImage(int imageId, ProductImageUpdateRequest request)
        {
            var thumbImage = _context.ProductImages.FirstOrDefault(c =>  c.Id == imageId);
            if (thumbImage == null)
            {
                throw new EShopException($"You can't find image withImageId : {imageId}");
            }
            thumbImage.Caption = request.Caption;
            thumbImage.IsDefault = request.IsDefault;
            thumbImage.SortOrder = request.SortOrder;
            if (request.ImageFile == null) return -1;
            if (request.ImageFile != null)
            {
                thumbImage.ImagePath = await SaveFile(request.ImageFile);
                thumbImage.FileSize = request.ImageFile.Length;
            }
            _context.ProductImages.Add(thumbImage);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdatePrice(int productId, decimal newPrice)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Can't find a product with id: {productId}");
            product.Price = newPrice;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateStock(int productId, int addedQuantity)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new EShopException($"Can't find a product with id: {productId}");
            product.Stock = addedQuantity;
            return await _context.SaveChangesAsync() > 0;
        }
        private async Task<string> SaveFile(IFormFile file)
        {
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            return fileName;
        }
    }
}
