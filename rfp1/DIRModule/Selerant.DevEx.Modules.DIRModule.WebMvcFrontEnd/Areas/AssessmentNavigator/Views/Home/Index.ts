namespace DX.DIRModule.AssessmentNavigator.Home
{
	export class Index extends DX.Mvc.NavigatorBase
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.Home.Index";

        private massageAlertForNotValidDestination: string = "";
    
		__applyJSONData()
		{
            // (OVERRIDE)
            super.__applyJSONData();
            this.massageAlertForNotValidDestination = this._getDOMData<string>("warningMessageDestination");
		}

		protected _initialize()
		{
			// (OVERRIDE)
			super._initialize();

            if (this.massageAlertForNotValidDestination)
            {
                this.showAlert(this.massageAlertForNotValidDestination, undefined, 10000);
            }
		}
	}

	export class Header extends DX.Mvc.NavigatorHeader
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.Home.Header";

		private lcStageStepsPartial: LCStageSteps = null;

		Copy_Click(sender: JQuery, eventArgs: DX.Mvc.HtmlMenuItemClickEventArgs)
		{
			this.invokeOnHeaderMenuItemClick(null, eventArgs);
		}
		AssessmentDashboard_Click(sender: JQuery, eventArgs: DX.Mvc.HtmlMenuItemClickEventArgs)
		{
			this.invokeOnHeaderMenuItemClick(null, eventArgs);
		}

		protected _initialize()
		{
			super._initialize();

			const navigator = DX.Ctrl.findParentControlOfType(this.get_element(), DX.Mvc.NavigatorBase) as DX.Mvc.NavigatorBase;
			this.lcStageStepsPartial = DX.Ctrl.findControlOfType(navigator.get_element(), DX.DIRModule.AssessmentNavigator.LCStageSteps);

			this.lcStageStepsPartial.onManageStagesClick = () =>
			{
				this._invokeController<Mvc.TypedDataAjaxResult<{ activity: JSOpenDialogActivity }>>("ManageAssessmentRaiseDialog", { entityType: EntityType.LcStage })
					.success(this, (result) =>
					{
						const activity = result.data().activity;
						activity.execute().success((jsPlan) =>
						{
							const returnButtonKey = activity.getButtonClicked();

							if (returnButtonKey === 'Ok')
							{
								DX.Page.getHostPage().refreshActiveTab();
							}
						});
					});
			}
		}

	}
}