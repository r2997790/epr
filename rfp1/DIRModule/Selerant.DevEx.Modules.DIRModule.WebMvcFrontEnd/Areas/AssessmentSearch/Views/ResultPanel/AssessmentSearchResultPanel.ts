namespace DX.Mvc
{
	export class AssessmentSearchResultPanel extends DX.Mvc.SearchResultPanel
	{
		static $$className = "DX.Mvc.AssessmentSearchResultPanel";

		constructor(element: HTMLElement)
		{
			super(element);
		}

		Continue_Click(source: JQuery, eventArgs: DX.JQuery.ElementEvent)
		{
			var identifiableString: string = DX.DOM.toJQElements(source).data("id") as string;

			this._invokeController<DX.Mvc.JSActivityAjaxResult<JSOpenDialogActivity>>("ContinueDrafAssessment", { identifiableString: identifiableString })
				.success(this, result =>
				{
					DX.JSPlan.execute(result.getActivity(), null, null)
						.success((jsPlan) => this._reloadGrid());
				});
		}
	}
}