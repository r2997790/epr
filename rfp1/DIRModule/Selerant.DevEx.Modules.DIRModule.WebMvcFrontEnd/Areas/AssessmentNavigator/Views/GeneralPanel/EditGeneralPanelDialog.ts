namespace DX.DIRModule.AssessmentNavigator
{
    export class EditGeneralPanelDialog extends DX.Mvc.DialogViewControl
    {

		static $$className = "DX.DIRModule.AssessmentNavigator.EditGeneralPanelDialog";

		generalPanelPartial: GeneralPanelPartial;

        constructor(element: HTMLElement) {
            super(element);
            this.generalPanelPartial = null;
        }

        protected _initialize(): void {
			// (OVERRIDE)	
            super._initialize();
			this.generalPanelPartial = this.$$("GeneralPanelPartial");
			
		}
		__applyJSONData(): void
		{
            // (OVERRIDE)
			super.__applyJSONData();
			
        }

		protected _updateGeneralInformation(closeDialog?: boolean): void
		{
			if (!this.generalPanelPartial.IsValid())
			   return;
			const generalModelPartialJson: string = JSON.stringify(this.generalPanelPartial.createGeneralModelPartialJson());
			this._invokeController("Update", { "generalModelPartialJson": generalModelPartialJson }).success(this, DX.OOP.createDelegate(this, function (): void
			{
                if (closeDialog)
                    DX.Page.closeDialog();
            }));
        }

		protected _onMenuItemClick(eventArgs?: DX.Mvc.MenuItemClickEventArgs): void
		{
            // (OVERRIDE)
            super._onMenuItemClick(eventArgs);
            if (eventArgs.getHandled())
                return;
            switch (eventArgs.getMenuItem().getKey()) {
                case "OkButton":
					{
						this._saveDialog(true);
                    }
                    break;

                case "CancelButton":
                    {
                        eventArgs.setHandled(true);
                        DX.Page.closeDialog();
                    }
                    break;
            }
		}
		private _saveDialog(closeDialog?: boolean): void
		{
            this._updateGeneralInformation(closeDialog);
        }
    }
}
