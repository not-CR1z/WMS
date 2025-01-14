using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using Microsoft.AspNetCore.Authorization;
using WMS.Share.Models.Location;

namespace WMS.FrontEnd.Pages.Location.Wineries
{
    [Authorize]
    public partial class WineriesCreate
    {
        private Winery Model = new();
        public WineriesForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override void OnInitialized()
        {
            if(Model==null)
            {
                Model = new Winery();
                Model.Branch= new Branch();
            }
        }
        private async Task CreateAsync()
        {
            if (Model.BranchId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Sucursal", SweetAlertIcon.Warning);
                return;
            }
            var httpResponse = await Repository.PostAsync("/api/wineries", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            NavigationManager.NavigateTo("/wineries");
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/wineries");
        }
    }
}