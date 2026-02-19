using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers
{
    public static class MaterialHelpers
    {
        public static DxMaterial CreateNewMaterial(string materialDescription, string categoryType, out bool result)
        {
            result = true;

            DxMaterialType materialType = new DxMaterialType(Constants.MaterialType.DIR_RESOURCE);
            string matCode = DxMaterial.GetAutoNumberingKey(materialType.Code, DxPlant.NONE, DxUser.CurrentUser);

            DxMaterial material = new DxMaterial(DxPlant.NONE, matCode)
            {
                Description = materialDescription,
                MaterialType = materialType,
                CreatedBy = DxUser.CurrentUser.LoginName,
                CreateDate = DateTime.Now,
                Status = "00",
                Flags = decimal.Zero
            };

            result &= material.Create();

            DxAttribute attribute = material.GetOrLoadAttribute("DXDIR_RESOURCE_TYPE");
            DxAttributeValue newValue = attribute.NewValue();
            newValue.Data = categoryType;

            attribute.AddValue(newValue);
            result &= attribute.Create();

            if (DxUser.CurrentUser.IsExternal)
            {
                DxPartnerMaterial partnerMaterial = new DxPartnerMaterial(DxPlant.NONE, matCode, DxUser.CurrentUser.PartnerOrganizationCode)
                {
                    IsShared = false
                };

                result &= partnerMaterial.Create();
            }

            return material;
        }
    }
}