using System.Collections.Generic;
using System.Data;
using Selerant.ApplicationBlocks.Data;

namespace Selerant.DevEx.Modules.DIRModule.BackEnd.Helpers
{
	public static class BizDsCreateParametersHelper
	{
		public static List<IDbDataParameter> CreatePrimaryKeysParameters(params IDbDataParameter[] primaryKeysColumns)
		{
			var parameters = new List<IDbDataParameter>(primaryKeysColumns.Length);
			parameters.AddRange(primaryKeysColumns);
			return parameters;
		}

		public static List<IDbDataParameter> AddParametersIfNotReadOrDelete(this List<IDbDataParameter> input, GenericDbOperationType operationType, params IDbDataParameter[] otherColumns)
		{
			if (operationType != GenericDbOperationType.ReadRecord && operationType != GenericDbOperationType.DeleteRecord)
			{
				input.AddRange(otherColumns);
			}
			return input;
		}
	}
}
