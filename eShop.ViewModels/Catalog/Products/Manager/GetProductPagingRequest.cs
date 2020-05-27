using eShop.ViewModels.Common;
using System.Collections.Generic;

namespace eShop.ViewModels.Catalog.Products.Manager
{
    public class GetProductPagingRequest : PagingRequestBase
    {
        public string keyword { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}
