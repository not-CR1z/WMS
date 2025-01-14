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

namespace WMS.FrontEnd.Pages.Magister.Products
{
    [Authorize]
    public partial class ProductsCreate
    {
        private Product Model = new();
        public ProductsForm? form;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;

        protected override void OnInitialized()
        {
            if(Model==null)
            {
                Model = new Product();
                Model.ProductType = new ProductType();
            }
        }
        private async Task CreateAsync()
        {
            if (Model.ProductTypeId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Tipo Producto", SweetAlertIcon.Warning);
                return;
            }
            var httpResponse = await Repository.PostAsync("/api/products", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 1000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro guardado con éxito.");
            var product = (Product)httpResponse.Response!;
            NavigationManager.NavigateTo($"/products/edit/{product.Id}");

        }

        private void Return()
        {
            form!.FormPostedSuccessfully = true;
            NavigationManager.NavigateTo($"/products");
        }
    }
}