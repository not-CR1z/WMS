using Blazored.Modal.Services;
using Blazored.Modal;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using System.Net;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;
using WMS.Share.Models.Location;
using WMS.FrontEnd.Shared;
using WMS.Share.DTOs;

namespace WMS.FrontEnd.Pages.Location.SubWineries
{
    public partial class SubWineriesDeletes
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;

        [Parameter, SupplyParameterFromQuery] public string Page { get; set; } = string.Empty;
        [Parameter, SupplyParameterFromQuery] public int Filter { get; set; }

        public List<SubWinery>? MyList { get; set; }

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;
        public long? BranchId { get; set; }
        public string? NameBranch { get; set; }
        public string? DescriptionBranch { get; set; }

        public long? WineryId { get; set; }
        public string? NameWinery { get; set; }
        public string? DescriptionWinery { get; set; }

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
            var url = $"api/subwineries/getdeleteasync?page={page}";
            string FilterUrl = string.Empty;
            if (Filter != 0)
            {
                FilterUrl += $"&filter={Filter}";
            }
            if (BranchId != null && BranchId != 0)
            {
                FilterUrl += $"&filter2={BranchId}";
            }
            if (WineryId != null && WineryId != 0)
            {
                FilterUrl += $"&filter1={WineryId}";
            }
            if (!string.IsNullOrEmpty(FilterUrl))
            {
                url += FilterUrl;
            }

            var responseHttp = await Repository.GetAsync<List<SubWinery>>(url);
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
            var url = $"api/subwineries/deletetotalPages";
            string FilterUrl = string.Empty;
            if (Filter != 0)
            {
                FilterUrl += $"?filter={Filter}";
            }
            if (BranchId != null && BranchId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter2={BranchId}";
                }
                else
                {
                    FilterUrl += $"?filter2={BranchId}";
                }
            }
            if (WineryId != null && WineryId != 0)
            {
                if (!string.IsNullOrEmpty(FilterUrl))
                {
                    FilterUrl += $"&filter1={WineryId}";
                }
                else
                {
                    FilterUrl += $"?filter1={WineryId}";
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
            NameBranch = string.Empty;
            DescriptionBranch = string.Empty;
            WineryId = 0;
            NameWinery = string.Empty;
            DescriptionWinery = string.Empty;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            int page = 1;
            await LoadAsync(page);
            await SelectedPageAsync(page);
        }

        private async Task RestoreAsycn(SubWinery model)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estas seguro de restaurar: {model.Code}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.GetAsync<SubWinery>($"api/subwineries/restoreasync/{model.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/subwineries/deletes");
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
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro restaurado con éxito.");
        }

        private async Task DeleteAsycn(SubWinery model)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estas seguro de eliminar definitivamente: {model.Code}?",
                Icon = SweetAlertIcon.Question,
                ShowCancelButton = true,
            });
            var confirm = string.IsNullOrEmpty(result.Value);
            if (confirm)
            {
                return;
            }

            var responseHttp = await Repository.DeleteAsync<SubWinery>($"api/subwineries/deletefullasync/{model.Id}");
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/subwineries/deletes");
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

        private async Task ShowModal(SubWinery model)
        {
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Id", model.Id);
            ModalOptions mo = new ModalOptions
            {
                HideCloseButton = false,
                HideHeader = false,
                DisableBackgroundCancel = false,

            };
            modalReference = Modal.Show<SubWineriesClock>(model.Code.ToString(), parameters, mo);


            var result = await modalReference.Result;
            //if (result.Confirmed)
            //{
            //    await LoadAsync();
            //}
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
    }
}