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

namespace WMS.FrontEnd.Pages.Location.Bins
{
    [Authorize]
    public partial class BinsCreate
    {
        private Bin Model = new();
        public BinsForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override void OnInitialized()
        {
            if(Model==null)
            {
                Model = new Bin();
                Model.SubWinery= new SubWinery();
                Model.BinType = new BinType();
            }
        }
        private async Task CreateAsync()
        {
            if (Model.SubWineryId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Sub-Bodega", SweetAlertIcon.Warning);
                return;
            }
            if (Model.BinTypeId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Tipo Ubicación", SweetAlertIcon.Warning);
                return;
            }
            var httpResponse = await Repository.PostAsync("/api/bins", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            NavigationManager.NavigateTo("/bins");
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/bins");
        }
    }
}