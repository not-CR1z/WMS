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

namespace WMS.FrontEnd.Pages.Location.Bins
{
    public partial class BinsIndex
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public int Filter { get; set; }

        public List<Bin>? MyList { get; set; }

        public long? BinTypeId { get; set; }
        public string? NameBinType { get; set; }

        public long? BranchId { get; set; }
        public string? NameBranch { get; set; }
        public string? DescriptionBranch { get; set; }

        public long? WineryId { get; set; }
        public string? NameWinery { get; set; }
        public string? DescriptionWinery { get; set; }

        public long? SubWineryId { get; set; }
        public int? CodeSubWinery { get; set; }
        public string? DescriptionSubWinery { get; set; }

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
            var url = $"api/bins/getasync?page={page}";
            string FilterUrl = string.Empty;
            if (BinTypeId!=null && BinTypeId != 0)
            {
                FilterUrl += $"&filter4={BinTypeId}";
            }
            if (BranchId != null && BranchId != 0)
            {
                FilterUrl += $"&filter3={BranchId}";
            }
            if (WineryId != null && WineryId != 0)
            {
                FilterUrl += $"&filter2={WineryId}";
            }
            if (SubWineryId != null && SubWineryId != 0)
            {
                FilterUrl += $"&filter1={SubWineryId}";
            }
            if (Filter != 0)
            {
                FilterUrl += $"&filter={Filter}";
            }
            if (!string.IsNullOrEmpty(FilterUrl))
            {
                url += FilterUrl;
            }
            var responseHttp = await Repository.GetAsync<List<Bin>>(url);
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
            var url = $"api/bins/totalPages";
            string FilterUrl=string.Empty ;
            if (Filter!=0)
            {
                FilterUrl += $"?filter={Filter}";
            }
            if (BinTypeId != null && BinTypeId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter4={BinTypeId}";
                }
                else
                {
                    FilterUrl += $"?filter4={BinTypeId}";
                }
            }
            if (BranchId!=null && BranchId!=0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter3={BranchId}";
                }
                else
                {
                    FilterUrl += $"?filter3={BranchId}";
                }                
            }
            if (WineryId!=null && WineryId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter2={WineryId}";
                }
                else
                {
                    FilterUrl += $"?filter2={WineryId}";
                }
            }
            if (SubWineryId!=null && SubWineryId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter1={SubWineryId}";
                }
                else
                {
                    FilterUrl += $"?filter1={SubWineryId}";
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
            Filter = 0;
            BranchId = 0;
            NameBranch=string.Empty ;
            DescriptionBranch=string.Empty ;
            WineryId = 0;
            NameWinery=string.Empty ;
            DescriptionWinery=string.Empty ;
            SubWineryId = 0;
            CodeSubWinery = 0;
            DescriptionSubWinery = string.Empty;
            BinTypeId = 0;
            NameBinType = string.Empty;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }

        private async Task DeleteAsycn(Bin model)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estas seguro de eliminar : {model.BinCode}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<Bin>($"api/bins/deleteasync/{model.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/bins");
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
            var url = $"api/bins/downloadasync";
            string FilterUrl = string.Empty;
            if (BinTypeId != null && BinTypeId != 0)
            {
                FilterUrl += $"&filter4={BinTypeId}";
            }
            if (Filter != 0)
            {
                FilterUrl += $"?filter={Filter}";
            }
            if (BranchId != null && BranchId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter3={BranchId}";
                }
                else
                {
                    FilterUrl += $"?filter3={BranchId}";
                }
            }
            if (WineryId != null && WineryId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter2={WineryId}";
                }
                else
                {
                    FilterUrl += $"?filter2={WineryId}";
                }
            }
            if (SubWineryId != null && SubWineryId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter1={SubWineryId}";
                }
                else
                {
                    FilterUrl += $"?filter1={SubWineryId}";
                }
            }
            if (!string.IsNullOrEmpty(FilterUrl))
            {
                url += FilterUrl;
            }

            var responseHttp = await Repository.GetAsync<List<Bin>>(url);
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
                sheet.Cell(1, 2).Value = "Nombre Sucursal";
                sheet.Cell(1, 3).Value = "Nombre Bodega";
                sheet.Cell(1, 4).Value = "Codigo Sub-Bodega";
                sheet.Cell(1, 5).Value = "Tipo Ubicación";
                sheet.Cell(1, 6).Value = "Codigo ABC";
                sheet.Cell(1, 7).Value = "Codigo Ubicación";
                sheet.Cell(1, 8).Value = "Descripción Ubicación";
                sheet.Cell(1, 9).Value = "Largo Centimetros";
                sheet.Cell(1, 10).Value = "Ancho Centimetros";
                sheet.Cell(1, 11).Value = "Profundida Centimetros";
                sheet.Cell(1, 12).Value = "Peso Kilogramos";
                sheet.Cell(1, 13).Value = "Porcentaje USO";
                sheet.Cell(1, 14).Value = "Activa";

                if (ListDownload == null || ListDownload.Count == 0)
                {
                    sheet.Cell(2, 1).Value = 0;
                    sheet.Cell(2, 2).Value = "Sucursal";
                    sheet.Cell(2, 3).Value = "Bodega Principal";
                    sheet.Cell(2, 4).Value = "4";
                    sheet.Cell(2, 5).Value = "Picking";
                    sheet.Cell(2, 6).Value = "A";
                    sheet.Cell(2, 7).Value = "01A000101";
                    sheet.Cell(2, 8).Value = "Ubicacion que almacena multiples productos";
                    sheet.Cell(2, 9).Value = 150;
                    sheet.Cell(2, 10).Value = 150;
                    sheet.Cell(2, 11).Value = 50;
                    sheet.Cell(2, 12).Value = 200;
                    sheet.Cell(2, 13).Value = 0;
                    sheet.Cell(2, 14).Value = 1;
                }
                else
                {
                    int i= 2;
                    foreach (var item in ListDownload)
                    {
                        sheet.Cell(i, 1).Value = 0;
                        sheet.Cell(i, 2).Value = item.SubWinery!.Winery!.Branch!.Name;
                        sheet.Cell(i, 3).Value = item.SubWinery!.Winery.Name;
                        sheet.Cell(i, 4).Value = item.SubWinery!.Code;
                        sheet.Cell(i, 5).Value = item.BinType!.Name;
                        sheet.Cell(i, 6).Value = item.BinCodeABC;
                        sheet.Cell(i, 7).Value = item.BinCode;
                        sheet.Cell(i, 8).Value = item.BinDescription;
                        sheet.Cell(i, 9).Value = item.HeightCM;
                        sheet.Cell(i, 10).Value = item.WidthCM;
                        sheet.Cell(i, 11).Value = item.DepthCM;
                        sheet.Cell(i, 12).Value = item.WeightKG;
                        sheet.Cell(i, 13).Value = item.PercentUsed;
                        sheet.Cell(i, 14).Value = item.Active == true ? 1 : 0;
                        i++;
                    }
                }
                
                using(var memory= new MemoryStream())
                {
                    book.SaveAs(memory);
                    await JSRuntime.InvokeAsync<object>(
                            "DownloadExcel",
                            $"{DateTime.Now.ToString("yyyyMMdd")}_Ubicaciones.xlsx",
                            Convert.ToBase64String(memory.ToArray())
                    );
                }
            }
        }

        private async Task ShowModal(Bin model)
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
            modalReference = Modal.Show<BinsClock>(model.BinCode.ToString(), parameters, mo);


            var result = await modalReference.Result;
            //if (result.Confirmed)
            //{
            //    await LoadAsync();
            //}
        }
        private async Task SearchBinType()
        {
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Label", "Típo Ubicación");
            parameters.Add("Url", "api/bintypes/genericsearch");

            var modalOptions = new ModalOptions();
            modalOptions.HideCloseButton = false;
            modalOptions.DisableBackgroundCancel = false;
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters, modalOptions);

            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                NameBinType = ItemSelect.Name;
                BinTypeId = ItemSelect.Id;
            }
            return;
        }
        private async Task SearchBranch()
        {
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Label", "Sucursal");
            parameters.Add("Url", "api/branches/genericsearch");

            var modalOptions = new ModalOptions();
            modalOptions.HideCloseButton = false;
            modalOptions.DisableBackgroundCancel = false;
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters, modalOptions);

            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                NameBranch = ItemSelect.Name;
                DescriptionBranch = " - " + ItemSelect.Description;
                BranchId = ItemSelect.Id;
                WineryId = 0;
                NameWinery = string.Empty;
                DescriptionWinery = string.Empty;
            }
            return;
        }

        private async Task SearchWinery()
        {
            if (BranchId == null || BranchId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Sucursal", SweetAlertIcon.Warning);
                return;
            }
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Label", "Bodega");
            parameters.Add("Url", $"api/wineries/genericsearch?Filter1={BranchId}");

            var modalOptions = new ModalOptions();
            modalOptions.HideCloseButton = false;
            modalOptions.DisableBackgroundCancel = false;
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters, modalOptions);



            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                NameWinery = ItemSelect.Name;
                DescriptionWinery = " - " + ItemSelect.Description;
                WineryId = ItemSelect.Id;
            }
            return;
        }

        private async Task SearchSubWinery()
        {
            if (BranchId == null || BranchId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Sucursal", SweetAlertIcon.Warning);
                return;
            }
            if (WineryId == null || WineryId == 0)
            {
                await SweetAlertService.FireAsync("Advertencia", "Debe Seleccionar Bodega", SweetAlertIcon.Warning);
                return;
            }
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Label", "Sub-Bodega");
            parameters.Add("Url", $"api/subwineries/genericsearch?Filter2={BranchId}&Filter1={WineryId}");

            var modalOptions = new ModalOptions();
            modalOptions.HideCloseButton = false;
            modalOptions.DisableBackgroundCancel = false;
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters, modalOptions);



            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                CodeSubWinery = Convert.ToInt32(ItemSelect.Name);
                DescriptionSubWinery = " - " + ItemSelect.Description;
                SubWineryId = ItemSelect.Id;
            }
            return;
        }

    }
}