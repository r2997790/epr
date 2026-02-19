namespace DX.DIRModule.AssessmentNavigator.ResourceManagement
{
	export class Index extends DX.Mvc.NavigatorPanelBase implements IObserver
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Index";

		private readonly TABSTRIP_ID = 'tabStripResourcesGrids';

		private viewContainerClass: string;
		private tabstripControl: DX.Mvc.Tabstrip = null;
		private lcStageStepsPartial: LCStageSteps = null;

		private TAB_KEYS: Dictionary<GridTab> = null;

		constructor(element: HTMLElement)
		{
			super(element);
		}

		public __applyJSONData()
		{
			// (OVERRIDE)
			super.__applyJSONData();

			this.viewContainerClass = this._getJSONData<string>("viewContainerClass", '');
		}

		update(currentLcStageStep: LcStageStep, previousLcStageStep?: LcStageStep): void
		{ 
			// prevent switching LC Stage if grid in active tab is in loading progress
			if (!this.TAB_KEYS[this.tabstripControl.getSelectedTabKey()].IsGridLoading())
			{
				this._updateGrids(currentLcStageStep.StageId, previousLcStageStep.StageId);
			}
			else
				this.lcStageStepsPartial.revertLcStage(currentLcStageStep.StageId, previousLcStageStep.StageId);
		}

		private _updateGrids(currentLcStageId: number, previousLcStageId?: number)
		{
			const selectedTab = this.TAB_KEYS[this.tabstripControl.getSelectedTabKey()];
			if (!selectedTab.IsInEditMode)
			{
                this._changeLcStageOnGrids(currentLcStageId);
                this.TAB_KEYS[this.tabstripControl.getSelectedTabKey()].showLoadingPanel();
				this.TAB_KEYS[this.tabstripControl.getSelectedTabKey()].ReloadGrid();
			}
			else
			{
				this.lcStageStepsPartial.revertLcStage(currentLcStageId, previousLcStageId);

				selectedTab.SetErrorOutline(selectedTab.CurrentRowId);
				DevEx.HostPage.showAlert(selectedTab.unsavedChangesWarning, "warning", 2500);
			}
		}

		private _changeLcStageOnGrids(lcStageId: number)
		{
			Object.keys(this.TAB_KEYS).forEach((tabKey) => this.TAB_KEYS[tabKey].ChangeLcStage(lcStageId));
		}

		protected _initializePanel(): void
		{
			// (OVERRIDE_ABSTRACT)
			this._getMenuControl().enableControlEvents();
			
			this.tabstripControl = this.$$<Mvc.Tabstrip>(this.TABSTRIP_ID);

			const gridTabs: GridTab[] = [
				this.$$("InputsGridTabPartial"),
				this.$$<DestinationsGridTab>("DestinationsGridTabPartial"),
				this.$$<DestinationsGridTab>("NonFoodDestinationsGridTabPartial"),
				this.$$("OutputsGridTabPartial"),
				this.$$("BusinessCostGridTabPartial")
			];

			// pass references for after Manage grid column setting
			(gridTabs[1] as DestinationsGridTab).OtherDestinationTab = gridTabs[2] as DestinationsGridTab;
			(gridTabs[2] as DestinationsGridTab).OtherDestinationTab = gridTabs[1] as DestinationsGridTab;

			const notificationContainer = this.getNotificationsContainer();
			const panelContainerDomJQ = this.DOM().toJQ();

			gridTabs.forEach(tab =>
			{
				tab.outerInitialize();

				tab.ParentNotificationsContainer = notificationContainer;
				tab.OuterMainParent = panelContainerDomJQ;
				tab.SetResourceNoteOuterMainParent(panelContainerDomJQ);
			});

			const inputTab = gridTabs[0];
			inputTab.ViewContainerClass = this.viewContainerClass;

			const navigator = DX.Ctrl.findParentControlOfType(this.get_element(), DX.Mvc.NavigatorBase) as DX.Mvc.NavigatorBase;
			this.lcStageStepsPartial = DX.Ctrl.findControlOfType(navigator.get_element(), DX.DIRModule.AssessmentNavigator.LCStageSteps);

			this.lcStageStepsPartial.Timeline.subscribe(this);

			this.TAB_KEYS = ResourceManagement.Shared.buildStepReferenceDict(gridTabs, this.lcStageStepsPartial.Steps);

			// register tabstrip events
			this.tabstripControl.add_beforeSelectionChanged(DX.OOP.createDelegate(this, this.tabstripControlBeforeSelectionChanged));
			this.tabstripControl.add_selectionChanged(DX.OOP.createDelegate(this, this.tabstripControlSelectionChanged));
			this.tabstripControl.add_loadTabOnDemand(DX.OOP.createDelegate(this, this.tabstripControlLoadTabOnDemand));

			this._refreshGrids();

			const interval = setInterval(() =>
			{
				if (inputTab.getIsInitialized() && !inputTab.IsGridLoading())
				{
					this.TAB_KEYS[this.tabstripControl.getSelectedTabKey()].ReloadGrid();
					clearInterval(interval);
				}
			}, 1000);
		}

		protected _onMenuClicked(sender: DX.Mvc.HtmlMenu.Menu, args: DX.Mvc.HtmlMenu.ClickedEventArgs): void
		{
			//super._onMenuClicked(sender, args);

			switch (args.key)
			{
				case 'Refresh':
					{
						this._refreshGrids();
						break;
                    }
                case "Export":
                    {
                        this._exportResults();
                        break;
                    }
			}
		}

		private _refreshGrids()
		{
			const selectedLcStage = this.lcStageStepsPartial.getSelectedLcStage();
			this._updateGrids(selectedLcStage.Id);
        }

        private _exportResults()
        {
            const selectedLcStage = this.lcStageStepsPartial.getSelectedLcStage();

            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Status: boolean, ScriptToExecute: string, ErrorMessage: string }>>("Export", { "lcStage": selectedLcStage.Id }).success((result) =>
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

		private tabstripControlLoadTabOnDemand(sender: Mvc.Tabstrip, args: Mvc.TabstripLoadTabOnDemandEventArgs): void
		{
			if (args.afterLoadCallback)
				args.afterLoadCallback();
		}

		private tabstripControlBeforeSelectionChanged(sender: Mvc.Tabstrip, args: Mvc.TabstripBeforeSelectionChangedEventArgs): void
		{
			const previousTab = this.TAB_KEYS[args.previousKey];
			if (previousTab !== undefined && previousTab.IsInEditMode)
			{
				previousTab.SetErrorOutline(previousTab.CurrentRowId);
				args.isCancelled = true;
				DevEx.HostPage.showAlert(previousTab.unsavedChangesWarning, "warning");

				return;
			}
		}

		private tabstripControlSelectionChanged(sender: Mvc.Tabstrip, args: Mvc.TabstripSelectionChangedEventArgs): void
        {
            this.TAB_KEYS[this.tabstripControl.getSelectedTabKey()].showLoadingPanel();
			this.TAB_KEYS[args.selectedKey].ReloadGrid();
		}
	}

	type GridTab = ResourceManagement.Shared.GridTab;
	type DestinationsGridTab = ResourceManagement.Shared.DestinationsGridTab;
}