namespace DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared
{
	export class DestinationsGridTab extends GridTab
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.DestinationsGridTab";

		private readonly CATEGORY: string = 'Category';
		private readonly ACTIONS: string = 'Actions';
		private readonly PRODUCT: string = 'PRODUCT';
		private readonly PRODUCT_2: string = 'PRODUCT_2';
		private readonly PRODUCT_3: string = 'PRODUCT_3';
		private readonly COPRODUCT: string = 'COPRODUCT';
		private readonly COPRODUCT_2: string = 'COPRODUCT_2';
		private readonly FOOD_RESCUE: string = 'FOOD_RESCUE';
		private readonly PRODUCT_COPRODUCT_COL = 'ProductCoproductArray';

        private _errorMessage: string = '';
		private visibleColumns: Dictionary<number>;

		private parentsInAddingOrEditing: Dictionary<ModificationType> = Object.create(null) as Dictionary<ModificationType>;
		private savingInProgress: boolean = false;
		private otherDestinationTab: DestinationsGridTab = null

		public set OtherDestinationTab(reference: DestinationsGridTab)
		{
			this.otherDestinationTab = reference;
		}

		constructor(element: HTMLElement)
		{
			super(element);
		}

		public __applyJSONData()
		{
			// (OVERRIDE)
			super.__applyJSONData();

			this.visibleColumns = this.getCustomData<Dictionary<number>>("visibleColumns");
		}

