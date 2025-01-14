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

namespace WMS.FrontEnd.Pages.Location.Wineries
{
    public partial class WineriesForm
    {
        private EditContext editContext = null!;
        private bool loading;

        [EditorRequired, Parameter]
        public Winery Model { get; set; } = null!;

        [EditorRequired, Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        [CascadingParameter]
        IModalService Modal { get; set; } = default!;

        public bool FormPostedSuccessfully { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        //protected override void OnInitialized()
        //{
        //    editContext = new(Model);
        //}

        protected override async Task OnParametersSetAsync()
        {
            editContext = new(Model);
            if(Model.Branch!=null)
            {
                Name = Model.Branch!.Name;
                Description = " - "+Model.Branch!.Description;
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
                Name= ItemSelect.Name;
                Description = " - " + ItemSelect.Description;
                Model.BranchId = ItemSelect.Id;                
            }
            return;
        }

        private async Task SearchBranchName(ChangeEventArgs e)
        {
            if(e.Value!=null)
            {
                //buscar e.Value
            }
            return;
        }
    }
}