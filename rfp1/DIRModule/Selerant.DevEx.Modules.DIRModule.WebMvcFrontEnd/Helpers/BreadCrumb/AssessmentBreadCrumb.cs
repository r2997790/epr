using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.WebMvcModules.Helpers.Abstract;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Helpers.BreadCrumb
{
	public sealed class AssessmentBreadCrumb : BreadCrumb<DxAssessment>
    {
        public AssessmentBreadCrumb(DxAssessment assessment) : base(assessment) { }

        protected override void SetupBar()
        {
            //link to material
            //if (targetObject.IsQuestionnaireMaterialType())
            //{
            //    targetObject.Materials.Load();
            //    foreach (DxMaterial mat in targetObject.Materials)
            //    {
            //        AddLink(EntityToLink(mat));
            //    }
            //}

            AddLink(EntityToLink(new DxMaterial("NONE","NONE",true)));
        }
    }
}