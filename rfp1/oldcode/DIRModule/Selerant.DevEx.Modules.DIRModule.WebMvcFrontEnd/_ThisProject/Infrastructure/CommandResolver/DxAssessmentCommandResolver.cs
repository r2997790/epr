using System.Web.Mvc;
using Selerant.DevEx.CommonComponents;
using Selerant.DevEx.Web.SearchManager;
using Selerant.DevEx.WebPages;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd._ThisProject.Infrastructure.CommandResolver
{
    public class DxAssessmentCommandResolver : Selerant.DevEx.CommonComponents.CommandResolver
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceQualityRecord"></param>
        public DxAssessmentCommandResolver(DxAssessment sourceQualityRecord)
            : base(sourceQualityRecord)
        {
            // set specific actions
            _actions.AddAction(CreateNewAction(CommandResolverAction.OpenSearchDialogActionName));
            _actions.AddAction(CreateNewAction(CommandResolverAction.GetOpenSearchDialogReferenceActionName));
            _actions.AddAction(CreateNewAction(CommandResolverAction.CanGetOpenSearchDialogReferenceActionName));
            _actions.AddAction(CreateNewAction(CommandResolverAction.GetNavigatorUrlActionName));
            _actions.AddAction(CreateNewAction(CommandResolverAction.GetOpenReportDialogReferenceActionName));
        }

        #endregion

        #region Implemented Methods

        /// <summary>
        /// This method handle the execution of the selected action on this DxMyScopeBusinessEntity inside the specified context
        /// </summary>
        protected override void InnerExecute()
        {
            switch (Action.Name)
            {
                case CommandResolverAction.OpenSearchDialogActionName:
                    OpenSearchDialog();
                    break;
                case CommandResolverAction.GetOpenSearchDialogReferenceActionName:
                    GetOpenSearchDialogReference();
                    break;
                case CommandResolverAction.CanGetOpenSearchDialogReferenceActionName:
                    CanGetOpenSearchDialogReference();
                    break;
                case CommandResolverAction.GetNavigatorUrlActionName:
                    GetNavigatorUrl();
                    break;
                case CommandResolverAction.GetOpenReportDialogReferenceActionName:
                    GetOpenReportDialogReference();
                    break;
            }
        }

        #endregion

        #region EventExecution Methods

        #region MVC specific

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="openDialogData"></param>
        protected override void FillOpenSearchDialogData(DialogSearchCommandResolverParameter parameter, Selerant.DevEx.CommonComponents.CommandResolver.OpenDialogData openDialogData)
        {
            openDialogData.Url = CurrentRequestData.Instance.MvcUrlHelper.Action(MVC_DIR.AssessmentSearch.Home.IndexDialog());
            base.FillOpenSearchDialogData(parameter, openDialogData);
        }

        #endregion MVC specific

        #endregion EventExecution Methods
    }
}