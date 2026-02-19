namespace DX.DIRModule.AssessmentNavigator
{
    export class FoodLossContainer extends DX.Mvc.ViewControl
    {
        static $$className = 'DX.DIRModule.AssessmentNavigator.FoodLossContainer';

        private foodLossType: number = null;
        
        protected _initialize()
        {
            super._initialize();
        }

        public __applyJSONData()
        {
            super.__applyJSONData();

            this.foodLossType = this._getJSONData('foodLossType');
        }

        public refresh(assessments: string[])
        {
            this._invokeController('RefreshFoodLossPartial', { objectIdentifiableStrings: assessments, foodLossType: this.foodLossType })
                .success(this, (result: string) =>
                {
                    $(this.get_element()).html(result);
                });
        }
    }
}