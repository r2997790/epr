using System;
using System.Collections.Generic;
using Selerant.DevEx.BusinessLayer;
using Selerant.DevEx.BusinessLayer.AttributeManagement;
using Selerant.DevEx.Modules.DIRModule.BackEnd.AssessmentManagement.Attributes;
using Selerant.DevEx.Modules.DIRModule.ModuleBlocks._ThisProject;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd._ThisProject.Infrastructure.AttributesProvider
{
	/// <summary>
	/// implementation of <see cref="IAttributeElementProvider"/> for DIR module
	/// </summary>
	public class DIRAttributeElementProvider : IAttributeElementProvider
	{
		/// <summary>
		///  information about who is providing data
		/// </summary>
		public string ProvidedBy => DIRModuleInfo.Instance.ModuleCode;

		public Dictionary<string, Func<DxAttributeDef, string, decimal, DxAttributeElement>> ProvideBuilders()
		{
			var list = new Dictionary<string, Func<DxAttributeDef, string, decimal, DxAttributeElement>>();
			list.Add(DxAssessmentAttributeScope.NAME, (def, objPKey, index) =>
			{
				if (def == null)
					throw new ArgumentNullException(nameof(def));
				if (String.IsNullOrEmpty(objPKey))
					throw new ArgumentNullException(nameof(objPKey));

				return new DxAssessmentAttributeElement(objPKey, def.Name, def.Id, index);
			});
			return list;
		}

        public Dictionary<string, Func<DxAttributeElementCollection>> ProvideCollections()
        {
            var list = new Dictionary<string, Func<DxAttributeElementCollection>>()
            {
                { DxAssessmentAttributeScope.NAME, () => new DxAssessmentAttributeElementCollection() }
            };

            return list;
        }
    }
}