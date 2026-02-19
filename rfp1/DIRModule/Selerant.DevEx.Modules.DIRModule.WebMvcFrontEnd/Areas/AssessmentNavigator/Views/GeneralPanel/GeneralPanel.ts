namespace DX.DIRModule.AssessmentNavigator
{
	export class GeneralPanel extends DX.Mvc.NavigatorPanelBase
    {
        static $$className = "DX.DIRModule.AssessmentNavigator.GeneralPanel";		

        protected _initializePanel(): void
        {
        }

        Edit_Click(source: JQuery, eventArgs: DX.JQuery.ElementEvent): void
		{
			const self = this;
			this._invokeController("EditGeneralPanelDialogActivity")
				.success(this, DX.OOP.createDelegate(this, function (result: DX.Mvc.DataAjaxResult): void
				{
					const activity: JSActivity = result.getDataValue("EditGeneralPanelDialogActivity");

					if (activity != null)
					{
						activity.execute().success(self, function (): void
                        {
                            self.refresh();
                            const navigator = DX.Ctrl.findParentControlOfType(self.get_element(), DX.Mvc.NavigatorBase);
                            const lcStageStepsPartial = DX.Ctrl.findControlOfType(navigator.get_element(), DX.DIRModule.AssessmentNavigator.LCStageSteps);

                            navigator.getPanelInstanceAsync("DIRModule_TabAssessmentResults").then((resultPanel) =>
                            {
                                if (resultPanel !== null && resultPanel.navigatorPanel !== null)
                                    (resultPanel.navigatorPanel as DX.DIRModule.AssessmentNavigator.AssessmentResults).refreshAll(new LcStageStep(lcStageStepsPartial.getSelectedLcStage()));
                            });

                        });  
                    }
					else
					{
						return;                    
                    }
				}));
        }

        Export_Click()
        {
            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Status: boolean, ScriptToExecute: string, ErrorMessage: string }>>("Export")
                .success((result) =>
            {
                const response = result.data();

                if (response.ScriptToExecute)
                {
                    const execFn = new Function(response.ScriptToExecute)
                    execFn();
                }
                else if (response.ErrorMessage)
                    this.showAlert(response.ErrorMessage);
            });
        }

		refresh(): void
		{
            super.refresh();
        }       
    }
}

