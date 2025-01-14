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

namespace WMS.FrontEnd.Pages.Location.SubWineries
{
    public partial class SubWineriesForm
    {
        private EditContext editContext = null!;
        private bool loading;

        [EditorRequired, Parameter]
        public SubWinery Model { get; set; } = null!;

        [EditorRequired, Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;

        public bool FormPostedSuccessfully { get; set; }



        public long? BranchId { get; set; }
        public string? NameBranch { get; set; }

        public string? DescriptionBranch { get; set; }

        public string? NameWinery { get; set; }

        public string? DescriptionWinery { get; set; }

        //protected override void OnInitialized()
        //{
        //    editContext = new(Model);
        //}

        protected override async Task OnParametersSetAsync()
        {
            editContext = new(Model);
            if(Model.Winery!=null)
            {
                NameWinery = Model.Winery!.Name;
                BranchId= Model.Winery!.BranchId;
                NameBranch = Model.Winery!.Branch!.Name;
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

        private async Task SearchBranch()
        {
            IModalReference modalReference;
            var parameters = new ModalParameters();
            parameters.Add("Label", "Sucursal");
            parameters.Add("Url", "api/branches/genericsearch");

            var modalOptions = new ModalOptions();
            modalOptions.HideCloseButton = false;
            modalOptions.DisableBackgroundCancel = false;
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters,modalOptions);
  


            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                NameBranch= ItemSelect.Name;
                DescriptionBranch = " - " + ItemSelect.Description;
                BranchId = ItemSelect.Id;
                Model.WineryId = 0;
                NameWinery=string.Empty;
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
                Model.WineryId = ItemSelect.Id;
            }
            return;
        }
    }
}