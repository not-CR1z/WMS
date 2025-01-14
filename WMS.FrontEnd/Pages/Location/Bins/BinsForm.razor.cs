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

namespace WMS.FrontEnd.Pages.Location.Bins
{
    public partial class BinsForm
    {
        private EditContext editContext = null!;
        private bool loading;

        [EditorRequired, Parameter]
        public Bin Model { get; set; } = null!;

        [EditorRequired, Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;

        public bool FormPostedSuccessfully { get; set; }

        public string? NameBinType { get; set; }

        public long? BranchId { get; set; }
        public string? NameBranch { get; set; }

        public string? DescriptionBranch { get; set; }

        public long? WineryId { get; set; }
        public string? NameWinery { get; set; }

        public string? DescriptionWinery { get; set; }

        public int? CodeSubWinery { get; set; }

        public string? DescriptionSubWinery { get; set; }

        //protected override void OnInitialized()
        //{
        //    editContext = new(Model);
        //}

        protected override async Task OnParametersSetAsync()
        {
            editContext = new(Model);
            if (Model.SubWinery != null)
            {
                CodeSubWinery = Model.SubWinery!.Code;
                WineryId = Model.SubWinery!.Winery!.Id;
                NameWinery = Model.SubWinery!.Winery!.Name;
                BranchId = Model.SubWinery!.Winery!.BranchId;
                NameBranch = Model.SubWinery!.Winery!.Branch!.Name;
            }
            if(Model.BinType!= null)
            {
                NameBinType = Model.BinType.Name;
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
                Model.BinTypeId = ItemSelect.Id;
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
            modalReference = Modal.Show<SearchForm>(string.Empty, parameters,modalOptions);
  


            var result = await modalReference.Result;
            if (result.Confirmed)
            {
                var ItemSelect = (GenericSearchDTO)result.Data!;
                NameBranch= ItemSelect.Name;
                DescriptionBranch = " - " + ItemSelect.Description;
                BranchId = ItemSelect.Id;
                WineryId = 0;
                NameWinery=string.Empty;
                DescriptionWinery = string.Empty;
                Model.SubWineryId = 0;
                CodeSubWinery = 0;
                DescriptionSubWinery = string.Empty;                
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
                Model.SubWineryId = 0;
                CodeSubWinery = 0;
                DescriptionSubWinery = string.Empty;
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
                Model.SubWineryId = ItemSelect.Id;
            }
            return;
        }
    }
}