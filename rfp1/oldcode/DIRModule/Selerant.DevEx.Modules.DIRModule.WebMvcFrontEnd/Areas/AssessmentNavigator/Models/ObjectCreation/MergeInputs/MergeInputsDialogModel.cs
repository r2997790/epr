using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.BackEnd.Common;
using Selerant.DevEx.Modules.DIRModule.BackEnd.InputManagement;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement;
using Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers;
using Selerant.DevEx.Web;
using Selerant.DevEx.WebControls;
using Selerant.DevEx.WebMvcModules.Infrastructure;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls;
using Selerant.DevEx.WebMvcModules.Infrastructure.Controls.SpreadsheetControl;
using Selerant.DevEx.WebPages;
using System.Collections.Generic;
using System.Linq;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.ObjectCreation.MergeInputs
{
    public class MergeInputsDialogModel : DialogViewIndexModel
    {
        private const string ID = "Id";
        private const string CATEGORY_TYPE = "CategoryType";
        private const string CATEGORY = "Category";
		private const string PRODUCT_SOURCE = "ProductSource";
		private const string PRODUCT_TITLE = "Product";
		private const string MERGED_NAME = "MergedName";

        public List<DxInputCategory> Categories { get; }
        public Spreadsheet SpreadsheetControl { get; private set; }
        private DxAssessment Assessment { get; }
        private decimal LcStageId { get; }
        private decimal NextLcStageId { get; }
        private DxInputCollection Inputs { get; }

		public MergeInputsDialogModel(string controllerUrl, ViewControlControllerData controllerData, string identifiableString, decimal lcStageId, decimal nextLcStageId) 
            : base(controllerUrl, controllerData)
        {
            Assessment = (DxAssessment)DxObject.ParseIdentifiableString(identifiableString);
            LcStageId = lcStageId;
            NextLcStageId = nextLcStageId;

			Inputs = DxInputCollection.GetCarriedInputs(Assessment.Code, lcStageId, true);
            Categories = Inputs.LoadItemsInputCategory();            
        }

        protected override void InitializeForView()
        {
            base.InitializeForView();

            SetMenuButtons();
            InitializeSpreadsheet();
            FillSpreadsheetWithValues();
        }

        protected override void FillScriptControlDescriptor(GxScriptControlDescriptor scriptControlDescriptor)
        {
            base.FillScriptControlDescriptor(scriptControlDescriptor);

            scriptControlDescriptor.TypeName = "DX.DIRModule.AssessmentNavigator.MergeInputsDialog";
            scriptControlDescriptor.Data["identifiableString"] = Assessment.IdentifiableString;
            scriptControlDescriptor.Data["lcStageId"] = LcStageId;
            scriptControlDescriptor.Data["nextLcStageId"] = NextLcStageId;
        }

        protected override void InitializeResources(List<ControlResource> resources)
        {
            base.InitializeResources(resources);

            resources.Add(new ControlFilesGroup(Infrastructure.StaticResources.CoreBundleNames.BUNDLE_MVCSIMPLECONTROLS));
            resources.Add(new ControlFilesGroup(Infrastructure.StaticResources.CoreBundleNames.BUNDLE_MVCSPREADSHEET));
            resources.Add(new ControlJsFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.MergeInputs.MergeInputsDialog_ts));
            resources.Add(new ControlCssFile(DIRLinks.Areas.AssessmentNavigator.Views.CreateAssessmentDialog.MergeInputs.MergeInputsDialog_css));
        }

        private void SetMenuButtons()
        {
            MenuModel
                .AddButton(DevExDialogPage.BUTTON_CANCEL_ID, ResText.Controls.GetString("DlgCancel"))
                .AddButton(DevExDialogPage.BUTTON_OK_ID, ResText.Controls.GetString("DlgOk"), cssClass: "confirm");
        }

        private void InitializeSpreadsheet()
        {
            SpreadsheetControl = Spreadsheet.New("spreadsheet")
                .SetPercWidth(100)
                .SetHeight(600)
                .SetEditInputFocusOut(EditInputFocusOutType.OutsideOfControl)
                .SetEditInputPosition(EditInputPositionType.InScrollAndInFront);

            SpreadsheetControl.WithColumn(ID)
                    .SetCaption("Id")
                    .InitCellType(c => new SpreadsheetNumericCellType()
                            .SetContainerReadOnly(c, true)
                            .SetContainerIsHidden(c, true));

            SpreadsheetControl.WithColumn(CATEGORY_TYPE)
                    .SetCaption("CategoryType")
                    .InitCellType(c => new SpreadsheetTextCellType()
                            .SetContainerReadOnly(c, true)
                            .SetContainerIsHidden(c, true));

            SpreadsheetControl.WithColumn(CATEGORY)
                    .SetCaption(ResText.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("InputsGridColumn_Category"))
                    .SetWidth(100)
                    .SetWidthStarSizing(2)
                    .InitCellType(c => new SpreadsheetTextCellType()
                        .SetContainerIsMultiValue(c, false)
                        .SetContainerReadOnly(c, true));

			SpreadsheetControl.WithColumn(PRODUCT_SOURCE)
				.SetCaption("ProductSource")
				.InitCellType(c => new SpreadsheetTextCellType()
						.SetContainerReadOnly(c, true)
						.SetContainerIsHidden(c, true));

			SpreadsheetControl.WithColumn(PRODUCT_TITLE)
				.SetCaption(ResText.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("InputsGridPoPLovProductOptionBase"))
				.SetWidth(50)
				.SetWidthStarSizing(2)
				.InitCellType(c => new SpreadsheetTextCellType()
					.SetContainerIsMultiValue(c, false)
					.SetContainerReadOnly(c, true));

			var typeFilter = new DxFilterMemberCollection();

            typeFilter.Add(new DxFilterMember("type", DxFilterMember.FilterType.Include,
                new string[] { Constants.MaterialType.DIR_RESOURCE }));

            typeFilter.Add(new DxFilterMember("Attributes[DXDIR_RESOURCE_TYPE]", DxFilterMember.FilterType.Include, new string[] { }));

            SpreadsheetControl.WithColumn(MERGED_NAME)
                    .SetCaption(ResText.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("MergeInputsDialogMergedInputNameColumnTitle"))
                    .SetWidth(100)
                    .SetWidthStarSizing(2)
                    .InitCellType(c => new DIRResourceTypeCellType()
                        .SetContainerIsMultiValue(c, false)
                        .SetContainerRefScope(c, DxAttributeScope.MATERIAL)
                        .SetContainerSearchFilter(c, typeFilter)
                        .SetContainerValidationSettings(c, new ObjectReferenceValidationSettings { IsRequired = true }));
        }

        private void FillSpreadsheetWithValues()
        {
			Dictionary<string, string> productsCodeTitle = GridHelpers.Instance.GetProductsCodeTitle();
			HashSet<(decimal categoryId, string productSource)> categoryIdProductSource = new HashSet<(decimal categoryId, string productSource)>(Categories.Count * productsCodeTitle.Count);

			Categories.ForEach(category =>
			{
				foreach (string productCode in productsCodeTitle.Keys)
					categoryIdProductSource.Add((category.Id, productCode));
			});

			foreach (DxInput input in Inputs)
			{
				var uniqueCategoryProductKey = (input.InputCategoryId, input.ProductSource);

				if (categoryIdProductSource.Contains(uniqueCategoryProductKey))
				{
					SpreadsheetControl.WithRow()
						.InitCell<SpreadsheetNumericCellType>(ID, (ct, c) => ct.SetCellValue(c, input.InputCategoryId))
						.InitCell<SpreadsheetTextCellType>(CATEGORY_TYPE, (ct, c) => ct.SetCellValue(c, input.InputCategory.Type))
						.InitCell<SpreadsheetTextCellType>(CATEGORY, (ct, c) => ct.SetCellValue(c, input.InputCategory.Title))
						.InitCell<SpreadsheetTextCellType>(PRODUCT_SOURCE, (ct, c) => ct.SetCellValue(c, input.ProductSource))
						.InitCell<SpreadsheetTextCellType>(PRODUCT_TITLE, (ct, c) => ct.SetCellValue(c, productsCodeTitle[input.ProductSource]));

					// only single unique row needs to be added to spreadsheet
					categoryIdProductSource.Remove(uniqueCategoryProductKey);
				}
			}
        }

        public bool SaveMergedInputRows(Spreadsheet.ChangesAjaxParameter gridParam)
        {
            bool success = true;

            HashSet<decimal> addedCategories = new HashSet<decimal>();

            Assessment.LoadAttributes("DXDIR_DATA_QUALITY");
            decimal.TryParse(Assessment.Attributes["DXDIR_DATA_QUALITY"].Value.Data as string, out decimal measurement);

            using (DxUnitOfWork unitOfWork = DxUnitOfWork.New())
            {
                gridParam.Rows.ForEach((row, index) =>
                {
                    if (row.Status == SpreadsheetRowStatus.Changed)
                    {
                        decimal categoryId = row.GetCellValue<decimal>(ID);
                        DxInputCategory category = Categories.First(x => x.Id == categoryId);

                        string productIdentifiableString = row.GetCellValue<string>(MERGED_NAME);
                        DxMaterial material = DxObject.ParseIdentifiableString<DxMaterial>(productIdentifiableString);

                        string productSource = row.GetCellValue<string>(PRODUCT_SOURCE);

                        (decimal mergedMass, decimal mergedCost) = Inputs.Where(x => x.InputCategoryId == category.Id && x.ProductSource == productSource)
                                                                            .Aggregate<DxInput, (decimal mergedMass, decimal mergedCost)>((mergedMass: .0m, mergedCost: .0m), (current, next) =>
                                                                            {
                                                                                return (current.mergedMass += next.Mass ?? .0m,
                                                                                        current.mergedCost += next.Cost ?? .0m);
                                                                            });
                        DxInput mergedInput = new DxInput()
                        {
                            AssessmentCode = Assessment.Code,
                            AssessmentLcStageId = NextLcStageId,
                            Material = material,
                            InputCategory = category,
                            Mass = mergedMass,
                            Cost = mergedCost,
                            Measurement = (DxCarriedInput.Measure)measurement,
                            PartOfProductCoproduct = false,
                            ProductSource = productSource,
                            InedibleParts = 0,
                            CategorySortOrder = category.SortOrder,
                            InputSortOrder = index
                        };

                        success &= mergedInput.Create();
                    }
                });

                if (success)
                    unitOfWork.Commit();
                else
                    unitOfWork.Abort();
            }

            return success;
		}
    }
}