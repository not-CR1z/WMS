using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using System.Net;
using WMS.FrontEnd.Repositories;
using WMS.Share.DTOs;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Magister.UMs
{
    [Authorize]
    public partial class UMsClock
    {
        [EditorRequired, Parameter] public long Id { get; set; }
        private UM? model;
        private UserUpdateDTO? userUpdateDTO;
        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        private bool loading;
        protected override async Task OnParametersSetAsync()
        {
            loading = true;
            var responseHttp = await Repository.GetAsync<UM>($"/api/ums/{Id}");
            loading = false;
            if (responseHttp.Error)
            {
                if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
                {
                    NavigationManager.NavigateTo("/ums");
                }
                else
                {
                    var messsage = await responseHttp.GetErrorMessageAsync();
                    await SweetAlertService.FireAsync("Error", messsage, SweetAlertIcon.Error);
                }
            }
            else
            {
                model = responseHttp.Response;
                userUpdateDTO = new UserUpdateDTO
                {
                    CreateUserId = model!.CreateUserId,
                    CreateDate = model!.CreateDate,
                    UpdateUserId = model!.UpdateUserId,
                    UpdateDate = model!.UpdateDate,
                    ChangeStateUserId = model!.ChangeStateUserId,
                    ChangeStateDate = model!.ChangeStateDate,
                    DeleteUserId = model!.DeleteUserId,
                    DeleteDate = model!.DeleteDate,
                };
            }
        }
    }
}