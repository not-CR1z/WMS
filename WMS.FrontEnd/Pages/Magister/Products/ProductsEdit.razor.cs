using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Location;
using WMS.FrontEnd.Pages.Location.SubWineries;
using WMS.FrontEnd.Pages.Magister.Products;

namespace WMS.FrontEnd.Pages.Magister.Products
{
    public partial class ProductsEdit
    {
        private Product Model = new();
        public ProductsForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Parameter] public long Id { get; set; }


        protected override async Task OnParametersSetAsync()
        {
            var httpResponse = await Repository.GetAsync<Product>($"/api/products/{Id}");
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
            if (Model.ProductTypeId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Tipo Producto", SweetAlertIcon.Warning);
                return;
            }
            if(Model.ProductProductClassificationDetails!.Count>0)
            {
                foreach (var item in Model.ProductProductClassificationDetails)
                {
                    if(item.ProductClassificationDetailId == 0)
                    {
                        await SweetAlertService.FireAsync("Advertencia", $"Debe Seleccionar {item.ProductClassification!.Name}", SweetAlertIcon.Warning);
                        return;
                    }
                    
                }
            }
            var httpResponse = await Repository.PutAsync("/api/products", Model);
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
            NavigationManager.NavigateTo("/products");
        }
    }
}