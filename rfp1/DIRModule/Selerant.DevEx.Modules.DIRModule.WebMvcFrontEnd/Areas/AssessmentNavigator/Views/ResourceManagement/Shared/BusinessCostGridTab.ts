namespace DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared
{
    export class BusinessCostGridTab extends GridTab
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.BusinessCostGridTab";

		private readonly OTHER: string = 'OTHER';

		private reservedRowTempIds: number = 0;
		private rowDynamicControlsLogIds: Dictionary<Record<BusinessCostsGridColumn, string>> = Object.create(null) as Dictionary<Record<BusinessCostsGridColumn, string>>;
		private dynamicControlLogicIds: number = 0;

		private inAddingEditing: ModificationType = null;
		private savingInProgress: boolean = false;

		private businessCostJSON: string;
		private businessCostsWebItemsCollection: DX.Ctrl.WebListItemCollection = null;

		//#region Resource Texts

		private checkRedHighlightedMsg: string;
		private deleteBusinessCost: string;

		//#endregion Resource Texts

		private get NewRowTempId(): string
		{
			return `tmp_${(++this.reservedRowTempIds)}`;
		}

		private get TypeName(): string
		{
			return DX.OOP.getTypeName(this);
		}

		private get BusinessCostsWebItemsCollection()
		{
			if (this.businessCostsWebItemsCollection === null)
				this.businessCostsWebItemsCollection = DX.Ctrl.WebListItemCollection.parseJSon(this.businessCostJSON);
			return this.businessCostsWebItemsCollection;
		}

        private RetrieveRowCell(columnName: string, rowId: string): JQuery<HTMLElement>
		{
			return this.TableJQcontext.find(`tr#${this.EscapeId(rowId)} > td._${columnName}`);
		}

		constructor(element: HTMLElement)
		{
			super(element);
		}

		public __applyJSONData()
		{
			// (OVERRIDE)
			super.__applyJSONData();

			this.businessCostJSON = this._getJSONData('businessCostItems');
		}

		protected _initialize(): void
		{
			// (OVERRIDE)
			super._initialize();

			this.IsInEditMode = false;

			DX.Loc.TextDescriptor.newByResource("Global", "Invalid Value").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
			{
				this.checkRedHighlightedMsg = result.text;
			});

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "DeleteRowPromptMessage").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
			{
				this.deleteBusinessCost = result.text;
			});

		}

		public outerInitialize(): void
        {
            super.outerInitialize();

			this.add_initialized(() =>
			{
                this.Grid.add_actionExecuted((sender, args) =>
				{
					switch (args.key)
					{
						case "Edit":
							this.EditRow_GridAction(args.data["id"]);
							break;

						case "Delete":
							this.DeleteRow_GridAction(args.data["id"], ResourceRowType.BusinessCost);
							break;
					}
				})
				.add_loadCompleted((data: JQueryExtensions.JQGrid.GridLoadCompleteData) =>
                {
					this.IsInEditMode = false;
					this.inAddingEditing = null;
                    this.RefreshAssessmentResultPanel();
                    this.HideShowButtonNewBusinessCost();
					this.hideLoadingPanel();
				});
			})
        }

        public ChangeLcStage(lcStageId: number)
        {
            super.ChangeLcStage(lcStageId);
			this.RefreshBusinessCostDropdownItems();
			this.CheckValidateAssessmentLcStage(this.LcStage);
        }

		public AddBusinessCost()
		{
			this.IsInEditMode = true;

			if (this.IsInModification(ModificationType.Create))
				return;

			const rowId = this.NewRowTempId;
			this.CurrentRowId = rowId;

            this.JQGrid.addRowData(rowId, this.getEmptyData(rowId));
            this.InsertLinksAfterActionsCellDiv(rowId, this.BuildActionLinks(rowId, this.TypeName));

			this.RemoveCarriedOverBussinessCostLOVItems(); // remove Titles already present, they can be show as carried

			for (let columnName in BusinessCostsGridColumn)
			{
				this.PlaceControlInCell(DX.OOP.enumParse<BusinessCostsGridColumn>(columnName, BusinessCostsGridColumn), rowId, ModificationType.Create);
			}

			this.AddKeydownEvent(rowId);
		}

		public DeleteRow_GridAction(rowId: string, type: ResourceRowType)
		{
			this.showPrompt(this.deleteBusinessCost, () =>
			{
				this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult>>('DeleteRow', { rowIdentifiableString: rowId, rowType: type })
					.success((result: DX.Mvc.TypedDataAjaxResult<BaseActionResult>) =>
					{
						if (result.data().Success)
						{
							const data = this.JQGrid.getRowData(rowId) as BusinessCostRow;

                            const isOtherCost = $(data.Title).data('val') as string === this.OTHER

                            this.SetVisibilityOfOtherCosts(!isOtherCost);
							this.RefreshBusinessCostDropdownItems();
							this.GridDataChanged = true;
							this.ReloadGrid();
                        }
						else
							throw new Error('Deleting bussiness cost failed: ' + result.getData());
					});
			});
			
		}

		public EditRow_GridAction(rowId: string, iCol?: number)
		{
			// (OVERRIDE)
			if (this.IsInModification(ModificationType.Edit))
				return;

			super.EditRow_GridAction(rowId);

			const row = this.JQGrid.getRowData(rowId) as BusinessCostRow;

			// collect data
			const dataForEditing: Dictionary<string | number> = {
                Title: row.Title,
                Cost: $(row.Cost as string).data('val') as number
			};
			
			// generate empty row with action links
            this.JQGrid.setRowData(rowId, this.getEmptyData(rowId));
            this.InsertLinksAfterActionsCellDiv(rowId, this.BuildActionLinks(rowId, this.TypeName));

			const columnToFocus: BusinessCostsGridColumn = this.GetColumnToFocus(iCol);

			this.RemoveCarriedOverBussinessCostLOVItems(); // remove Titles already present, they can be show as carried

			for (let columnName in BusinessCostsGridColumn)
			{
				this.PlaceControlInCell(DX.OOP.enumParse<BusinessCostsGridColumn>(columnName, BusinessCostsGridColumn), rowId, ModificationType.Edit, dataForEditing[columnName], columnToFocus);
			}
		}

		private GetColumnToFocus(iCol?: number): BusinessCostsGridColumn
		{
			if (typeof iCol === 'undefined')
				return BusinessCostsGridColumn.Title;

			if (iCol === 1)
				return BusinessCostsGridColumn.Title;
			else if (iCol === 2)
				return BusinessCostsGridColumn.Cost;
			else
				return BusinessCostsGridColumn.Title;
		}

		private RemoveCarriedOverBussinessCostLOVItems()
		{
			let gridItems = this.JQGrid.getRowData() as { Title: string }[];
			if (gridItems.length)
			{
				const actualBusinessCosts = new Ctrl.WebListItemCollection();

				this.businessCostsWebItemsCollection.getItems().forEach(item =>
				{
					if (!gridItems.some(row => (~(row.Title as string).indexOf('val') ? $(row.Title).data('val') as string : null) === item.value))
						actualBusinessCosts.addItem(item.value, item.text);
				});
				this.businessCostsWebItemsCollection = actualBusinessCosts;
			}
		}

		//#endregion Dynamic Actions handlers (Save, Cancel)

		protected SaveRow(rowId: string): void
		{
			// (OVERRIDE)
			// prevent fast double click on Save action
			if (this.savingInProgress)
				return;
			this.savingInProgress = true;
			this.showLoadingPanel();
			
			if (this.ValidateControls(rowId))
			{
				super.SaveRow(rowId);

				const newResourceRow = this.CollectRowData(rowId, this.inAddingEditing);
				
				this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult>>('SaveBusinessCostRow', { businessCostRow: JSON.stringify(newResourceRow) })
					.success((result: DX.Mvc.TypedDataAjaxResult<BaseActionResult>) =>
					{
						if (result.data().Success)
						{
							this.inAddingEditing = null;
							delete this.rowDynamicControlsLogIds[rowId];

                            this.SetVisibilityOfOtherCosts(newResourceRow.Title === this.OTHER); 
							this.RefreshBusinessCostDropdownItems();
							this.GridDataChanged = true;
							this.ReloadGrid();
							this.savingInProgress = false;
						}
						else
						{
							const response: string = result.getData();
							this.hideLoadingPanel();
							throw new Error('Bussiness cost Insert/Update failed: ' + response);
                        }
                    }).error(() => this.hideLoadingPanel());
			}
			else
			{
				this.hideLoadingPanel();
				this.EnableGridActions(rowId, true);
				this.savingInProgress = false;
			}
        }

        private RefreshBusinessCostDropdownItems()
		{
            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ BusinessCostsJSON: string }>>('GetAvailableBusinessCostDropdownItems', { lcStage: this.LcStage })
                .success(result =>
				{
					this.businessCostsWebItemsCollection = DX.Ctrl.WebListItemCollection.parseJSon(result.data().BusinessCostsJSON);
					this.HideShowButtonNewBusinessCost();
                });
        }

        private HideShowButtonNewBusinessCost()
        {
            const toolbar = this.$$<DX.Mvc.HtmlMenu.Menu>('BusinessCostToolbar');
            const isDataAvailable = this.BusinessCostsWebItemsCollection !== null && this.BusinessCostsWebItemsCollection.getItemsCount() > 0;

            if (toolbar)
                toolbar.setIsVisible('NewBusinessCost', isDataAvailable);
        }

		private getEmptyData(rowId: string): Object
		{
            return {
                IdentifiableString: rowId,
				Title: "",
				Cost: "",
				Actions: ""
			};
		}

		private DisposeControls(rowId: string)
        {
            const bussinessCostCell: JQuery<HTMLElement> = this.RetrieveRowCell(BusinessCostsGridColumn.Title, rowId);
            bussinessCostCell.trigger('click');

			const controlsInRowLogicIds: Record<BusinessCostsGridColumn, string> = this.rowDynamicControlsLogIds[rowId];
			this.$$<DX.Mvc.LOVControl>(controlsInRowLogicIds.Title).dispose();
			this.$$<DX.Mvc.NumericControl>(controlsInRowLogicIds.Cost).dispose();
		}

		protected CancelRow(rowId: string): void
		{
			// (OVERRIDE)
			super.CancelRow(rowId);

			let reloadGrid = true;

			if (typeof this.rowDynamicControlsLogIds[rowId] !== 'undefined')
			{
				this.DisposeControls(rowId);

				if (this.inAddingEditing === ModificationType.Create)
					reloadGrid = false;

				// clean up tracking data
				this.inAddingEditing = null;
				delete this.rowDynamicControlsLogIds[rowId];

				if (!reloadGrid)
					this.JQGrid.delRowData(rowId);

				this.RefreshBusinessCostDropdownItems();
			}

            if (reloadGrid)
            {
                this.showLoadingPanel();
                this.ReloadGrid();
            }
		}

		//#endregion Dynamic Actions handlers (Save, Cancel)

		private IsInModification(modType: ModificationType): boolean
		{
			if (this.inAddingEditing !== null)
				return true;
			else
			{
				this.inAddingEditing = modType;
				return false;
			}
		}

		private PlaceControlInCell(columnName: BusinessCostsGridColumn, rowTempId: string, modType: ModificationType, editData?: string | number, columnToFocus?: BusinessCostsGridColumn): void
		{
			const logicId = this.GetDynamicControlLogicId(columnName, rowTempId);
			var control: DX.Mvc.NumericControl | DX.Mvc.LOVControl;

			if (columnName == BusinessCostsGridColumn.Title)
			{
				control = DX.Ctrl.newControl(DX.Mvc.LOVControl).setLogicId(logicId)
					.setSkin("SkForm").setStyle("width", "80%")
					.setItems(this.BusinessCostsWebItemsCollection)
					.setValidationSettings(new Ctrl.SimpleValueValidationSettings().setIsRequired(true))
					.appendTo(this.GetCellPlaceHolder(rowTempId, columnName));

				if (typeof editData !== 'undefined' && editData !== null)
				{
                    const cellJQ: JQuery<HTMLElement> = $(editData as string);
					const value = ~(editData as string).indexOf('val') ? cellJQ.data('val') as string : null;
					const text = cellJQ.text();
					if (value)
						control.getItems().addItem(value, text);
					control.setValue(value);
				}

				control.initializeControl();	
			}
			else // columnName == BusinessCostsGridColumn.Cost
			{
				const numValidationSettings = new Ctrl.NumericValidationSettings()
					.setIsRequired(true)
					.setMaxLengthFrcPart(2)
					.setMaxValue(1e10);

                control = DX.Ctrl.newControl(DX.Mvc.NumericControl).setLogicId(logicId)
					.setSkin("SkForm").setStyle("width", "80%")
					.setReadOnly(false)
					.setValidationSettings(numValidationSettings)
                    .setValue(editData as number)
                    .appendTo(this.GetCellPlaceHolder(rowTempId, columnName));

				control.initializeControl();
			}

			if (modType === ModificationType.Create)
				columnToFocus = BusinessCostsGridColumn.Title;

			if (columnName === columnToFocus)
				control.focus();
		}

		private GetDynamicControlLogicId(columnName: BusinessCostsGridColumn, rowTempId: string): string
		{
			const ctrlLogId = `tmp${columnName.toString()}_${this.dynamicControlLogicIds}_${rowTempId}`;

			if (typeof this.rowDynamicControlsLogIds[rowTempId] === 'undefined')
			{
				let ctrlColumn = {} as Record<BusinessCostsGridColumn, string>;
				ctrlColumn[columnName] = ctrlLogId;

				this.rowDynamicControlsLogIds[rowTempId] = ctrlColumn;
			}
			else
			{
				this.rowDynamicControlsLogIds[rowTempId][columnName] = ctrlLogId;
			}

			return ctrlLogId;
		}

		private GetCellPlaceHolder(rowId: string, columnName: BusinessCostsGridColumn): JQuery
		{
			return $(`tr#${this.EscapeId(rowId)} td._${columnName}`, this.JQGrid);
		}

		private ValidateControls(rowTempId: string): Boolean
        {
            this.RetrieveRowCell(BusinessCostsGridColumn.Cost, rowTempId).find(":input").trigger('blur');
            this.RetrieveRowCell(BusinessCostsGridColumn.Title, rowTempId).find(":input").trigger('blur');

			const controlsInRowLogicIds = this.rowDynamicControlsLogIds[rowTempId];

			const titleControl: DX.Mvc.LOVControl = this.$$<DX.Mvc.LOVControl>(controlsInRowLogicIds.Title);
			const costControl: DX.Mvc.NumericControl = this.$$<DX.Mvc.NumericControl>(controlsInRowLogicIds.Cost);

			if (![titleControl.validate(), costControl.validate()]
				.every(isValid => isValid === true))
            {
                DevEx.HostPage.showAlert(this.checkRedHighlightedMsg, "error", 2500);
				return false;
            }

            return true;
		}

		private CollectRowData(rowId: string, modType: ModificationType): BusinessCostRow
		{
			const requestData: Partial<BusinessCostRow> = {};

			requestData.LcStageId = this.LcStage;

			if (modType === ModificationType.Edit)
				requestData.IdentifiableString = rowId;

			const controlsInRowLogicIds = this.rowDynamicControlsLogIds[rowId];

			requestData.Cost = this.$$<DX.Mvc.NumericControl>(controlsInRowLogicIds.Cost).getValue();
            requestData.Title = this.$$<DX.Mvc.LOVControl>(controlsInRowLogicIds.Title).getValue();

			return requestData as BusinessCostRow;
		}

		private SetVisibilityOfOtherCosts(visibility: boolean)
        {
            const otherNotesEl = this.$().find("#BusinessCostOtherNotes");

            if (visibility)
                otherNotesEl.show();
            else
            {
                otherNotesEl.hide();
                this.ResourceNotes.BusinessCostOther = "";
            }
		}

	}

	enum BusinessCostsGridColumn
	{
		Title = 'Title',
		Cost = 'Cost'
	}

	type BusinessCostRow = {
		IdentifiableString?: string,
        LcStageId?: number,
        Title: string,
        Cost: number | string
	};
}