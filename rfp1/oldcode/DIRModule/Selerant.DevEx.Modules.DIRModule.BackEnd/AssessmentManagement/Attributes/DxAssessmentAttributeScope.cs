using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.DataMapping;
using Selerant.DevEx.BusinessLayer.Navigation.Customization;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using Selerant.DevEx.Modules.DIRModule.Configuration.PathManagement;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes
{
	public class DxAssessmentAttributeScope : DxAttributeScope
	{
		public const string NAME = "ASSESSMENT";

		/// <summary>
		/// Default Constructor
		/// </summary>
		public DxAssessmentAttributeScope() : base(NAME)
		{
			Unique3LettersCode = "ASM";
			ScopeName = NAME;
			ShortScopeName = "ASMT";
			MasterTableName = "DXDIR_ASSESSMENT";
			SpecificStorageTableName = "DXDIR_ATTVALUE_ASSESSMENT";
			SupportPersistence = true;
			DisableOpenConnectorSupport();
			SupportCascadeDeleteOnForeignKeyToMasterTable = false;
			AuditPackage = null;

			SetContainerMapper(new DxObjectDataMapper(typeof(DxAssessment)));

			// calculated property
			SupportMaterialization = this.SupportPersistence && (this.Unique3LettersCode != null);
			SupportAudit = !string.IsNullOrEmpty(this.AuditPackage);

			UdfBaseClass = "Selerant.DevEx.WebModules.UserForms.AssessmentUserForm";
			UdfNamespace = "Selerant.DevEx.Configuration.UserForms.Assessment";

		}

		#region Public Methods

		/// <summary>
		/// Specifying the position of the udfs
		/// </summary>
		/// <param name="udfWebTechnology"></param>
		/// <returns></returns>
		public override string GetUserFormFolderKey(WebTechnology udfWebTechnology)
		{
			return udfWebTechnology == WebTechnology.WebForms ? FolderKeys.AssessmentUserFormsFolder :
				   udfWebTechnology == WebTechnology.Mvc ? FolderKeys.AssessmentMvcUserFormsFolder :
				   string.Empty;
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return base.ToString();
		}

		#endregion
	}
}
