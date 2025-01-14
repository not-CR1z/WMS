using Blazored.Modal;
using Blazored.Modal.Services;
using CurrieTechnologies.Razor.SweetAlert2;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using WMS.FrontEnd.Repositories;
using WMS.Share.DTOs;
using WMS.Share.Models.Location;

namespace WMS.FrontEnd.Shared
{
    public partial class SearchForm
    {
        [CascadingParameter] BlazoredModalInstance BlazoredModal { get; set; } = default!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Parameter,EditorRequired] public string Label { get; set; } =null!;
        [Parameter,EditorRequired] public string Url { get; set; } = null!;
        //[Parameter, EditorRequired] public string Href { get; set; } = null!;
        public string? Filter { get; set; }

        //public GenericSearchDTO? SelectItem { get; set; }
        public List<GenericSearchDTO>? MyList { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            await LoadListAsync();
        }

        private async  Task SelectAsync(GenericSearchDTO model)
        {
            if (BlazoredModal != null)
            {
                await BlazoredModal.CloseAsync(ModalResult.Ok(model));
            }
        }

        private async Task ClosedAsync()
        {
            if (BlazoredModal != null)
            {
                await BlazoredModal.CloseAsync(ModalResult.Cancel());
            }
        }

        private async Task CleanFilterAsync()
        {
            Filter = string.Empty;
            await ApplyFilterAsync();
        }

        private async Task ApplyFilterAsync()
        {
            await LoadListAsync();
        }

        private async Task LoadListAsync()
        {
            var url = Url;
            if (!string.IsNullOrEmpty(Filter))
            {
                if(url.Contains("?"))
                {
                    url += $"&filter={Filter}";
                }
                else
                {
                    url += $"?filter={Filter}";
                }               
            }

            var responseHttp = await Repository.GetAsync<List<GenericSearchDTO>>(url);
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
            }
            MyList = responseHttp.Response;
        }

    }
}