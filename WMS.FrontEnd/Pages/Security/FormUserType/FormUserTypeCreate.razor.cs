using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security;
using WMS.FrontEnd.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace WMS.FrontEnd.Pages.Security.FormUserType
{
    [Authorize]
    public partial class FormUserTypeCreate
    {
        private WMS.Share.Models.Security.FormUserType Model = new();
        public FormUserTypeForm? form;
        [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public long FormParentId { get; set; }
        [Parameter] public long FormId { get; set; }

        private async Task CreateAsync()
        {
            if(Model.UserTypeId==0)
            {

                await SweetAlertService.FireAsync("Alerta", "Debe Seleccionar Rol", SweetAlertIcon.Error);
                return;
            }
            Model.FormId= FormId;
            var httpResponse = await Repository.PostAsync("/api/formusertypes", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            if (BlazoredModal != null)
            {
                await BlazoredModal.CloseAsync(ModalResult.Ok());
            }
            Return();
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/formparent/details/formusertypeindex/{FormParentId}/{FormId}");
        }
    }
}