using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using System.Linq;
using System.Net;
using WMS.FrontEnd.Helpers;
using WMS.FrontEnd.Repositories;
using WMS.FrontEnd.Services;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;
using WMS.Share.Models.Security;

namespace WMS.FrontEnd.Pages.Magister.Users
{
    [Authorize]
    public partial class UserEdit
    {
        [EditorRequired, Parameter]
        public long Id_Local { get; set; }
        private EditContext editContext = null!;
        private User Model = new();
        private bool loading;
        private List<DocumentTypeUser>? DocumentTypeUsersList;
        private List<MultipleSelectorModel> selected { get; set; } = new();
        private List<MultipleSelectorModel> nonSelected { get; set; } = new();
        public List<UserType> SelectedUserTypes { get; set; } = new();
        public List<UserType> NonSelectedUserTypes { get; set; } = new();

        public EventCallback OnValidSubmit { get; set; }

        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private ILoginService LoginService { get; set; } = null!;

        public string? imageUrl;
        public bool FormPostedSuccessfully { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            loading = true;
            //User user = new();
            var responseHttpuser = await Repository.GetAsync<User>($"/api/users/GetIdLocalAsync/{Id_Local}");
            if (responseHttpuser.Error)
            {
                if (responseHttpuser.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/users");
                }
                else
                {
                    var messsage = await responseHttpuser.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", messsage, SweetAlertIcon.Error);
                }
            }
            else
            {
                Model = responseHttpuser.Response!;
            }

            var httpResponse2 = await Repository.GetAsync<List<DocumentTypeUser>>("/api/DocumentTypeUsers/full");
            if (httpResponse2.Error)
            {
                var message = await httpResponse2.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            DocumentTypeUsersList = httpResponse2.Response!;


            List<UserType> UserTypeList = [];
            var responseHttpusertype = await Repository.GetAsync<List<UserType>>("/api/usertype");
            if (responseHttpusertype.Error)
            {
                if (responseHttpusertype.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    await SweetAlertService.FireAsync("Error", "No se encuentra API002", SweetAlertIcon.Error);
                }
                else
                {
                    var messsage = await responseHttpusertype.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", messsage, SweetAlertIcon.Error);
                }
                return;
            }
            else
            {
                UserTypeList = responseHttpusertype.Response!;
            }

            List<UserType> RolesList = [];
            var responseHttproles = await Repository.GetAsync<List<UserType>>($"/api/users/GetRoleUserAsync/{Model.Id_Local}");
            if (responseHttproles.Error)
            {
                if (responseHttproles.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    await SweetAlertService.FireAsync("Error", "No se encuentra API001", SweetAlertIcon.Error);
                }
                else
                {
                    var messsage = await responseHttproles.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", messsage, SweetAlertIcon.Error);
                }
                return;
            }
            else
            {
                RolesList = responseHttproles.Response!;
            }
            Model.UserTyepIds =
                (from item in RolesList
                 select item.Id).ToList();
            SelectedUserTypes = RolesList;
            var nonoselect = UserTypeList.ExceptBy(RolesList.Select(x => x.Id), x => x.Id).ToList();
            NonSelectedUserTypes = nonoselect;

            selected = SelectedUserTypes.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
            nonSelected = NonSelectedUserTypes.Select(x => new MultipleSelectorModel(x.Id.ToString(), x.Name)).ToList();
            editContext = new(Model);

            if (!string.IsNullOrEmpty(Model!.Photo))
            {
                imageUrl = Model.Photo;
                Model.Photo = null;
            }

            loading = false;
        }

        private async Task SaveChangesAsync()
        {
            Model.UserTyepIds = selected.Select(x => long.Parse(x.Key)).ToList();
            var httpResponse = await Repository.PutAsync("/api/Users", Model);
            if (httpResponse.Error)
            {
                var message = await httpResponse.GetErrorMessageAsync();
                await SweetAlertService.FireAsync("Error", message, SweetAlertIcon.Error);
                return;
            }

            Return();
        }

        private void ImageSelected(string imagenBase64)
        {
            Model.Photo = imagenBase64;
            imageUrl = null;
        }

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

        private UserDTO TouserDTO(User user)
        {
            return new UserDTO
            {
                Id = user.Id,
                Document = user.Document,
                Address = user.Address,
                DocumentTypeUserId = user.DocumentTypeUserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id_Local = user.Id_Local,
                Password = string.Empty,
                PasswordConfirm = string.Empty,
                PhoneNumber = user.PhoneNumber,
                Photo = user.Photo,
                UserName = user.UserName,
            };
        }

        private void Return()
        {
            FormPostedSuccessfully = true;
            NavigationManager.NavigateTo("/users");
        }
    }
}