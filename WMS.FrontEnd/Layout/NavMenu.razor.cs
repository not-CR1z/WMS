using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Services;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;

namespace WMS.FrontEnd.Layout
{
    public partial class NavMenu
    {
        public List<FormParentDTO>? menus;
        //bool to send to MainLayout for shrinking sidebar and showing/hide menu text
        private bool IconMenuActive { get; set; } = false;
        [Inject] private ILoginService LoginService { get; set; } = null!;


        //EventCallback for sending bool to MainLayout
        [Parameter]
        public EventCallback<bool> ShowIconMenu { get; set; }

        private bool collapseNavMenu = true;

        private string? NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        private bool collapseMenu = true;
        //private bool expandSubMenu;
        //private string? MenuCssClass => collapseMenu ? "collapse" : null;

        private bool collapseMenu2 = true;
        //private bool expandSubMenu2;
        //private string? MenuCssClass2 => collapseMenu2 ? "collapse" : null;
        private void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
            collapseMenu = !collapseMenu;
            collapseMenu2 = !collapseMenu2;
        }

        private void ExpandItem(FormParentDTO item)
        {
            if (item.Expand)
            {
                item.Expand = false;
            }
            else
            {
                item.Expand = true;
            }
        }

        private void ExpandItem(FormSubParentDTO item)
        {
            if (item.Expand)
            {
                item.Expand = false;
            }
            else
            {
                item.Expand = true;
            }
        }

        //Method to toggle IconMenuActive bool and send bool via EventCallback
        private async Task ToggleIconMenu()
        {
            IconMenuActive = !IconMenuActive;
            await ShowIconMenu.InvokeAsync(IconMenuActive);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (menus == null || menus.Count == 0)
                menus = await LoginService.GetMenu();
        }
    }
}