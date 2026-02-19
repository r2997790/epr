namespace DX.DIRModule.AssessmentNavigator.Home
{
    export class IndexEdit extends DX.Mvc.DialogViewControl
    {
        static $$className = "DX.DIRModule.AssessmentNavigator.Home.IndexEdit";
		private _isCreating: boolean;

		private promptMessageButtonCancel: string;
		private promptButtonYes: string;
		private promptButtonNo: string;

        constructor(element: HTMLElement)
        {
            super(element);
        }

        __applyJSONData() {
            // (OVERRIDE)
            super.__applyJSONData();

            this._isCreating = this._getDOMData<boolean>("isCreating");
        }

        protected _initialize(): void {
            // (OVERRIDE)
			super._initialize();

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "AMPromptConfirmAssessmentDelete")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptMessageButtonCancel = result.text);

			DX.Loc.TextDescriptor.newByResource("Controls", "DlgYes")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptButtonYes = result.text);

			DX.Loc.TextDescriptor.newByResource("Controls", "DlgNo")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptButtonNo = result.text);
        }

        protected _onMenuItemClick(eventArgs?: DX.Mvc.MenuItemClickEventArgs): void {
            // (OVERRIDE)
            super._onMenuItemClick(eventArgs);

            if (eventArgs.getHandled())
                return;
            var _self = this;
            switch (eventArgs.getMenuItem().getKey()) {
                case "OkButton":
                    {
                        eventArgs.setHandled(true);
                        DX.Page.closeDialog(false).setPressedButtonKey("Ok");;
                    }
                    break;

                case "CancelButton":
					{
                        eventArgs.setHandled(true);
						this.showPrompt(this.promptMessageButtonCancel, [
							{
								caption: this.promptButtonYes, type: "confirm", func: DX.OOP.createDelegate(_self, function ()
								{
									DX.Page.closeDialog(false).setPressedButtonKey("Cancel");
								})
							},
							{
								caption: this.promptButtonNo, type: "cancel", func: DX.OOP.createDelegate(_self, function ()
								{
								})
							}], false);
                        
                    }
                    break;
            }
        }

        private deleteAssessment() : void {

        }
        
    }
}
