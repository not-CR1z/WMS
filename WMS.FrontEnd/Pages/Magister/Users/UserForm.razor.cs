using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using WMS.FrontEnd.Helpers;
using WMS.FrontEnd.Repositories;
using WMS.FrontEnd.Services;
using WMS.FrontEnd.Shared;
using WMS.Share.DTOs;
using WMS.Share.Models;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;

namespace WMS.FrontEnd.Pages.Magister.Users
{
    [Authorize]
    public partial class UserForm
    {
        private EditContext editContext = null!;
        private bool loading;

        private List<DocumentTypeUser>? DocumentTypeUsersList;
        private List<MultipleSelectorModel> selected { get; set; } = new();
        private List<MultipleSelectorModel> nonSelected { get; set; } = new();

        [EditorRequired, Parameter]
        public UserDTO Model { get; set; } = null!;

        [EditorRequired, Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [EditorRequired, Parameter]
        public EventCallback ReturnAction { get; set; }

        [Parameter]
        public List<UserType> SelectedUserTypes { get; set; } = new();

        [Parameter, EditorRequired]
        public List<UserType> NonSelectedUserTypes { get; set; } = new();

        [Parameter]
        public bool IsCreate { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;

        public string? imageUrl;

        public bool FormPostedSuccessfully { get; set; }

        private void ImageSelected(string imagenBase64)
        {
            Model.Photo = imagenBase64;
            imageUrl = null;
        }
        protected async override Task OnParametersSetAsync()
        {
            await LoadDocumentTypeUserAsync();
            selected = SelectedUserTypes.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
            nonSelected = NonSelectedUserTypes.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
            editContext = new(Model);
            if (!string.IsNullOrEmpty(Model!.Photo))
            {
                imageUrl = Model.Photo;
                Model.Photo = null;
            }
        }
        private async Task LoadDocumentTypeUserAsync()
        {
            loading = true;
            var responseHttp = await Repository.GetAsync<List<DocumentTypeUser>>("/api/DocumentTypeUsers/full");
            loading = false;
            if (responseHttp.Error)
            {
                var message = await responseHttp.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            DocumentTypeUsersList = responseHttp.Response;
        }

        //private async Task CreateAsync()
        //{
        //    Model.UserName = Model.Email;
        //    loading = true;
        //    var responseHttp = await Repository.PostAsync<UserDTO, TokenDTO>("/api/users/CreateUser", Model);
        //    loading = false;

        //    if (responseHttp.Error)
        //    {
        //        var message = await responseHttp.GetErrorMessageAsync();
        //        await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
        //        return;
        //    }
        //    //await LoginService.LoginAsync(responseHttp.Response!.Token);
        //    NavigationManager.NavigateTo("/users");
        //}
        private async Task OnDataAnnotationsValidatedAsync()
        {
            Model.UserTyepIds = selected.Select(x => long.Parse(x.Key)).ToList();
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