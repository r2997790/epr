namespace DX.DIRModule.AssessmentNavigator
{
	export class CopyModel extends DX.Mvc.DialogViewControl
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.CopyModel";
		
		private txtCompanyName: DX.Mvc.TextControl;
		private txtComments: DX.Mvc.TextControl;
		private txtDescription: DX.Mvc.TextControl;
		private txtAssessmentTimeFrameFrom: DX.Mvc.DateTimeControl;
		private txtAssessmentTimeFrameTo: DX.Mvc.DateTimeControl;
		private txtPhoneNumber: DX.Mvc.TextControl;
		private objRefProdClassification: DX.Mvc.ObjRefControl;
		private ddlOrgStructure: DX.Mvc.ObjLOVControl;
		private objRefLocation: DX.Mvc.ObjRefControl;
		private ddlDataQuality: DX.Mvc.ObjLOVControl;

		private successMessage: string;
		private errorMessage: string;

		constructor(element: HTMLElement)
		{
			super(element);
		}

		public __applyJSONData()
		{
			// (OVERRIDE)
			super.__applyJSONData();
		}

		private IsValid(): boolean
		{
			return [
				this.txtDescription.validate(),
				this.objRefProdClassification.validate(),
				this.objRefLocation.validate(),
                this.txtAssessmentTimeFrameFrom.validate(),
                this.txtAssessmentTimeFrameTo.validate(),
				this._isValidDate(),
				this.ddlDataQuality.validate()
			].every(currentValue => currentValue === true);
		}

		private _isValidDate(): boolean
		{
			if (this.txtAssessmentTimeFrameFrom.getValue() != null && this.txtAssessmentTimeFrameTo.getValue() != null && this.txtAssessmentTimeFrameFrom.getValue().getTime() >= this.txtAssessmentTimeFrameTo.getValue().getTime())
			{
				this.txtAssessmentTimeFrameFrom.setValidation(false);
				return false;
			}

			return true;
		}

		protected _initialize(): void
		{
			// (OVERRIDE)
			super._initialize();

			this.txtCompanyName = this.$$("txtCompanyName");
			this.txtComments = this.$$("txtComments");
			this.txtDescription = this.$$("txtDescription");
			this.txtAssessmentTimeFrameFrom = this.$$("dtAssessmentTimeFrameFrom");
			this.txtAssessmentTimeFrameTo = this.$$("dtAssessmentTimeFrameTo");
			this.txtPhoneNumber = this.$$("txtPhoneNumber");
			this.objRefProdClassification = this.$$("objRefProdClassification");
			this.ddlOrgStructure = this.$$("ddlOrgStructure");
			this.objRefLocation = this.$$("objRefLocation");
			this.ddlDataQuality = this.$$("ddlDataQuality");

			this.objRefProdClassification.setIsSearchEnabled(false);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "CopyAssessmentAction_Success")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.successMessage = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "CopyAssessmentAction_Error")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.errorMessage = result.text);

		}

		protected _onMenuItemClick(eventArgs?: DX.Mvc.MenuItemClickEventArgs): void
		{
			// (OVERRIDE)
			super._onMenuItemClick(eventArgs);

			if (eventArgs.getHandled())
				return;

			switch (eventArgs.getMenuItemKey())
			{
				case "CancelButton":
					DX.Page.closeDialog();
					break;
				case "OkButton":
					{
						if (!this.IsValid())
							return;

						this._showLoadingPanel();

						const assessmentData: string = JSON.stringify(this.createAssessmenCopytDataJson());
						this._invokeController<DX.Mvc.TypedDataAjaxResult<CopyAssessmentResult>>("CopyAssessment", { assessmentData: assessmentData })
							.success(this, (result: DX.Mvc.TypedDataAjaxResult<CopyAssessmentResult>) =>
							{
								this._hideLoadingPanel();
								DX.Page.closeDialog(false);

								const success = result.data().Success;
								if (success)
								{
									DevEx.HostPage.showAlert(this.successMessage, "info");
									DevEx.HostPage.openTab(result.data().Url);
								}
								else
								{
									DevEx.HostPage.showAlert(this.errorMessage, "error");
								}
							});
					}

					break;
			}
		}

		public createAssessmenCopytDataJson(): ModelAssessmentDataJson
		{
			const copyDataJson: ModelAssessmentDataJson =
			{
				CompanyName: this.txtCompanyName.getValue(),
				Comments: this.txtComments.getValue(),
				Description: this.txtDescription.getValue(),
				TimeframeFrom: this.txtAssessmentTimeFrameFrom.getValue(),
				TimeframeTo: this.txtAssessmentTimeFrameTo.getValue(),
				PhoneNumber: this.txtPhoneNumber.getValue(),
				ProdClassification: this.objRefProdClassification.getValue(),
				OrgStructure: this.ddlOrgStructure.getValue(),
				Location: this.objRefLocation.getValue(),
				DataQuality: this.ddlDataQuality.getValue()
			};

			return copyDataJson;
		}
	}

	interface ModelAssessmentDataJson 
	{
		Description: string;
		ProdClassification: string;
		TimeframeFrom: Date;
		TimeframeTo: Date;
		CompanyName: string;
		Comments: string;
		PhoneNumber: string;
		OrgStructure: string;
		Location: string;
		DataQuality: string
	};

	export type CopyAssessmentResult =
	{
		Success: boolean,
		Url: string
	}
}