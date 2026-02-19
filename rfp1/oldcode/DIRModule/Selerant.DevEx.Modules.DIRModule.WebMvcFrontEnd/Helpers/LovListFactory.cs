using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement;
using Selerant.DevEx.WebControls;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers
{
	public static class LovListFactory
	{
		private const string WASTE = BackEnd.Common.Constants.BussinessCostPhrase.WASTE;
		private const string MATLOSS = BackEnd.Common.Constants.BussinessCostPhrase.MATLOSS;

		public static WebListItemCollection GetProdClassifications()
		{
			DxBaseList prodClassificationList = DxListFactory.New(new DxPhraseType(11m, "DXDIR_ASSESSMENT.PROD_CLASSIF"));
			prodClassificationList.ShowEmpty = false;
			prodClassificationList.Culture = DxUser.CurrentUser.ProgramCulture;
			prodClassificationList.Populate();

			var webList = new WebListItemCollection();
			foreach (DxListItem listItem in prodClassificationList.Items)
				webList.Add(new WebListItem(listItem.Value, listItem.Text));

			return webList;
		}

		public static WebListItemCollection GetOrgStructures()
		{
			DxBaseList list = DxListFactory.New(new DxPhraseType(11m, "DXDIR_ASSESSMENT.ORG_STRUCTURE"));
			list.ShowEmpty = false;
			list.Culture = DxUser.CurrentUser.ProgramCulture;
			list.Populate();

			var webList = new WebListItemCollection();
			foreach (DxListItem item in list.Items)
				webList.Add(new WebListItem(item.Value, item.Text));

			return webList;
		}

		public static WebListItemCollection GetDataQualities()
		{
			DxBaseList list = DxListFactory.New(new DxPhraseType(4m, "99511"), DxUser.CurrentUser, null);
			list.ShowEmpty = false;
			list.Culture = DxUser.CurrentUser.ProgramCulture;
			list.OrderListBy = OrderListBy.Value;
			list.Populate();

			var webList = new WebListItemCollection();
			foreach (DxListItem item in list.Items)
				webList.Add(new WebListItem(item.Value, item.Text));

			return webList;
		}

		public static List<SelectListItem> GetAssessmentTypes()
		{
			DxAssessmentTypeCollection asmTypeList = null;
			if (!DxUser.CurrentUser.IsExternal)
			{
				asmTypeList = new DxAssessmentTypeCollection(DxAssessmentTypeCollection.Filter.Active, true);
				asmTypeList.Load();
			}
			else
			{
				asmTypeList = DxAssessmentTypeCollection.NewByPartnerOrgCode(DxUser.CurrentUser.PartnerOrganizationCode, true);
			}

			List<SelectListItem> items = asmTypeList.Select(x => new SelectListItem()
			{
				Text = x.Description,
				Value = x.Code
			}).ToList();

			return items;
		}

		public static WebListItemCollection GetBusinessCosts()
		{
			DxBaseList list = DxListFactory.New(new DxPhraseType(11m, "DXDIR_BUSINESS_COST.TITLE"));
			list.ShowEmpty = false;
			list.Culture = DxUser.CurrentUser.ProgramCulture;
			list.Populate();

			WebListItemCollection webList = new WebListItemCollection();
            foreach (DxListItem item in list.Items)
            {
                if (item.Value != WASTE && item.Value != MATLOSS)
                    webList.Add(new WebListItem(item.Value, item.Text));
				else
					continue;
			}

			return webList;
		}

	}
}