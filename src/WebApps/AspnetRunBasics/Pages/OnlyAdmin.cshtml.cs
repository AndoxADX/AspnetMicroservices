using AspnetRunBasics.Models;
using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace AspnetRunBasics.Pages
{
    [Authorize(Roles = "admin")]
    public class OnlyAdminModel : PageModel
    {
        private readonly ICatalogService _catalogService;

        public OnlyAdminModel(ICatalogService catalogService)
        {
            _catalogService = catalogService ?? throw new ArgumentNullException(nameof(catalogService));
        }


        public UserInfoViewModel UserInfoViewModel { get; set; } = null;

        public async Task<IActionResult> OnGetAsync()
        {
            UserInfoViewModel = await _catalogService.GetUserInfo();
            return Page();
        }
    }
}
