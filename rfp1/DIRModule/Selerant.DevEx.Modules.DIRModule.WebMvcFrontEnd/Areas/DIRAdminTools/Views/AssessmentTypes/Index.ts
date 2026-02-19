namespace DX.DIRModule.DIRAdminTools.AssessmentTypes
{
    export class Index extends DX.Mvc.AdmTools.IndexView
    {
        public static $$className = "DX.DIRModule.DIRAdminTools.AssessmentTypes.Index";

        private dirmodule_AssessmentTypesGrid: string;

        constructor(element: HTMLElement) {
            super(element);
        }

        public __applyJSONData(): void {
            // (OVERRIDE)
            super.__applyJSONData();
            this.dirmodule_AssessmentTypesGrid = this.getCustomData<string>("gridID");
        }

        protected _initialize(): void {
            // (OVERRIDE)
            super._initialize();

            this.$$<DX.Mvc.HtmlGrid.Grid>(DX.Mvc.HtmlGrid.getRealLogicId(this.dirmodule_AssessmentTypesGrid))
                .add_actionExecuted((sender, args) => {
                    switch (args.key) {
                        case "Activate":
                            this.handlerGridAction_Activate(args.data["id"]);
                            break;

                        case "Edit":
                            this.handlerGridAction_Edit(args.data["id"]);
                            break;

                        case "Delete":
                            this.handlerGridAction_Delete(args.data["id"]);
                            break;
                    }
                });
        }
        // Grid data loading complete
        onGridLoadComplete(data: JQueryExtensions.JQGrid.GridLoadCompleteData): void {
            this.gridLoadComplete(data, this.$(), this.$().find("#AssessmentTypesGridEmpty"));
            //tell body to resize, in order to resize the grid
            $('body').trigger('resize');
            if (data.total >= 0)
                DX.Mvc.HtmlGrid.__setPagerPagesCount(this.$(this.dirmodule_AssessmentTypesGrid), data.total);
        }
        // Event handlers
        AddAssessmentType_Click() {
            var _self: Index = this;
            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ activity: JSOpenDialogActivity }>>("AddAssessmentTypeClick")
                .success(this, function (result) {
                    result.data().activity.execute().success(() => {
                        const returnValue = result.data().activity.getOutButtonClicked();
                        if (returnValue === "Ok") {
                            const messages = (<DX.Mvc.DataAjaxResult>result).getDataValue<string>("message");
                            _self.showAlert(messages, "info", 1000);
                        }
                        _self._refreshGrid(_self._getGridId());
                    });
                });
        }
        // Grid actions
        private handlerGridAction_Activate(id: string) {
            this._invokeController("ActivateAssessmentType", { identifiableString: id })
                .success((result) => {
                    const messages = (<DX.Mvc.DataAjaxResult>result).getDataValue<string>("message");
                    this.showAlert(messages, "info", 1000);
                    this._refreshGrid(this._getGridId());
                });
        }

        private handlerGridAction_Edit(id: string) {
            var _self: Index = this;
            const reguestParams = {
                identifiableString: id
            };
            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ activity: JSOpenDialogActivity }>>("EditAssessmentType", reguestParams)
                .success(this, function (result) {
                    result.data().activity.execute().success(()=> {
                        const returnValue = result.data().activity.getOutButtonClicked();

                        if (returnValue === "Ok") {
                            const messages = (<DX.Mvc.DataAjaxResult>result).getDataValue<string>("message");
                            _self.showAlert(messages, "info", 1000);
                        }

                        _self._refreshGrid(_self._getGridId());
                    });
                });
        }

        private handlerGridAction_Delete(id: string) {
            this._invokeController("DeleteAssessmentType", { identifiableString: id })
                .success((result) => {
                    const messages = (<DX.Mvc.DataAjaxResult>result).getDataValue<string>("message");
                    this.showAlert(messages, "info", 1000);
                    this._refreshGrid(this._getGridId());
                });
        }
    }
}
