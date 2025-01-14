using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Share.Models.Location;
using WMS.Share.Responses;

namespace WMS.Share.Helpers
{
    public static class ValidateBinCode
    {
        public static ActionResponse<int> Validate(string BinCode)
        {
            int validateSubwinery = 0;
            int validateBinCode = 0;
            char validatechar = new();
            try
            {
                validateSubwinery = Convert.ToInt32(BinCode.Substring(0, 2));
                validateBinCode = Convert.ToInt32(BinCode.Substring(3, 6));
            }
            catch (Exception)
            {
                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "Codigo ubicación no conserva la estructura correcta"
                }; 
            }
            try
            {
                validatechar= Convert.ToChar(BinCode.Substring(2, 1));
                if(!Char.IsLetter(validatechar))
                {
                    return new ActionResponse<int>
                    {
                        WasSuccess = false,
                        Message = "Codigo ubicación no conserva la estructura correcta"
                    };
                }
            }
            catch (Exception)
            {

                return new ActionResponse<int>
                {
                    WasSuccess = false,
                    Message = "Codigo ubicación no conserva la estructura correcta"
                };
            }

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result=validateSubwinery
            };
        }
    }
}
