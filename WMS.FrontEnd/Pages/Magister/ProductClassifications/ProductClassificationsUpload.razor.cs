using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.ComponentModel.DataAnnotations;
using System.Net;
using WMS.FrontEnd.Repositories;
using WMS.Share.Models.Magister;
using WMS.Share.Responses;

namespace WMS.FrontEnd.Pages.Magister.ProductClassifications
{
    public partial class ProductClassificationsUpload
    {
        private int currentPage = 1;
        private int totalPages;

        [Inject] private IRepository Repository { get; set; } = null!;
        [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
        [Inject] private NavigationManager NavigationManager { get; set; } = null!;
        public List<ProductClassification>? MyList { get; set; }

        public bool loading { get; set; }

        protected override void OnInitialized()
        {
            loading = false;
            MyList = new List<ProductClassification>();
        }
        private async Task DeleteAsycn(ProductClassification model)
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
            var httpResponse = await Repository.PostAsync<List<ProductClassification>,ActionResponse<List<ProductClassification>>>("/api/productclassifications/uploadasync", MyList);
            loading = false;

            var response = (ActionResponse<List<ProductClassification>>)httpResponse.Response!;
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
                NavigationManager.NavigateTo("/productclassifications");
                return;
                
            }
            await SweetAlertService.FireAsync("Error", response.Message, SweetAlertIcon.Error);
            MyList = (List<ProductClassification>)response.Result!;            
        }

        //private async Task OnChange(InputFileChangeEventArgs e)
        //{
        //    loading = true;
        //    var fileStream = e.File.OpenReadStream();
        //    if (fileStream != null)
        //    {
        //        var ms = new MemoryStream();
        //        await fileStream.CopyToAsync(ms);
        //        fileStream.Close();
        //        ms.Position = 0;

        //        ISheet sheet;
        //        var xsswb = new XSSFWorkbook(ms);

        //        sheet = xsswb.GetSheetAt(0);
        //        IRow hr = sheet.GetRow(0);
        //        var rl = new List<string>();
        //        int cc = hr.LastCellNum;
        //        MyList = [];
        //        for (var j = (sheet.FirstRowNum + 1); j <= sheet.LastRowNum; j++)
        //        {
        //            var r = sheet.GetRow(j);
        //            ProductClassification binType = new ProductClassification();
        //            binType.Row = j;
        //            for (var i = r.FirstCellNum; i < cc; i++)
        //            {
                        
        //                switch (i)
        //                {
        //                    case 0:
        //                        if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
        //                            binType.Name = r.GetCell(i).ToString()!;
        //                        break;
        //                    case 1:
        //                        if (!String.IsNullOrEmpty(r.GetCell(i).ToString()))
        //                            binType.Description = r.GetCell(i).ToString()!;
        //                        break;
        //                    case 2:
        //                        if (r.GetCell(i) != null)
        //                            try
        //                            {
        //                                binType.Picking = Convert.ToBoolean(Convert.ToInt32(r.GetCell(i).ToString()));
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                binType.StrError = $"Columna C Fila {i} {ex.Message}";
        //                            }
        //                        break;
        //                    case 3:
        //                        if (r.GetCell(i) != null)
        //                            try
        //                            {
        //                                binType.OrderPicking = Convert.ToInt32(r.GetCell(i).NumericCellValue);
        //                            }
        //                            catch (Exception ex)
        //                            {
        //                                binType.StrError = $"Columna D Fila {i} {ex.Message}";
        //                            }
        //                        break;
        //                }
        //            }
        //            MyList.Add(binType);
        //            rl.Clear();
        //        }
        //        loading = false;
        //        var toast = SweetAlertService.Mixin(new SweetAlertOptions
        //        {
        //            Toast = true,
        //            Position = SweetAlertPosition.BottomEnd,
        //            ShowConfirmButton = true,
        //            Timer = 1000
        //        });
        //        await toast.FireAsync(icon: SweetAlertIcon.Success, message: "Archivo cargado con éxito.");
        //    }
        //}

    }
}