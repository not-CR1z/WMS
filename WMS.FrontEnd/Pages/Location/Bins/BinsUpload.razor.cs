using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.FrontEnd.Pages.Location.Bins
{
    public partial class BinsUpload
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        public List<Bin>? MyList { get; set; }

        public bool loading { get; set; }

        protected override void OnInitialized()
        {
            loading = false;
            MyList = new List<Bin>();
        }
        private async Task DeleteAsycn(Bin model)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estas seguro de eliminar : {model.BinCode}?",
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
            if (MyList == null || MyList.Count == 0)
            {
                await SweetAlertService.FireAsync("Error", "Sin registros", SweetAlertIcon.Error);
                return;
            }
            loading = true;
            try
            {
                var httpResponse = await Repository.PostAsync<List<Bin>, ActionResponse<List<Bin>>>("/api/bins/uploadasync", MyList);
                loading = false;
                var response = (ActionResponse<List<Bin>>)httpResponse.Response!;
                if (response.WasSuccess)
                {
                    var toast = SweetAlertService.Mixin(new SweetAlertOptions
                    {
                        Toast = true,
                        Position = SweetAlertPosition.BottomEnd,
                        ShowConfirmButton = true,
                        Timer = 1000
                    });
                    await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Registros subidos con éxito.");
                    NavigationManager.NavigateTo("/bins");
                    return;

                }
                await SweetAlertService.FireAsync("Error", response.Message, SweetAlertIcon.Error);
                MyList = (List<Bin>)response.Result!;
            }
            catch (Exception ex)
            {
                await SweetAlertService.FireAsync("Error",$"{ex.Message} /si tiene inconvenientes descargar plantilla y revisar estructura", SweetAlertIcon.Error);
                loading= false;
                return;
            }
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
                    Bin model = new Bin();
                    model.Row = j;
                    for (var i = r.FirstCellNum; i < cc; i++)
                    {
                        switch (i)
                        {
                            case 0://A
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.Update = Convert.ToBoolean(Convert.ToInt32(r.GetCell(i).ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna A Fila {i} {ex.Message}";
                                    }
                                break;
                            case 1://B
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                {
                                    model.GenericSearchName3 = r.GetCell(i).ToString()!;
                                }
                                break;
                            case 2://C
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    model.GenericSearchName2 = r.GetCell(i).ToString()!;
                                break;
                            case 3://D
                                if (r.GetCell(i) != null)
                                    if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                        model.GenericSearchName1= r.GetCell(i).ToString()!;
                                break;
                            case 4://E
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    model.GenericSearchName4 = r.GetCell(i).ToString()!;
                                break;
                            case 5://F
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    model.BinCodeABC = r.GetCell(i).ToString()!;
                                break;
                            case 6://G
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    model.BinCode = r.GetCell(i).ToString()!;
                                break;
                            case 7://H
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    model.BinDescription = r.GetCell(i).ToString()!;
                                break;
                            case 8://I
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.HeightCM = Convert.ToInt32(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna D Fila {i} {ex.Message}";
                                    }
                                break;
                            case 9://J
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.WidthCM = Convert.ToInt32(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna J Fila {i} {ex.Message}";
                                    }
                                break;
                            case 10://K
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.DepthCM = Convert.ToInt32(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna K Fila {i} {ex.Message}";
                                    }
                                break;
                            case 11://L
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.WeightKG = Convert.ToInt32(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna L Fila {i} {ex.Message}";
                                    }
                                break;
                            case 12://M
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.PercentUsed = Convert.ToInt32(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna M Fila {i} {ex.Message}";
                                    }
                                break;
                            case 13://N
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.Active = Convert.ToBoolean(Convert.ToInt32(r.GetCell(i).ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna N Fila {i} {ex.Message}";
                                    }
                                break;

                        }
                    }
                    MyList.Add(model);
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