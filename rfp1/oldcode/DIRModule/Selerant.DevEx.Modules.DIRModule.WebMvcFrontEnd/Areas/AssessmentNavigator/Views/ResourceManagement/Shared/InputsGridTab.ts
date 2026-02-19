namespace DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared
{
	export class InputsGridTab extends GridTab
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.InputsGridTab";

		private readonly TEMP_ID_PREFIX = 'tmpId_';
		private readonly TOTALROW_ID = 'totalRow';
		// column codes/obj props
        private readonly CATEGORY = 'Category';
        private readonly CATEGORY_ACTION = 'CategoryAction';
		private readonly LOVMS_PARTOFPRODUCTCOPRODUCT = 'PartOfProductCoproduct';
		private readonly BOOL_PACKAGING = 'Packaging';
		private readonly MASS_COLUMNNAME = 'Mass';
		private readonly COST_COLUMNNAME = 'Cost';
		private readonly INEDIBLE_PARTS = 'InedibleParts';
		private readonly LOV_MEASUREMENT = 'Measurement';

		private readonly FOOD_TYPE = 'FOOD';
		private readonly NONFOOD_TYPE = 'NONFOOD';

		private readonly EditableColumnsFood: string[] =
			[
				this.CATEGORY,
				this.LOVMS_PARTOFPRODUCTCOPRODUCT,
				this.MASS_COLUMNNAME,
				this.COST_COLUMNNAME,
				this.INEDIBLE_PARTS, // FOOD only
				this.LOV_MEASUREMENT
			];
		private readonly EditableColumnsNonFood: string[] =
			[
				this.CATEGORY,
				this.LOVMS_PARTOFPRODUCTCOPRODUCT,
				this.BOOL_PACKAGING, // NONFOOD only
				this.MASS_COLUMNNAME,
				this.COST_COLUMNNAME,
				this.LOV_MEASUREMENT
			];

		private readonly ColumnsDecimalFraction: ReadonlyDictionary<number> = Object.freeze({
			'Mass': 3,
			'Cost': 2,
			'InedibleParts': 0
		});

		private partialViewContext: JQuery = null;
        private measurementJSON: string;
        private selectedMeasurement: string;
		private measurementWebItemsCollection: DX.Ctrl.WebListItemCollection = null;
		private addIconUrl: string;
		private addMaterialIconUrl: string;
		private inputCategoryToolbar: DX.Mvc.HtmlMenu.Menu = null;

		private rowIdModificationStatus: Dictionary<ModificationType> = Object.create(null) as Dictionary<ModificationType>;
		private reservedRowTempIds: number = 0;
		private canCreateMaterial: boolean;
		private categoryKeydownFunc: (event: KeyboardEvent) => void = null;
		private inAddingNewCategory: boolean;

		private currencyDisplayFormat: string;
		private massNumericFormat: Loc.NumericFormatData = null;
		private costNumericFormat: Loc.NumericFormatData = null;

		private partOfProductsCoProductsWebItemsCollection: DX.Ctrl.WebListItemCollection = null;

		private get PartialViewContext(): JQuery
		{
			if (this.partialViewContext === null)
				this.partialViewContext = DX.DOM.findJQElements(`${this.ViewContainerClass} .res-mgmt-partial-input-tab`);
			return this.partialViewContext;
		}

        private get NewRowTempId(): string
		{
			return `${this.TEMP_ID_PREFIX}${(++this.reservedRowTempIds)}`;
        }

		private get InputCategoryToolbar(): DX.Mvc.HtmlMenu.Menu
		{
			if (this.inputCategoryToolbar === null)
				this.inputCategoryToolbar = this.$$<DX.Mvc.HtmlMenu.Menu>('inputCategoryToolbar');
			return this.inputCategoryToolbar;
		}

		private get MeasurementWebItemsCollection()
		{
			if (this.measurementWebItemsCollection === null)
				this.measurementWebItemsCollection = DX.Ctrl.WebListItemCollection.parseJSon(this.measurementJSON);
			return this.measurementWebItemsCollection;
		}

		private get AddIconUrl(): string
		{
			return this.addIconUrl;
		}

		private get AddMaterialIconUrl(): string
		{
			return this.addMaterialIconUrl;
		}

		private get TypeName(): string
		{
			return DX.OOP.getTypeName(this);
		}

		private set MassNumericFormat(value: DX.Loc.NumericFormatData)
		{
			this.massNumericFormat = value.clone();
			this.massNumericFormat.setDecimalDigits(this.ColumnsDecimalFraction[this.MASS_COLUMNNAME]);
		}
		private get MassNumericFormat(): DX.Loc.NumericFormatData
		{
			return this.massNumericFormat;
		}

		private set CostNumericFormat(value: DX.Loc.NumericFormatData)
		{
			this.costNumericFormat = value.clone();
			this.costNumericFormat.setDecimalDigits(this.ColumnsDecimalFraction[this.COST_COLUMNNAME]);
		}
		private get CostNumericFormat(): DX.Loc.NumericFormatData
		{
			return this.costNumericFormat;
		}
		
		private get CategoryKeydownFunc(): (event: KeyboardEvent) => void
		{
			if (this.categoryKeydownFunc === null)
				this.categoryKeydownFunc = (event: KeyboardEvent): void =>
				{
					if (event.type === this.keydownEvent)
					{
						if (event.key === this.enterKey)
						{
							(event.currentTarget as HTMLElement).id === 'panelCreateCategory' ?
								this.NewCategory() :
								this.AddExistingCategory(); // panelSelectExistingCategory
						}
						else if (event.key === this.escKey)
						{
							(event.currentTarget as HTMLElement).id === 'panelCreateCategory' ?
								this.CancelNewCategory() :
								this.CancelAddExistingCategory(); // panelSelectExistingCategory
						}
					}
				};
			return this.categoryKeydownFunc;
		}

		private get PartOfProductsCoProductsWebItemsCollection()
		{
			if (this.partOfProductsCoProductsWebItemsCollection === null)
			{
				const items: Ctrl.WebListItem[] = [
					{ value: 'PRODUCT', text: `${this.productLovOptionPrefix} 1` },
					{ value: 'PRODUCT_2', text: `${this.productLovOptionPrefix} 2` },
					{ value: 'PRODUCT_3', text: `${this.productLovOptionPrefix} 3` },
					{ value: 'COPRODUCT', text: `${this.coProductLovOptionPrefix} 1` },
					{ value: 'COPRODUCT_2', text: `${this.coProductLovOptionPrefix} 2` }
				];
				this.partOfProductsCoProductsWebItemsCollection = new Ctrl.WebListItemCollection(items);
			}
			return this.partOfProductsCoProductsWebItemsCollection;
		}

		//#region Resource Texts

		private categoryAddIconTitle: string;
		private deletePromptMsgInput: string;
		private deletePromptMsgCategory: string;
		private deletePromptMsgCategoryWithInputs: string;
		private addMaterialIconTitle: string;
		private newMaterialCreationPromptMessage: string;
		private newMaterialSuccessfullyCreated: string;
		private newMaterialFailedCreation: string;
		private newMaterialCreationWarning: string;
		private promptButtonYes: string;
		private promptButtonNo: string;
		private linkEditTitle: string;
		private linkEditText: string;
		private linkDeleteTitle: string;
		private linkDeleteText: string;
		private totalRowTitle: string;
		private productLovOptionPrefix: string;
        private coProductLovOptionPrefix: string;

        private productSourceTooltipPlaceholder: string;
        private productSourceResource: Dictionary<string> = {};

		//#endregion Resource Texts

		constructor(element: HTMLElement)
		{
			super(element);

			const type = DX.OOP.getType(this);

			DX.OOP.setPropertyValue(type, "_instance", this);
			DX.OOP.setPropertyValue(type, "getInstance", function () { return DX.OOP.getPropertyValue(type, "_instance"); });
		}

		public __applyJSONData()
		{
			// (OVERRIDE)
			super.__applyJSONData();
			this.measurementJSON = this._getJSONData('measurementItems');
            this.selectedMeasurement = this._getJSONData('selectedMeasurement');
			this.canCreateMaterial = this._getJSONData('canCreateMaterial');
			this.currencyDisplayFormat = this._getJSONData('currencyDisplayFormat');
		}

		protected _initialize(): void
		{
			// (OVERRIDE)
			super._initialize();

			this.IsInEditMode = false;

			this.addIconUrl = DX.Paths.mapVirtualPath('~/WebMvcModules/Content/Images/toolbar_icons/DxAddUltraDarkGrey.svg');
			this.addMaterialIconUrl = DX.Paths.mapVirtualPath('~/WebMvcModules/Content/Images/toolbar_icons/DxAddMaterialUltraDarkGrey.svg');

			if (this.InputCategoryToolbar)
				this.InputCategoryToolbar.setIsVisible('AddExistingCategory', false);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "InputsGrid_CategoryAddIconTitle")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.categoryAddIconTitle = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_DeletePromptMsgInput")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.deletePromptMsgInput = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_DeletePromptMsgCategory")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.deletePromptMsgCategory = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_DeletePromptMsgCategoryWithInputs")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.deletePromptMsgCategoryWithInputs = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_AddMaterialIconTitle")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.addMaterialIconTitle = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_MaterialCreationPromptMessage")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.newMaterialCreationPromptMessage = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_SuccessfullyMaterialCreation")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.newMaterialSuccessfullyCreated = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_MaterialCreationFailed")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.newMaterialFailedCreation = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_MaterialCreationWarning")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.newMaterialCreationWarning = result.text);

			DX.Loc.TextDescriptor.newByResource("Controls", "DlgYes")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptButtonYes = result.text);

			DX.Loc.TextDescriptor.newByResource("Controls", "DlgNo")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptButtonNo = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_Controls", "GridAction_EditTT")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.linkEditTitle = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_Controls", "GridAction_Edit")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.linkEditText = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_Controls", "GridAction_DeleteTT")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.linkDeleteTitle = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_Controls", "GridAction_Delete")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.linkDeleteText = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_TotalRow")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.totalRowTitle = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "InputsGridPoPLovProductOptionBase")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.productLovOptionPrefix = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "InputsGridPoPLovCoProductOptionBase")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.coProductLovOptionPrefix = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "CarriedOverFromPreviousStageTT")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.productSourceTooltipPlaceholder = result.text);            

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ProductSource_P1")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.productSourceResource['P1'] = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ProductSource_P2")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.productSourceResource['P2'] = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ProductSource_P3")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.productSourceResource['P3'] = result.text);

			this.MassNumericFormat = DX.Loc.getCurrentUserNumericFormatData();
			this.CostNumericFormat = DX.Loc.getCurrentUserNumericFormatData();
		}

		public CategoryActionCellFunc(rowId: string, val: string, rawObject: CellValue[], cm: ReadonlyDictionary<CellValue | Function>, rdata: boolean): string
		{
			return 'style="padding: 0px; text-align: center; "';
		}

		public ReloadGrid()
		{
			// (OVERRIDE)
			super.ReloadGrid();

            this._afterReloadGrid();

            if (!this.IsProductCarried)
			    this._reloadInputCategories();

			this.ClearAllRowsModificationTracking();
		}

		private _afterReloadGrid()
        {
            if (this.canEditOrDelete)
            {
                this.CancelNewCategory();
                this.CancelAddExistingCategory();
            }
			
			this.CheckValidateAssessmentLcStage(this.LcStage);
			this.RefreshAssessmentResultPanel();
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

					case "Delete":
						this.DeleteRow_GridAction(args.data["id"], ResourceRowType.Input);
						break;

					case "DeleteInputCategory":
						this.DeleteInputCategory_GridAction(args.data["id"]);
						break;
				}
			})
			.add_loadCompleted((data: JQueryExtensions.JQGrid.GridLoadCompleteData) =>
			{
                DX.Mvc.HtmlGrid.resizeGrid(this.JQGrid);
                this.hideLoadingPanel();
			});
		}

		// #region Helpers & handlers

		private GetRowParentNodeId(rowId: string): string
		{
			return this.JQGrid.getLocalRow(rowId)['parent'] as string;
		}

		private GetParentCategoryType(parentRowId: string): string
		{
			return this.JQGrid.getLocalRow(parentRowId)['CategoryType'] as string;
		}

		private AddRowToModificationTracking(rowId: string, modType: ModificationType)
		{
			this.rowIdModificationStatus[rowId] = modType;

			this.IsInEditMode = true;
			this.CurrentRowId = rowId;
		}

		private IsExistingRowInEdit(rowId: string): boolean
		{
			const rowModType = this.rowIdModificationStatus[rowId];
			return typeof rowModType !== 'undefined' ? (rowModType === ModificationType.Edit) : false;
		}

		private RemoveRowFromModificationTracking(rowId: string)
		{
			if (typeof this.rowIdModificationStatus[rowId] !== 'undefined')
			{
				this.RemoveKeydownEvent(rowId);

				delete this.rowIdModificationStatus[rowId];

				this.RemoveErrorOutline(rowId);

				// used to update EditMode data
				let counter = 0, lastRowId: string = null;
				for (const propertyRowId in this.rowIdModificationStatus)
				{
					counter++;
					lastRowId = propertyRowId;
				}
				this.IsInEditMode = counter > 0;
				this.CurrentRowId = lastRowId;
			}
		}

		private ClearAllRowsModificationTracking(): void
		{
			this.rowIdModificationStatus = Object.create(null) as Dictionary<ModificationType>;

			this.IsInEditMode = false;
			this.CurrentRowId = null;
		}

		private _hideShowExistsAddCategory(numberExistsCategory: number)
		{
			if (this.InputCategoryToolbar)
				this.InputCategoryToolbar.setIsVisible('AddExistingCategory', numberExistsCategory !== 1);
		}

		// #endregion Helpers - tracking grid state

		// #region Grid cells manipulation

		private GetDynamicControlLogicId(columnName: string, rowTempId: string): string
		{
			return `inpt_${columnName.toString()}_${rowTempId}`;
		}

		private RetrieveCategoryRowCell(columnName: string, rowId: string): JQuery
		{
			const $span = DX.DOM.createElement("span").toJQ().addClass('cell-wrapper').css('display', 'inline-block')[0].outerHTML;
			this.JQGrid.setCell(rowId, columnName, $span);

			return this.TableJQcontext.find(`tr#${this.EscapeId(rowId)} td._${columnName} .cell-wrapper`);
		}

		private RetrieveRowCell(columnName: string, rowId: string): JQuery
		{
			return this.TableJQcontext.find(`tr#${this.EscapeId(rowId)} > td._${columnName}`);
		}

		private GetEditableColumns(categoryType: string)
		{
			if (categoryType === this.FOOD_TYPE)
				return this.EditableColumnsFood;
			else
				return this.EditableColumnsNonFood;
		}

		// returns columnName by index, by which input control will be focused, or returns default
		private GetColumnToFocus(categoryType: string, iCol?: number): string
		{
			if (typeof iCol === 'undefined')
				return this.CATEGORY;

			let focusableColumns: string[] = [];
			focusableColumns[1] = this.CATEGORY;
			focusableColumns[6] = this.MASS_COLUMNNAME;
			focusableColumns[7] = this.COST_COLUMNNAME;
			focusableColumns[10] = this.LOV_MEASUREMENT;

			if (categoryType === this.FOOD_TYPE)
				focusableColumns[9] = this.INEDIBLE_PARTS;

			return focusableColumns[iCol] ? focusableColumns[iCol] : focusableColumns[1];
		}

		private PlaceControlInCell(columnName: string, rowId: string, categoryType: string, modType: ModificationType, editData?: CellValue, columnToFocus?: string): void
		{
			const logicId = this.GetDynamicControlLogicId(columnName, rowId);

			if (typeof categoryType === 'undefined' || categoryType === null)
				categoryType = this.NONFOOD_TYPE;

			let control: CellControls = null;

			// #region Control placement

			switch (columnName)
			{
				case this.CATEGORY:
					{
						const filterMemeberCollection = new DX.Mvc.MvcFilterMemberCollection();
						const filterMember = new DX.Mvc.MvcFilterMember();
						filterMember.setName("type");
						filterMember.setType(SearchFilterType.Include);
						filterMember.setValues(['DIR_RESOURCE']);
						filterMemeberCollection.addItem(filterMember);

						const foodNonFoodFilterMember = new DX.Mvc.MvcFilterMember();
						foodNonFoodFilterMember.setName("Attributes[DXDIR_RESOURCE_TYPE]");
						foodNonFoodFilterMember.setType(SearchFilterType.Include);
						foodNonFoodFilterMember.setValues([categoryType]);
						filterMemeberCollection.addItem(foodNonFoodFilterMember);

						control = DX.Ctrl.newControl(DX.Mvc.ObjRefControl).setLogicId(logicId)
							.setSkin("SkForm").setStyle("width", "78%")
							.setScope('MATERIAL')
							.setFilterMembers(filterMemeberCollection)
							.setReadOnly(false)
							.setValidationSettings(new Ctrl.ObjectReferenceValidationSettings().setIsRequired(true))
                            .appendTo(this.RetrieveCategoryRowCell(columnName, rowId));

                        if (typeof editData !== 'undefined')
                            control.setValue(editData as string);

                        const eventHandler = () =>
                        {
                            this.JQGrid.setRowData(rowId, {
                                CategoryAction: null
                            });
                        };

                        control.add_valueChanged(eventHandler);
                        control.add_notifySetValue(eventHandler);

						control.__applyServerJSONData({ "ise": true });
						break;
                    }
                case this.CATEGORY_ACTION:
                    {
                        const productSourceShortened = this.GetProductSourceShortend(editData as string);
                        const tooltip = String.format(this.productSourceTooltipPlaceholder, this.productSourceResource[productSourceShortened]);

                        const productSource = DX.DOM.createElement("span").toJQ()
                            .attr('data-val', editData as string)
                            .attr('title', tooltip)
                            .text(this.GetProductSourceShortend(editData as string))[0].outerHTML;

                        this.JQGrid.setRowData(rowId,
                            {
                                CategoryAction: productSource
                            });
                        break;
                    }
				case this.LOVMS_PARTOFPRODUCTCOPRODUCT:
					{
						control = DX.Ctrl.newControl(DX.Mvc.LOVControl).setLogicId(logicId)
							.setSkin("SkForm lovMultiSelectJqGridFix")
							.setStyle("width", "78%")
							.setMultiValue(true)
							.setItems(this.PartOfProductsCoProductsWebItemsCollection)
							.appendTo(this.RetrieveRowCell(columnName, rowId).empty());

						// multiselect LOV needs to initlized to set values
						control.initializeControl();

						if (typeof editData !== 'undefined')
							control.setValues($(editData as string).data('val') as string[], undefined, true);

						break;
					}
				case this.BOOL_PACKAGING:
					{
						// don't display Packaging check box in food types
						if (categoryType === this.FOOD_TYPE)
							break;

						control = DX.Ctrl.newControl(DX.Mvc.BooleanControl).setLogicId(logicId)
							.setSkin("SkForm")
							.appendTo(this.RetrieveRowCell(columnName, rowId).empty());

						if (typeof editData !== 'undefined')
							control.setValue($(editData as string).data('val') as boolean);

						break;
					}
				case this.LOV_MEASUREMENT:
					{
						control = DX.Ctrl.newControl(DX.Mvc.LOVControl).setLogicId(logicId)
							.setSkin("SkForm").setStyle("width", "78%")
							.setValidationSettings(new Ctrl.SimpleValueValidationSettings().setIsRequired(true))
							.setItems(this.MeasurementWebItemsCollection)
							.appendTo(this.RetrieveRowCell(columnName, rowId).empty());

						if (typeof editData !== 'undefined')
							control.setValue($(editData as string).data('val') as string);
                        else
                            control.setValue(this.selectedMeasurement);

						break;
					}
				default: // numeric controls
					{
						const numValidationSettings = new Ctrl.NumericValidationSettings()
							.setMaxLengthFrcPart(this.ColumnsDecimalFraction[columnName]);

						if (columnName === this.INEDIBLE_PARTS)
						{
							numValidationSettings.setMinValue(0).setMaxValue(100);
						}
						else if (columnName === this.COST_COLUMNNAME)
						{
							numValidationSettings.setIsRequired(false);
							numValidationSettings.setMaxValue(1e10);
						}
						else // Mass column name
						{
							numValidationSettings.setIsRequired(true);
							numValidationSettings.setIsZeroValueAllowed(false);
							numValidationSettings.setMaxValue(1e10);
						}

						control = DX.Ctrl.newControl(DX.Mvc.NumericControl).setLogicId(logicId)
							.setSkin("SkForm").setStyle("width", "90%")
							.setReadOnly(false)
							.setValidationSettings(numValidationSettings)
							.appendTo(this.RetrieveRowCell(columnName, rowId).empty());

						if (typeof editData !== 'undefined')
							control.setValue(~(editData as string).indexOf('val') ? $(editData as string).data('val') as number : null);
					}
			}

			// #endregion Control placement

			if (control !== null)
			{
				if (columnName !== this.LOVMS_PARTOFPRODUCTCOPRODUCT) // no need to double initlized
					control.initializeControl();

				if (typeof columnToFocus === 'undefined')
					columnToFocus = this.CATEGORY;

				if (columnName === columnToFocus)
				{
					control.focus();
				}

				if (columnName === this.CATEGORY)
				{
					this.PlaceMaterialCreateIcon(rowId, control as Mvc.ObjRefControl);
				}
			}
		}

        PlaceValueInCell(columnName: string, rowId: string, data: CellValue)
        {
            let control: DX.Mvc.ObjRefControl | DX.Mvc.LOVControl | DX.Mvc.BooleanControl | DX.Mvc.NumericControl;
            const cell: HTMLElement = this.RetrieveRowCell(columnName, rowId)[0];

            switch (columnName)
            {
                case this.CATEGORY:
                    control = DX.Ctrl.findControlOfType(cell, DX.Mvc.ObjRefControl, true);
                    control.setValue(data as string);
                    break;
				case this.LOVMS_PARTOFPRODUCTCOPRODUCT:
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.LOVControl, false);
					control.setValues(data as string[], undefined, true);
					break;
                case this.BOOL_PACKAGING:
                    control = DX.Ctrl.findControlOfType(cell, DX.Mvc.BooleanControl, false);
                    control.setValue(data as boolean);
                    break;
                case this.LOV_MEASUREMENT:
                    control = DX.Ctrl.findControlOfType(cell, DX.Mvc.LOVControl, false);
                    control.setValue(data as string);
                    break;
                default:
                    control = DX.Ctrl.findControlOfType(cell, DX.Mvc.NumericControl, false);
                    control.setValue(data as number);
                    break;

            }
        }

		private PlaceMaterialCreateIcon(rowId: string, objRefControl: Mvc.ObjRefControl): void
		{
			if (this.canCreateMaterial)
			{
				const self = this;
				objRefControl.setOnQuickSearchShown(function ()
				{
                    if ((this as DX.QuickSearch.Box).getItems().length == 0)
                        self.JQGrid.setRowData(rowId,
                            {
                                CategoryAction: self.BuildAddMateralAction(rowId, self.TypeName)
                            });
                    else
                    {
                        const { CategoryAction } = self.JQGrid.getRowData(rowId) as { CategoryAction: string };

                        self.JQGrid.setRowData(rowId,
                            {
                                CategoryAction: CategoryAction
                            });
                    }
				});
			}
		}

		private AddEmptyRow(rowId: string, parentNodeId: string)
		{
			this.JQGrid.addChildNode(rowId, parentNodeId,
				{
					IdentifiableString: "",
					Category: "",
					Mass: "",
					Cost: "",
					Food: "",
					InedibleParts: "",
					Measurement: "",
                    Actions: ""
                });
            this.InsertLinksAfterActionsCellDiv(rowId, this.BuildActionLinks(rowId, this.TypeName));
		}

		private _reloadInputCategories()
		{
			this._invokeController('ReloadInputCategories', { lcStage: this.LcStage })
				.success(this, (result: Dictionary<Object>) =>
				{
					const existingInputCategories: DX.Ctrl.WebListItemCollection = DX.Ctrl.WebListItemCollection.parseJSon(result["InputCategoryTypes"]);

					const lovExistingCategory = this.$$<Mvc.ListOfValuesExtendedControl>('lovExistingCategory');
					if (lovExistingCategory != undefined)
					{
						lovExistingCategory.getItems().clear();
						lovExistingCategory.setItems(existingInputCategories);
					}

					this._hideShowExistsAddCategory(existingInputCategories.getItemsCount());
				});
		}

        private GetProductSourceShortend(productSource: string)
        {
            if (typeof productSource === 'undefined' || productSource === null || productSource === '')
                return '';

            let productSourceShortend = productSource.substr(0, 1);
            const matcher = /\d+/;
            const matchResult = productSource.match(matcher);

            productSourceShortend += matchResult ? matchResult[0] : '1';

            return productSourceShortend;
        }

		public BuildAddMateralAction(rowId: string, typeName: string)
		{
			return DX.DOM.createElement('img')
				.addClass("category-icon-add")
				.setAttributes(
					{
						src: this.AddMaterialIconUrl,
						title: this.addMaterialIconTitle,
						onclick: `DX.Ctrl.findParentControlOfType(this, ${typeName}).AddMaterial('${rowId}');`
					})
				.toElement()
				.outerHTML;
		}

		// #endregion Grid cells manipulation

		//#region Grid Actions

		public AddMaterial(rowId: string): void
		{
			const cell: HTMLElement = this.RetrieveRowCell(this.CATEGORY, rowId)[0];
			const control: DX.Mvc.ObjRefControl = DX.Ctrl.findControlOfType(cell, DX.Mvc.ObjRefControl, true);
			const regex = new RegExp('[*]', 'g');
			const materialDescription: string = control.getQuickSearchTextBoxValue().replace(regex, '');

			if (materialDescription === '')
			{
				DevEx.HostPage.showAlert(this.newMaterialCreationWarning, "warning");
				return;
			}

			const rowData = this.JQGrid.getLocalRow(rowId) as InputRow;
			const categoryType = this.GetParentCategoryType(rowData.parent);

			this.showPrompt(String.format(this.newMaterialCreationPromptMessage, materialDescription), [
				{
					caption: this.promptButtonYes, type: 'confirm', func: () => { this.CreateMaterial(rowId, materialDescription, categoryType) }
				},
				{
					caption: this.promptButtonNo, type: 'cancel', func: () => { this.JQGrid.setRowData(rowId, { CategoryAction: '' }) }
				}
			], false);
		}

		public CreateMaterial(rowId: string, materialDescription: string, categoryType: string)
		{
			this.showLoadingPanel();
			this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Success: boolean, IdentifiableString: string }>>('CreateNewMaterial', { materialDescription: materialDescription, categoryType: categoryType })
				.success(this, (result: DX.Mvc.TypedDataAjaxResult<{ Success: boolean, IdentifiableString: string }>) =>
				{
					if (result.data().Success)
					{
						const cell: HTMLElement = this.RetrieveRowCell(this.CATEGORY, rowId)[0];
						var control: DX.Mvc.ObjRefControl = DX.Ctrl.findControlOfType(cell, DX.Mvc.ObjRefControl, true);

						control.setValue(result.data().IdentifiableString);
						this.JQGrid.setRowData(rowId,
							{
								CategoryAction: ''
							});
						this.hideLoadingPanel();
						DevEx.HostPage.showAlert(this.newMaterialSuccessfullyCreated, "info");
					}
					else
					{
						this.hideLoadingPanel();
						DevEx.HostPage.showAlert(this.newMaterialFailedCreation, "error");
					}
				})
				.error(() => this.hideLoadingPanel());
		}

        public AddRow(parentNodeId: string, rowData?: InputRow): void
		{
			const rowId = this.NewRowTempId;

			this.AddEmptyRow(rowId, parentNodeId);
			this.AddRowToModificationTracking(rowId, ModificationType.Create);

			const categoryType = this.GetParentCategoryType(parentNodeId);

			this.GetEditableColumns(categoryType).forEach(columnName =>
            {
                if (rowData)
                {
                    this.PlaceControlInCell(columnName, rowId, categoryType, ModificationType.Create);
                    this.PlaceValueInCell(columnName, rowId, rowData[columnName]);
                }
                else
                    this.PlaceControlInCell(columnName, rowId, categoryType, ModificationType.Create);
            });

            if (rowData && rowData.ProductSource)
                this.PlaceControlInCell(this.CATEGORY_ACTION, rowId, categoryType, ModificationType.Create, rowData.ProductSource);

			this.AddKeydownEvent(rowId);
        }

		public EditRow_GridAction(rowId: string, iCol?: number)
		{
			// (OVERRIDE)
			if (this.rowIdModificationStatus[rowId]) // on dbclick prevents accidentaly clearnig row values with new controls
				return; 

			this.AddRowToModificationTracking(rowId, ModificationType.Edit);

			const rowData = this.JQGrid.getLocalRow(rowId) as InputRow;

			// place material identifiable string in Category prop, to easly pass it in PlaceControlInCell by column name
			// in case of setting cell client side somehow it's retriving elements wrapped in span
			let categoryElems = $(rowData.Category);
			if (categoryElems.length < 3)
				categoryElems = categoryElems.children();
			rowData.Category = categoryElems.siblings('a').first().data('identifiable') as string;

			const categoryType = this.GetParentCategoryType(rowData.parent);

			const rowEditData: InputRow = { ...rowData }; // need to make copy, because below setRowData changes references

			this.JQGrid.setRowData(rowId,
				{
					Category: "",
					Mass: "",
					Cost: "",
					Food: "",
					InedibleParts: "",
					Measurement: "",
                    Actions: ""
                });
            this.InsertLinksAfterActionsCellDiv(rowId, this.BuildActionLinks(rowId, this.TypeName));

			const columnToFocus: string = this.GetColumnToFocus(categoryType, iCol);

			this.GetEditableColumns(categoryType).forEach(columnName =>
			{
				this.PlaceControlInCell(columnName, rowId, categoryType, ModificationType.Edit, rowEditData[columnName], columnToFocus);
            });

            if (rowEditData.ProductSource)
                this.PlaceControlInCell(this.CATEGORY_ACTION, rowId, categoryType, ModificationType.Edit, rowEditData.ProductSource);

			this.AddKeydownEvent(rowId);
		}

		public DeleteRow_GridAction(rowId: string, resourceType: ResourceRowType)
		{
			this.showPrompt(this.deletePromptMsgInput, () =>
			{
				this.showLoadingPanel();
				this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult>>('DeleteRow', { rowIdentifiableString: rowId, rowType: resourceType })
					.success((result: DX.Mvc.TypedDataAjaxResult<BaseActionResult>) =>
					{
						if (result.data().Success)
						{
							this.GridDataChanged = true;
							this.JQGrid.delTreeNode(rowId);

							this.AddUpdateOrRemoveTotalRow();

							this.hideLoadingPanel();
						}
						else
						{
							this.hideLoadingPanel();
							throw new Error('Deleting input failed: ' + result.getData());
						}
					})
					.error(() => this.hideLoadingPanel());

			}, false);
		}

		public DeleteInputCategory_GridAction(inputCategoryId: string)
		{
			type JQNodeBase = { expanded: boolean, isLeaf: boolean, _id_: string };

			const categoryRow = this.Grid.getDataRow(inputCategoryId) as { Category: string, CategoryType: string } & JQNodeBase;
			const inputs = this.JQGrid.getNodeChildren(categoryRow as JQNodeBase) as { _id_: string }[]

			const inputsIdents: string[] = inputs.map<string>(item => item._id_)
				.filter(item => item.indexOf(this.TEMP_ID_PREFIX) === -1); // exclude temp rows in adding

			const propmptMessage: string = (inputs.length == 0) ?
				this.deletePromptMsgCategory :
				this.deletePromptMsgCategoryWithInputs;

			const lovExistingCategory = this.$$<Mvc.ListOfValuesExtendedControl>('lovExistingCategory');

			const RetriveCategoryToDropDown = () =>
			{
				const items = lovExistingCategory.getItems().getItems();

				let retrivedCategory: Ctrl.WebListItem = {
					value: inputCategoryId,
					text: categoryRow.Category
				};

				if (categoryRow.CategoryType)
					retrivedCategory.Metadata = { type: categoryRow.CategoryType };

				items.push(retrivedCategory);
			};

			const RemoveTempRowInAdding = () =>
			{
				// remove temp in adding row (usually it's single)
				if (inputs.length)
				{
					inputs.map<string>(item => item._id_)
						.filter(item => item.indexOf(this.TEMP_ID_PREFIX) === 0)
						.forEach(tempInputId =>
						{
							this.DisposeControls(tempInputId, inputCategoryId);

							this.RemoveRowFromModificationTracking(tempInputId);

							this.JQGrid.delTreeNode(tempInputId);
						});
				}
			};

			this.showPrompt(propmptMessage, () =>
			{
				if (inputsIdents.length == 0)
				{
					// push category back to Add Existing Category
					RetriveCategoryToDropDown();
					this._hideShowExistsAddCategory(lovExistingCategory.getItems().getItemsCount());

					RemoveTempRowInAdding();
					// and just removed it from grid
					this.Grid.deleteRow(inputCategoryId);

					// remove last single row with hardcoded id of Total row,
					if (this.JQGrid.getDataIDs().every(rowId => this.TOTALROW_ID === rowId))
						this.Grid.deleteRow(this.TOTALROW_ID);
				}
				else
				{
					this.showLoadingPanel();
					this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult>>('DeleteInputs', { inputsIdentStrings: inputsIdents })
						.success((result: DX.Mvc.TypedDataAjaxResult<BaseActionResult>) =>
						{
							if (result.data().Success)
							{
								this.Grid.deleteRow(inputCategoryId);
								inputsIdents.forEach(inputRowId =>
								{
									if (this.IsExistingRowInEdit(inputRowId))
									{
										this.DisposeControls(inputRowId, inputCategoryId);
										this.RemoveRowFromModificationTracking(inputRowId);
									}
									this.Grid.deleteRow(inputRowId);
								});

								RemoveTempRowInAdding();

								// Add category back to drop down because it's not delete, push category back to Add Existing Category
								RetriveCategoryToDropDown();
								this._hideShowExistsAddCategory(lovExistingCategory.getItems().getItemsCount());

								this.GridDataChanged = true;

								this.AddUpdateOrRemoveTotalRow();
							}
							else
								throw new Error('Failed deleting Inputs');

							this.hideLoadingPanel();
						});
				}

				this.CancelAddExistingCategory();
			});
		}

		

		private CollectRowData(rowId: string, modType: ModificationType): { requestData: InputRow, valid: boolean }
		{
			const parentId = this.GetRowParentNodeId(rowId);
			const parentCategoryType = this.GetParentCategoryType(parentId);
			const validations: boolean[] = [];

			// result
			const requestData: InputRow = {
				CategoryIdentifiableString: parentId,
				LcStageId: this.LcStage,
				CategoryType: parentCategoryType
			};

			if (modType === ModificationType.Edit)
				requestData.IdentifiableString = rowId;

			let control: DX.Mvc.ObjRefControl | DX.Mvc.LOVControl | DX.Mvc.BooleanControl | DX.Mvc.NumericControl;

			const getBooleanFromCell: Function = (cell: HTMLElement): boolean =>
			{
				control = DX.Ctrl.findControlOfType(cell, DX.Mvc.BooleanControl, false);
				return (control.getValue() || false);
			}

			this.GetEditableColumns(parentCategoryType).forEach(columnName =>
			{
				const cell: HTMLElement = this.RetrieveRowCell(columnName, rowId)[0];

				if (columnName === this.CATEGORY)
				{
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.ObjRefControl, true);
					validations.push(control.validate());
					requestData['MaterialIdentifiableString'] = control.getValue();
				}
				else if (columnName === this.LOVMS_PARTOFPRODUCTCOPRODUCT)
				{
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.LOVControl, false);
					requestData[columnName] = control.getValues();
				}
				else if (columnName === this.BOOL_PACKAGING)
				{
					if (requestData.CategoryType === this.NONFOOD_TYPE)
						requestData[columnName] = getBooleanFromCell(cell) as boolean;
					else
						return;
				}
				else if (columnName === this.LOV_MEASUREMENT)
				{
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.LOVControl, false);
					validations.push(control.validate());
					requestData[columnName] = Number(control.getValue());
				}
				else
				{
                    control = DX.Ctrl.findControlOfType(cell, DX.Mvc.NumericControl, false);
                    $(cell).find(":input").trigger('blur');
					validations.push(control.validate());
					requestData[columnName] = control.getValue() || 0;
				}
            });

            let productSource: string = null;
            const productSourceCell: HTMLElement = this.RetrieveRowCell(this.CATEGORY_ACTION, rowId)[0];

            if ($(productSourceCell).find('span').length)
            {
                productSource = $(productSourceCell).find('span').data('val') as string;
                requestData.ProductSource = productSource
            }

			return {
				requestData: requestData,
				valid: validations.every(controlValid => controlValid)
			};
		}

		private DisposeControls(rowId: string, parentId: string)
		{
			this.GetEditableColumns(this.GetParentCategoryType(parentId)).forEach(columnName =>
			{
				const cell: HTMLElement = this.RetrieveRowCell(columnName, rowId)[0];
				let control: CellControls = null;

				if (columnName === this.CATEGORY)
				{
					cell.click(); // simulate click on cell to close possibly opened quickSearch box
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.ObjRefControl, true);
				}
				else if (columnName === this.LOVMS_PARTOFPRODUCTCOPRODUCT)
				{
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.LOVControl, false);
				}
				else if (columnName === this.BOOL_PACKAGING)
				{
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.BooleanControl, false);
				}
				else if (columnName === this.LOV_MEASUREMENT)
				{
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.LOVControl, false);
				}
				else
				{
					control = DX.Ctrl.findControlOfType(cell, DX.Mvc.NumericControl, false);
				}

				if (control !== null)
					control.dispose();
			});
		}

        private AddRowDirectly(rowId: string, modType: ModificationType, parentId: string, response: InputResponseModel): void
		{
			this.DisposeControls(rowId, parentId);

			const buildMaterialRefLink = (response: InputResponseModel): string =>
			{
				const imgTag = DX.DOM.createElement("img").toJQ()
					.attr('src', response.MaterialIconUrl);

				const aTag = DX.DOM.createElement("a").toJQ()
					.addClass('row-link-item')
					.attr({
						'data-identifiable': response.MaterialIdentifiable,
                        'href': 'javascript:void(0)',
						'onclick': response.MaterialOpenTabHrefScript
					})
					.text(response.MaterialDescription);

				return DX.DOM.createElement("span").toJQ()
					.addClass('cell-wrapper')
					.append(imgTag, ' ', aTag)[0].outerHTML;
			};

			const toClientEditable = (value: boolean | number, text: string): string =>
			{
				if (typeof value === 'object') // null value
					return '';

				return DX.DOM.createElement("span").toJQ()
					.attr('data-val', value.toString())
					.text(text)[0].outerHTML;
			};

			const toClientEditableArray = (value: string[], text: string): string =>
			{
				return DX.DOM.createElement("span").toJQ()
					.attr('data-val', JSON.stringify(value))
					.text(text)[0].outerHTML;
            };

            const toClientReadonly = (value: string, text: string): string =>
            {
                if (typeof value === 'object') // null value
                    return '';

                const tooltip = String.format(this.productSourceTooltipPlaceholder, this.productSourceResource[text]);

                return DX.DOM.createElement("span").toJQ()
                    .attr('data-val', value)
                    .attr('title', tooltip)
                    .text(text)[0].outerHTML;
            };

			const buildActionLink = (type: 'edit' | 'delete', rowId: string): string =>
			{
				let title: string, text: string, onclickScript: string;
				if (type === 'edit')
				{
					title = this.linkEditTitle;
					text = this.linkEditText;
					onclickScript = `DX.Ctrl.findParentControlOfType(this, ${this.TypeName}).EditRow_GridAction('${rowId}');`;
				}
				else
				{
					title = this.linkDeleteTitle;
					text = this.linkDeleteText;
					onclickScript = `DX.Ctrl.findParentControlOfType(this, ${this.TypeName}).DeleteRow_GridAction('${rowId}', ${ResourceRowType.Input});`;
				}

				return DX.DOM.createElement("a").toJQ()
					.addClass('row-action-item')
					.attr({
                        'href': 'javascript:void(0)',
						'title': title,
						'onclick': onclickScript
					})
					.css('white-space', 'nowrap')
					.text(text)[0].outerHTML;
			};

			let cellData = {
				IdentifiableString: response.IdentifiableString,
                Category: buildMaterialRefLink(response),
                CategoryAction: response.ProductSource ? toClientReadonly(response.ProductSource, this.GetProductSourceShortend(response.ProductSource)) : null,
				PartOfProductCoproduct: toClientEditableArray(response.PartOfProductCoproductCodes, response.PartOfProductCoproductFormatted),
				Packaging: toClientEditable(response.Packaging, response.PackagingFormatted),
				Mass: toClientEditable(response.Mass, response.MassFormatted),
				Cost: toClientEditable(response.Cost, response.CostFormatted),
				Food: response.FoodFormatted,
				InedibleParts: toClientEditable(response.InedibleParts, response.InediblePartsFormatted),
				Measurement: toClientEditable(response.Measurement, response.MeasurementDisplayValue),
                Actions: ""
			};

			if (modType === ModificationType.Create)
			{
				this.JQGrid.delTreeNode(rowId);
				this.JQGrid.addChildNode(response.IdentifiableString, parentId, cellData);
			}
			else // ModificationType.Edit
                this.JQGrid.setRowData(rowId, cellData);

            const actions = `${buildActionLink('edit', response.IdentifiableString)} ${buildActionLink('delete', response.IdentifiableString)}`;
            this.InsertLinksAfterActionsCellDiv(response.IdentifiableString, actions);

			this.EnableGridActions(response.IdentifiableString, true);
		}

		private AddUpdateOrRemoveTotalRow(): void
		{
			type TotalRowData = { Mass: string, Cost: string, _id_?: string };

			const gridData = this.Grid.getData() as TotalRowData[];

			// if there aren't any inputs in grid, remove totalRow
			if (!gridData.some(x => x._id_.startsWith('DxInput(')))
			{
				this.Grid.deleteRow(this.TOTALROW_ID);
				return;
			}

			// calc latest totalRow sums from grid data
			const total = gridData.reduce((acc, currVal) =>
			{
				// only saved input rows and which are not in Edit
				if (currVal._id_.startsWith('DxInput(') && typeof this.rowIdModificationStatus[currVal._id_] === 'undefined')
				{
					acc.Mass += $(currVal.Mass).data('val') as number;
					acc.Cost += $(currVal.Cost).data('val') as number;
					return acc;
				}
				else
					return acc;
			}, { Mass: 0.0, Cost: 0.0 });

			// take care of js float number precision
			total.Mass = +total.Mass.toFixed(this.ColumnsDecimalFraction[this.MASS_COLUMNNAME]);
			total.Cost = +total.Cost.toFixed(this.ColumnsDecimalFraction[this.COST_COLUMNNAME]);

			const decimalFormatter = (amount: number, numericFormatData: DX.Loc.NumericFormatData): string =>
				DX.htmlEncode(DX.Loc.NumericFormatData.format(amount, numericFormatData, Loc.NumericFormats.Fixed));

			// used to create data for Mass and Cost grid cells
			const totalRowDataBuilder = (mass: number, cost: number): TotalRowData =>
			{
				const massFormattedText = decimalFormatter(mass, this.MassNumericFormat);
				const costFormattedText = this.currencyDisplayFormat.replace("{0}", decimalFormatter(cost, this.CostNumericFormat));
				return {
					Mass: DX.DOM.createElement("span").toJQ().attr('data-val', mass).text(massFormattedText)[0].outerHTML,
					Cost: DX.DOM.createElement("span").toJQ().attr('data-val', cost).text(costFormattedText)[0].outerHTML
				};
			};

			const currentTotalRow = gridData.filter(x => x._id_ === this.TOTALROW_ID);
			if (currentTotalRow.length > 0)
			{
				// found exising total row
				const currTotalMass = $(currentTotalRow[0].Mass).data('val') as number;
				const currTotalCost = $(currentTotalRow[0].Cost).data('val') as number;
				// refresh exising total row with new data
				if (total.Mass !== currTotalMass || total.Cost !== currTotalCost)
				{
					this.JQGrid.setRowData(this.TOTALROW_ID, totalRowDataBuilder(total.Mass, total.Cost));
				}
			}
			else
			{
				const newTotalRowData = totalRowDataBuilder(total.Mass, total.Cost);

				this.JQGrid.addRowData(this.TOTALROW_ID, {
					IdentifiableString: this.TOTALROW_ID,
					Category: this.totalRowTitle,
					Mass: newTotalRowData.Mass,
					Cost: newTotalRowData.Cost
				}, 'last'); // add as last row
			}
		}

		// Save, Cancel

		protected SaveRow(rowId: string): void
		{
			// (OVERRIDE)
			const modType = this.rowIdModificationStatus[rowId];
			const { requestData, valid } = this.CollectRowData(rowId, modType);

			if (valid)
			{
				this.showLoadingPanel();
				this._invokeController<DX.Mvc.TypedDataAjaxResult<EntityActionResponse>>('SaveInputRow', { inputRow: JSON.stringify(requestData) })
					.success((result) =>
					{
						if (result.data().Success)
						{
							this.RemoveRowFromModificationTracking(rowId);

							this.GridDataChanged = true;

							this.AddRowDirectly(rowId, modType, requestData.CategoryIdentifiableString, result.data().Entity);
							this.AddUpdateOrRemoveTotalRow();

							this.hideLoadingPanel();

							if (result.data().DestinationDeletedMsg)
								DevEx.HostPage.showAlert(result.data().DestinationDeletedMsg, "warning");

							this._afterReloadGrid();
						}
						else
						{
							this.hideLoadingPanel();
							const message: string = result.data().Message;
							DevEx.HostPage.showAlert(message, "error");
							this.ReloadGrid();
						}
					})
					.error(() => this.hideLoadingPanel());
			}
			else
			{
				this.EnableGridActions(rowId, true);
			}
		}

		protected CancelRow(rowId: string): void
		{
			// (OVERRIDE)
			const retriveRowDataFromServer = this.IsExistingRowInEdit(rowId);

			this.showLoadingPanel();

			// clean up tracking data
			this.RemoveRowFromModificationTracking(rowId);

			const parentId = this.GetRowParentNodeId(rowId);
			this.DisposeControls(rowId, parentId);

			if (retriveRowDataFromServer)
			{
				const parentCategoryType = this.GetParentCategoryType(parentId);

				this._invokeController<DX.Mvc.TypedDataAjaxResult<EntityActionResponse>>('GetInputRow', { inputIdentifiableString: rowId, inputCategoryType: parentCategoryType })
					.success((result) =>
					{
						if (result.data().Success)
						{
							this.AddRowDirectly(rowId, ModificationType.Edit, parentId, result.data().Entity);
							this.AddUpdateOrRemoveTotalRow();
							this.hideLoadingPanel();
						}
						else
						{
							this.hideLoadingPanel();
							throw new Error('Error canceling: ' + result.getData());
						}
					})
					.error(() => this.hideLoadingPanel());
			}
			else
			{
				this.JQGrid.delTreeNode(rowId);
				this.hideLoadingPanel();
			}

			this.EnableGridActions(rowId, true);
		}

		//#endregion Dynamic Actions handlers (Save, Cancel)

		// #region Category section

		public ShowAddExistingCategory(show: boolean = true)
		{
			if (show)
			{
				this.PartialViewContext.find('#panelCategoryButtons').hide();
				this.AddKeydownEventCategorySection(this.PartialViewContext.find('#panelSelectExistingCategory').show());
			}
			else
			{
				this.PartialViewContext.find('#panelCategoryButtons').show();
				this.RemoveKeydownEventCategorySection(this.PartialViewContext.find('#panelSelectExistingCategory').hide());
			}
		}

		public ShowNewCategory(show: boolean = true)
		{
			if (show)
			{
				this.PartialViewContext.find('#panelCategoryButtons').hide();
				this.AddKeydownEventCategorySection(this.PartialViewContext.find('#panelCreateCategory').show());
			}
			else
			{
				this.PartialViewContext.find('#panelCategoryButtons').show();
				this.RemoveKeydownEventCategorySection(this.PartialViewContext.find('#panelCreateCategory').hide());
			}
		}

		AddInputCategoryToGrid(identifiable: string, title: string, typeCode: string)
		{
			const categoryAddIcon = DX.DOM.createElement('img')
				.addClass("category-icon-add")
				.setAttributes(
					{
						src: this.AddIconUrl,
						title: this.categoryAddIconTitle,
						onclick: `DX.Ctrl.findParentControlOfType(this, ${this.TypeName}).AddRow('${identifiable}');`
					})
				.toElement()
				.outerHTML;

			const deleteActionLink = DX.DOM.createElement("a")
				.addClass('row-action-item')
				.setAttributes(
					{
						'data-dxlogicid': identifiable + '_Cancel',
                        'href': 'javascript:void(0)',
						'onclick': `DX.Ctrl.findParentControlOfType(this, ${this.TypeName}).DeleteInputCategory_GridAction('${identifiable}');`
					})
				.setText(this.deleteActionText)
				.toElement()
				.outerHTML;

			// to move Total row to bottom, fetch it here and remove
			const totalRow = this.Grid.getDataRow(this.TOTALROW_ID);
			this.Grid.deleteRow(this.TOTALROW_ID);

			// add category row
			this.JQGrid.addChildNode(identifiable, null, {
				IdentifiableString: identifiable,
				Category: title,
				CategoryType: typeCode,
				CategoryAction: categoryAddIcon,
                Actions: ""
            });
            this.InsertLinksAfterActionsCellDiv(identifiable, deleteActionLink);

			// then add Total row after last
			this.JQGrid.addRowData(this.TOTALROW_ID, totalRow, 'after', identifiable);
		}

		private AddKeydownEventCategorySection(section: JQuery): void
		{
			section.attr('tabindex', -1).get(0).addEventListener(this.keydownEvent, this.CategoryKeydownFunc);
		}

		private RemoveKeydownEventCategorySection(section: JQuery): void
        {
            section.removeAttr('tabindex').trigger('click').get(0).removeEventListener(this.keydownEvent, this.CategoryKeydownFunc);
			this.categoryKeydownFunc = null;
		}

        // #endregion Category section

        // #region Add Existing Category

        public AddExistingCategory()
        {
            const lovExistingCategory = this.$$<Mvc.ListOfValuesExtendedControl>('lovExistingCategory');

            if (lovExistingCategory.validate())
            {
                let selectedItems: Ctrl.WebListItem[] = null;
                // DX.Mvc.ListOfValuesExtendedControl.SelectedItems - control property is bugged
                if (lovExistingCategory.getValues() && lovExistingCategory.getValues().length > 0)
                {
                    const selectedValues: string[] = lovExistingCategory.getValues().map(val => val.toString()); // internally in JS it's number
                    selectedItems = lovExistingCategory.getItems().getItems().filter(x => selectedValues.indexOf(x.value) !== -1);
                }

                if (selectedItems && selectedItems.length > 0)
                {
                    const item = selectedItems[0];

                    this.AddInputCategoryToGrid(item.value, item.text, (typeof item.Metadata !== 'undefined') ? item.Metadata['type'] : null);

                    this.ShowAddExistingCategory(false);

                    lovExistingCategory.setEmptyValue();
					lovExistingCategory.getItems().removeItemsByValue(item.value);

					this._hideShowExistsAddCategory(lovExistingCategory.getItems().getItemsCount());
                }
            }
        }

        public CancelAddExistingCategory()
        {
            this.ShowAddExistingCategory(false);
            // clear values
            const lovExistingCategory = this.$$<Mvc.ListOfValuesExtendedControl>('lovExistingCategory');
            if (lovExistingCategory != undefined)
                lovExistingCategory.setEmptyValue();
        }

        // #endregion Add Existing Category

        // #region New Category

		public NewCategory()
		{
			type CreateInputCategoryActionResult = BaseActionResult & { InputCategoryIdentifiable: string };
			type PostRequest = { title: string, typeCode?: string };

			if (this.inAddingNewCategory)
				return;
			this.inAddingNewCategory = true;

			const newCategoryTitle = this.$$<Mvc.TextControl>('txbCreateCategory');

			if (newCategoryTitle.validate())
			{
				this.showLoadingPanel();

				const postRequest: PostRequest = {
					title: newCategoryTitle.getValue()
				};

				const categoryTypeLov = this.$$<Mvc.LOVControl>('lovCategoryType');

				if (categoryTypeLov.getValue())
					postRequest.typeCode = categoryTypeLov.getValue();
				else
					postRequest.typeCode = null;

				this._invokeController<DX.Mvc.TypedDataAjaxResult<CreateInputCategoryActionResult>>("CreateNewInputCategory", postRequest)
					.success(this, (result: DX.Mvc.TypedDataAjaxResult<CreateInputCategoryActionResult>) =>
					{
						if (result.data().Success)
						{
							this.AddInputCategoryToGrid(result.data().InputCategoryIdentifiable, postRequest.title, postRequest.typeCode);

							this.CancelNewCategory();
							this.hideLoadingPanel();
						}
						else
						{
							this.hideLoadingPanel();
							throw new Error('Error in creation of input category, see trace.');
						}
						this.inAddingNewCategory = false;
					});
			}
			else
				this.inAddingNewCategory = false;
		}
	
        public CancelNewCategory()
        {
            this.$$<Mvc.TextControl>('txbCreateCategory').setEmptyValue();
            this.$$<Mvc.LOVControl>('lovCategoryType').setEmptyValue();

            this.ShowNewCategory(false);
        }

        // #endregion New Category
    }

    interface ReadonlyDictionary<T>
    {
        readonly [key: string]: T
    }

	type CellValue = string | number | boolean | Array<string>;

	type CellControls = DX.Mvc.ObjRefControl | DX.Mvc.LOVControl | DX.Mvc.BooleanControl | DX.Mvc.NumericControl;

    export type InputRow = {
        [key: string]: CellValue,
        parent?: string
        IdentifiableString?: string,
        CategoryIdentifiableString: string,
        CategoryType: string,
        CategoryTitle?: string,
		MaterialIdentifiableString?: string,
		PartOfProductCoproduct?: Array<string>,
		Packaging?: boolean,
        LcStageId: number,
        Category?: string,
        Mass?: number,
        Cost?: number,
        InedibleParts?: number,
		Measurement?: number,
		ProductSource?: string
    };

	interface EntityActionResponse
	{
		Success: BaseActionResult;
		Entity: InputResponseModel;
		DestinationDeletedMsg?: string;
		Message?: string;
	}

	type InputResponseModel = {
		IdentifiableString: string,
		CategoryIdentifiableString?: string,
		MaterialIconUrl: string,
		MaterialIdentifiable: string,
		MaterialOpenTabHrefScript: string,
		MaterialDescription: string,
		PartOfProductCoproductCodes: string[],
		PartOfProductCoproductFormatted: string,
		Packaging?: boolean | null,
		PackagingFormatted: string,
		Mass?: number | null,
		MassFormatted: string,
		Cost?: number | null,
		CostFormatted: string,
		FoodFormatted: string,
		InedibleParts?: number | null,
		InediblePartsFormatted: string,
		Measurement?: number,
        MeasurementDisplayValue: string,
        ProductSource?: string
	};
}