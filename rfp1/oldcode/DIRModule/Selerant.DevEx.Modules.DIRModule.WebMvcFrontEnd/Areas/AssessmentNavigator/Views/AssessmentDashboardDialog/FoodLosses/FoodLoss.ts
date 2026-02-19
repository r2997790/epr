namespace DX.DIRModule.AssessmentNavigator
{
    export class FoodLoss extends DX.Mvc.ViewControl
    {
        static $$className = 'DX.DIRModule.AssessmentNavigator.FoodLoss';

        private chartId: string = null;
        private chart: DX.Mvc.ChartControl = null;

        public __applyJSONData()
        {
            super.__applyJSONData();

            this.chartId = this._getJSONData('chartId');
        }

        public get Chart()
        {
            if (this.chart === null)
                this.chart = this.$$(this.chartId);

            return this.chart;
        }

        public _initialize()
        {
            super._initialize();

            const resize = () => setTimeout(() => this.resizeChart(), 500);

            resize();
            $(window).on('resize', () => resize());
        }

        private resizeChart()
        {
            const windowWidth = $(window).width();
            const padding = 20;
            const chartWidth = windowWidth / 4 - padding * 2;

            const activeChart = this.Chart.getActiveChart();

            activeChart.getChartOptions().w = chartWidth;
            activeChart.__drawChart(`#${this.Chart.getClientId()}`, activeChart.getData(), activeChart.getChartOptions());
            $(`#${this.Chart.getClientId()} .chartTooltip`).addClass('hidden');
        }
    }
}