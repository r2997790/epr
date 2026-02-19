namespace DX.DIRModule.AssessmentNavigator
{
    export class AssessmentResults extends DX.Mvc.NavigatorPanelBase implements IObserver
    {
        static $$className = "DX.DIRModule.AssessmentNavigator.AssessmentResults";

        private static WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE = 1550;

        private costOfWasteLegend: JQuery = null;
        private foodWastePerDestinationLegend: JQuery = null;

        private lcStageStepsPartial: LCStageSteps = null;
        private lcStageId: number = null;
        private productionRatiosGridID: string;
        private keyFinantialIndicatorsGridID: string;
        private foodWastePerDestinationGridID: string;
        private costOfWasteGridID: string;

        private costOfWasteChart: DX.Mvc.PieChart = null;
        private productionRatiosChart: DX.Mvc.HorizontalGroupedBarChart = null;
        private keyFinantialIndicatorsChart: DX.Mvc.HorizontalGroupedBarChart = null;
		private pyramideDestinationChart: DX.Mvc.PyramidChart = null;
        private requireRefresh: boolean = false;
		private resultHtmlGrids: Mvc.HtmlGrid.Grid[] = null;
		private gridRefreshCounter: number = 0;

        protected get ProductionRatiosGridID(): string
        {
            return this.productionRatiosGridID;
        }

        protected get KeyFinantialIndicatorsGridID(): string
        {
            return this.keyFinantialIndicatorsGridID;
        }

        protected get FoodWastePerDestinationGridID(): string
        {
            return this.foodWastePerDestinationGridID;
        }

        protected get CostOfWasteGridID(): string
        {
            return this.costOfWasteGridID;
        }

        private getProductionRatiosGrid(): JQueryExtensions.JQGrid.Control
        {
            return this.$<JQueryExtensions.JQGrid.Control>(this.ProductionRatiosGridID);
        }

        private getKeyFinantialIndicatorsGrid(): JQueryExtensions.JQGrid.Control
        {
            return this.$<JQueryExtensions.JQGrid.Control>(this.KeyFinantialIndicatorsGridID);
        }

        private getFoodWastePerDestinationGrid(): JQueryExtensions.JQGrid.Control
        {
            return this.$<JQueryExtensions.JQGrid.Control>(this.FoodWastePerDestinationGridID);
        }

        private getCostOfWasteGrid(): JQueryExtensions.JQGrid.Control
        {
            return this.$<JQueryExtensions.JQGrid.Control>(this.CostOfWasteGridID);
        }

        private get LcStageId()
        {
            return this.lcStageId;
        }

        private get CostOfWasteLegend()
        {
            if (this.costOfWasteLegend === null)
                this.costOfWasteLegend = DX.DOM.findJQElements('.dir-module-bus-nav-assessment-results-legend-table--cost-of-waste');

            return this.costOfWasteLegend;
        }

        private get FoodWastePerDestinationLegend()
        {
            if (this.foodWastePerDestinationLegend === null)
                this.foodWastePerDestinationLegend = DX.DOM.findJQElements('.dir-module-bus-nav-assessment-results-legend-table--food-waste-per-destination');

            return this.foodWastePerDestinationLegend;
        }

        private get CostOfWasteChart()
        {
            if (this.costOfWasteChart === null)
                this.costOfWasteChart = this.$$('CostOfWasteChart');

            return this.costOfWasteChart;
        }

        private get ProductionRatiosChart()
        {
            if (this.productionRatiosChart === null)
                this.productionRatiosChart = this.$$('ProductionRatiosChart');

            return this.productionRatiosChart;
        }

        private get KeyFinantialIndicatorsChart()
        {
            if (this.keyFinantialIndicatorsChart === null)
                this.keyFinantialIndicatorsChart = this.$$('KeyFinantialIndicatorsChart');

            return this.keyFinantialIndicatorsChart;
        }

        private get PyramideDestinationChart()
        {
            if (this.pyramideDestinationChart === null)
                this.pyramideDestinationChart = this.$$('pyramidDestinationChart');

            return this.pyramideDestinationChart;
        }

        public set RequireRefresh(value: boolean)
        {
            this.requireRefresh = value;
        }

		public get ResultHtmlGrids(): Mvc.HtmlGrid.Grid[]
		{
			return this.resultHtmlGrids;
		}

        constructor(element: HTMLElement)
        {
            super(element);
        }

        public __applyJSONData()
        {
            super.__applyJSONData();

            this.productionRatiosGridID = this._getJSONData<string>('productionRatiosGridID');
            this.keyFinantialIndicatorsGridID = this._getJSONData<string>('keyFinantialIndicatorsGridID');
            this.foodWastePerDestinationGridID = this._getJSONData<string>('foodWastePerDestinationGridID');
            this.costOfWasteGridID = this._getJSONData<string>('costOfWasteGridID');
            this.lcStageId = this._getJSONData<number>('lcStageId');
        }

		protected _initializePanel(): void
		{
			this._getMenuControl().enableControlEvents();
			this._initializeEvents();

			const navigator = DX.Ctrl.findParentControlOfType(this.get_element(), DX.Mvc.NavigatorBase) as DX.Mvc.NavigatorBase;
			this.lcStageStepsPartial = DX.Ctrl.findControlOfType(navigator.get_element(), DX.DIRModule.AssessmentNavigator.LCStageSteps);

			this.lcStageStepsPartial.Timeline.subscribe(this);

			this.lcStageStepsPartial.hideLoadingPanel(); // initially hide loading panel if someone reloads panel in middle of loading

			const productionRatiosMvcHtmlGrid = this.$$<Mvc.HtmlGrid.Grid>(Mvc.HtmlGrid.getRealLogicId(this.ProductionRatiosGridID));
			productionRatiosMvcHtmlGrid.add_loadCompleted(data =>
			{
				this.hideLoadingPanel();
				this.lcStageStepsPartial.hideLoadingPanel();
				DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ResultsProductionRatiosGrid_HeaderInfoBox")
					.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => headerInfoBoxPlacer('productionRatiosGridInfoBoxId', '#jqgh_DIRModule_ProductionRatios_Title', result.text));
			});

			const keyFinantialIndicatorsMvcHtmlGrid = this.$$<Mvc.HtmlGrid.Grid>(Mvc.HtmlGrid.getRealLogicId(this.KeyFinantialIndicatorsGridID));
			keyFinantialIndicatorsMvcHtmlGrid.add_loadCompleted(data =>
			{
				this.hideLoadingPanel();
				this.lcStageStepsPartial.hideLoadingPanel();
				DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "KeyFinantialIndicatorsGrid_HeaderInfoBox")
					.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => headerInfoBoxPlacer('keyFinantialIndicatorsGridInfoBoxId', '#jqgh_DIRModule_KeyFinantialIndicators_Title', result.text));
			});

			const foodWastePerDestinationMvcHtmlGrid = this.$$<Mvc.HtmlGrid.Grid>(Mvc.HtmlGrid.getRealLogicId(this.FoodWastePerDestinationGridID));
			foodWastePerDestinationMvcHtmlGrid.add_loadCompleted(data =>
			{
				this.hideLoadingPanel();
				this.lcStageStepsPartial.hideLoadingPanel();
				DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "FoodWastePerDestinationGrid_HeaderInfoBox")
					.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => headerInfoBoxPlacer('foodWastePerDestinationGridID', '#jqgh_DIRModule_FoodWastePerDestination_Title', result.text));
			});

			const costOfWasteMvcHtmlGrid = this.$$<Mvc.HtmlGrid.Grid>(Mvc.HtmlGrid.getRealLogicId(this.CostOfWasteGridID));
			costOfWasteMvcHtmlGrid.add_loadCompleted(data =>
			{
				this.hideLoadingPanel();
				this.lcStageStepsPartial.hideLoadingPanel();
				DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "CostOfWasteGrid_HeaderInfoBox")
					.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => headerInfoBoxPlacer('costOfWasteGridID', '#jqgh_DIRModule_CostOfWaste_Title', result.text));

			});

			this.resultHtmlGrids = [
				productionRatiosMvcHtmlGrid,
				keyFinantialIndicatorsMvcHtmlGrid,
				foodWastePerDestinationMvcHtmlGrid,
				costOfWasteMvcHtmlGrid
			];

			const selectedStage = this.lcStageStepsPartial.getSelectedLcStage();
			this.lcStageId = selectedStage.Id;

			this.refreshAll(new LcStageStep(selectedStage));

			const thisNavNode = navigator.getNodeDataById("DIRModule_TabAssessmentResults");
            // click on minimized panel header will trigger refresh if flag this.requireRefresh is true
            $(thisNavNode.getPanelHeaderMinimized()).on('click', (e) =>
			{
				if (this.requireRefresh && thisNavNode.getIsLoaded())
				{
					this.requireRefresh = false;
					this.refreshAll();
				}
			});

			const headerInfoBoxPlacer = (infoBoxId: string, headerDivId: string, resourceText: string): void =>
			{
				const headerDiv = $(headerDivId);
				if (headerDiv.has('div.DX_Mvc_InfoBoxControl').length === 0)
				{
					DX.Ctrl.newControl(DX.Mvc.InfoBoxControl).setLogicId(infoBoxId)
						.setDescriptor(new DX.Ctrl.InfoBoxDescriptor()
							.setShowPin(true)
							.setFadeinSpeed(0)
							.setStartDocked(false)
							.setShowOnClick(false)
							.setType(DX.Ctrl.InfoBoxDescriptorTypes.Hint)
							.setInfoboxOffsetY(0)
							.setInfoboxOffsetX(0)
							.setInfoboxOffsetUsesMouse(false)
							.setInfoboxContent(resourceText)
							.setWidth('460')
						).setContainer(headerDiv.get(0))
						.initializeControl()
						.appendTo(headerDiv);
				}
			};

			this._styleTotalRowOfCostOfWasteGrid();
		}

		update(currentLcStageStep: LcStageStep, previousLcStageStep?: LcStageStep): void
		{
			if (this.gridRefreshCounter == 0 && !this._isAnyGridLoading())
			{
				this.refreshAll(currentLcStageStep);
			}
			else
			{
				this.lcStageStepsPartial.revertLcStage(currentLcStageStep.StageId, previousLcStageStep.StageId);
			}
        }

		public refreshAll(lcStage?: LcStageStep)
		{
			if (typeof lcStage === 'undefined')
				this.lcStageId = this.lcStageStepsPartial.getSelectedLcStage().Id;
			else
				this.lcStageId = lcStage.StageId;

			this.showLoadingPanel();
			this.lcStageStepsPartial.showLoadingPanel();

            this._refreshGrids();
            this._refreshCharts();
        }

		private _isAnyGridLoading(): boolean
		{
			return this.ResultHtmlGrids.some(hmtlGrid => hmtlGrid.$().find(`div#load_${hmtlGrid.getOriginalLogicId()}`).css('display') === 'block');
		}

        private _refreshGrids()
		{
            this._refreshResultsGrid(this.getProductionRatiosGrid(), this.LcStageId, AssessmentResultType.MASS);
            this._refreshResultsGrid(this.getKeyFinantialIndicatorsGrid(), this.LcStageId, AssessmentResultType.PERCENTAGE);
            this._refreshFoodWastePerDestinationGrid(this.LcStageId);
            this._refreshCostOfWasteGrid(this.lcStageId);
        }

        private _refreshCharts()
        {
            this._invokeController('RefreshCharts', { lcStage: this.LcStageId }).success(this, (result: DX.Mvc.TypedDataAjaxResult<RefreshChartResponse>) =>
            {
                const data = result.getData<RefreshChartResponse>();

                if (typeof data.timeFrom !== 'undefined' && typeof data.timeTo !== 'undefined')
                {
                    this._refreshCostOfWasteChart(data.costOfWasteChartData);
                    this._refreshProductionRatiosChart(data.productionRatiosChartData);
                    this._refreshKeyFinantialIndicatorsChart(data.keyFinantialIndicatorsChartData);
                    this._refreshPyramideDestinationChart(data.pyramidChartData, data.pyramidColors);
                }
            });
        }

        private _refreshResultsGrid(grid: JQueryExtensions.JQGrid.Control, lcStage: number, resultType: string)
		{
			this.gridRefreshCounter++;
            this._invokeController("RefreshResultsGrid", { "lcStage": lcStage, "resultType": resultType })
                .success(this, (result: DX.Mvc.DataAjaxResult) =>
				{
					this.gridRefreshCounter--;
                    grid.setGridParam({ url: result.getDataValue("gridUrl"), page: 1 }).trigger('reloadGrid');
				})
				.error(this, () => this.gridRefreshCounter--);
        }

        private _refreshFoodWastePerDestinationGrid(lcStage: number)
		{
			this.gridRefreshCounter++;
            this._invokeController('RefreshFoodWastePerDestinationGrid', { 'lcStage': lcStage })
                .success(this, (result: DX.Mvc.DataAjaxResult) =>
				{
					this.gridRefreshCounter--;
                    this.getFoodWastePerDestinationGrid().setGridParam({ url: result.getDataValue('gridUrl'), page: 1 }).trigger('reloadGrid');
				})
				.error(this, () => this.gridRefreshCounter--);
        }

        private _refreshCostOfWasteGrid(lcStage: number)
		{
			this.gridRefreshCounter++;
            this._invokeController('RefreshCostOfWasteGrid', { 'lcStage': lcStage })
                .success(this, (result: DX.Mvc.DataAjaxResult) =>
				{
					this.gridRefreshCounter--;
                    this.getCostOfWasteGrid().setGridParam({ url: result.getDataValue('gridUrl'), page: 1 }).trigger('reloadGrid');
				})
				.error(this, () => this.gridRefreshCounter--);
        }

        private _refreshCostOfWasteChart(chartData: Mvc.IChartData)
        {
            this._refreshChart(this.CostOfWasteChart, chartData, () =>
            {
                this.CostOfWasteChart.getActiveChart().setData(chartData);

                this.CostOfWasteLegend.empty();

                const costOfWasteLegendData = (chartData as Mvc.NameValueChartData[]).map(x => x.name);

                if (costOfWasteLegendData.length > 0)
                {
                    this.CostOfWasteLegend.show();

                    let html = '';
                    costOfWasteLegendData.forEach((legendTitle, index) => html += `<tr><td><span style="background: ${this.CostOfWasteChart.getColors()[index]};"> </span><p>${legendTitle}</p></td></tr>`);

                    this.CostOfWasteLegend.append(html);
                }
                else
                {
                    this.CostOfWasteLegend.hide();
                }
            });
        }

        private _refreshProductionRatiosChart(chartData: Mvc.IChartData)
        {
            this._refreshChart(this.ProductionRatiosChart, chartData);
        }

        private _refreshKeyFinantialIndicatorsChart(chartData: Mvc.IChartData)
        {
            this._refreshChart(this.KeyFinantialIndicatorsChart, chartData);
        }

        private _refreshPyramideDestinationChart(chartData: Mvc.IChartData, chartColors: string[])
        {
            this._refreshChart(this.PyramideDestinationChart, chartData, (chartColors) =>
            {
                this.PyramideDestinationChart.getActiveChart().setColors(chartColors as string[]);
                this.PyramideDestinationChart.getActiveChart().setData(chartData);

                this.FoodWastePerDestinationLegend.empty();

                const pyramidChartLegendData = (chartData as Mvc.NameValueChartData[]).map(x => x.name);

                if (pyramidChartLegendData.length > 0)
                {
                    this.FoodWastePerDestinationLegend.show();

                    let html = '';
                    pyramidChartLegendData.forEach((legendTitle, index) => html += `<tr><td><span style="background: ${this.PyramideDestinationChart.getActiveChart().getColors()[index]};"> </span><p>${legendTitle}</p></td></tr>`);

                    this.FoodWastePerDestinationLegend.append(html);
                }
                else
                {
                    this.FoodWastePerDestinationLegend.hide();
                }

            }, chartColors);
        }

        private _refreshChart(chart: DX.Mvc.ChartControl, chartData: Mvc.IChartData, callback?: (additionalData: object) => void, additionalData?: object)
        {

            if (typeof callback !== 'undefined')
                callback(additionalData);

            chart.getActiveChart().setData(chartData);

            chart.getActiveChart().__drawChart(`#${chart.getClientId()}`, chartData, chart.getActiveChart().getChartOptions());

        }

        private _styleTotalRowOfCostOfWasteGrid()
        {
            const costOfWasteGrid = this.$$<Mvc.HtmlGrid.Grid>(Mvc.HtmlGrid.getRealLogicId(this.CostOfWasteGridID));
            costOfWasteGrid.add_loadCompleted(data =>
            {
                $(costOfWasteGrid.__getJQGrid()).jqGrid('setRowData', 0, false, 'grid-row-bolded');
            });
        }

		

        protected _refreshLegendTitle(timeFrom: string, timeTo: string): void
        {
            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ResultsCostOfWasteChartTitle").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                if (result.text != null)
                    $('#costOfWasteChart')[0].innerText = String.format(result.text, timeFrom, timeTo);
            });

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ResultsPyramidChartFoodDestination").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                if (result.text != null)
                    $("#pyramidChartOutput")[0].innerText = String.format(result.text, timeFrom, timeTo);
            });

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ResultsProdRatiosChartTitle").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                if (result.text != null)
                    $("#productionRatiosChart")[0].innerText = String.format(result.text, timeFrom, timeTo);
            });

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ResultsFinIndChartTitle").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                if (result.text != null)
                    $("#finalIndicatorsChart")[0].innerText = String.format(result.text, timeFrom, timeTo);
            });
        }
        protected _onMenuClicked(sender: DX.Mvc.HtmlMenu.Menu, args: DX.Mvc.HtmlMenu.ClickedEventArgs): void
        {
            switch (args.key)
            {
                case 'Refresh':
                    {
                        const selectedLcStage = this.lcStageStepsPartial.getSelectedLcStage();
                        this.refreshAll(new LcStageStep(selectedLcStage));

                        break;
                    }
                case "Export":
                    {
                        this._exportResults();
                        break;
                    }
            }
        }

        private _exportResults()
        {
            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Status: boolean, ScriptToExecute: string, ErrorMessage: string }>>("ExportResults", { "lcStage": this.lcStageId }).success((result) =>
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

        private _initializeEvents(): void
        {
            const timeout = () => setTimeout(() => this._resizeCharts(), 500);

            if ($(window).width() < AssessmentResults.WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE)
                timeout();

            $(window).on('resize', () => timeout());
        }

        private _resizeCharts()
        {
            const windowWidth = $(window).width();

            [
                this.ProductionRatiosChart,
                this.KeyFinantialIndicatorsChart
            ].forEach(chart => this._resizeChart(chart, (windowWidth < AssessmentResults.WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE ? 570 : 670)));

            const pyramidDestinationChartWidth = windowWidth < AssessmentResults.WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE ? 400 : 570;
            const costOfWasteChartSize = windowWidth < AssessmentResults.WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE ? { width: 380, height: 380 } : { width: 430, height: 430 };

            this._resizeChart(this.PyramideDestinationChart, pyramidDestinationChartWidth);
            this._resizeChart(this.CostOfWasteChart, costOfWasteChartSize.width, costOfWasteChartSize.height);
        }

        private _resizeChart(chart: Mvc.ChartControl, width?: number, height?: number)
        {
            const activeChart = chart.getActiveChart();

            if (width)
                activeChart.getChartOptions().w = width;

            if (height)
                activeChart.getChartOptions().h = height;

            activeChart.__drawChart(`#${chart.getClientId()}`, activeChart.getData(), activeChart.getChartOptions());
        }

        AssessmentDashboard_Click(): void
        {
            const objectIdentifiableString = this._getControllerData().idString;
            this._invokeController("AssessmentDashboard", { "objectIdentifiableString": objectIdentifiableString })
                .success((result: DX.Mvc.DataAjaxResult) =>
                {
                    const message = (<DX.Mvc.DataAjaxResult>result).getDataValue<string>("Notification");
                    if (message)
                    {
                        DX.Notif.showNotifications(this._getNotificationsContainer(), [new DX.Notif.Notification(DX.Notif.NotificationTypes.Error, DX.Loc.TextDescriptor.newByText(message))], true);
                        return;
                    }
                    else
                    {
                        const activity: JSOpenDialogActivity = result.getDataValue<JSOpenDialogActivity>("Activity");
                        activity.execute().success(result, () =>
                        {
                            const returnValue = activity.getClosingDialogData().getMainReturnValue() as string;
                            if (returnValue)
                            {
                                this.showAlert(returnValue, "info");
                            }
                        });
                    }
                });
        }
    }

    enum AssessmentResultType
    {
        MASS = 'M',
        VALUE = 'V',
        PERCENTAGE = 'P'
    }

    interface RefreshChartResponse
    {
        timeFrom: string;
        timeTo: string;
        costOfWasteChartData: Mvc.IChartData[];
        costOfWasteLegendData: string[];
        keyFinantialIndicatorsChartData: Mvc.IChartData[];
        productionRatiosChartData: Mvc.IChartData[];
        pyramidChartData: Mvc.IChartData[];
        pyramidColors: string[]
    }
}
