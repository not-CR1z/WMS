using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using Microsoft.AspNetCore.Authorization;

namespace WMS.FrontEnd.Pages.Magister.ProductTypes
{
    [Authorize]
    public partial class ProductTypesCreate
    {
        private ProductType Model = new();
        public ProductTypesForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        private async Task CreateAsync()
        {
            var httpResponse = await Repository.PostAsync("/api/producttypes", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            NavigationManager.NavigateTo("/producttypes");
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/producttypes");
        }
    }
}