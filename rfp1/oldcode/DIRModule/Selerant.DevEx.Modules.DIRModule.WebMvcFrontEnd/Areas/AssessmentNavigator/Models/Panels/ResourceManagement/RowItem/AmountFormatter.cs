using System.Globalization;
using Selerant.DevEx.BusinessLayer;

namespace Selerant.DevEx.Modules.DIRModule.WebMvcFrontEnd.Areas.AssessmentNavigator.Models.Panels.ResourceManagement
{
    public sealed class SimplePercentageFormatter
	{
		private readonly NumberFormatInfo nfi;

		public SimplePercentageFormatter()
		{
			nfi = DxUser.CurrentUser.GetCulture().NumberFormat;
			nfi.PercentPositivePattern = 1; // force pattern without whitespace between number and % sign
		}

		public string FormatToPercentage(decimal? amount)
		{
			return amount.HasValue ?
				   (amount / 100).Value.ToString("P1", nfi) :
				   null;
		}

        public string FormatToPercentage(bool isLeaf, decimal? amount)
        {
            return isLeaf ? FormatToPercentage(amount.HasValue ? amount.Value : 0.0m) : FormatToPercentage(amount);
        }
	}

	public sealed class AmountFormatter
	{
		public const string CURRENCY_FORMAT = "{0}{1}"; // $amount

		#region Fields

		private string asmtCurrencySymbol;
		private NumberFormatInfo nfiForDisplay;
		private NumberFormatInfo massNfiForDisplay;

		#endregion

		private AmountFormatter() { }

		public AmountFormatter(string currencySymbol)
		{
			asmtCurrencySymbol = currencySymbol;
			nfiForDisplay = DxUser.CurrentUser.GetCulture().NumberFormat;

			massNfiForDisplay = nfiForDisplay.Clone() as NumberFormatInfo;
			massNfiForDisplay.NumberDecimalDigits = 3;

			nfiForDisplay.PercentPositivePattern = 1; // force pattern without whitespace between number and % sign
		}

		#region Methods

		public string ToTwoDecimals(decimal? amount)
		{
			return amount.HasValue ?
				   amount.Value.ToString("n", nfiForDisplay) : // check this if it's like "{0:0.00}"
				   null;
		}

		public string ToThreeDecimals(decimal? amount)
		{
			return amount.HasValue ?
				   amount.Value.ToString("n", massNfiForDisplay) :
				   null;
		}

		public string FractionToPercentage(decimal? amount)
		{
			return amount.HasValue ?
				   amount.Value.ToString("P0", nfiForDisplay) :
				   null;
		}

		public string ToCurrency(decimal? amount)
		{
			return amount.HasValue ?
				  string.Format(CURRENCY_FORMAT, asmtCurrencySymbol, amount.Value.ToString("n", nfiForDisplay)) :
				  null;
		}

		#endregion

		public static string CurrencyDisplayFormat(string currencySymbol)
		{
			return CURRENCY_FORMAT.Replace("{0}", currencySymbol).Replace('1', '0');
		}
	}
}