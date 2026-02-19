namespace DX.DIRModule.AssessmentNavigator
{
    export class MergeInputsDialog extends DX.Mvc.DialogViewControl
    {
        static $$className = 'DX.DIRModule.AssessmentNavigator.MergeInputsDialog';
        private static CATEGORY_TYPE_COLUMN = 'CategoryType';
        private static MERGED_NAME_COLUMN = 'MergedName';

        private assessmentIdentifiableString: string = null;
        private lcStageId: number = null;
        private nextLcStageId: number = null;
        private spreadsheet: Mvc.Spreadsheet.Control = null;

        private addMaterialImgUrl: string = null;
        private addMaterialImgTitle: string = null;
        private addMaterialWarning: string = null;
        private addMaterialPrompMessage: string = null;
        private addMaterialCreateSucceeded: string = null;
        private addMaterialCreateFailed: string = null;
        private saveFailed: string = null;

        private dialogInvalidMessage: string = null;
        private promptMessageButtonCancel: string = null;
        private promptButtonYes: string = null;
        private promptButtonNo: string = null;

        public __applyJSONData()
        {
            super.__applyJSONData();

            this.assessmentIdentifiableString = this._getJSONData('identifiableString');
            this.lcStageId = this._getJSONData('lcStageId');
            this.nextLcStageId = this._getJSONData('nextLcStageId');
        }

        protected _initialize()
        {
            super._initialize();

            this.initializeSpreadsheet();
            this.initalizeResources();
        }

        private initializeSpreadsheet()
        {
            this.spreadsheet = this.$$<Mvc.Spreadsheet.Control>('spreadsheet');
            this.spreadsheet.setRowsHeadersMode(Mvc.Spreadsheet.RowsHeadersMode.RowCounter);
            this.spreadsheet.setIsRowsHeadersHumanNumbering(true);
            this.spreadsheet.setSelectionMode(Mvc.Spreadsheet.SelectionMode.Row);
            this.spreadsheet.clearSelection();
            this.spreadsheet.add_initialized(() => this.setEditableCellToBeInvalid());
        }

        private initalizeResources()
        {
            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "MergeInputsDialogInvalidMessage").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) => this.dialogInvalidMessage = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_AddMaterialIconTitle")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.addMaterialImgTitle = result.text);

            this.addMaterialImgUrl = DX.Paths.mapVirtualPath('~/WebMvcModules/Content/Images/toolbar_icons/DxAddMaterialUltraDarkGrey.svg');

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "MergeInputsDialog_PromptMessageCancelButton")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptMessageButtonCancel = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_MaterialCreationWarning")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.addMaterialWarning = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_MaterialCreationPromptMessage")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.addMaterialPrompMessage = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_SuccessfullyMaterialCreation")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.addMaterialCreateSucceeded = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "Inputs_MaterialCreationFailed")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.addMaterialCreateFailed = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "MergeInputsDialogSaveFailed")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.saveFailed = result.text);
            
            DX.Loc.TextDescriptor.newByResource("Controls", "DlgYes")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptButtonYes = result.text);

            DX.Loc.TextDescriptor.newByResource("Controls", "DlgNo")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptButtonNo = result.text);
        }

        protected _onMenuItemClick(eventArgs?: DX.Mvc.MenuItemClickEventArgs)
        {
            super._onMenuItemClick(eventArgs);

            if (eventArgs.getHandled())
                return;

            switch (eventArgs.getMenuItem().getKey())
            {
                case 'CancelButton':
                    {
                        eventArgs.setHandled(true);

                        this.showPrompt(this.promptMessageButtonCancel, [
                            {
                                caption: this.promptButtonYes,
                                type: 'confirm',
                                func: () => DX.Page.closeDialog(false).setPressedButtonKey('Cancel')
                            },
                            {
                                caption: this.promptButtonNo,
                                type: 'cancel',
                                func: () => {}
                            }
                        ], false);

                    }
                    break;
                case 'OkButton':
                    {
                        if (!this.isPanelValid())
                        {
                            this.showAlert(this.dialogInvalidMessage, "error");
                            return;
                        }
                        else 
                        {
                            this._showLoadingPanel();

                            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Success: boolean }>>('CarryMergedProductOverToTheNextStage', { identifiableString: this.assessmentIdentifiableString, lcStageId: this.lcStageId, nextLcStageId: this.nextLcStageId, gridParam: this.spreadsheet.getChangesAjaxParameter() })
                                .success(this, (result) =>
                                {
                                    const response = result.data();

                                    if (response.Success)
                                    {
                                        eventArgs.setHandled(true);
                                        DX.Page.getClosingDialogData()
                                            .setPressedButtonKey('Ok');
                                        DX.Page.closeDialog();
                                    }
                                    else
                                    {
                                        this._hideLoadingPanel();
                                        DX.Notif.showNotifications($('#notificationContainer'), [new DX.Notif.Notification(DX.Notif.NotificationTypes.Error, this.saveFailed)], true);
                                    }
                                });
                        }   
                    }
                    break;
            }
        }

        private setEditableCellToBeInvalid()
        {
            this.spreadsheet.rows.forEach(row =>
            {
                const mergedNameCellIdentifier = { row: row.key, col: MergeInputsDialog.MERGED_NAME_COLUMN };

                const mergedNameCellType = this.spreadsheet.getCellType(mergedNameCellIdentifier) as DIRResourceTypeCellType;

                mergedNameCellType.add_editModeInitialized((_, args) =>
                {
                    const mergedNameCell = this.spreadsheet.getCellType(args.cellTypeContainer) as DIRResourceTypeCellType;
                    const categoryTypeCellIdentifier = { row: (args.cellTypeContainer as DX.Mvc.Spreadsheet.CellIdentifier).row, col: MergeInputsDialog.CATEGORY_TYPE_COLUMN };
                    const categoryTypeCell = this.spreadsheet.getCellType(categoryTypeCellIdentifier) as DX.Mvc.Spreadsheet.TextCellType;
                    const categoryType = categoryTypeCell.getCellValue(categoryTypeCellIdentifier) as string;

                    mergedNameCell.onSearchBoxRendered = (box, searchValue) =>
                    {
                        const addMaterialElement = $('<span></span>')
                            .addClass('DIRModule-merge-dialog-add-material-span');

                        if (box.getItems().length == 0)
                        {
                            const materialDescription = searchValue.replace(new RegExp('[*]', 'g'), '');

                            const addMaterialImgElement = $('<img />')
                                .addClass('DIRModule-merge-dialog-add-material-icon')
                                .attr({
                                    src: this.addMaterialImgUrl,
                                    title: this.addMaterialImgTitle
                                });

                            addMaterialImgElement.on('click', () =>
                            {
                                this.addMaterial({
                                    description: materialDescription,
                                    categoryType,
                                    cell: mergedNameCellType,
                                    cellIdentifier: { row: (args.cellTypeContainer as DX.Mvc.Spreadsheet.CellIdentifier).row, col: MergeInputsDialog.MERGED_NAME_COLUMN }
                                });
                            });
                            
                            $(box.getDiv()).find('.QSFooter')
                                .prepend($(addMaterialElement).append(addMaterialImgElement));
                        }
                        else
                        {
                            $(addMaterialElement).remove();
                        }
                    }

                    const filter = mergedNameCell.getContainerSearchFilter(args.cellTypeContainer);                   
                    filter.getItemByName("Attributes[DXDIR_RESOURCE_TYPE]").setValues([categoryType]);
                });
                
                mergedNameCellType.setCellInvalidData(mergedNameCellIdentifier, {
                    isValid: false,
                    invalidCellDataClass: 'DIRModule-merge-dialog-spreadsheet-invalid-cell'
                })
            });

            this.spreadsheet.refresh();
        }

        public addMaterial(material: AddMaterialObj)
        {
            if (material.description.length === 0)
            {
                DevEx.HostPage.showAlert(this.addMaterialWarning, "warning");
                return;
            }

            this.showPrompt(String.format(this.addMaterialPrompMessage, material.description), [
                {
                    caption: this.promptButtonYes,
                    type: 'confirm',
                    func: () => { this.createMaterial(material) }
                },
                {
                    caption: this.promptButtonNo,
                    type: 'cancel',
                    func: () =>
                    {
                        material.cell.refresh('');
                        this.spreadsheet.clearSelection();
                        this.spreadsheet.refresh();
                    }
                }
            ])
        }

        public createMaterial(material: AddMaterialObj)
        {
            this._invokeController<DX.Mvc.TypedDataAjaxResult<({ Success: boolean, IdentifiableString: string })>>('CreateMaterial', { description: material.description, categoryType: material.categoryType })
                .success(this, (result) =>
                {
                    if (result.data().Success)
                    {
                        material.cell.refresh(result.data().IdentifiableString);
                        DevEx.HostPage.showAlert(this.addMaterialCreateSucceeded, 'info');
                    }
                    else
                    {
                        material.cell.refresh('');
                        DevEx.HostPage.showAlert(this.addMaterialCreateFailed, 'error');
                    }

                    this.spreadsheet.clearSelection();
                    this.spreadsheet.refresh();        
                });
        }

        private isPanelValid()
        {
            const isValid = this.spreadsheet.rows.every(row =>
            {
                return this.isCellValid({ row: row.key, col: MergeInputsDialog.MERGED_NAME_COLUMN });
            });

            return isValid;
        }

        private isCellValid(cellIdentifier: DX.Mvc.Spreadsheet.CellIdentifier)
        {
            const validationData = this.spreadsheet.getCellType(cellIdentifier).getCellInvalidData(cellIdentifier);
            return (validationData === null) ? true : validationData.isValid;
        }
    }

    type AddMaterialObj = {
        description: string,
        categoryType: string,
        cell: DIRResourceTypeCellType,
        cellIdentifier: { row: Mvc.Spreadsheet.RowIdentifier, col: string }
    }

    export class DIRResourceTypeCellType extends DX.Mvc.Spreadsheet.RefObjectCellType
    {
        static $$className = "DX.DIRModule.AssessmentNavigator.DIRResourceTypeCellType";

        public onSearchBoxRendered: (box: DX.QuickSearch.Box, searchValue: string) => void;

        public refresh(value: string)
        {
            this.setEditingValue(value);
            this.__endEditing(true);
            this.__bubbleKeyPressEvent(13, false, false);
        }

        public _onCreatedQuickSearchBox(box: DX.QuickSearch.Box)
        {
            box.setOnConpletedScript(() =>
            {
                const value = box.getTextBox().value;
                this.onSearchBoxRendered(box, value);
            });
        }
    }
}

DX.Mvc.Spreadsheet.Control.__registerCellTypeType(DX.DIRModule.AssessmentNavigator.DIRResourceTypeCellType, 'DX.DIRModule.AssessmentNavigator.DIRResourceTypeCellType');