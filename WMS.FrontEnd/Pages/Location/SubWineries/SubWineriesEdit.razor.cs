using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Location;
using WMS.FrontEnd.Pages.Location.SubWineries;

namespace WMS.FrontEnd.Pages.Location.SubWineries
{
    public partial class SubWineriesEdit
    {
        private SubWinery Model = new();
        public SubWineriesForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public long Id { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            var httpResponse = await Repository.GetAsync<SubWinery>($"/api/subwineries/{Id}");
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            Model = httpResponse.Response!;
        }

        private async Task SavedAsync()
        {
            if (Model.WineryId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Bodega", SweetAlertIcon.Warning);
                return;
            }
            var httpResponse = await Repository.PutAsync("/api/subwineries", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            Return();
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/subwineries");
        }
    }
}