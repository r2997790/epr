using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.ExceptionManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement.PartnerAssessmentType;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement
{
	[IsDxObject]
	[BoundTable(typeof(BizDsAssessmentType), TableName = "DXDIR_ASSESSMENT_TYPE")]
    [BoundAutoNumberingMethodInfo(DIRModuleInfo.MODULE_CODE, "ASSESSMENT_TYPE", "AssessmentTypeNewKey")]
    public partial class DxAssessmentType : DxObject, IShareableObject
    {
		#region Constants

		private const string configElementName = "dxAssessmentType";

		#endregion

		#region Fields

		[BoundColumn("CODE", true, 0, DbType = GenericDbType.Varchar)]
		protected string code;

		[BoundColumn("DESCRIPTION", DbType = GenericDbType.Nvarchar)]
		protected string description;

		[BoundColumn("ACTIVE", DbType = GenericDbType.Decimal)]
		protected decimal active;

        private DxPartnerAssessmentTypeCollection partnerAssessmentTypes;
        #endregion

        #region Properties

        [BoundProperty(new string[] { nameof(code) })]
		public string Code
		{
			get { return code; }
		}

		[BoundProperty(new string[] { nameof(description) })]
		public string Description
		{
			get { return description; }
			set { Change(x => description = x, value); }
		}

		[BoundProperty(new string[] { nameof(active) })]
		public bool Active
		{
			get { return active == 1.0m; }
			set { Change(x => active = (x ? 1.0m : 0.0m), value); }
		}

        [BoundRelation(new string[] { nameof(code) },
           new string[] { "assessmentTypeCode" })]
        [BoundProperty(new string[] { nameof(code) })]
        public DxPartnerAssessmentTypeCollection PartnerAssessmentTypes
        {
            get
            {
                if (partnerAssessmentTypes == null)
                    partnerAssessmentTypes = DxPartnerAssessmentTypeCollection.New(this);

                return partnerAssessmentTypes;
            }
            set { Change(x => partnerAssessmentTypes = x, value); }
        }

		#endregion

		#region Constructors

		public DxAssessmentType() : base()
		{
		}

		public DxAssessmentType(string code) : this(code, false)
		{
		}

		public DxAssessmentType(string code, bool load) : this()
		{
			this.code = code;
			if (load)
				Load();
		}

        #endregion

        #region Configuration

        protected override void OnConfigure()
		{
			base.OnConfigure();
			Configure();
		}

		private void Configure()
		{
			ReadConfigurationElement(configElementName);
			//Add any specific configuration
		}

        #endregion

        #region IShareableObject

        public bool IsShared(DxUser dxUser)
        {
            if (!string.IsNullOrEmpty(dxUser.PartnerOrganizationCode))
            {
                DxPartnerAssessmentType dxPartnerAssessmentType = new DxPartnerAssessmentType(Code, dxUser.PartnerOrganizationCode);
                if (dxPartnerAssessmentType.Exists())
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public bool IsPublic()
        {
            var dxPartnerObject = new DxPartnerAssessmentType(Code, DxSecureObject.publicPartnerCode, true);
            return dxPartnerObject.IsShared;
        }

        public void MakePublic()
        {
            ShareWithPartner(DxSecureObject.publicPartnerCode);
        }

        public void MakePrivate()
        {
            ShareRemoveSinglePartner(DxSecureObject.publicPartnerCode);
        }

        public void ShareWithPartner(string partnerCode)
        {
            var dxPartnerObject = new DxPartnerAssessmentType(Code, partnerCode, true);
            if (!dxPartnerObject.Exists())
            {
                dxPartnerObject.IsShared = true;
                dxPartnerObject.Create();
            }
        }

        public void ShareRemoveSinglePartner(string partnerCode)
        {
            var dxPartnerObject = new DxPartnerAssessmentType(Code, partnerCode, true);
            if (dxPartnerObject.Exists())
            {
                if (ValidateDelete(dxPartnerObject))
                    dxPartnerObject.Delete();
                else
                    throw new DevExException("This link cannot be removed.");
            }
        }

        public void ShareRemoveAllPartners()
        {
            if (PartnerAssessmentTypes.PersistenceStatus != DxPersistenceStatus.UpToDate)
                PartnerAssessmentTypes.Load();

            foreach (DxPartnerAssessmentType dxPartnerAssessmentType in PartnerAssessmentTypes)
                if (ValidateDelete(dxPartnerAssessmentType))
                    dxPartnerAssessmentType.Delete();
        }

        public DxOrganizationCollection LoadPartners()
        {
            var dxPartnerTypeCollection = new DxPartnerAssessmentTypeCollection(this);
            dxPartnerTypeCollection.Load();

            var organizationCollection = new DxOrganizationCollection();
            foreach (DxPartnerAssessmentType partner in dxPartnerTypeCollection)
            {
                if (partner.PartnerOrgCode != DxSecureObject.publicPartnerCode)
                {
                    organizationCollection.Add(new DxOrganization(partner.PartnerOrgCode, true));
                }
            }

            organizationCollection.LoadItems();
            return organizationCollection;
        }

        private bool ValidateDelete(DxPartnerAssessmentType partnerAssessmentType)
        {
            if (partnerAssessmentType.PartnerOrgCode != DxSecureObject.publicPartnerCode)
            {
                if (!partnerAssessmentType.IsShared)
                    return false;
            }

            return true;
        }

        #endregion

        #region Auto Numbering

        /// <summary>
        /// Obtains a new key by using the auto numbering feature.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static string GetAutoNumberingKey(DxUser user, string type = "STANDARD")
        {
            return GetAutoNumberingKey(typeof(DxAssessmentType), user, type);
        }

        #endregion
    }
}
