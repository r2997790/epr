using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.Resources;
using Selerant.DevEx.WebMvcModules.Infrastructure.Resources.Text;
using Selerant.DevEx.WebPages;
using System.Web.Mvc;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.CommandResolver
{
    /// <summary>
	/// This class inherites from CommandResolver. It represents a concrete command resolver
	/// used to execute actions for a DxAssessmentCollection.
	/// </summary>
    public class DxAssessmentCollectionCommandResolver : BaseCollectionCommandResolver
    {

        #region Constructor

        public DxAssessmentCollectionCommandResolver(DxAssessmentCollection sourceObjects)
            : base(sourceObjects)
        {
            _actions.AddAction(CreateNewAction(CommandResolverAction.GetOpenInDialogReferenceActionName));
        }

        #endregion

        #region Implemented Methods

        /// <summary>
        /// This method handle the execution of the selected action on this SpecificationCollectionCommandResolver inside the specified context
        /// </summary>
        /// <param name="action">the specified action to execute</param>
        /// <param name="context">the context in which execute the action</param>
        protected override void InnerExecute()
        {
            switch (Action.Name)
            {
                case CommandResolverAction.GetOpenInDialogReferenceActionName:
                    GetOpenInDialogReference();
                    break;
                default:
                    base.InnerExecute();
                    break;
            }
        }

        #endregion

        #region Overrided Methods

        protected override OpenInDialogData BuildMvcOpenInDialogData(OpenInDialogCommandResolverParameter param)
        {
            DxAssessment targetObject = ((DxObjectCollection)_sourceEntity)[0] as DxAssessment;
            var openDialogData = new OpenInDialogData();
            openDialogData.Url = CurrentRequestData.Instance.MvcUrlHelper.Action(MVC_DIR.AssessmentNavigator.GeneralPanel.EditGeneralPanelDialog(targetObject.IdentifiableString));
            openDialogData.Title = param.EditMode
              ? TextResource.Instance.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("EditAssessment")
                : TextResource.Instance.GetResourceModuleByName(ResourceFiles.AssessmentManager).GetLocalizedString("AssessmentView");
            openDialogData.Width = 800;
            openDialogData.Height = 400;
            return openDialogData;
        }

        #endregion
    }
}