		protected _initialize(): void
		{
			// (OVERRIDE)
			super._initialize();

			this.IsInEditMode = false;
            
			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "RowPercentageOverHundred")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this._errorMessage = result.text);
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
			.add_loadCompleted((data: JQueryExtensions.JQGrid.GridLoadCompleteData) =>
			{
				DX.Mvc.HtmlGrid.resizeGrid(this.JQGrid);
				this.IsInEditMode = false;
				this.parentsInAddingOrEditing = Object.create(null) as Dictionary<ModificationType>;

				this.RefreshAssessmentResultPanel();
				this.hideLoadingPanel();
			});

			this.$$<Mvc.ButtonControl>('btnManangeDestinations').add_click(() =>
			{
				const entityType = EntityType.Destination;

				this._invokeController<Mvc.TypedDataAjaxResult<{ activity: JSOpenDialogActivity }>>("ManageAssessmentRaiseDialog", { entityType: entityType, lcStageId: this.LcStage })
					.success(this, (result) =>
					{
						const activity = result.data().activity;
						activity.execute().success(this, () =>
						{
							if (activity.getButtonClicked() === 'Ok')
							{
								const closingDlgResponse = activity.getClosingDialogData().getReturnValues() as DestinationManagementDlgResponse;

								if (closingDlgResponse.success === false)
								{
									DevEx.HostPage.showAlert(closingDlgResponse.message, "error");
									throw new Error('DIRECT Destination Management: ' + closingDlgResponse.message);
								}

								this.showLoadingPanel();

								if (this.IsFoodDestination())
								{
									this.ChangeGridColumnsAfterManageDlg(closingDlgResponse.removedFood, closingDlgResponse.addedFood);
									this.otherDestinationTab.ChangeGridColumnsAfterManageDlg(closingDlgResponse.removedNonFood, closingDlgResponse.addedNonFood);
								}
								else
								{
									this.ChangeGridColumnsAfterManageDlg(closingDlgResponse.removedNonFood, closingDlgResponse.addedNonFood);
									this.otherDestinationTab.ChangeGridColumnsAfterManageDlg(closingDlgResponse.removedFood, closingDlgResponse.addedFood);
								}

								this.hideLoadingPanel();

								this.RefreshAssessmentResultPanel(true);

								DevEx.HostPage.showAlert(closingDlgResponse.message, "info");
							}
						});
					});
			});
		}

		public ChangeGridColumnsAfterManageDlg(removedCodes: string[], addedCodeSorts: Dictionary<number>)
		{
			removedCodes.forEach(code =>
			{
				if (this.visibleColumns[code])
				{
					delete this.visibleColumns[code];
					this.JQGrid.hideCol(code);

					this.JQGrid.getDataIDs()
						.filter(rowId => !rowId.startsWith('DxOutputCategory('))
						.forEach(rowId =>
						{
							this.JQGrid.setCell(rowId, code, '', undefined, undefined, true);
						});
				}
			});

			Object.keys(addedCodeSorts).forEach(code =>
			{
				if (typeof this.visibleColumns[code] === "undefined")
				{
					this.visibleColumns[code] = addedCodeSorts[code];
					this.JQGrid.showCol(code);
				}
			});

			this.Grid.resize();
		}

		// #region Helpers - tracking grid state

		private GetRowParentNodeId(rowId: string): string
		{
			return this.JQGrid.getLocalRow(rowId)['parent'] as string;
		}

		private GetRowProductCoproductArray(rowId: string): string
		{
			return this.JQGrid.getLocalRow(rowId)['ProductCoproductArray'] as string;
		}

		private IsParentNodeInModification(categoryNodeId: string, modType: ModificationType): boolean
		{
			if (typeof this.parentsInAddingOrEditing[categoryNodeId] !== 'undefined')
				return true;
			else
			{
				this.parentsInAddingOrEditing[categoryNodeId] = modType;
				return false;
			}
		}

		private RemoveParentNodeFromModification(rowId: string)
		{
			const parentNodeId = this.GetRowParentNodeId(rowId);

			if (typeof this.parentsInAddingOrEditing[parentNodeId] !== 'undefined')
				delete this.parentsInAddingOrEditing[parentNodeId];
		}

		// #endregion Helpers = tracking grid state

		// #region Generating Controls

		private GetDynamicControlLogicId(columnName: string, rowTempId: string): string
		{
			return `tmp${columnName.toString()}_${rowTempId}`;
		}

		private RetrieveRowCell(columnName: string, rowId: string): JQuery
		{
			return this.TableJQcontext.find(`tr#${this.EscapeId(rowId)} > td._${columnName}`);
		}

		private PlaceControlInCell(columnName: string, rowId: string, focusColumn: string, editData?: string | number): void
		{
			const logicId = this.GetDynamicControlLogicId(columnName, rowId);

			if (columnName !== this.CATEGORY && columnName !== this.ACTIONS)
			{
				const control = DX.Ctrl.newControl(DX.Mvc.NumericControl).setLogicId(logicId)
					.setSkin("SkForm").setStyle("width", "90%")
					.setReadOnly(false)
					.appendTo(this.RetrieveRowCell(columnName, rowId));

				if (typeof editData !== 'undefined')
					control.setValue(~(editData as string).indexOf('val') ? $(editData as string).data('val') as number : editData as number);

				const numValidationSettings = control.getValidationSettings() as Ctrl.NumericValidationSettings;
				numValidationSettings.setMaxLengthFrcPart(1);
				numValidationSettings.setMinValue(0);
				numValidationSettings.setMaxValue(100);
				control.setValidationSettings(numValidationSettings);

				control.initializeControl();

				if (columnName === focusColumn)
				{
                    control.focus();
				}
			}
		}

		// #endregion Generating Controls

		public GetEditableColumns(productsCoProducsCellData: Object): Dictionary<number>
		{
			const prodCoprodCodesEditable: string[] = JSON.parse(htmlDecode(productsCoProducsCellData as string)) as string[];

			const productCoproductFoodRescueCols: string[] = [this.PRODUCT, this.PRODUCT_2, this.PRODUCT_3, this.COPRODUCT, this.COPRODUCT_2, this.FOOD_RESCUE];

			const partOfProdCoprod: boolean = prodCoprodCodesEditable.length > 0;
			// add food rescue column  as it's allways visible
			prodCoprodCodesEditable.push(this.FOOD_RESCUE);

			// build list of editable columns
			const editableColumns: Dictionary<number> = {};

			if (partOfProdCoprod)
			{
				for (const columnName in this.visibleColumns)
				{
					if ((productCoproductFoodRescueCols.indexOf(columnName) !== -1 && prodCoprodCodesEditable.indexOf(columnName) !== -1)
						|| productCoproductFoodRescueCols.indexOf(columnName) === -1)
					{
						editableColumns[columnName] = this.visibleColumns[columnName];
					}
				}
			}
			else
			{
				Object.keys(this.visibleColumns)
					.filter(prop => productCoproductFoodRescueCols.indexOf(prop) === -1)
					.forEach(prop => editableColumns[prop] = this.visibleColumns[prop]);
			}

			return editableColumns;
		}

		public EditRow_GridAction(rowId: string, iCol?: number)
        {
            if (this.IsInEditMode)
                return;

			// (OVERRIDE)
			const rowData = this.JQGrid.getLocalRow(rowId);

			if (this.IsParentNodeInModification(rowData['parent'] as string, ModificationType.Edit))
				return;

			super.EditRow_GridAction(rowId);

			rowData.Category = $(rowData.Category).siblings('a').first().data('identifiable') as string;

			const rowEditData = { ...rowData };

			let rowObj: Dictionary<string> = {};

			for (const visibleColumn in this.visibleColumns)
			{
                if (visibleColumn !== this.CATEGORY)
                    rowObj[visibleColumn] = '';
			}

            this.JQGrid.setRowData(rowId, rowObj);
            this.InsertLinksAfterActionsCellDiv(rowId, this.BuildActionLinks(rowId, DX.OOP.getTypeName(this)));

			const editableColumns: Dictionary<number> = this.GetEditableColumns(rowEditData[this.PRODUCT_COPRODUCT_COL]);
			const columnToFocus: string = this.GetColumnToFocus(editableColumns, iCol);

			for (const columnName in editableColumns)
			{
				this.PlaceControlInCell(columnName, rowId, columnToFocus, rowEditData[columnName] as string);
			}
		}

		private GetColumnToFocus(editableColumns: Dictionary<number>, iCol?: number): string
		{
			let index: number;
			let smallestIndex = Number.MAX_VALUE;
			let columnNameWithSmallestIndex: string;

			for (const columnName in editableColumns)
			{
				index = editableColumns[columnName];
				if (index === 0)
					continue;

				if (typeof iCol !== 'undefined' && index === iCol)
				{
					return columnName; // FOUND
				}
				else if (index < smallestIndex)
				{
					smallestIndex = index;
					columnNameWithSmallestIndex = columnName;
				}
			}
			// column by iCol index NOT FOUND - take FOUND or fallback to OTHER column
			return smallestIndex !== Number.MAX_VALUE ? columnNameWithSmallestIndex : 'OTHER';
		}

		private IsFoodDestination(): boolean
		{
			switch (this.GridID)
			{
				case 'DIRModule_DestinationsTabGrid':
					return true;
				case 'DIRModule_NonFoodDestinationsTabGrid':
					return false;
				default:
					throw new Error('New destination GridID introduced, correctly implement isFood func resolution.');
			}
		}

		//#region Dynamic Actions handlers (Save, Cancel)

		private CollectRowData(rowId: string): { valid: boolean, requestData: DestinationItem }
		{
			const parentId = this.GetRowParentNodeId(rowId);

			const requestData: DestinationItem = {
				InputId: rowId,
				OutputCategoryId: parentId,
				LcStageId: this.LcStage,
				DestinationPercentage: []
			};

			const editableColumns = this.GetEditableColumns(this.GetRowProductCoproductArray(rowId));
			let sum: number = 0;
			for (const columnName in editableColumns)
			{
				const cell: HTMLElement = this.RetrieveRowCell(columnName, rowId)[0];
				if (columnName !== this.CATEGORY && columnName !== this.PRODUCT_COPRODUCT_COL && columnName !== this.ACTIONS)
				{
					const ctrl = DX.Ctrl.findControlOfType(cell, DX.Mvc.NumericControl, false);
					if (ctrl != null)
					{
                        $(cell).find(":input").trigger('blur');
						const percentage: number = (ctrl.getValue() == null || typeof ctrl.getValue() != 'number') ? 0 : ctrl.getValue();
						requestData.DestinationPercentage.push({ DestinationCode: columnName, Percentage: percentage })
						sum += percentage;
					}
				}
			}

			const valid = this.ValidateRowData(rowId, window.Math.round(sum * 10) / 10, editableColumns);
			return { valid, requestData };
		}

		private ValidateRowData(rowId: string, sum: number, editableColumns: Dictionary<number>): boolean
		{
			if (sum > 100 || (sum > 0 && sum < 100))
			{
				for (const columnName in editableColumns)
				{
					const cell: HTMLElement = this.RetrieveRowCell(columnName, rowId)[0];
					if (columnName !== this.CATEGORY && columnName !== this.PRODUCT_COPRODUCT_COL && columnName !== this.ACTIONS)
					{
						const ctrl = DX.Ctrl.findControlOfType(cell, DX.Mvc.NumericControl, false)
						if (ctrl !== null)
						{
							const percentage: number = (ctrl.getValue() == null || typeof ctrl.getValue() !== 'number') ? 0 : ctrl.getValue();
							if (percentage > 0)
							{
								ctrl.setIsValidated(false);
								ctrl.applyIsValidated();
							}
						}
					}
				}

				return false;
			}

			return true;
		}

		protected SaveRow(rowId: string): void
		{
			// (OVERRIDE)
			if (this.savingInProgress)
				return;

			let { valid, requestData }: { valid: boolean, requestData: DestinationItem } = this.CollectRowData(rowId);

			if (!valid)
			{
				DevEx.HostPage.showAlert(this._errorMessage, "error");
				this.EnableGridActions(rowId, true);
				return;
			}

			this.savingInProgress = true;
            super.SaveRow(rowId);
            this.showLoadingPanel();

			let isFood: boolean = this.IsFoodDestination();

			this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult & { ErrorMessage: string }>>("SaveDestinationRow", { destRow: JSON.stringify(requestData), isFood: isFood })
				.success(this, (result: DX.Mvc.TypedDataAjaxResult<BaseActionResult & { ErrorMessage: string }>) =>
				{
					const success = result.data().Success;
					const errorMessage = result.data().ErrorMessage;
					if (!success)
						DevEx.HostPage.showAlert(errorMessage, "error");

					this.RemoveParentNodeFromModification(rowId);

					this.GridDataChanged = true;
					this.ReloadGrid();
					this.savingInProgress = false;
                    this.CheckValidateAssessmentLcStage(this.LcStage);
				})
                .error(() =>
                {
                    this.savingInProgress = false
                    this.hideLoadingPanel();
                });
		}

		protected CancelRow(rowId: string): void
		{
			// (OVERRIDE)
            super.CancelRow(rowId);

            this.showLoadingPanel();

			for (const columnName in this.visibleColumns)
			{
				const cell: HTMLElement = this.RetrieveRowCell(columnName, rowId)[0];
				let ctrl: DX.Mvc.ObjRefControl | DX.Mvc.NumericControl = null;
				if (columnName !== this.CATEGORY && columnName !== this.ACTIONS)
				{
					ctrl = DX.Ctrl.findControlOfType(cell, DX.Mvc.NumericControl, false)
				}

				if (ctrl !== null)
					ctrl.dispose();
            }

			this.RemoveParentNodeFromModification(rowId);

			this.ReloadGrid();
		}

        public IsValid()
        {
            const rowsData = this.JQGrid.getGridParam('data') as Dictionary<string>[];

            for (let row of rowsData)
            {
                let sum = 0;

                for (let key in row)
                {
                    if (typeof row[key] === "string" && ~(row[key] as string).indexOf('val'))
                        sum += parseFloat($(row[key]).data('val'));
				}

				//prevents floating point number errors
				sum = parseFloat(sum.toFixed(2));

                if ((sum > 100 || (sum > 0 && sum < 100)))
                    return false;
            }

            return true;
        }

		//#endregion Dynamic Actions handlers (Save, Cancel)
	}

	enum ModificationType { Edit }

	type DestinationItem = {
		InputId: string,
		OutputCategoryId: string,
		LcStageId: number,
		DestinationPercentage: { DestinationCode: string, Percentage: number }[]
	}

	type DestinationManagementDlgResponse = DX.DIRModule.AssessmentNavigator.DestinationManagementDlgResponse;
}