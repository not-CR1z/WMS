using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using WMS.FrontEnd.Repositories;
using WMS.Share.DTOs;
using WMS.Share.Models.Security;

namespace WMS.FrontEnd.Pages.Security.FormUserType
{
    [Authorize]
    public partial class FormUserTypeForm
    {
        private EditContext editContext = null!;
        private bool loading;
        private List<UserType>? UserTypeList;

        [EditorRequired, Parameter]
        public WMS.Share.Models.Security.FormUserType Model { get; set; } = null!;

        [EditorRequired, Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        public string? imageUrl;

        public bool FormPostedSuccessfully { get; set; }
        protected override async Task OnParametersSetAsync()
        {
            editContext = new(Model);
            await LoadDocumentTypeUserAsync();
        }

        private async Task LoadDocumentTypeUserAsync()
        {
            loading = true;
            var responseHttp = await Repository.GetAsync<List<UserType>>("/api/UserType/full");
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }
        
            UserTypeList = responseHttp.Response;
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
    }
}