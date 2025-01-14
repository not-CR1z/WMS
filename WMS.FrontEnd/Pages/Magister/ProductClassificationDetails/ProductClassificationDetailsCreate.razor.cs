using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using Microsoft.AspNetCore.Authorization;

namespace WMS.FrontEnd.Pages.Magister.ProductClassificationDetails
{
    [Authorize]
    public partial class ProductClassificationDetailsCreate
    {
        private ProductClassificationDetail Model = new();
        public ProductClassificationDetailsForm? form;
        [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        [Parameter] public long ProductClassificationId { get; set; }

        private async Task CreateAsync()
        {
            Model.ProductClassificationId= ProductClassificationId;
            var httpResponse = await Repository.PostAsync("/api/productclassificationdetails", Model);
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
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/productclassifications/details/{ProductClassificationId}");
        }
    }
}