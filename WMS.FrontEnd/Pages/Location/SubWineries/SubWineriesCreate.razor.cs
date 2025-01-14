using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using Microsoft.AspNetCore.Authorization;
using WMS.Share.Models.Location;
using WMS.FrontEnd.Pages.Location.SubWineries;

namespace WMS.FrontEnd.Pages.Location.SubWineries
{
    [Authorize]
    public partial class SubWineriesCreate
    {
        private SubWinery Model = new();
        public SubWineriesForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override void OnInitialized()
        {
            if(Model==null)
            {
                Model = new SubWinery();
                Model.Winery= new Winery();
            }
        }
        private async Task CreateAsync()
        {
            if (Model.WineryId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Bodega", SweetAlertIcon.Warning);
                return;
            }
            var httpResponse = await Repository.PostAsync("/api/subwineries", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            NavigationManager.NavigateTo("/subwineries");
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/subwineries");
        }
    }
}