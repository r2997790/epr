namespace DX.DIRModule.AssessmentNavigator
{
    export class AssessmentDashboardDialog extends DX.Mvc.DialogViewControl
    {
        static $$className = "DX.DIRModule.AssessmentNavigator.AssessmentDashboardDialog";

        private static WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE = 1500;
        private static MAX_NUM_OF_ASSESSMENTS = 4;
        private isResizeEnabled = true;

        private readonly DASHBOARDMENU = 'DashboardMenu';
        private readonly REMOVEGROUPMENU = 'RemoveGroup';
        private readonly MASS = 'MASS';
        private readonly COST = 'COST';

        private radioButtonMassCost: JQuery = null;
        private container: JQuery = null;
        private hotSpotChart: DX.Mvc.GroupedBarChart = null;
        private spiderChartCostOfWaste: DX.Mvc.SpiderChart = null;
        private productionRatiosChart: DX.Mvc.HorizontalGroupedBarChart = null;
        private keyFinantialIndicatorsChart: DX.Mvc.HorizontalGroupedBarChart = null;
        //private showInDialog: Boolean = null;
        private _assessments: string[] = [];
        private assessmentPresentMsg: string;
        private maxNumExceeded: string;
        private cantRemoveLastAssessmentMsg: string;
        private hotSpotGridID: string;
        private productionRatiosGridID: string;
        private keyFinantialIndicatorsGridID: string;
        private costOfWasteGridID: string;

        private foodLossesNotIncludedInedibleParts: FoodLossContainer = null;
        private foodLossesInediblePartsOnly: FoodLossContainer = null;

        //region Helper methods

        private _isAssessmentPresent(idStrings: string[]): boolean
        {
            return this._assessments.some(assessment => idStrings.indexOf(assessment) !== -1);
        }

        private _createLinkForMenuItem(menuItemKey: string, menuItemTitle: string, objectIdString: string, onclickFunctionName: string): JQuery
        {

            const span: JQuery = DX.DOM.createElement("span")
                .toJQ()
                .attr({ 'class': 'btn-text' })
                .text(menuItemTitle);

            const anchor: JQuery = DX.DOM.createElement("a")
                .toJQ()
                .attr({
                    'class': 'clickable-menuitem',
                    'data-automation-id': "_" + menuItemKey + "_",
                    'data-identifiable-string': objectIdString,
                    'data-menu-clickable': menuItemKey,
                    'data-onclick': menuItemKey + "_Click",
                    'href': 'javascript:void(0)',
                    'onclick': 'DX.Ctrl.findParentControlOfType(this, DX.DIRModule.AssessmentNavigator.AssessmentDashboardDialog).' + onclickFunctionName + '(this);return false;',
                    'title': menuItemTitle,
                });

            return anchor.append(span);
        }

        private _addItemToMenuGroup(menuGroup: JQuery, menuItemKey: string, menuItemTitle: string, objectIdString: string, onclickFunctionName: string): void
        {
            const anchor: JQuery = this._createLinkForMenuItem(menuItemKey, menuItemTitle, objectIdString, onclickFunctionName)

            const listItem: JQuery = DX.DOM.createElement("li")
                .toJQ()
                .attr({ 'data-dxmenuitemkey': menuItemKey })

            menuGroup.append(listItem.append(anchor));
        }

        private _addItemsToGroupInMenu(menu: JQuery, menuGroupKey: string, menuItemPrefix: string, objectIdsArr: string[], funcName?: string): void
        {
            const menuGroup: JQuery = menu.find('*[data-dxmenuitemkey="' + menuGroupKey + '"]').find('ul.dropdown-menu');
            let menuGroupLength: number = menuGroup.children("li").length;

            this._invokeController('GetAssessmentNames', { 'objectIdentifiableStrings': objectIdsArr }).success(this, (result: DX.Mvc.DataAjaxResult) =>
            {
                const assessmentDescriptions = result.getDataValue('assessmentDescriptions') as string[];

                for (let i = 0; i < objectIdsArr.length; i++)
                {
                    const menuItemKey: string = menuItemPrefix + (++menuGroupLength).toString();
                    const title: string = assessmentDescriptions[i];
                    this._addItemToMenuGroup(menuGroup, menuItemKey, title, objectIdsArr[i], funcName);
                }
            });
        }

        private _removeItemFromMenuGroup(menu: JQuery, menuGroupKey: string, identifiableStringId: string): void
        {
            const removeGroupList: JQuery = menu.find('*[data-dxmenuitemkey="' + menuGroupKey + '"]').find('ul.dropdown-menu');
            removeGroupList.find('a[data-identifiable-string="' + identifiableStringId + '"]').parent().remove();
        }

        private _removeItemFromStringCollection(collection: string[], item: string)
        {
            const index: number = collection.indexOf(item);
            if (index > -1)
                collection.splice(index, 1);
        }

        //endregion Helper methods

        //region Hotspot chart

        private get RadioButtonMassCost(): JQuery
        {
            if (this.radioButtonMassCost === null)
                this.radioButtonMassCost = DX.DOM.findJQElements('[data-dxlogicid="logicIdRadioButtonHotSpot"]');

            return this.radioButtonMassCost;
        }

        private get HotSpotChart()
        {
            if (this.hotSpotChart === null)
                this.hotSpotChart = this.$$('HotSpotBarChart');

            return this.hotSpotChart;
        }

        private get SpiderChartCostOfWaste()
        {
            if (this.spiderChartCostOfWaste === null)
                this.spiderChartCostOfWaste = this.$$('SpiderChartCostOfWaste');

            return this.spiderChartCostOfWaste;
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

        private get HotSpotGrid(): JQueryExtensions.JQGrid.Control
        {
            return this.$<JQueryExtensions.JQGrid.Control>(this.hotSpotGridID);
        }

        private get ProductionRatiosGrid(): JQueryExtensions.JQGrid.Control
        {
            return this.$<JQueryExtensions.JQGrid.Control>(this.productionRatiosGridID);
        }

        private get KeyFinantialIndicatorsGrid(): JQueryExtensions.JQGrid.Control
        {
            return this.$<JQueryExtensions.JQGrid.Control>(this.keyFinantialIndicatorsGridID);
        }

        private get CostOfWasteGrid(): JQueryExtensions.JQGrid.Control
        {
            return this.$<JQueryExtensions.JQGrid.Control>(this.costOfWasteGridID);
        }

        private get Container()
        {
            if (this.container === null)
                this.container = $("#hotSpotContainerId");

            return this.container;
        }

        private get FoodLossesNotIncludedInedibleParts()
        {
            if (this.foodLossesNotIncludedInedibleParts === null)
                this.foodLossesNotIncludedInedibleParts = this.$$('FoodLossesNotIncludedInediblePartsPartial');

            return this.foodLossesNotIncludedInedibleParts;
        }

        private get FoodLossesInediblePartsOnly()
        {
            if (this.foodLossesInediblePartsOnly === null)
                this.foodLossesInediblePartsOnly = this.$$('FoodLossesInediblePartsOnlyPartial');

            return this.foodLossesInediblePartsOnly;
        }

        constructor(element: HTMLElement)
        {
            super(element);
        }

        public __applyJSONData()
        {
            super.__applyJSONData();
            //this.showInDialog = this._getJSONData<boolean>("showHotSpotInDialog", false);
            this._assessments = this._getJSONData<string[]>("assmts", []);
            this.productionRatiosGridID = this._getJSONData<string>('productionRatiosGridID');
            this.keyFinantialIndicatorsGridID = this._getJSONData<string>('keyFinantialIndicatorsGridID');
            this.costOfWasteGridID = this._getJSONData<string>('costOfWasteGridID');
            this.hotSpotGridID = this._getJSONData<string>('hotSpotGridID');
        }

        protected _initialize(): void
        {
            // (OVERRIDE)	
            super._initialize();

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "AssessmentAlreadyPresent").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                this.assessmentPresentMsg = result.text;
            });

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "DashboardMaxNumExceeded").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                this.maxNumExceeded = result.text;
            });

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "CantRemoveLastAssessment").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                this.cantRemoveLastAssessmentMsg = result.text;
            });

            this._initializeEvents();
        }

        private _initializeEvents(): void
        {
            this.RadioButtonMassCost.on("change", (e) =>
            {
                const currentHotSpotValue: string = e.currentTarget.getAttribute("value");
                this._refreshHotSpot(currentHotSpotValue);
            });

            this._assignChartsOnInitializedHandler();

            const timeout = () => setTimeout(() => this._resizeCharts(), 500);
            const resize = () =>
            {
                if (($(window).width() < AssessmentDashboardDialog.WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE) && this.isResizeEnabled)
                    timeout();
            };

            resize();

            $(window).on('resize', () => resize());
        }

        //endregion Hotspot chart

        AddAssessment_Click(source: JQuery, eventArgs: DX.JQuery.ElementEvent)
        {
            this.isResizeEnabled = false;
            const menu = this.$().findByLogicId(this.DASHBOARDMENU);

            const activityExecutionParameters = [
                DX.JSOpenSearchDialogActivity.paramScope, "ASSESSMENT",
                DX.JSOpenSearchDialogActivity.paramObjectType, "ASSESSMENT",
                DX.JSOpenSearchDialogActivity.paramMultiSelect, true,
                DX.JSOpenSearchDialogActivity.paramAutoExecuteSearch, false,
                DX.JSOpenSearchDialogActivity.paramCustomPrefillCriterias, {}
            ];

            const activity: JSOpenSearchDialogActivity = new DX.JSOpenSearchDialogActivity(null);
            activity.execute(activityExecutionParameters).success(this, () =>
            {
                const addedAssessments = <string[]>activity.getOutputParam(DX.JSOpenSearchDialogActivity.paramOutSelIdStrings);

                if (!addedAssessments || addedAssessments.length === 0)
                    return;

                if (this._assessments.length + addedAssessments.length > AssessmentDashboardDialog.MAX_NUM_OF_ASSESSMENTS)
                {
                    DX.Notif.showNotifications($('#notificationContainer'), [new DX.Notif.Notification(DX.Notif.NotificationTypes.Error, String.format(this.maxNumExceeded, AssessmentDashboardDialog.MAX_NUM_OF_ASSESSMENTS))], true);
                    return;
                }

                if (this._isAssessmentPresent(addedAssessments))
                {
                    DX.Notif.showNotifications($('#notificationContainer'), [new DX.Notif.Notification(DX.Notif.NotificationTypes.Error, this.assessmentPresentMsg)], true);
                    return;
                }

                if (addedAssessments !== undefined && addedAssessments !== null && addedAssessments.length > 0)
                {
                    this._addItemsToGroupInMenu(menu, this.REMOVEGROUPMENU, 'Remove_', addedAssessments, 'RemoveAssessment_Click');
                }

                for (let entry of addedAssessments)
                    this._assessments.push(entry);

                this._refreshAll().then(() => this.isResizeEnabled = true);
            });
        }

        RemoveAssessment_Click(source?: JQuery)
        {
            const assessmentId: string = $(source)[0].dataset["identifiableString"];

            if (this._assessments.length == 1)
            {
                DX.Notif.showNotifications($('#notificationContainer'), [new DX.Notif.Notification(DX.Notif.NotificationTypes.Error, this.cantRemoveLastAssessmentMsg)], true);
                return;
            }

            if (this._isAssessmentPresent([assessmentId]))
            {
                const menu: JQuery = this.$().findByLogicId(this.DASHBOARDMENU);
                this._removeItemFromMenuGroup(menu, this.REMOVEGROUPMENU, assessmentId);
                this._removeItemFromStringCollection(this._assessments, assessmentId)

                this._refreshAll();
            }
        }

        Export_Click()
        {
            this._exportResults();
        }

        private _resizeCharts()
        {
            const windowWidth = $(window).width();

            [
                this.ProductionRatiosChart,
                this.KeyFinantialIndicatorsChart
            ].forEach(chart => this._resizeChart(chart, (windowWidth < AssessmentDashboardDialog.WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE ? 600 : 680)));

            const hotSpotChartWidth = windowWidth < AssessmentDashboardDialog.WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE ? 580 : 680;
            const costOfWasteChartSize = windowWidth < AssessmentDashboardDialog.WINDOW_WIDTH_TO_TRIGGER_CHART_RESIZE ? { width: 600, height: 425 } : { width: 680, height: 425 };

            this._resizeChart(this.HotSpotChart, hotSpotChartWidth);
            this._resizeChart(this.SpiderChartCostOfWaste, costOfWasteChartSize.width, costOfWasteChartSize.height);
        }

        private _assignChartsOnInitializedHandler()
        {
            const initializedEventHandler = (chart: Mvc.ChartControl) => this._trimLegendItems(chart);

            this.HotSpotChart.add_initialized(initializedEventHandler);
            this.KeyFinantialIndicatorsChart.add_initialized(initializedEventHandler);
            this.ProductionRatiosChart.add_initialized(initializedEventHandler);
        }

        private _resizeChart(chart: Mvc.ChartControl, width?: number, height?: number)
        {
            const activeChart = chart.getActiveChart();

            if (width)
                activeChart.getChartOptions().w = width;

            if (height)
                activeChart.getChartOptions().h = height;

            activeChart.__drawChart(`#${chart.getClientId()}`, activeChart.getData(), activeChart.getChartOptions());
            this._trimLegendItems(activeChart);
        }

        private _refreshAll()
        {
            return new Promise((resolve) =>
            {
                const currentHotSpotValue = $('[data-dxlogicid="logicIdRadioButtonHotSpot"]:checked').val() as string;
                this._refreshHotSpot(currentHotSpotValue);
                this._refreshCostOfWasteChart();
                this._refreshProductionRatiosChart();
                this._refreshKeyFinantialIndicatorsChart();

                this._refreshGrid(this.HotSpotGrid, 'RefreshHotSpotGrid', { hotSpotValue: currentHotSpotValue });
                this._refreshGrid(this.ProductionRatiosGrid, 'RefreshResultsGrid', { resultType: AssessmentResultType.MASS });
                this._refreshGrid(this.KeyFinantialIndicatorsGrid, 'RefreshResultsGrid', { resultType: AssessmentResultType.PERCENTAGE });
                this._refreshGrid(this.CostOfWasteGrid, 'RefreshCostOfWasteGrid');

                this.FoodLossesNotIncludedInedibleParts.refresh(this._assessments);
                this.FoodLossesInediblePartsOnly.refresh(this._assessments);

                resolve();
            });
        }

        private _refreshHotSpot(currentHotSpotValue: string): void
        {
            this._invokeController("MassCostChange", { "hotSpotValue": currentHotSpotValue, "objectIdentifiableStrings": this._assessments }).success(this, (result: DX.Mvc.DataAjaxResult) =>
            {
                const data = result.getData<{ chartData: Mvc.ConnectedBarChartData[], legendTitles: string[] }>();
                
                this.HotSpotChart.getActiveChart().setData(data.chartData);
                const legendTitles = data.legendTitles.map(title => title = this._getTrimmedText(title));
                
                this.HotSpotChart.getActiveChart().setLegendText(legendTitles);
                this.HotSpotChart.getActiveChart().__drawChart(`#${this.HotSpotChart.getClientId()}`, data.chartData, this.HotSpotChart.getActiveChart().getChartOptions());
                this._refreshGrid(this.HotSpotGrid, 'RefreshHotSpotGrid', { hotSpotValue: currentHotSpotValue });

                //if (this.showInDialog)
                //{
                //    this.Container.dialog({ title: "HOTSPOT " + currentHotSpotValue })
                //    this.GroupedBarChart.setChartTitle("HOTSPOT " + currentHotSpotValue);
                //}

                switch (currentHotSpotValue)
                {
                    case this.COST: {
                        this._setHotspotChartAndTitle("AssessmentDashboard_CostHotspotTitle", "AssessmentDashboard_YAxisTitleHotspotCost");

                        break;
                    }
                    case this.MASS: {
                        this._setHotspotChartAndTitle("AssessmentDashboard_MassHotspotTitle", "AssessmentDashboard_YAxisTitleHotspotMass");

                        break;
                    }
                    default: {

                        break;
                    }
                } 
            });
        }

        private _setHotspotChartAndTitle(chartTitleResourceNameString: string, yAxisTitleResourceNameString: string)
        {
            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", chartTitleResourceNameString).getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                $('#hotspotChartTitle').text(result.text);
            });

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", yAxisTitleResourceNameString).getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
            {
                d3.select('[data-dxlogicid="HotSpotBarChart"]').select("g.y.axis>text:last-child").text(result.text);
            });
            
        }

        private _refreshCostOfWasteChart()
        {
            this._refreshChart(this.SpiderChartCostOfWaste, 'CostOfWasteChange', (result) =>
            {
                const data = result.getData<{ chartData: DX.Mvc.ChartData[], legendTitles: string[] }>();
                
                this.SpiderChartCostOfWaste.getActiveChart().setData(data.chartData);
                this.SpiderChartCostOfWaste.getActiveChart().setLegendText(data.legendTitles);
                (this.SpiderChartCostOfWaste.getActiveChart() as DX.Mvc.SpiderChart).setDataSpider([]);
                (this.SpiderChartCostOfWaste.getActiveChart() as DX.Mvc.SpiderChart).setScenariosActive([]);
            });
        }

        private _refreshProductionRatiosChart()
        {
            this._refreshChart(this.ProductionRatiosChart, 'KeyProductionRatiosChange');
        }

        private _refreshKeyFinantialIndicatorsChart()
        {
            this._refreshChart(this.KeyFinantialIndicatorsChart, 'KeyFinantialIndicatorsChange');
        }

        private _refreshChart(chart: DX.Mvc.ChartControl, actionMethod: string, callback?: (result: DX.Mvc.DataAjaxResult) => void)
        {
            this._invokeController(actionMethod, { "objectIdentifiableStrings": this._assessments }).success(this, (result: DX.Mvc.DataAjaxResult) =>
            {
                const data = result.getData<{ chartData: DX.Mvc.IChartData[] }>();
                let chartData = data.chartData;
                
                if (isDictionaryNameValueChartData(chartData))
                    (chartData as Mvc.DictionaryNameValueChartData[]).forEach(item => item.key = this._getTrimmedText(item.key));

                if (typeof callback !== 'undefined')
                    callback(result);

                chart.getActiveChart().setData(chartData);
                chart.getActiveChart().__drawChart(`#${chart.getClientId()}`, chartData, chart.getActiveChart().getChartOptions());
            });
        }

        private _refreshGrid(grid: JQueryExtensions.JQGrid.Control, actionName: string, ...params: object[])
        {
            let actionParameters = {
                'objectIdentifiableStrings': this._assessments.join('|')
            };

            actionParameters = $.extend({}, actionParameters, ...params);

            this._invokeController(actionName, actionParameters)
                .success(this, (result: DX.Mvc.DataAjaxResult) =>
                {
                    grid.setGridParam({ url: result.getDataValue("gridUrl"), page: 1 }).trigger('reloadGrid');
                })
        }

        private _trimLegendItems(chart: DX.Mvc.ChartControl, textLength: number = 26)
        {
            $(chart.get_element())
                .find('*[class$="legend"]')
                .find('text')
                .each((_, el) =>
                {
                    if (el.textContent.length > textLength)
                        el.textContent = this._getTrimmedText(el.textContent);
                });
        }

        private _getTrimmedText(text: string, textLength: number = 26)
        {
            if (text.length > textLength)
                text = `${text.substring(0, 23)}...`;

            return text;
        }

        private _exportResults()
        {
            this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Status: boolean, ScriptToExecute: string, ErrorMessage: string }>>("ExportResults", { "objectIdentifiableStrings": this._assessments }).success((result) =>
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
    }

    function isDictionaryNameValueChartData(data: Mvc.IChartData[]): data is Mvc.DictionaryNameValueChartData[]
    {
        try {
            return (data as Mvc.DictionaryNameValueChartData[])[0].key !== undefined;
        }
        catch {
            return false;
        }
    }

    enum AssessmentResultType
    {
        MASS = 'M',
        VALUE = 'V',
        PERCENTAGE = 'P'
    }
}