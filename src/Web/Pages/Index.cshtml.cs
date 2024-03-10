using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.eShopWeb.Web.Services;
using Microsoft.eShopWeb.Web.ViewModels;
using Microsoft.FeatureManagement;

namespace Microsoft.eShopWeb.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ICatalogViewModelService _catalogViewModelService;

    public IndexModel(ICatalogViewModelService catalogViewModelService)
    {
        _catalogViewModelService = catalogViewModelService;
    }

    public required CatalogIndexViewModel CatalogModel { get; set; } = new CatalogIndexViewModel();

    private readonly IFeatureManager _featureManager;
    public async Task OnGet(CatalogIndexViewModel catalogModel, int? pageId)
    {
        if (await _featureManager.IsEnabledAsync("onsales"))
        {
            //set up empty list
            CatalogModel = await _catalogViewModelService.GetCatalogItems(pageId ?? 0, Constants.ITEMS_PER_PAGE, catalogModel.BrandFilterApplied, catalogModel.TypesFilterApplied);

            //add onsales deal
            CatalogModel.CatalogItems.Clear();
            CatalogModel.CatalogItems.Add(new CatalogItemViewModel()
            {
                Id = 0,
                Name = "This item is on sale",
                Price = 9.99M,
                PictureUri = "http://externalcatalogbaseurltobereplaced/api/pic/1"
            });

            CatalogModel.PaginationInfo.TotalItems = 1;
            CatalogModel.PaginationInfo.TotalPages = 1;
        }
        else
        {
            CatalogModel = await _catalogViewModelService.GetCatalogItems(pageId ?? 0, Constants.ITEMS_PER_PAGE, catalogModel.BrandFilterApplied, catalogModel.TypesFilterApplied);
        }
    }
}
