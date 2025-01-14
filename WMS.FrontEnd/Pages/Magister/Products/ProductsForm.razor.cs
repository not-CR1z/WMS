using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Security;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Location;
using DocumentFormat.OpenXml.Wordprocessing;
using Blazored.Modal;
using DocumentFormat.OpenXml.Office2010.Excel;
using System.Collections.Generic;
using WMS.FrontEnd.Pages.Security.FormUserType;
using Blazored.Modal.Services;
using WMS.FrontEnd.Shared;
using WMS.Share.DTOs;
using System.Net.Http.Headers;
using DocumentFormat.OpenXml.EMMA;
using System.Net;

namespace WMS.FrontEnd.Pages.Magister.Products
{
    public partial class ProductsForm
    {
        private EditContext editContext = null!;
        private bool loading;

        [EditorRequired, Parameter]
        public Product Model { get; set; } = null!;

        [EditorRequired, Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;

        public bool FormPostedSuccessfully { get; set; }

        public long? ProductTypeId { get; set; }
        public string? NameProductType { get; set; }

        //public List<ProductProductClassificationDetail>? MyList { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            editContext = new(Model);
            if(Model.ProductType!= null)
            {
                NameProductType = Model.ProductType.Name;
            }
            if(Model.ProductProductClassificationDetails == null)
            {
                Model.ProductProductClassificationDetails = [];
            }
        }

        private async Task OnDataAnnotationsValidatedAsync()
        {
            await OnValidSubmit.InvokeAsync();
        }

        private async Task OnBeforeInternalNavigation(LocationChangingContext context)
        {
            var formWasEdited = editContext.IsModified();
            if (!formWasEdited || FormPostedSuccessfully)
            {
                return;
            }

            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = "¿Deseas abandonar la página y perder los cambios?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = !string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }
            context.PreventNavigation();
        }

        private async Task SearchProductType()
        {
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Label", "Típo Producto");
            parameters.Add("Url", "api/producttypes/genericsearch");

            var modalOptions = new ModalOptions();
            modalOptions.HideCloseButton = false;
            modalOptions.DisableBackgroundCancel = false;
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters, modalOptions);

            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                NameProductType = ItemSelect.Name;
                Model.ProductTypeId = ItemSelect.Id;
            }
            return;
        }

        private async Task AddClasification()
        {
            if(Model.Id==0)
            {
                await SweetAlertService.FireAsync("Alerta", "Debe Guardar antes de agregar clasificaciones", SweetAlertIcon.Warning);
                return;
            }
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Label", "Típo Producto");
            parameters.Add("Url", "api/productclassifications/genericsearch");

            var modalOptions = new ModalOptions();
            modalOptions.HideCloseButton = false;
            modalOptions.DisableBackgroundCancel = false;
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters, modalOptions);

            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                var productProductClassificationDetail = Model.ProductProductClassificationDetails!.Where(w => w.ProductClassificationId == ItemSelect.Id).FirstOrDefault();
                if (productProductClassificationDetail != null)
                {
                    await SweetAlertService.FireAsync("Alerta", "Esta clasificación ya se encuentra asignada", SweetAlertIcon.Warning);
                    return;
                }
                if(Model.ProductProductClassificationDetails == null)
                {
                    Model.ProductProductClassificationDetails = [];
                }
                var httpResponse = await Repository.GetAsync<ProductClassification>($"/api/productclassifications/{ItemSelect.Id}");
                if (httpResponse.Error)
                {
                    var message = await httpResponse.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                    return;
                }
                var productClassification = (ProductClassification)httpResponse.Response!;
                Model.ProductProductClassificationDetails.Add(
                    new ProductProductClassificationDetail
                    {
                        ProductId = Model.Id,
                        ProductClassificationId = productClassification.Id,
                        ProductClassification = productClassification,
                        ListDetail = productClassification.ProductClassificationDetails!.ToList()

                    });                
                }
            return;
        }

        private async Task DeleteClasification(ProductProductClassificationDetail model)
        {
            //var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            //{
            //    Title = "Confirmación",
            //    Text = $"¿Estas seguro de eliminar : {model.ProductClassification!.Name}?",
            //    Icon = SweetAlertIcon.Question,
            //    ShowCancelButton = true,
            //});
            //var confirm = string.IsNullOrEmpty(result.Value);
            //if (confirm)
            //{
            //    return;
            //}

            
            if (model.Id != 0)
            {
                var responseHttp = await Repository.DeleteAsync<ProductProductClassificationDetail>($"api/products/deleteclasificationasync/{model.Id}");
                if (responseHttp.Error)
                {
                    if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                    {
                        await SweetAlertService.FireAsync("Error", "No se encuentra el registro a eliminar", SweetAlertIcon.Error);
                        Model.ProductProductClassificationDetails!.Remove(model);
                    }
                    else
                    {
                        var mensajeError = await responseHttp.GetErrorMessageAsync();
                        await SweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                    }
                }
            }
            Model.ProductProductClassificationDetails!.Remove(model);
            return;





            //var toast = SweetAlertService.Mixin(new SweetAlertOptions
            //{
            //    Toast = true,
            //    Position = SweetAlertPosition.BottomEnd,
            //    ShowConfirmButton = true,
            //    Timer = 3000
            //});
            //await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro eliminado con éxito.");
        }
    }
}