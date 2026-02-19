namespace DX.DIRModule.AssessmentNavigator
{
    export class GeneralPanelPartial extends DX.Mvc.ViewControl
    {
		static $$className = "DX.DIRModule.AssessmentNavigator.GeneralPanelPartial";
		targetIdentifiableString: string;
		txtCompanyName: DX.Mvc.TextControl;
		txtComments: DX.Mvc.TextControl;
		txtDescription: DX.Mvc.TextControl;
		txtAssessmentTimeFrameFrom: DX.Mvc.DateTimeControl;
		txtAssessmentTimeFrameTo: DX.Mvc.DateTimeControl;
        txtPhoneNumber: DX.Mvc.TextControl;
		objRefProdClassification: DX.Mvc.ObjRefControl;
		txtOrgStructure: DX.Mvc.TextControl;
		objLocation: DX.Mvc.ObjRefControl;
		ddlDataQuality: DX.Mvc.ObjLOVControl;
        displayMode: DisplayModeType;

        constructor(element: HTMLElement)
        {
			super(element);	
        }

        public __applyJSONData()
        {
            // (OVERRIDE)
            super.__applyJSONData();		
			var defaultDisplayMode: number = DisplayModeType.View;
            this.displayMode = this._getDOMData("DisplayMode", defaultDisplayMode);
		}

        public IsValid(): boolean
		{
			return [
                this.txtDescription.validate(),
				this.objRefProdClassification.validate(),
				this.txtOrgStructure.validate(),
                this.txtAssessmentTimeFrameFrom.validate(),
                this.txtAssessmentTimeFrameTo.validate(),
				this.objLocation.validate(),
				this.ddlDataQuality.validate(),
                this._isValidDate()
			].every(currentValue => currentValue === true);
        }

        private _isValidDate(): boolean
        {
            if (this.txtAssessmentTimeFrameFrom.getValue() != null && this.txtAssessmentTimeFrameTo.getValue() != null && this.txtAssessmentTimeFrameFrom.getValue().getTime() >= this.txtAssessmentTimeFrameTo.getValue().getTime())
            {
                this.txtAssessmentTimeFrameTo.setValidation(false);
                return false;
			}

            return true;
        }
        
		protected _initialize(): void
		{		
			// (OVERRIDE)
			super._initialize();
            this.targetIdentifiableString = DX.Url.getUrlKeyValues()["targetIdentifiableString"];
			this.txtCompanyName = this.$$("txtCompanyName");
			this.txtComments = this.$$("txtComments");
			this.txtDescription = this.$$("txtDescription");
			this.txtAssessmentTimeFrameFrom = this.$$("dtAssessmentTimeFrameFrom");
			this.txtAssessmentTimeFrameTo = this.$$("dtAssessmentTimeFrameTo");
            this.txtPhoneNumber = this.$$("txtPhoneNumber");
			this.objRefProdClassification = this.$$("objRefProdClassification");
			this.txtOrgStructure = this.$$("txtOrgStructure");
			this.objLocation = this.$$("objLocation");
			this.ddlDataQuality = this.$$("ddlDataQuality");

			const currentUserDateTimeFormat = DX.Loc.getCurrentUserDateTimeFormatData();
			this.txtAssessmentTimeFrameFrom.setFormat(currentUserDateTimeFormat);
			this.txtAssessmentTimeFrameTo.setFormat(currentUserDateTimeFormat);

			this.objRefProdClassification.setIsSearchEnabled(false);

		}

        public createGeneralModelPartialJson(): ModelPartialJson
		{			
			const generalModelPartialJson: ModelPartialJson =
			{
                TargetIdentifiableString: this.targetIdentifiableString,              
				CompanyName: this.txtCompanyName.getValue(),
				Comments: this.txtComments.getValue(),
				Description: this.txtDescription.getValue(),
				TimeframeFrom: this.txtAssessmentTimeFrameFrom.getValue(),
				TimeframeTo: this.txtAssessmentTimeFrameTo.getValue(),
                PhoneNumber: this.txtPhoneNumber.getValue(),
				ProdClassification: this.objRefProdClassification.getValue(),
				OrgStructure: this.txtOrgStructure.getValue(),
				Location: this.objLocation.getValue(),
				DataQuality: this.ddlDataQuality.getValue()
            };

            return generalModelPartialJson;
        }
    }

    interface ModelPartialJson 
	{
		TargetIdentifiableString: string;
		Description: string;
        ProdClassification: string;
		TimeframeFrom: Date;
		TimeframeTo: Date;
		CompanyName: string;
		Comments: string;
        PhoneNumber: string;
        OrgStructure: string;
		Location: string;
		DataQuality: string;
    };

    enum DisplayModeType
    {
        Edit = 1,
        View = 2,
    }
}