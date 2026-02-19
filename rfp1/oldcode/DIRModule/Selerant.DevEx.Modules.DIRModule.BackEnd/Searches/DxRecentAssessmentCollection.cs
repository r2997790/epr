using IQToolkit;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.Linq;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Searches
{
    [Serializable]
    [BoundBusinessObject(typeof(DxRecentAssessment))]
    [IsDxObjectCollection]
    public partial class DxRecentAssessmentCollection : DxRecentObjectCollection
    {
        #region Queries

        protected static readonly Func<DxUser, IQueryable<DxRecentAssessment>> queryByUser
          = QueryCompiler.Compile((DxUser user) =>
          (from o in new DxQueryable<DxRecentAssessment>()
           where o.User.Id == user.Id
           select o));

        #endregion

        #region Constants

        private const string configElementName = "dxRecentAssessmentCollection";

        #endregion

        #region Constructors

        public DxRecentAssessmentCollection() : base()
        {

        }

        public DxRecentAssessmentCollection(DxUser user)
        {
            loadFilter = new LoadFilter(Filter.User, new object[] { user });
        }

        #endregion

        #region Indexers Overrides

        public new DxRecentAssessment this[int index]
        {
            get { return (DxRecentAssessment)base[index]; }
        }

        public DxRecentAssessment this[decimal userId, string code]
        {
            get { return (DxRecentAssessment)base[DxObject.GetIdentityKey(userId, code)]; }
        }

        #endregion

        #region Overrides

        protected override IQueryable GetLoadQuery()
        {
            if (loadFilter != null)
            {
                switch ((Filter)loadFilter.Filter)
                {
                    case Filter.User:
                        return queryByUser(loadFilter.Parameters[0] as DxUser);
                    default:
                        throw new NotImplementedException(String.Format("Filter {0} is not implemented", loadFilter.Filter));
                }
            }

            return null;
        }

        public override DxObjectCollection GetItemsBoundObject()
        {
            var boundObjects = new DxAssessmentCollection();

            foreach (DxRecentAssessment item in this)
            {
                DxAssessment boundObject = boundObjects[item.Assessment.IdentityKey];

                if (!boundObjects.Contains(boundObject))
                    boundObjects.Add(item.Assessment);
                else
                    item.Assessment = boundObject;
            }

            return boundObjects;
        }

        protected override void OnConfigure()
        {
            base.OnConfigure();
            Configure();
        }

        private void Configure()
        {
            ReadConfigurationElement(configElementName);
        }

        #endregion
    }
}
