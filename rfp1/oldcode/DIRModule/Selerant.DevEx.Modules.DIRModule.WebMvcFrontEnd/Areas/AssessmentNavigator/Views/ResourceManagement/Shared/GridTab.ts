namespace DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared
{
	export class GridTab extends DX.Mvc.ViewControl implements EventListenerObject 
	{
		static $$className: string = 'DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.GridTab';

		public readonly keydownEvent: string = 'keydown';
		public readonly enterKey: string = 'Enter';
		public readonly escKey: string = 'Escape';

        private gridID: string;
        private gridUrl: string;
		private grid: Mvc.HtmlGrid.Grid = null;
		private jqGrid: JQueryExtensions.JQGrid.Control = null;
		private tableJQcontext: JQuery = null;
		private viewContainerClass: string;
		private lcStage: number;
		protected canEditOrDelete: boolean;
		private isInEditMode: boolean;
		private currentRowId: string;
		private gridDataChanged: boolean;
		private parentNotificationsContainer: JQuery = null;
        private outerMainParent: JQuery<HTMLElement> = null;
        private isProductCarried: boolean = null;
        private resourceNotes: ResourceNote = null;

		//#region Resource Texts

		public saveActionText: string;
		public cancelActionText: string;
		public deleteActionText: string;
		public unsavedChangesWarning: string;

		//#endregion Resource Texts

		//#region Properties

		protected get GridID(): string
		{
			return this.gridID;
        }

        protected get GridUrl(): string
        {
            return this.gridUrl;
        }

		protected get Grid(): Mvc.HtmlGrid.Grid
		{
			if (this.grid === null)
				this.grid = this.$$<Mvc.HtmlGrid.Grid>(Mvc.HtmlGrid.getRealLogicId(this.gridID));
			return this.grid;
		}

		protected get JQGrid(): JQueryExtensions.JQGrid.Control
		{
			if (this.jqGrid === null)
				this.jqGrid = this.Grid.__getJQGrid();
			return this.jqGrid;
		}

		protected get TableJQcontext(): JQuery
		{
			if (this.tableJQcontext === null)
				this.tableJQcontext = DX.DOM.findJQElements(`#${this.gridID}`);
			return this.tableJQcontext;
		}

		public set ViewContainerClass(cssClass: string)
		{
			this.viewContainerClass = '.' + cssClass;
		}
		public get ViewContainerClass()
		{
			return this.viewContainerClass;
		}

		public set LcStage(lcStageId: number)
		{
			this.lcStage = lcStageId;
		}
		public get LcStage()
		{
			return this.lcStage;
		}

		public get IsInEditMode(): boolean
		{
			return this.isInEditMode;
		}
		public set IsInEditMode(editMode: boolean)
		{
			this.isInEditMode = editMode;
		}

		public get CurrentRowId(): string
		{
			return this.currentRowId;
		}

		public set CurrentRowId(currentRowId: string)
		{
			this.currentRowId = currentRowId;
		}

		public set GridDataChanged(value: boolean)
		{
			this.gridDataChanged = value;
		}

		public get ParentNotificationsContainer()
		{
			return this.parentNotificationsContainer;
		}

		public set ParentNotificationsContainer(value: JQuery)
		{
			this.parentNotificationsContainer = value;
        }

        public get IsProductCarried()
        {
            return this.isProductCarried;
        }

        public set IsProductCarried(value: boolean)
        {
            this.isProductCarried = value;
        }

		public get OuterMainParent(): JQuery<HTMLElement>
		{
			return this.outerMainParent;
		}
	    /**
		 * Top parent of partial view
		 * @param value jquery element of dialog or panel
		 */ 
		public set OuterMainParent(value: JQuery<HTMLElement>)
		{
			this.outerMainParent = value;
        }

        public get ResourceNotes()
        {
            if (this.resourceNotes === null)
                this.resourceNotes = this.$$<Shared.ResourceNote>('ResourceNotePartial');

            return this.resourceNotes;
        }

		//#endregion Properties

		constructor(element: HTMLElement)
		{
			super(element);
			this.gridDataChanged = false;
		}

		public __applyJSONData()
		{
			// (OVERRIDE)
			super.__applyJSONData();
            this.gridID = this._getJSONData<string>("gridID");
            this.gridUrl = this._getJSONData<string>("gridUrl");
			this.lcStage = this._getJSONData<number>("lcStageId");
			this.canEditOrDelete = this._getJSONData<boolean>("canEditOrDelete", false);
		}

		protected _initialize(): void
		{
			// (OVERRIDE)
            super._initialize();
			this._initLocalizationResources();
        }

        public outerInitialize(): void
		{
			const gridParams: JQueryExtensions.JQGrid.SetGridParamOptions = {
				url: this.gridUrl,
				page: 1
			}; 

			if (this.canEditOrDelete)
			{
				gridParams.ondblClickRow = (rowid: string, iRow: number, iCol: number, e: Event) =>
				{
					if (rowid.startsWith('DxInput(') ||
						rowid.startsWith('DxFoodInputDestination(') ||
						rowid.startsWith('DxNonFoodInputDestination(') ||
						rowid.startsWith('DxOutputGridItem(') ||
						rowid.startsWith('DxBusinessCostGridItem('))
					{
						this.EditRow_GridAction(rowid, iCol);
					}
				};
			}

			this.JQGrid.setGridParam(gridParams);

        }

		private _initLocalizationResources()
		{
			DX.Loc.TextDescriptor.newByResource('Controls', 'Save')
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.saveActionText = result.text);

			DX.Loc.TextDescriptor.newByResource('Controls', 'Cancel')
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.cancelActionText = result.text);

			DX.Loc.TextDescriptor.newByResource('Controls', 'Delete')
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.deleteActionText = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "BusinessDataDialog_UnsavedChangesWarning")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.unsavedChangesWarning = result.text);
		}

		public SetResourceNoteOuterMainParent(outerParentElement: JQuery<HTMLElement>)
		{
			this.ResourceNotes.TopOuterParentElement = outerParentElement.toDOMElement().toElement();
		}

		public showPrompt(text: string, data: DX.Notif.ShowPromptData, cancelOnOverlayClick?: boolean)
		{
			// (OVERRIDE)
			Notif.showPrompt(this.ParentNotificationsContainer, text, data, this.outerMainParent, cancelOnOverlayClick);
		}

		public showLoadingPanel(): void
		{
			// (OVERRIDE)
			if (this.outerMainParent.hasClass('dir-module-dlg-business-data'))
			{
				// it's on dialog show loading div over whole dialog
				LP.showLoadingPanel(this.outerMainParent.toDOMElement().toElement());
			}
			else
				super.showLoadingPanel();
		}

		public hideLoadingPanel(): void
		{
			// (OVERRIDE)
			if (this.outerMainParent.hasClass('dir-module-dlg-business-data'))
			{
				LP.hideLoadingPanel(this.outerMainParent.toDOMElement().toElement());
			}
			else
				super.hideLoadingPanel();
		}

		//#region Virtual functions

		public ReloadGrid()
		{
			// VIRTUAL
			if (this.CurrentRowId)
				this.RemoveKeydownEvent(this.CurrentRowId);

			this.IsInEditMode = false;

			this.Grid.reload();
		}

		public EditRow_GridAction(rowId: string, iCol?: number): void
		{
			// VIRTUAL
			this.AddKeydownEvent(rowId);

			this.IsInEditMode = true;
			this.CurrentRowId = rowId;
		}

		protected SaveRow(rowId: string): void
		{
			// VIRTUAL
			this.RemoveKeydownEvent(rowId);

			this.IsInEditMode = false;
			this.CurrentRowId = null;
		}

		protected CancelRow(rowId: string): void
		{
			// VIRTUAL
			this.IsInEditMode = false;
			this.CurrentRowId = null;
			this.GridDataChanged = false;

			this.RemoveKeydownEvent(rowId);
		}

		//#endregion

		//#region Element Event Handling

		public handleEvent(evt: KeyboardEvent): void
		{
			if (evt.type === this.keydownEvent)
			{
				if (evt.key === this.enterKey)
				{
					const rowId: string = (evt.currentTarget as HTMLElement).id;
					this.SaveRow(rowId);
				}
				else if (evt.key === this.escKey)
				{
					const rowId: string = (evt.currentTarget as HTMLElement).id;
					this.CancelRow(rowId);
				}
			}
		}

		public AddKeydownEvent(rowId: string): void
		{
			this.RetrieveRow(rowId).attr('tabindex', -1).get(0).addEventListener(this.keydownEvent, this, true);
		}

		public RemoveKeydownEvent(rowId: string): void
		{
			this.RetrieveRow(rowId).removeAttr('tabindex').get(0).removeEventListener(this.keydownEvent, this, true);
		}

		//#endregion

		public EscapeId(id: string): string
		{
			if (id.indexOf('(') >= 0 || id.indexOf(')') >= 0)
				return id.replace(/(?=[() \t])/g, '\\');
			else
				return id;
		}

		public RetrieveRow(rowId: string): JQuery
		{
			return this.TableJQcontext.find(`tr#${this.EscapeId(rowId)}`);
        }

        // Suddenly, after merges to main, probably for click riw selection, wild div appeared inside _Actions cells
        // add messed/made impossible adding link to Actions column with addChildNode and setCell 
        public InsertLinksAfterActionsCellDiv(rowId: string, actionLinks: string)
        {
            const $ActionCelldiv = this.TableJQcontext.find(`tr#${this.EscapeId(rowId)} td._Actions div[data-col-name='Actions']`);
            $ActionCelldiv.after(actionLinks);
        }

		public RemoveErrorOutline(rowId: string): void
		{
			const rowJq = this.RetrieveRow(rowId);
			if (rowJq.length)
				rowJq.css("outline", "");
		}

		public SetErrorOutline(rowId: string): void
		{
			this.RetrieveRow(rowId).css("outline", "thin solid red");
        }

        public IsValid()
        {
            return true;
        }

		public BuildActionLinks(rowId: string, typeName: string)
		{
			const $aSave = DX.DOM.createElement("a").toJQ();
			$aSave.addClass('row-action-item')
                .css('white-space', 'nowrap')
                .attr('href', 'javascript:void(0)')
				.attr("onclick", `DX.Ctrl.findParentControlOfType(this, ${typeName}).SaveRow('${rowId}');`)
				.text(this.saveActionText);

			const $aCancel = DX.DOM.createElement("a").toJQ();
			$aCancel.addClass('row-action-item')
                .css('white-space', 'nowrap')
                .attr('href', 'javascript:void(0)')
				.attr("onclick", `DX.Ctrl.findParentControlOfType(this, ${typeName}).CancelRow('${rowId}');`)
				.text(this.cancelActionText);

			return `${$aSave[0].outerHTML} ${$aCancel[0].outerHTML}`;
		}

        public ChangeLcStage(lcStageId: number)
		{
            this.JQGrid.setGridParam({ postData: { 'lcStageId': lcStageId }, page: 1 });
            this.LcStage = lcStageId;
		}

		public EnableGridActions(rowId: string, activate: boolean): void
		{
			this.RetrieveRow(rowId).css({ "pointer-events": activate ? "auto" : "none"});
		}

		public RefreshAssessmentResultPanel(force?: boolean)
		{
			if (force)
				this.gridDataChanged = true;

			if (this.gridDataChanged === false)
				return;

			const navigator = DX.Ctrl.findParentControlOfType(this.get_element(), DX.Mvc.NavigatorBase);
			if (navigator)
			{
				const navNode = navigator.getNodeDataById("DIRModule_TabAssessmentResults");

				if (navNode && navNode.getIsLoaded())
				{
					navigator.getPanelInstanceAsync(navNode).then(resultPanel =>
					{
						if (resultPanel !== null && resultPanel.navigatorPanel !== null)
						{
							// only refresh loaded and opened (visible) panel
							if (navNode.getIsOpen())
							{
								(resultPanel.navigatorPanel as DX.DIRModule.AssessmentNavigator.AssessmentResults).refreshAll();
							}
							else // collapsed panel mark  for reload
							{
								(resultPanel.navigatorPanel as DX.DIRModule.AssessmentNavigator.AssessmentResults).RequireRefresh = true;
							}
							this.gridDataChanged = false;
						}
					});
				}
			}
		}

		public IsGridLoading(): boolean
		{
			return this.Grid.$().find(`div#load_${this.GridID}`).css('display') === 'block';
		}

		public CheckValidateAssessmentLcStage(lcStageId: number)
		{
			this._invokeController<DX.Mvc.TypedDataAjaxResult<{ WarningMassageForInvalidLcStage: string }>>("CheckValidateAssessmentLcStage", { lcStageId: lcStageId })
				.success(this, (result: DX.Mvc.TypedDataAjaxResult<{ WarningMassageForInvalidLcStage: string }>) =>
				{
					if (result.data().WarningMassageForInvalidLcStage)
					{
						DX.Notif.showNotifications(this.ParentNotificationsContainer, [new DX.Notif.Notification(DX.Notif.NotificationTypes.Warning, result.data().WarningMassageForInvalidLcStage)], true);
					}
					else
					{
						this.ParentNotificationsContainer.empty();
					}
				});
		}
	}

	export function buildStepReferenceDict(references: GridTab[], steps: string[]): Dictionary<GridTab>
	{
		if(references.length !== steps.length)
		    throw new Error('references and steps length is differente');

		return references.reduce<Dictionary<GridTab>>((accumulator: Dictionary<GridTab>, curVal: GridTab, idx: number): Dictionary<GridTab> =>
		{
			accumulator[steps[idx]] = curVal;
			return accumulator;
		}, {} as Dictionary<GridTab>);
	}

	export enum ModificationType { Create, Edit }

	export enum ResourceRowType
	{
		Input,
		Destination,
		Output,
		BusinessCost
	}

	export type BaseActionResult =
	{
		Success: boolean
	}

	export enum EntityType
	{
		LcStage = 0,
		Destination = 1,
	}
}