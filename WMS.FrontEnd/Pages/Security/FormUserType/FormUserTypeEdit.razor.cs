using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace WMS.FrontEnd.Pages.Security.FormUserType
{
    [Authorize]
    public partial class FormUserTypeEdit
    {
        private WMS.Share.Models.Security.FormUserType Model = new();
        private long FormId { get; set; } = 0;
        public FormUserTypeForm? form;

        [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public long FormParentId { get; set; }
        [Parameter] public long Id { get; set; }
       

        protected override async Task OnParametersSetAsync()
        {
            var httpResponse = await Repository.GetAsync<WMS.Share.Models.Security.FormUserType>($"/api/formusertypes/{Id}");
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            Model = httpResponse.Response!;
            FormId=Model.FormId;
            
        }

        private async Task SavedAsync()
        {
            if (Model.UserTypeId == 0)
            {

                await SweetAlertService.FireAsync("Alerta", "Debe Seleccionar Rol", SweetAlertIcon.Error);
                return;
            }
            var httpResponse = await Repository.PutAsync("/api/formusertypes", Model);
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