using System;
using System.Collections.Generic;
using System.Linq;
using Selerant.ApplicationBlocks.Data;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.BusinessObjects.Attributes.DataProviders;
using Selerant.DevEx.BusinessLayer.DataMapping;
using Selerant.DevEx.ExceptionManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects
{
    [Serializable]
    [BoundTable(typeof(BizDsAssessment), TableName = "DXDIR_ASSESSMENT")]
    [BoundAttributesScope(DxAssessmentAttributeScope.NAME, ElementCollectionType = typeof(DxAssessmentAttributeElementCollection))]
    [BoundAutoNumberingMethodInfo(DIRModuleInfo.MODULE_CODE, "ASSESSMENT", "AssessmentNewKey")]
    [IsDxObject]
    public partial class DxAssessment : DxSecureObject, IAttributesContainer, INavigableObject, ISearchableObject, IShareableObject
    {
        #region Constants

        private const string configElementName = "dxAssessment";

        #endregion

        #region Enum

        public enum AssessmentStatus
        {
            DEVELOPMENT,
            DRAFT,
            SUBMITTED,
            APPROVED,
            CURRENT,
            HISTORIC
        }

        #endregion

        #region Fields

        [BoundColumn("CODE", true, 0, DbType = GenericDbType.Varchar)]
        protected string code;

        [BoundColumn("TYPE_CODE", DbType = GenericDbType.Varchar)]
        protected string typeCode;

        [BoundColumn("DESCRIPTION", DbType = GenericDbType.Nvarchar)]
        protected string description;

		[BoundColumn("STATUS", DbType = GenericDbType.Varchar)]
		[PhraseMapping(DxPhraseTypeCategory.ListOfValuesStandard, "DXDIR_ASSESSMENT.STATUS", PhraseMappingAttribute.ApplyToQueryMode.NotInSelect)]
		protected AssessmentStatus status;

        [BoundColumn("COMPANY_NAME", DbType = GenericDbType.Nvarchar)]
        protected string companyName;

        [BoundColumn("COMPLETING_BY")]
        protected decimal completingBy;

        [BoundColumn("PHONE", DbType = GenericDbType.Varchar)]
        protected string phone;

        [BoundColumn("EMAIL", DbType = GenericDbType.Nvarchar)]
        protected string email;

        [BoundColumn("TIMEFRAME_FROM")]
        protected NullableDateTime timeframeFrom;

        [BoundColumn("TIMEFRAME_TO")]
        protected NullableDateTime timeframeTo;

        [BoundColumn("WASTE_WATER_DISCHARGE_RATIO")]
        protected decimal wasteWaterDischargeRatio;

        [BoundColumn("CURRENCY", DbType = GenericDbType.Nvarchar)]
        protected string currency;

        [BoundColumn("CREATE_DATE")]
        protected DateTime createDate;

        [BoundColumn("CREATED_BY")]
        protected decimal createdBy;

        [BoundColumn("MOD_DATE")]
        protected NullableDateTime modDate;

        [BoundColumn("MODIFIED_BY")]
        protected NullableDecimal modifiedBy;

        [BoundColumn("AUTHORIZATION_ROLE")]
        protected NullableDecimal authorizationRoleId;

        [BoundColumn("PROD_CLASSIF", DbType = GenericDbType.Varchar)]
        protected string prodClassification;

        [BoundColumn("ORG_STRUCTURE", DbType = GenericDbType.Varchar)]
        protected string orgStructure;

        [BoundColumn("LOCATION", DbType = GenericDbType.Varchar)]
        protected string location;

        private DxAttributeCollection attributes;

        private DxNavigatorController navigatorController;
        private DxDialogNavigatorController dialogNavigatorController;

        [NonSerialized]
        protected DxUser completionist;
        [NonSerialized]
        protected DxUser creator;
        [NonSerialized]
        protected DxUser modifier;
        [NonSerialized]
        protected DxAuthorizationRole authorizationRole;
        [NonSerialized]
        protected DxAssessmentType assessmentType;
        [NonSerialized]
        protected string currencySymbol;
        public string DataQualityAttribute;
        public string AttributeComments;
        private DxPartnerAssessmentCollection partnerAssessments;
        private DxAssessmentLcStageCollection lcStages;

        #endregion

        #region Properties

        [BoundProperty(new string[] { nameof(code) })]
        public string Code
        {
            get { return code; }
        }

        [BoundProperty(new string[] { nameof(typeCode) })]
        public string TypeCode
        {
            get { return typeCode; }
            set { Change(x => typeCode = x, value); }
        }

        [BoundProperty(new string[] { nameof(description) })]
        public string Description
        {
            get { return description; }
            set { Change(x => description = x, value); }
        }

        [BoundProperty(new string[] { nameof(status) })]
        public AssessmentStatus Status
        {
            get { return status; }
            set { Change(x => status = x, value); }
        }

        [BoundProperty(new string[] { nameof(status) })]
        public string StatusString
        {
            get { return Status.ToString(); }
        }

        [BoundProperty(IsValid = false)]
        public string StatusDescription
        {
            get
            {
                DxPhraseText phraseText;

                if (DxUser.CurrentUser != null)
                    phraseText = DxPhraseText.GetBest(DxPhraseTypeCategory.ListOfValuesStandard, "DXDIR_ASSESSMENT.STATUS", Status.ToString(), DxUser.CurrentUser.ProgramCulture);
                else
                    phraseText = DxPhraseText.GetBest(DxPhraseTypeCategory.ListOfValuesStandard, "DXDIR_ASSESSMENT.STATUS", Status.ToString(), (DxCulture)null);

                return phraseText.Text;
            }
        }

        [BoundProperty(new string[] { nameof(companyName) })]
        public string CompanyName
        {
            get { return companyName; }
            set { Change(x => companyName = x, value); }
        }

        [BoundRelation(new string[] { nameof(completingBy) }, new string[] { nameof(DxUser.Id) })]
        [BoundProperty(new string[] { nameof(completingBy) })]
        public DxUser Completionist
        {
            get
            {
                if (completionist == null)
                    completionist = DxUser.Get(completingBy);
                return completionist;
            }
            set
            {
                Change(x => completingBy = x.Id, value);
                completionist = value;
            }
        }

        [BoundProperty(new string[] { nameof(phone) })]
        public string Phone
        {
            get
            {
                if (phone != null)
                    return phone;
                else
                    return Creator.Phone;
            }
            set { Change(x => phone = x, value); }
        }

        [BoundProperty(new string[] { nameof(email) })]
        public string Email
        {
            get { return email; }
            set { Change(x => email = x, value); }
        }

        [BoundProperty(new string[] { nameof(timeframeFrom) })]
        public NullableDateTime TimeframeFrom
        {
            get { return timeframeFrom; }
            set { Change(x => timeframeFrom = x, value); }
        }

        [BoundProperty(new string[] { nameof(timeframeTo) })]
        public NullableDateTime TimeframeTo
        {
            get { return timeframeTo; }
            set { Change(x => timeframeTo = x, value); }
        }

        [BoundProperty(new string[] { nameof(wasteWaterDischargeRatio) })]
        public decimal WasteWaterDischargeRatio
        {
            get { return wasteWaterDischargeRatio; }
            set { Change(x => wasteWaterDischargeRatio = x, value); }
        }

        [BoundProperty(new string[] { nameof(currency) })]
        public string Currency
        {
            get { return currency; }
            set { Change(x => currency = x, value); }
        }

        [BoundProperty(new string[] { nameof(createDate) })]
        public DateTime CreatDate
        {
            get { return createDate; }
            set { Change(x => createDate = x, value); }
        }

        [BoundRelation(new string[] { nameof(createdBy) }, new string[] { nameof(DxUser.Id) })]
        [BoundProperty(new string[] { nameof(createdBy) })]
        public DxUser Creator
        {
            get
            {
                if (creator == null)
                    creator = DxUser.Get(createdBy);
                return creator;
            }
            set
            {
                Change(x => createdBy = x.Id, value);
                creator = value;
            }
        }

        [BoundProperty(new string[] { nameof(modDate) })]
        public NullableDateTime ModDate
        {
            get { return modDate; }
            set { Change(x => modDate = x, value); }
        }

        [BoundRelation(new string[] { nameof(modifiedBy) }, new string[] { nameof(DxUser.Id) })]
        [BoundProperty(new string[] { nameof(modifiedBy) })]
        public DxUser Modifier
        {
            get
            {
                if (modifier == null && modifiedBy != null)
                    modifier = DxUser.Get(modifiedBy.Value);
                return modifier;
            }
            set
            {
                Change(x => modifiedBy = x.Id, value);
                modifier = value;
            }
        }

        [IncludeInDeepOperations]
        [DomainModelIgnore]
        public DxAuthorizationRole AuthorizationRole
        {
            get
            {
                if (authorizationRole == null && authorizationRoleId != null)
                    authorizationRole = new DxAuthorizationRole((decimal)authorizationRoleId);

                return authorizationRole;
            }
            set
            {
                Change(x => authorizationRole = x, value);
                authorizationRoleId = (value == null ? null : (NullableDecimal)value.Id);
            }
        }

        [BoundProperty(new string[] { nameof(prodClassification) })]
        public string ProdClassification
        {
            get { return prodClassification; }
            set { Change(x => prodClassification = x, value); }
        }

        [BoundProperty(new string[] { nameof(orgStructure) })]
        public string OrgStructure
        {
            get { return orgStructure; }
            set { Change(x => orgStructure = x, value); }
        }

        [BoundProperty(new string[] { nameof(location) })]
        public string Location
        {
            get { return location; }
            set { Change(x => location = x, value); }
        }

        [BoundRelation(new string[] { "typeCode" }, new string[] { "code" })]
        public DxAssessmentType AssessmentType
        {
            get
            {
                if (assessmentType == null && !string.IsNullOrEmpty(typeCode))
                    assessmentType = new DxAssessmentType(typeCode);

                return assessmentType;
            }
            set
            {
                Change(x => assessmentType = x, value);
                typeCode = (value == null) ? null : value.Code;
            }
        }

        [BoundProperty(IsValid = false)]
        public string AssessmentTypeDescription
        {
            get
            {
                if (AssessmentType == null)
                    return null;

                if (AssessmentType.PersistenceStatus == DxPersistenceStatus.Unknown)
                    AssessmentType.Load();

                return AssessmentType.Description;
            }
        }

        [BoundProperty(IsValid = false)]
        public string CurrencySymbol
        {
            get
            {
                if (string.IsNullOrEmpty(currencySymbol) && !string.IsNullOrEmpty(currency))
                {
                    DxCurrency dxCurrency = DxCurrency.Get(currency);
                    if (dxCurrency != null && dxCurrency.PersistenceStatus.IsUpToDate)
                        currencySymbol = dxCurrency.Symbol;
                }
                return currencySymbol;
            }
        }

        [BoundProperty(IsValid = false)]
        public string DataQualityAttributeName
        {
            get { return "DXDIR_DATA_QUALITY"; }
        }

        [BoundProperty(IsValid = false)]
        public string DataQuality
        {
            get
            {
                if (string.IsNullOrEmpty(DataQualityAttribute))
                {
                    if (this.Attributes[this.DataQualityAttributeName] == null || this.Attributes[this.DataQualityAttributeName].PersistenceStatus == DxPersistenceStatus.Unknown)
                        this.LoadAttributes(this.DataQualityAttributeName);
                    DataQualityAttribute = (string)DxAttribute.GetAttributeValueByContainer(this, this.DataQualityAttributeName) ?? string.Empty;
                }
                return DataQualityAttribute;
            }
        }

        [BoundProperty(IsValid = false)]
        public string CommentsAttributeName
        {
            get { return "DXDIR_ASSESSMENT_COMMENT"; }
        }

        [BoundProperty(IsValid = false)]
        public string Comments
        {
            get
            {
                if (string.IsNullOrEmpty(AttributeComments))
                {
                    if (this.Attributes[this.CommentsAttributeName] == null || this.Attributes[this.CommentsAttributeName].PersistenceStatus == DxPersistenceStatus.Unknown)
                        this.LoadAttributes(this.CommentsAttributeName);
                    AttributeComments = (string)DxAttribute.GetAttributeValueByContainer(this, this.CommentsAttributeName) ?? string.Empty;
                }
                return AttributeComments;
            }
        }

        [BoundRelation(new string[] { nameof(code) }, new string[] { "assessmentCode" })]
        [BoundProperty(new string[] { nameof(code) })]
        public DxPartnerAssessmentCollection PartnerAssessments
        {
            get
            {
                if (partnerAssessments == null)
                    partnerAssessments = DxPartnerAssessmentCollection.New(this);
                return partnerAssessments;
            }
            set { Change(x => partnerAssessments = x, value); }
        }

        [BoundRelation(new string[] { nameof(code) }, new string[] { "assessmentCode" })]
        [BoundProperty(new string[] { nameof(code) })]
        public DxAssessmentLcStageCollection LcStages
        {
            get
            {
                if (lcStages == null)
                    lcStages = DxAssessmentLcStageCollection.New(this);
                return lcStages;
            }
            set { Change(x => lcStages = x, value); }
        }

        #endregion

        #region Constructors

        public DxAssessment() : base()
        {
            wasteWaterDischargeRatio = 0.9m;
            currency = "USD";
        }

        public DxAssessment(string code) : this(code, false)
        {
        }

        public DxAssessment(string code, bool load) : this()
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

        #region Auto Numbering

        /// <summary>
        /// Obtains a new key for the given type by using the auto numbering feature.
        /// </summary>
        /// <param name="typeCode">Code of Assessment type</param>
        /// <param name="user">User getting autonumbering</param>
        /// <returns></returns>
        public static string GetAutoNumberingKey(string typeCode, DxUser user)
        {
            return GetAutoNumberingKey(typeof(DxAssessment), user, typeCode);
        }

        #endregion

        #region IAttributesContainer

        public DxAttributeScope AttributesScope => (DxAttributeScope)TypedDataMapper.GetBoundAttributesScope();

        [BoundRelation(new string[] { "code" }, "DXDIR_ATTVALUE_ASSESSMENT", new string[] { "CODE" })]
        [BoundRelation(new string[] { "code" }, "DX_ATTVALUE", new string[] { "OBJECT_PK" }, MappingMethod = "Selerant.DevEx.BusinessLayer.DataMapping.DxColumnMapper.MapManyToOne", DataTransformationType = DxDataRelation.TransformationType.CastToString, Position = 1)]
        [BoundRelation(new string[] { "code" }, "DX_ATTVALUE_MEMO", new string[] { "OBJECT_PK" }, MappingMethod = "Selerant.DevEx.BusinessLayer.DataMapping.DxColumnMapper.MapManyToOne", DataTransformationType = DxDataRelation.TransformationType.CastToString, Position = 2)]
        public DxAttributeCollection Attributes
        {
            get
            {
                if (attributes == null)
                    attributes = DxAttributeCollection.New(this);

                return attributes;
            }
            set { Change(x => attributes = x, value); }
        }

        public DxAttribute NewAttribute(string name)
        {
            return NewAttribute(name, AttributeDataProviderType.Undefined, false);
        }

        public DxAttribute NewAttribute(string name, bool load)
        {
            return NewAttribute(name, AttributeDataProviderType.Undefined, load);
        }

        public DxAttribute NewAttribute(string name, AttributeDataProviderType dataProviderType, bool load)
        {
            DxAttribute dxAttribute = DxAttribute.CreateNewAttribute(this, name, dataProviderType);

            if (load)
                dxAttribute.Load();

            return dxAttribute;
        }

        public bool LoadAttributes(params string[] names)
        {
            return LoadAttributesByNamesDirect(names);
        }

        #endregion

        #region INavigableObject

        public DxNavigatorController NavigatorController
        {
            get
            {
                if (navigatorController == null)
                    navigatorController = DxNavigatorController.GetTreeByTargetObject(this);

                return navigatorController;
            }
        }

        public DxDialogNavigatorController DialogNavigatorController
        {
            get
            {
                if (dialogNavigatorController == null)
                    dialogNavigatorController = DxDialogNavigatorController.GetTreeByTargetObject(this);

                return dialogNavigatorController;
            }
        }

        public void ClearDialogNavigatorController()
        {
            navigatorController = null;
        }

        public void ClearNavigatorController()
        {
            dialogNavigatorController = null;
        }

        #endregion

        #region IShareableObject

        public bool IsShared(DxUser dxUser)
        {
            if (!string.IsNullOrEmpty(dxUser.PartnerOrganizationCode))
            {
                DxPartnerAssessment dxPartnerAssessment = new DxPartnerAssessment(Code, dxUser.PartnerOrganizationCode);
                if (dxPartnerAssessment.Exists())
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        public bool IsPublic()
        {
            var dxPartnerObject = new DxPartnerAssessment(Code, DxSecureObject.publicPartnerCode, true);
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
            var dxPartnerObject = new DxPartnerAssessment(Code, partnerCode, true);
            if (!dxPartnerObject.Exists())
            {
                dxPartnerObject.IsShared = true;
                dxPartnerObject.Create();
            }
        }

        public void ShareRemoveSinglePartner(string partnerCode)
        {
            var dxPartnerObject = new DxPartnerAssessment(Code, partnerCode, true);
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
            if (PartnerAssessments.PersistenceStatus != DxPersistenceStatus.UpToDate)
                PartnerAssessments.Load();

            foreach (DxPartnerAssessment dxPartnerAssessment in PartnerAssessments)
                if (ValidateDelete(dxPartnerAssessment))
                    dxPartnerAssessment.Delete();
        }

        private bool ValidateDelete(DxPartnerAssessment partnerAssessment)
        {
            if (partnerAssessment.PartnerOrgCode != DxSecureObject.publicPartnerCode)
            {
                //if DxPartnerAssessment row is present, but IS_SHARED = 0 => this is a record for supplier, for which this assessment was initially published. This link shouldn't be removed
                if (!partnerAssessment.IsShared)
                    return false;
            }

            return true;
        }

        public DxOrganizationCollection LoadPartners()
        {
            var dxPartnerCollection = new DxPartnerAssessmentCollection(this);
            dxPartnerCollection.Load();

            var organizationCollection = new DxOrganizationCollection();
            foreach (DxPartnerAssessment partner in dxPartnerCollection)
            {
                if (partner.PartnerOrgCode != DxSecureObject.publicPartnerCode)
                {
                    organizationCollection.Add(new DxOrganization(partner.PartnerOrgCode, true));
                }
            }

            organizationCollection.LoadItems();
            return organizationCollection;
        }

        #endregion

        protected override string GetMvcUrl()
        {
            return string.Format("~/Modules/DIRModule/AssessmentNavigator/Home/Index?code={0}", Utilities.UrlEncode(IdentifiableString));
        }

		#region Formatting Methods

		public override string ToString()
		{
			return $"Assessment {code}";
		}

		public override string ToFormattedString(DxCultureInfo culture, DxFormattingContext formattingContext)
		{
			this.LoadEntity();

			if (!string.IsNullOrEmpty(Description))
				return Description;
			else
				return Code;
		}

		public override string ToShortDescriptionString(DxCultureInfo cultureInfo, DxFormattingContext formattingContext)
		{
			if (!string.IsNullOrEmpty(Description))
				return Description;
			else
				return Code;
		}

		public override string ToLongDescriptionString(DxCultureInfo cultureInfo, DxFormattingContext formattingContext)
		{
			return Code;
		}

		#endregion

		#region Partner creation

		public bool CreatePartnerAssessment()
		{
			var dxPartnerAssessment = new DxPartnerAssessment(this.Code, DxUser.CurrentUser.PartnerOrganizationCode);
			dxPartnerAssessment.IsShared = false;

			return dxPartnerAssessment.Create();
		}

		#endregion Partner creation

		#region Updating attributes

		public bool CreateOrUpdateAttributeValue(string attributeName, string attributeValue)
		{
			DxAttribute attribute = DxAttribute.GetOrCreateAttributeInstance(this, attributeName, true) as DxAttribute;
			if (!string.IsNullOrEmpty(attributeValue))
			{
				attribute.AddOrModifyAttributeValue(attributeValue);

				if (attribute.PersistenceStatus == DxPersistenceStatus.Unknown)
					return attribute.Create();
				else
					return attribute.Update();
			}
			else
			{
				attribute.DeleteValue();
				return true;
			}

		}

		#endregion Updating attributes
	}
}