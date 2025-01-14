using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Magister.ProductClassifications
{
    public partial class ProductClassificationsEdit
    {
        private ProductClassification Model = new();
        public ProductClassificationsForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public long Id { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            var httpResponse = await Repository.GetAsync<ProductClassification>($"/api/productclassifications/{Id}");
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
            var httpResponse = await Repository.PutAsync("/api/productclassifications", Model);
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
            NavigationManager.NavigateTo("/productclassifications");
        }
    }
}