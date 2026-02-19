namespace DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared
{
    export class OutputsGridTab extends GridTab
    {
        static $$className = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.OutputsGridTab";

		//#region Resource Texts

        private outputCostInfoBoxDesc: string;
        private productValidationMessage: string;

		//#endregion Resource Texts

		constructor(element: HTMLElement)
		{
			super(element);
		}

        public __applyJSONData()
        {
            super.__applyJSONData();
        }

        protected _initialize(): void
        {
			super._initialize();

			this.IsInEditMode = false;

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "OutputsGridColumn_OutputCost_InfoBox")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.outputCostInfoBoxDesc = result.text);
            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ProductValidationMessage")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.productValidationMessage = result.text);
        }

        public outerInitialize(): void
        {
            super.outerInitialize();

			this.Grid.add_actionExecuted((sender, args) =>
			{
				switch (args.key)
				{
					case "Edit":
						this.EditRow_GridAction(args.data["id"]);
						break;
				}
			})
			.add_loadCompleted(() =>
			{
				const headerDiv = $('#jqgh_DIRModule_OutputsTabGrid_OutputCost');

				if (headerDiv.has('div.DX_Mvc_InfoBoxControl').length === 0)
				{
					DX.Ctrl.newControl(DX.Mvc.InfoBoxControl).setDescriptor(
						new DX.Ctrl.InfoBoxDescriptor()
							.setShowPin(true)
							.setFadeinSpeed(0)
							.setStartDocked(false)
							.setShowOnClick(false)
							.setType(DX.Ctrl.InfoBoxDescriptorTypes.Hint)
							.setInfoboxOffsetY(0)
							.setInfoboxOffsetX(0)
							.setInfoboxOffsetUsesMouse(false)
							.setInfoboxContent(this.outputCostInfoBoxDesc)
					).setContainer(headerDiv.get(0))
						.initializeControl()
						.appendTo(headerDiv);
				}

                this.IsInEditMode = false;
                this.RefreshAssessmentResultPanel();
                this.hideLoadingPanel();
			});
		}

		public EditRow_GridAction(rowId: string, iCol?: number)
		{
			// (OVERRIDE)
			if (this.IsInEditMode)
				return; 

			super.EditRow_GridAction(rowId);

            const rowData = this.JQGrid.getLocalRow(rowId) as OutputRow;

            const rowEditData: OutputRow = { ...rowData };

            this.JQGrid.setRowData(rowId, {
                OutputCost: '',
                Income: '',
                Actions: ''
            });
            this.InsertLinksAfterActionsCellDiv(rowId, this.BuildActionLinks(rowId, DX.OOP.getTypeName(this)));

			const columnToFocus: OutputGridColumn = this.GetColumnToFocus(iCol);
			let controlFocused = false;

            for (let columnName in OutputGridColumn)
            {
				controlFocused = this.PlaceControlInCell(columnName as OutputGridColumn, rowId, rowEditData[columnName], controlFocused, columnToFocus);
            }
		}

		private GetColumnToFocus(iCol?: number): OutputGridColumn
		{
			if (typeof iCol === 'undefined')
				return OutputGridColumn.OutputCost;

			if (iCol === 2)
				return OutputGridColumn.OutputCost;
			else if (iCol === 3)
				return OutputGridColumn.Income;
			else
				return OutputGridColumn.OutputCost;
		}        

		protected SaveRow(rowId: string): void
		{
			// (OVERRIDE)
            super.SaveRow(rowId);            
            this.showLoadingPanel();

            const outputRow = this.CollectRowData(rowId);

            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Success: string } & { ErrorMessage: string }>>('SaveOutputRow', { outputRow: JSON.stringify(outputRow) })
                .success(result =>
                {
                    const errorMessage = result.data().ErrorMessage;
                    if (errorMessage === "") 
                    {
                        this.GridDataChanged = true;
                        this.ReloadGrid();
                    }
                    else
                    {
                        DevEx.HostPage.showAlert(errorMessage, "error");
                        this.ReloadGrid();
                    }
                })
                .error(() =>
                {
                    this.hideLoadingPanel();
                });
        }

		protected CancelRow(rowId: string)
		{
			// (OVERRIDE)
			super.CancelRow(rowId);

            for (let columnName in OutputGridColumn)
            {
                const cell: HTMLElement = this.RetrieveRowCell(columnName as OutputGridColumn, rowId)[0];

                const ctrl = DX.Ctrl.findControlOfType(cell, DX.Mvc.NumericControl, false);

                if (ctrl !== null)
                    ctrl.dispose();
            }

            this.showLoadingPanel();
            this.ReloadGrid();
        }

		private PlaceControlInCell(columnName: OutputGridColumn, rowId: string, editData: string | number, controlFocused: boolean, columnToFocus: OutputGridColumn): boolean
		{
			const isEditable = ~(editData as string).indexOf('is-editable') ? $(editData as string).data('is-editable') as boolean : false
			if (isEditable)
			{
				const value = ~(editData as string).indexOf('val') ? $(editData as string).data('val') as number : null;

				const logicId = this.GetDynamicControlLogicId(columnName, rowId);

				const control = DX.Ctrl.newControl(DX.Mvc.NumericControl).setLogicId(logicId)
					.setSkin("SkForm").setStyle("width", "90%")
					.setReadOnly(false)
					.setValidationSettings(new Ctrl.NumericValidationSettings().setMinValue(0).setMaxLengthFrcPart(2).setMaxValue(1e10))
					.appendTo(this.RetrieveRowCell(columnName, rowId));

				control.setValue(value);
				control.initializeControl();

				// to focus in first input box
				if (!controlFocused || columnName === columnToFocus) 
				{
					control.focus();
					controlFocused = true;
				}
            }
            else
            {
                $(editData as string).appendTo(this.RetrieveRowCell(columnName, rowId));
			}

			return controlFocused;
        }

        private GetDynamicControlLogicId(columnName: OutputGridColumn, rowTempId: string): string
        {
            return `tmp${columnName.toString()}_${rowTempId}`;
        }

        private RetrieveRowCell(columnName: OutputGridColumn, rowId: string): JQuery
        {
            return this.TableJQcontext.find(`tr#${this.EscapeId(rowId)} > td._${columnName}`);
        }

        private CollectRowData(rowId: string)
        {
            const requestData: OutputRow = {};

            requestData.IdentifiableString = rowId;

            for (let columnName in OutputGridColumn)
            {
                const cell: HTMLElement = this.RetrieveRowCell(columnName as OutputGridColumn, rowId)[0];

                const ctrl = DX.Ctrl.findControlOfType(cell, DX.Mvc.NumericControl, false)

				if (ctrl !== null)
                {
                    $(cell).find(":input").trigger('blur');
                    const value = ctrl.getValue();
                    requestData[columnName] = value > 0 ? String(value) : '';
				}
				else
                {
                    const value = $(cell).find('span').data('val') as number;
                    requestData[columnName] = value;
                }
            }

            return requestData;
        }        
    }

    type OutputRow = {
        [key: string]: string | number,
        IdentifiableString?: string,
        Category?: string,
        OutputCost?: string,
        Income?: string,
    }

    enum OutputGridColumn
    {
        OutputCost = 'OutputCost',
        Income = 'Income'
    }
}