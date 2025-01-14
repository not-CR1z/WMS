using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Location;
using WMS.Share.Responses;
using WMS.Share.Models.Magister;

namespace WMS.FrontEnd.Pages.Magister.Products
{
    public partial class ProductsUpload
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        public List<Product>? MyList { get; set; }

        public bool loading { get; set; }

        protected override void OnInitialized()
        {
            loading = false;
            MyList = new List<Product>();
        }
        private async Task DeleteAsycn(Product model)
        {
            var result = await SweetAlertService.FireAsync(new SweetAlertOptions
            {
                Title = "Confirmación",
                Text = $"¿Estas seguro de eliminar : {model.Reference}?",
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
                var httpResponse = await Repository.PostAsync<List<Product>, ActionResponse<List<Product>>>("/api/products/uploadasync", MyList);
                loading = false;
                var response = (ActionResponse<List<Product>>)httpResponse.Response!;
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
                    NavigationManager.NavigateTo("/products");
                    return;

                }
                await SweetAlertService.FireAsync("Error", response.Message, SweetAlertIcon.Error);
                MyList = (List<Product>)response.Result!;
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
                    Product model = new Product();
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
                                    model.GenericSearchName1 = r.GetCell(i).ToString()!;
                                }
                                break;
                            case 2://C
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    model.Reference = r.GetCell(i).ToString()!;
                                break;
                            case 3://D
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    model.Description = r.GetCell(i).ToString()!;
                                break;
                            case 4://E
                                if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
                                    model.ExternalCode = r.GetCell(i).ToString()!;
                                break;
                            case 5://F
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.IsKey = Convert.ToBoolean(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna F Fila {i} {ex.Message}";
                                    }
                                break;
                            case 6://G
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.Fdimen = Convert.ToBoolean(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna G Fila {i} {ex.Message}";
                                    }
                                break;
                            case 7://H
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.WithLot = Convert.ToBoolean(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna H Fila {i} {ex.Message}";
                                    }
                                break;
                            case 8://I
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.WithSerial = Convert.ToBoolean(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna I Fila {i} {ex.Message}";
                                    }
                                break;
                            case 9://J
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.Length = Convert.ToDecimal(r.GetCell(i).ToString());
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
                                        model.Width = Convert.ToDecimal(r.GetCell(i).ToString());
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
                                        model.Height = Convert.ToDecimal(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna L Fila {i} {ex.Message}";
                                    }
                                break;
                            case 12://M Volumen se calcula solo
                                break;
                            case 13://N
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.Weight = Convert.ToDecimal(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna N Fila {i} {ex.Message}";
                                    }
                                break;
                            case 14://O
                                if (r.GetCell(i) != null)
                                    try
                                    {
                                        model.Active = Convert.ToBoolean(r.GetCell(i).ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        model.StrError = $"Columna O Fila {i} {ex.Message}";
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