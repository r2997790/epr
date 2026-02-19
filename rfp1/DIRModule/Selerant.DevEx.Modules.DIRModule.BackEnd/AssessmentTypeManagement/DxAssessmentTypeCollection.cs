using System;
using System.Linq;
using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentTypeManagement
{
	[IsDxObjectCollection]
	[BoundBusinessObject(typeof(DxAssessmentType))]
	public partial class DxAssessmentTypeCollection : DxObjectCollection
	{
		#region Constants

		private const string configElementName = "dxAssessmentTypeCollection";

		#endregion

		#region Queries

		protected static readonly Func<bool, IQueryable<DxAssessmentType>> queryByActive
			= QueryCompiler.Compile((bool active) =>
				(from o in new DxQueryable<DxAssessmentType>()
				 where o.Active == active
				 select o));

		#endregion

		#region Enums

		public enum Filter
		{
			Active
		}

		#endregion

		#region Constructors

		public DxAssessmentTypeCollection()
			: base()
		{
		}

        public DxAssessmentTypeCollection(bool load)
            : this()
        {
            if (load)
                Load();
        }

		public DxAssessmentTypeCollection(Filter filter, params object[] parameters)
			: this()
		{
			loadFilter = new LoadFilter(filter, parameters);
		}

		public static DxAssessmentTypeCollection NewByPartnerOrgCode(string partnerOrgCode, bool onlyActive = false)
        {
            var query = new DxQuery(typeof(DxAssessmentType));
            query.Options.IncludeAllBoundColumns = true;

			if (onlyActive)
			{
				DxQueryCriteria activeCriteria = DxQueryCriteria.CreateCriteria("active", DxQueryCriteria.SqlOperator.Equal, true);
				query.RootPredicate.AddCriteria(activeCriteria);
			}

			DxQueryCriteria partnerOrganizationCriteria = DxQueryCriteria.CreateCriteria("PartnerAssessmentTypes.partnerOrgCode", DxQueryCriteria.SqlOperator.Equal, partnerOrgCode);
			partnerOrganizationCriteria.CaseSensitive = true;

			DxQueryCriteria partnerIsSharedCriteria = DxQueryCriteria.CreateCriteria("PartnerAssessmentTypes.isShared", DxQueryCriteria.SqlOperator.Equal, 1);
			partnerIsSharedCriteria.CaseSensitive = true;

			DxQueryCriteria partnersOrgCriteria = DxQueryCriteria.CreateCriteria("PartnerAssessmentTypes.partnerOrgCode", DxQueryCriteria.SqlOperator.Equal, DxSecureObject.publicPartnerCode);
			partnersOrgCriteria.CaseSensitive = true;

			query.RootPredicate.AddANDPredicate();

			DxQueryPredicate mainAndPredicate = query.RootPredicate.AddANDPredicate();
			DxQueryPredicate orPredicate = mainAndPredicate.AddORPredicate();
			DxQueryPredicate andPredicate = orPredicate.AddANDPredicate();

			orPredicate.AddCriteria(partnerOrganizationCriteria);
			andPredicate.AddCriteria(partnersOrgCriteria);
			andPredicate.AddCriteria(partnerIsSharedCriteria);

			var queryResult = (DxAssessmentTypeCollection)DxObjectCollection.New(query, true);

            return queryResult;
        }

		#endregion

		#region Overrides

		protected override IQueryable GetLoadQuery()
		{
			if (loadFilter != null)
			{
				switch ((Filter)loadFilter.Filter)
				{
					case Filter.Active:
						return queryByActive((bool)loadFilter.Parameters[0]);
				}
			}

			return null;
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
	}
}
