using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Location;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.FrontEnd.Pages.Location.Branches
{
    public partial class BranchesUpload
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        public List<Branch>? MyList { get; set; }

        public bool loading { get; set; }

        protected override void OnInitialized()
        {
            loading = false;
            MyList = new List<Branch>();
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
            MyList!.Remove(model);

            var toast = SweetAlertService.Mixin(new SweetAlertOptions
            {
                Toast = true,
                Position = SweetAlertPosition.BottomEnd,
                ShowConfirmButton = true,
                Timer = 3000
            });
            await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registro eliminado con éxito.");
        }

        private async Task UploadAsycn()
        {
            if(MyList == null || MyList.Count==0)
            {
                await SweetAlertService.FireAsync("Error", "Sin registros", SweetAlertIcon.Error);
                return;
            }
            loading = true;
            var httpResponse = await Repository.PostAsync<List<Branch>,ActionResponse<List<Branch>>>("/api/branches/uploadasync", MyList);
            loading = false;

            var response = (ActionResponse<List<Branch>>)httpResponse.Response!;
            if(response.WasSuccess)
            {
                var toast = SweetAlertService.Mixin(new SweetAlertOptions
                {
                    Toast = true,
                    Position = SweetAlertPosition.BottomEnd,
                    ShowConfirmButton = true,
                    Timer = 1000
                });
                await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registros subidos con éxito.");
                NavigationManager.NavigateTo("/branches");
                return;
                
            }
            await SweetAlertService.FireAsync("Error", response.Message, SweetAlertIcon.Error);
            MyList = (List<Branch>)response.Result!;            
        }

        private async Task OnChange(InputFileChangeEventArgs e)
        {
            loading = true;
            var fileStream = e.File.OpenReadStream();
            if (fileStream != null)
            {
                var ms = new MemoryStream();
                await fileStream.CopyToAsync(ms);
                fileStream.Close();
                ms.Position = 0;

                ISheet sheet;
                var xsswb = new XSSFWorkbook(ms);

                sheet = xsswb.GetSheetAt(0);
                IRow hr = sheet.GetRow(0);
                var rl = new List<string>();
                int cc = hr.LastCellNum;
                MyList = [];
                for (var j = (sheet.FirstRowNum + 1); j <= sheet.LastRowNum; j++)
                {
                    var r = sheet.GetRow(j);
                    Branch branch = new Branch();
                    branch.Row = j;
                    for (var i = r.FirstCellNum; i < cc; i++)
                    {                        
                        switch (i)
                        {
                            case 0://A
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        branch.Update = Convert.ToBoolean(Convert.ToInt32(r.GetCell(i).ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        branch.StrError = $"Columna A Fila {i} {ex.Message}";
                                    }
                                break;
                            case 1://B
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    branch.Name = r.GetCell(i).ToString()!;
                                break;
                            case 2://C
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    branch.Description = r.GetCell(i).ToString()!;
                                break;
                            case 3://D
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    branch.Contact = r.GetCell(i).ToString()!;
                                break;
                            case 4://E
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    branch.PhoneContact = r.GetCell(i).ToString()!;
                                break;
                            case 5://F
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    branch.EmailContact = r.GetCell(i).ToString()!;
                                break;
                            case 6://G
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        branch.Contingency = Convert.ToBoolean(Convert.ToInt32(r.GetCell(i).ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        branch.StrError = $"Columna G Fila {i} {ex.Message}";
                                    }
                                break;
                            case 7://H
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    branch.EmailFromNotification = r.GetCell(i).ToString()!;
                                break;
                            case 8://I
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    branch.EmailFromNotificationPassword = r.GetCell(i).ToString()!;
                                break;
                            case 9://J
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    branch.EmailFromHost = r.GetCell(i).ToString()!;
                                break;
                            case 10://K
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        branch.EmailFromPort = Convert.ToInt32(r.GetCell(i).NumericCellValue);
                                    }
                                    catch (Exception ex)
                                    {
                                        branch.StrError = $"Columna K Fila {i} {ex.Message}";
                                    }
                                break;
                            case 11://L
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        branch.EmailFromSsl = Convert.ToBoolean(Convert.ToInt32(r.GetCell(i).ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        branch.StrError = $"Columna L Fila {i} {ex.Message}";
                                    }
                                break;
                        }
                    }
                    MyList.Add(branch);
                    rl.Clear();
                }
                loading = false;
                var toast = SweetAlertService.Mixin(new SweetAlertOptions
                {
                    Toast = true,
                    Position = SweetAlertPosition.BottomEnd,
                    ShowConfirmButton = true,
                    Timer = 1000
                });
                await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Archivo cargado con éxito.");
            }
        }

    }
}