using Blazored.Modal;
using Blazored.Modal.Services;
using ClosedXML.Excel;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Net;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;
using static System.Net.Mime.MediaTypeNames;

namespace WMS.FrontEnd.Pages.Magister.ProductClassificationDetails
{
    public partial class ProductClassificationDetailsIndex
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        public List<ProductClassificationDetail>? MyList { get; set; }

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadAsync();
        }

        private async Task SelectedPageAsync(int page)
        {
            if (!string.IsNullOrWhiteSpace(Page))
            {
                page = Convert.ToInt32(Page);
            }

            currentPage = page;
            await LoadAsync(page);
        }

        private async Task LoadAsync(int page = 1)
        {
            var ok = await LoadListAsync(page);
            if (ok)
            {
                await LoadPagesAsync();
            }
        }

        private async Task<bool> LoadListAsync(int page)
        {
            var url = $"api/productclassificationdetails/getasync?page={page}";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<ProductClassificationDetail>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return false;
            }
            MyList = responseHttp.Response;
            return true;
        }

        private async Task LoadPagesAsync()
        {
            var url = $"api/productclassificationdetails/totalPages";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"?filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<int>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            totalPages = responseHttp.Response;
        }

        private async Task CleanFilterAsync()
        {
            Filter = string.Empty;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }

        private async Task DeleteAsycn(ProductClassificationDetail model)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estas seguro de eliminar : {model.Name}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<ProductClassificationDetail>($"api/productclassificationdetails/deleteasync/{model.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/productclassificationdetails");
                }
                else
                {
                    var mensajeError = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", mensajeError, SweetAlertIcon.Error);
                }
                return;
            }

            await LoadAsync();
            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro eliminado con éxito.");
        }

        private async Task Export()
        {
            var url = $"api/productclassificationdetails/downloadasync";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"?filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<ProductClassificationDetail>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var ListDownload = responseHttp.Response;
            using (var book = new XLWorkbook())
            {
                IXLWorksheet sheet = book.Worksheets.Add("FormatoTipoUbicación");
                sheet.Cell(1, 1).Value = "Nombre";
                if (ListDownload == null || ListDownload.Count == 0)
                {
                    sheet.Cell(2, 1).Value = "Inventario";
                }
                else
                {
                    int i = 2;
                    foreach (var item in ListDownload)
                    {
                        sheet.Cell(i, 1).Value = item.Name;                 
                        i++;
                    }
                }
                using (var memory = new MemoryStream())
                {
                    book.SaveAs(memory);
                    await JSRuntime.InvokeAsync<object>(
                            "DownloadExcel",
                            $"{DateTime.Now.ToString("yyyyMMdd")}_TipoProducto.xlsx",
                            Convert.ToBase64String(memory.ToArray())
                    );
                }
            }
        }

        private async Task ShowModal(ProductClassificationDetail model)
        {
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Id", model.Id);
            ModalOptions mo = new ModalOptions
            {
                HideCloseButton = false,
                HideHeader = false,
                DisableBackgroundCancel=false,
                
            };
            modalReference = Modal.Show<ProductClassificationDetailsClock>(model.Name, parameters, mo);


            var result = await modalReference.Result;
            //if (result.Confirmed)
            //{
            //    await LoadAsync();
            //}
        }
    }
}