using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using Microsoft.AspNetCore.Authorization;

namespace WMS.FrontEnd.Pages.Magister.BinTypes
{
    [Authorize]
    public partial class BinTypesCreate
    {
        private BinType Model = new();
        public BinTypesForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        private async Task CreateAsync()
        {
            var httpResponse = await Repository.PostAsync("/api/bintypes", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            NavigationManager.NavigateTo("/bintypes");
        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/bintypes");
        }
    }
}