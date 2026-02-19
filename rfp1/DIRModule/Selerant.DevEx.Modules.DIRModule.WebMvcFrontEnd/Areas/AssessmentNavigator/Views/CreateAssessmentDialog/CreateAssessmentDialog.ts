namespace DX.DIRModule.AssessmentNavigator
{
    export class CreateAssessmentDialog extends DX.Mvc.DialogViewControl
    {
        static $$className = "DX.DIRModule.AssessmentNavigator.CreateAssessmentDialog";

        private _userNameTxtControl: DX.Mvc.TextControl = null;
		private _companyNameTxtControl: DX.Mvc.TextControl = null;
		private _productNameTxtControl: DX.Mvc.TextControl = null;
		private _timeFrameFromDateTimeControl: DX.Mvc.DateTimeControl = null;
        private _timeFrameToDateTimeControl: DX.Mvc.DateTimeControl = null;
		private _prodClassificationObjRefControl: DX.Mvc.ObjRefControl = null;
		private _orgStructureTxtControl: DX.Mvc.TextControl = null;
		private _locationTxtControl: DX.Mvc.TextControl = null;
        private _wasteDestinationLOVControl: DX.Mvc.ListOfValuesExtendedControl = null;
        private _industryLOVControl: DX.Mvc.LOVControl = null;
		private _lifecycleStagesLOVControl: DX.Mvc.LOVControl = null;
		private _dataQualityLOVControl: DX.Mvc.LOVControl = null;

		private readonly stepPanelDotClass = ".dx-step-panel";
		private readonly stepPanelHiddenClass = "dx-step-panel-hidden";
		private readonly stepPanelVisibleClass = "dx-step-panel-visible";
		private readonly stepKeys: string[] = Object.keys(Step);
		
		private activeStep: Step;
		private dialogJQcontext: JQuery = null;
		private _successMessage: string = "";
		private _canOpenBusinessDataDialog: boolean;

		private get DialogJQcontext(): JQuery
		{
			if (this.dialogJQcontext === null)
				this.dialogJQcontext = DX.DOM.findJQElements('.newAssessmentWizardDialog');
			return this.dialogJQcontext;
		}

		private get ProductNameTxtControl(): DX.Mvc.TextControl
		{
			if (this._productNameTxtControl === null)
				this._productNameTxtControl = this.$$<DX.Mvc.TextControl>('AssessmentProductName');
			return this._productNameTxtControl;
		}

		private get TimeFrameFromDateTimeControl(): DX.Mvc.DateTimeControl
		{
			if (this._timeFrameFromDateTimeControl === null)
				this._timeFrameFromDateTimeControl = this.$$<DX.Mvc.DateTimeControl>('AssessmentTimeFrameFrom');
			return this._timeFrameFromDateTimeControl;
		}

		private get TimeFrameToDateTimeControl(): DX.Mvc.DateTimeControl
		{
			if (this._timeFrameToDateTimeControl === null)
				this._timeFrameToDateTimeControl = this.$$<DX.Mvc.DateTimeControl>('AssessmentTimeFrameTo');
			return this._timeFrameToDateTimeControl;
		} 

		private get ProdClassificationObjRefControl(): DX.Mvc.ObjRefControl
        {
			if (this._prodClassificationObjRefControl === null)
				this._prodClassificationObjRefControl = this.$$<DX.Mvc.ObjRefControl>('AssessmentProdClassification');
			return this._prodClassificationObjRefControl;
		} 

		private get OrgStructureTxtControl(): DX.Mvc.TextControl
		{
			if (this._orgStructureTxtControl === null)
				this._orgStructureTxtControl = this.$$<DX.Mvc.TextControl>('AssessmentOrgStructure');
			return this._orgStructureTxtControl;
		} 

		private get LocationTxtControl(): DX.Mvc.TextControl
		{
			if (this._locationTxtControl === null)
				this._locationTxtControl = this.$$<DX.Mvc.TextControl>('AssessmentLocation');
			return this._locationTxtControl;
		} 

		private get WasteDestinationLOVControl(): DX.Mvc.ListOfValuesExtendedControl
		{
			if (this._wasteDestinationLOVControl === null)
				this._wasteDestinationLOVControl = this.$$<DX.Mvc.ListOfValuesExtendedControl>('AssessmentWasteDestination');
			return this._wasteDestinationLOVControl;
		}

        private get IndustryLOVControl(): DX.Mvc.LOVControl
		{
			if (this._industryLOVControl === null)
				this._industryLOVControl = this.$$<DX.Mvc.LOVControl>('AssessmentIndustry');
			return this._industryLOVControl;
		}

		private get LifecycleStagesLOVControl(): DX.Mvc.LOVControl
		{
			if (this._lifecycleStagesLOVControl === null)
				this._lifecycleStagesLOVControl = this.$$<DX.Mvc.LOVControl>('AssessmentLifecycleStages');
			return this._lifecycleStagesLOVControl; 
		}

		public get DataQualityLOVControl(): DX.Mvc.LOVControl
        {
			if (this._dataQualityLOVControl === null)
				this._dataQualityLOVControl = this.$$<DX.Mvc.LOVControl>('AssessmentDataQuality');
			return this._dataQualityLOVControl;
        }

		constructor(element: HTMLElement)
		{
			super(element);

			this.activeStep = Step.Step1st;
        }

		protected _initialize(): void
		{
            // (OVERRIDE)	
            super._initialize();
			DX.DOM.findJQElements("#" + this.activeStep).toggleClass(this.stepPanelHiddenClass).toggleClass(this.stepPanelVisibleClass);
            
			this._userNameTxtControl = this.$$("AssessmentUserName");
            this._companyNameTxtControl = this.$$("AssessmentCompanyName");
			this._productNameTxtControl = this.$$("AssessmentProductName");
			this._timeFrameFromDateTimeControl = this.$$("AssessmentTimeFrameFrom");
			this._timeFrameToDateTimeControl = this.$$("AssessmentTimeFrameTo");
			this._prodClassificationObjRefControl = this.$$("AssessmentProdClassification");
			this._orgStructureTxtControl = this.$$("AssessmentOrgStructure");
			this._locationTxtControl = this.$$("AssessmentLocation");
			this._wasteDestinationLOVControl = this.$$("AssessmentWasteDestination");
			this._industryLOVControl = this.$$("AssessmentIndustry");
			this._lifecycleStagesLOVControl = this.$$("AssessmentLifecycleStages");
			
			this._dataQualityLOVControl = this.$$('AssessmentDataQuality');

			const currentUserDateTimeFormat = DX.Loc.getCurrentUserDateTimeFormatData();
			this._timeFrameFromDateTimeControl.setFormat(currentUserDateTimeFormat);
			this._timeFrameToDateTimeControl.setFormat(currentUserDateTimeFormat);

			this._prodClassificationObjRefControl.setIsSearchEnabled(false);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "SucccessfullyCreatedAssessment").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
			{
                this._successMessage = result.text;
            });

            this._registerControlEvents();

            const firstValue = this._industryLOVControl.getItems().getItems()[0];
            if (firstValue)
				this._industryLOVControl.setValue(firstValue.value, true, true);
        }

		__applyJSONData(): void
		{
            // (OVERRIDE)
			super.__applyJSONData();

			this._canOpenBusinessDataDialog = this._getJSONData<boolean>("canOpenBusinessDataDialog");
		}

		protected _registerControlEvents(): void
		{
			let isFirstLoad = true;
			this._timeFrameFromDateTimeControl.add_valueChanged((sender) =>
			{
				if (sender.getValue() != null)
				{
					if (isFirstLoad)
					{
						const timeFrameFromDateValue = sender.getValue();
						const newValue = new Date(timeFrameFromDateValue.getFullYear() + 1, timeFrameFromDateValue.getMonth(), timeFrameFromDateValue.getDate());

						this.TimeFrameToDateTimeControl.setValue(newValue);
						const timeFrameToMinValue = new Date(timeFrameFromDateValue.getFullYear(), timeFrameFromDateValue.getMonth(), timeFrameFromDateValue.getDate() + 1);
						(this.TimeFrameToDateTimeControl.getValidationSettings() as Ctrl.DateTimeValidationSettings).setMinValue(timeFrameToMinValue);
					}
				}
				else
				{
					this.TimeFrameToDateTimeControl.setValue(null);
				}
				isFirstLoad = false;
			});

		}

		protected _onMenuItemClick(eventArgs?: DX.Mvc.MenuItemClickEventArgs): void
		{
            // (OVERRIDE)
			super._onMenuItemClick(eventArgs);

            if (eventArgs.getHandled())
                return;

			switch (eventArgs.getMenuItem().getKey())
			{
				case DialogButton.FinishBtn:
					{
						if (!this.IsValidSecondStep())
							return;

						this._createNewAssessmentAndCloseDialog(this._getJSONFormData(), eventArgs);
                    }
                    break;

				case DialogButton.CancelBtn:
                    {
                        eventArgs.setHandled(true);
                        DX.Page.closeDialog();
                    }
					break;

				case DialogButton.PreviousBtn:
					{
						this.SwitchBackTab();
						eventArgs.setHandled(true);
					}
					break;

				case DialogButton.NextStepBtn:
					{
						const stepKeys: string[] = Object.keys(Step);
						var currentStepIdx = stepKeys.indexOf(this.activeStep);
						if (currentStepIdx === 1)
						{
							if (!this.IsValidSecondStep())
								return;

							this._createNewAssessment(this._getJSONFormData(), eventArgs);
						}
						else
						{
							this.SwitchToNextTab();
							eventArgs.setHandled(true);
						}
					}
					break;
            }
		}

		private SetDialogMenuButtons(currentStep: Step)
		{
			let menu = this._getMenu();
			if (currentStep === Step.Step1st)
			{
				menu.getItem(DialogButton.PreviousBtn).setIsVisible(false);
				menu.getItem(DialogButton.NextStepBtn).setIsVisible(true);
				menu.getItem(DialogButton.FinishBtn).setIsVisible(false);
			}
			else if (currentStep === Step.Step2nd)
			{
				menu.getItem(DialogButton.CancelBtn).setIsVisible(true);
				menu.getItem(DialogButton.PreviousBtn).setIsVisible(true);
				menu.getItem(DialogButton.NextStepBtn).setIsVisible(this._canOpenBusinessDataDialog || false);
				menu.getItem(DialogButton.FinishBtn).setIsVisible(true);
			}
		}

		private SwitchToNextTab(): void
		{
			const currentStepIdx: number = this.stepKeys.indexOf(this.activeStep);
			let activeFound = false;
			DX.DOM.findJQElements(this.stepPanelDotClass).each((index, element) =>
			{
				let $elem = $(element);
				if ($elem.hasClass(this.stepPanelVisibleClass) && currentStepIdx === 0 && this.IsValidFirstStep())
				{
					activeFound = true;
					$("#Step1st").trigger('click').toggleClass(this.stepPanelVisibleClass).toggleClass(this.stepPanelHiddenClass);
				}
				else if ($elem.hasClass(this.stepPanelVisibleClass) && currentStepIdx === 1 && this.IsValidSecondStep())
				{
					activeFound = true;
					$("#Step2nd").trigger('click').toggleClass(this.stepPanelVisibleClass).toggleClass(this.stepPanelHiddenClass);
				}
				else if (activeFound && $elem.hasClass(this.stepPanelHiddenClass) && currentStepIdx === 0)
				{
					$("#Step2nd").trigger('click').toggleClass(this.stepPanelHiddenClass).toggleClass(this.stepPanelVisibleClass);
					activeFound = false;
					this.activeStep = DX.OOP.enumParse<Step>($elem.attr('id'), Step);
				}
			});
			this.SetDialogMenuButtons(this.activeStep);
		}

		private SwitchBackTab(): void
		{
			const currentStepIdx: number = this.stepKeys.indexOf(this.activeStep);

			if (!currentStepIdx)
				return;

			const previousStep = this.stepKeys[currentStepIdx - 1];
			this.DialogJQcontext.find('#' + this.activeStep.toString()).toggleClass(this.stepPanelVisibleClass).toggleClass(this.stepPanelHiddenClass);
			this.DialogJQcontext.find('#' + previousStep).trigger('click').toggleClass(this.stepPanelHiddenClass).toggleClass(this.stepPanelVisibleClass);
			this.activeStep = DX.OOP.enumParse<Step>(previousStep, Step);
			this.SetDialogMenuButtons(this.activeStep);
		}

		private IsValidFirstStep(): boolean
        {
            return [
                this.ProductNameTxtControl.validate(),
				this.ProdClassificationObjRefControl.validate(),
                this.TimeFrameFromDateTimeControl.validate(),
                this.TimeFrameToDateTimeControl.validate()
            ].every(isValid => isValid === true);
		}

		private IsValidSecondStep(): boolean
        {
            return [
                this.LocationTxtControl.validate(),
				this.OrgStructureTxtControl.validate(),
                this.WasteDestinationLOVControl.validate(),
                this.IndustryLOVControl.validate(),
                this.LifecycleStagesLOVControl.validate(),
				this.DataQualityLOVControl.validate()
            ].every(isValid => isValid === true);
		}

		protected _createNewAssessment(jsonFormData: string, eventArgs: DX.Mvc.MenuItemClickEventArgs)
		{
			this.showLoadingPanel();

			this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult & { Activity: JSOpenDialogActivity }>>("CreateNewAssessment", { formData: jsonFormData })
				.success(this, (result: DX.Mvc.TypedDataAjaxResult<BaseActionResult & { Activity: JSOpenDialogActivity }>) =>
				{
					eventArgs.setHandled(true);

					const errorMessage = result.data().ErrorMessage;
					if (errorMessage !== "")
					{
						DevEx.HostPage.showAlert(errorMessage, "error");
						this.hideLoadingPanel();
						DX.Page.closeDialog(false);
					}
					else
					{
						const activity = result.data().Activity;
						activity.execute().success(result, () =>
						{
							this.hideLoadingPanel();
							DX.Page.closeDialog(false);
						});
					}
				})
				.error(this, () =>
				{
					eventArgs.setHandled(true);
					this.hideLoadingPanel();
				});
		}

		protected _createNewAssessmentAndCloseDialog(jsonFormData: string, eventArgs: DX.Mvc.MenuItemClickEventArgs)
		{
			this.showLoadingPanel();

			this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult & { Url: string }>>("CreateNewAssessmentAndCloseDialog", { formData: jsonFormData })
				.success(this, (result) =>
				{
					this.hideLoadingPanel();
					eventArgs.setHandled(true);
					DX.Page.closeDialog(false);

					const errorMessage = result.data().ErrorMessage;
					if (errorMessage !== "")
					{
						DevEx.HostPage.showAlert(errorMessage, "error");
					}
					else
					{
						DevEx.HostPage.showAlert(this._successMessage, "info");
						DevEx.HostPage.openTab(result.data().Url);
					}
				})
				.error(this, () =>
				{
					eventArgs.setHandled(true);
					this.hideLoadingPanel();
				});
		}

		protected _getJSONFormData()
		{
			var _self = this;

			var jsonFormData: string = JSON.stringify(
				{
					"UserName": _self._userNameTxtControl.getValue(),
					"CompanyName": _self._companyNameTxtControl.getValue(),
					"ProductName": _self._productNameTxtControl.getValue(),
					"TimeFrameFrom": _self._timeFrameFromDateTimeControl.getValue(),
					"TimeFrameTo": _self._timeFrameToDateTimeControl.getValue(),
					"ProdClassification": _self._prodClassificationObjRefControl.getValue(),
					"OrgStructure": _self._orgStructureTxtControl.getValue(),
					"Location": _self._locationTxtControl.getValue(),
					"WasteDestinationCode": _self.WasteDestinationLOVControl.getValues(),
					"Industry": _self.IndustryLOVControl.getValue(),
					"LifecycleStages": _self._lifecycleStagesLOVControl.getValues(),
					"DataQuality": _self._dataQualityLOVControl.getValue()
				});

            return jsonFormData;
        }
	}

	enum Step
	{
		Step1st = 'Step1st',
		Step2nd = 'Step2nd'
	}

	enum DialogButton
	{
		PreviousBtn = 'PrevButton',
		CancelBtn = 'CancelButton',
		NextStepBtn = 'NextButton',
		FinishBtn = 'FinishButton'
	}
}
