using Blazored.Modal;
using Blazored.Modal.Services;
using ClosedXML.Excel;
using CurrieTechnologies.Razor.SweetAlert2;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections.Generic;
using System.Net;
using System.Security.Policy;
using WMS.FrontEnd.Pages.Security.FormUserType;
using WMS.FrontEnd.Repositories;
using WMS.FrontEnd.Shared;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;
using static System.Net.Mime.MediaTypeNames;

namespace WMS.FrontEnd.Pages.Magister.Products
{
    public partial class ProductsIndex
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter1 { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter2 { get; set; } = string.Empty;

        public List<Product>? MyList { get; set; }

        public long? ProductTypeId { get; set; }
        public string? NameProductType { get; set; }


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
            var url = $"api/products/getasync?page={page}";
            string FilterUrl = string.Empty;
            if (ProductTypeId != null && ProductTypeId != 0)
            {
                FilterUrl += $"&filter={ProductTypeId}";
            }
            if (!String.IsNullOrEmpty(Filter1))
            {
                FilterUrl += $"&filter1={Filter1}";
            }
            if (!String.IsNullOrEmpty(Filter2))
            {
                FilterUrl += $"&filter2={Filter2}";
            }
            if (!string.IsNullOrEmpty(FilterUrl))
            {
                url += FilterUrl;
            }
            var responseHttp = await Repository.GetAsync<List<Product>>(url);
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
            var url = $"api/products/totalPages";
            string FilterUrl=string.Empty ;
            if (ProductTypeId != null && ProductTypeId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter={ProductTypeId}";
                }
                else
                {
                    FilterUrl += $"?filter={ProductTypeId}";
                }
            }
            if (!String.IsNullOrEmpty(Filter1))
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter1={Filter1}";
                }
                else
                {
                    FilterUrl += $"?filter1={Filter1}";
                }
            }
            if (!String.IsNullOrEmpty(Filter2))
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter2={Filter2}";
                }
                else
                {
                    FilterUrl += $"?filter2={Filter2}";
                }
            }
            if (!string.IsNullOrEmpty(FilterUrl))
            {
                url += FilterUrl;
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
            ProductTypeId = 0;
            Filter1 = string.Empty;
            Filter2 = string.Empty;
            NameProductType = string.Empty;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }

        private async Task DeleteAsycn(Product model)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estas seguro de eliminar : {model.Reference}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<Product>($"api/products/deleteasync/{model.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/products");
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
            var url = $"api/products/downloadasync";
            string FilterUrl = string.Empty;
            if (ProductTypeId != null && ProductTypeId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter={ProductTypeId}";
                }
                else
                {
                    FilterUrl += $"?filter={ProductTypeId}";
                }
            }
            if (!String.IsNullOrEmpty(Filter1))
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter1={Filter1}";
                }
                else
                {
                    FilterUrl += $"?filter1={Filter1}";
                }
            }
            if (!String.IsNullOrEmpty(Filter2))
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter2={Filter2}";
                }
                else
                {
                    FilterUrl += $"?filter2={Filter2}";
                }
            }
            if (!string.IsNullOrEmpty(FilterUrl))
            {
                url += FilterUrl;
            }

            var responseHttp = await Repository.GetAsync<List<Product>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var ListDownload = responseHttp.Response;

            using (var book = new XLWorkbook())
            {
                IXLWorksheet sheet = book.Worksheets.Add("FormatoSucursal");
                sheet.Cell(1, 1).Value = "Actualiza";
                sheet.Cell(1, 2).Value = "Tipo Producto";
                sheet.Cell(1, 3).Value = "Referencia";
                sheet.Cell(1, 4).Value = "Descripción";
                sheet.Cell(1, 5).Value = "Codigo Externo";
                sheet.Cell(1, 6).Value = "Bajo Llave";
                sheet.Cell(1, 7).Value = "Fuera Dimensión";
                sheet.Cell(1, 8).Value = "Maneja Lote";
                sheet.Cell(1, 9).Value = "Maneja Serial";
                sheet.Cell(1, 10).Value = "Largo";
                sheet.Cell(1, 11).Value = "Ancho";
                sheet.Cell(1, 12).Value = "Alto";
                sheet.Cell(1, 13).Value = "Volumen";
                sheet.Cell(1, 14).Value = "Peso";
                sheet.Cell(1, 15).Value = "Activo";

                if (ListDownload == null || ListDownload.Count == 0)
                {
                    sheet.Cell(2, 1).Value = 0;
                    sheet.Cell(2, 2).Value = "Fabricado";
                    sheet.Cell(2, 3).Value = "010101";
                    sheet.Cell(2, 4).Value = "Producto Generico";
                    sheet.Cell(2, 5).Value = "123321";
                    sheet.Cell(2, 6).Value = 0;
                    sheet.Cell(2, 7).Value = 0;
                    sheet.Cell(2, 8).Value = 0;
                    sheet.Cell(2, 9).Value = 0;
                    sheet.Cell(2, 10).Value = 10;
                    sheet.Cell(2, 11).Value = 10;
                    sheet.Cell(2, 12).Value = 10;
                    sheet.Cell(2, 13).Value = 1000;
                    sheet.Cell(2, 14).Value = 10;
                    sheet.Cell(2, 15).Value = 1;
                }
                else
                {
                    int i= 2;
                    foreach (var item in ListDownload)
                    {
                        sheet.Cell(i, 1).Value = 0;
                        sheet.Cell(i, 2).Value = item.ProductType!.Name;
                        sheet.Cell(i, 3).Value = item.Reference;
                        sheet.Cell(i, 4).Value = item.Description;
                        sheet.Cell(i, 5).Value = item.ExternalCode;
                        sheet.Cell(i, 6).Value = item.IsKey;
                        sheet.Cell(i, 7).Value = item.Fdimen;
                        sheet.Cell(i, 8).Value = item.WithLot;
                        sheet.Cell(i, 9).Value = item.WithSerial;
                        sheet.Cell(i, 10).Value = item.Length;
                        sheet.Cell(i, 11).Value = item.Width;
                        sheet.Cell(i, 12).Value = item.Height;
                        sheet.Cell(i, 13).Value = item.Volume;
                        sheet.Cell(i, 14).Value = item.Weight;
                        sheet.Cell(i, 15).Value = item.Active == true ? 1 : 0;
                        i++;
                    }
                }
                
                using(var memory= new MemoryStream())
                {
                    book.SaveAs(memory);
                    await JSRuntime.InvokeAsync<object>(
                            "DownloadExcel",
                            $"{DateTime.Now.ToString("yyyyMMdd")}_Products.xlsx",
                            Convert.ToBase64String(memory.ToArray())
                    );
                }
            }
        }

        private async Task ShowModal(Product model)
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
            modalReference = Modal.Show<ProductsClock>(model.Reference.ToString(), parameters, mo);


            var result = await modalReference.Result;
            //if (result.Confirmed)
            //{
            //    await LoadAsync();
            //}
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
                ProductTypeId = ItemSelect.Id;
            }
            return;
        }
    }
}