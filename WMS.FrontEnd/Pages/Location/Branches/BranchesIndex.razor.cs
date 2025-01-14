using Blazored.Modal;
using Blazored.Modal.Services;
using ClosedXML.Excel;
using CurrieTechnologies.Razor.SweetAlert2;
using DocumentFormat.OpenXml.Spreadsheet;
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
using WMS.Share.DTOs;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;
using static System.Net.Mime.MediaTypeNames;

namespace WMS.FrontEnd.Pages.Location.Branches
{
    public partial class BranchesIndex
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IJSRuntime JSRuntime { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

        public List<Branch>? MyList { get; set; }

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
            var url = $"api/branches/getasync?page={page}";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"&filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<Branch>>(url);
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
            var url = $"api/branches/totalPages";
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

        private async Task DeleteAsycn(Branch model)
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

            var responseHttp = await Repository.DeleteAsync<Branch>($"api/branches/deleteasync/{model.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/branches");
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
            var url = $"api/branches/downloadasync";
            if (!string.IsNullOrEmpty(Filter))
            {
                url += $"?filter={Filter}";
            }

            var responseHttp = await Repository.GetAsync<List<Branch>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
            var ListDownload = responseHttp.Response;

            using (var book=new XLWorkbook())
            {
                IXLWorksheet sheet = book.Worksheets.Add("FormatoSucursal");
                sheet.Cell(1, 1).Value = "Actualiza";
                sheet.Cell(1, 2).Value = "Nombre";
                sheet.Cell(1, 3).Value = "Descripción";
                sheet.Cell(1, 4).Value = "Contacto";
                sheet.Cell(1, 5).Value = "Telefono Contacto";
                sheet.Cell(1, 6).Value = "Email Contacto";
                sheet.Cell(1, 7).Value = "Contingencia";
                sheet.Cell(1, 8).Value = "Correo Envio de Notificaciones";
                sheet.Cell(1, 9).Value = "Clave Para Envio de Notificaciones";
                sheet.Cell(1, 10).Value = "Host";
                sheet.Cell(1, 11).Value = "Puerto";
                sheet.Cell(1, 12).Value = "SSL";
                if (ListDownload == null || ListDownload.Count == 0)
                {
                    sheet.Cell(2, 1).Value = 0;
                    sheet.Cell(2, 2).Value = "Sucursal1";
                    sheet.Cell(2, 3).Value = "Sucursal de almacenamiento masivo";
                    sheet.Cell(2, 4).Value = "Hernan Gallego";
                    sheet.Cell(2, 5).Value = "0579999999";
                    sheet.Cell(2, 6).Value = "correo@gmail.com";
                    sheet.Cell(2, 7).Value = 0;
                    sheet.Cell(2, 8).Value = "sendnotificaciones@gmail.com";
                    sheet.Cell(2, 9).Value = "123456789**";
                    sheet.Cell(2, 10).Value = "smtp.gmail.com";
                    sheet.Cell(2, 11).Value = 465;
                    sheet.Cell(2, 12).Value = 1;
                }
                else
                {
                    int i= 2;
                    foreach (var item in ListDownload)
                    {
                        sheet.Cell(i, 1).Value = 0;
                        sheet.Cell(i, 2).Value = item.Name;
                        sheet.Cell(i, 3).Value = item.Description;
                        sheet.Cell(i, 4).Value = item.Contact;
                        sheet.Cell(i, 5).Value = item.PhoneContact;
                        sheet.Cell(i, 6).Value = item.EmailContact;
                        sheet.Cell(i, 7).Value = item.Contingency==true?1:0;
                        sheet.Cell(i, 8).Value = item.EmailFromNotification;
                        sheet.Cell(i, 9).Value = item.EmailFromNotificationPassword;
                        sheet.Cell(i, 10).Value = item.EmailFromHost;
                        sheet.Cell(i, 11).Value = item.EmailFromPort;
                        sheet.Cell(i, 12).Value = item.EmailFromSsl == true ? 1 : 0;
                        i++;
                    }
                }
                
                using(var memory= new MemoryStream())
                {
                    book.SaveAs(memory);
                    await JSRuntime.InvokeAsync<object>(
                            "DownloadExcel",
                            $"{DateTime.Now.ToString("yyyyMMdd")}_Sucursales.xlsx",
                            Convert.ToBase64String(memory.ToArray())
                    );
                }
            }
        }

        private async Task ShowModal(Branch model)
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
            modalReference = Modal.Show<BranchesClock>(model.Name, parameters, mo);


            var result = await modalReference.Result;
            //if (result.Confirmed)
            //{
            //    await LoadAsync();
            //}
        }

    }
}