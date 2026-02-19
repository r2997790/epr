namespace DX.DIRModule.AssessmentNavigator
{
	export class BusinessDataDialog extends DX.Mvc.DialogViewControl implements IObserver
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.BusinessDataDialog";

		private viewContainerClass: string;
		private identifiableString: string;
		private lcStageStepsPartial: LCStageSteps = null;
		private timeline: Timeline = null;

		private tabStrip: DX.Mvc.Tabstrip = null;
		private stepsPartialViewReference: Dictionary<GridTab> = null;
		private cancelOrCloseDeletes: boolean;

		// resource text, lables
		private promptMessageButtonCancel: string;
		private promptButtonYes: string;
		private promptButtonNo: string;
        private unsavedChangesWarning: string;
        private massBalanceErrorMessage: string;
		private successMessage: string;
		private carryProductOverToNextStageMessage: string;
		private mergeInputsOfCarriedOverProductMessage: string;

		private get LcStageStepsPartial(): LCStageSteps
		{
			if (this.lcStageStepsPartial === null)
				this.lcStageStepsPartial = this.$$('LcStageStepsPartial');

			return this.lcStageStepsPartial;
		}

		private get Timeline(): Timeline
		{
			if (this.timeline === null)
				this.timeline = this.LcStageStepsPartial.Timeline;

			return this.timeline;
		}

		constructor(element: HTMLElement)
		{
			super(element);
		}

		public __applyJSONData(): void
		{
			// (OVERRIDE)
			super.__applyJSONData();

			this.viewContainerClass = this._getJSONData<string>("viewContainerClass", '');
			this.identifiableString = this._getJSONData<string>("identifiableString", '');
			this.cancelOrCloseDeletes = this._getJSONData<boolean>("cancelOrCloseDeletes", true);
		}

		update(lcStage: LcStageStep): void
		{
			Object.keys(this.stepsPartialViewReference).forEach((key) => this.stepsPartialViewReference[key].ChangeLcStage(lcStage.StageId));
		}

		protected _initialize(): void
		{
			// (OVERRIDE)	
			super._initialize();

			this.tabStrip = this.$$('tabStripResourcesGrids');
			this._displayOnlyActiveTab();

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
			const dialogDomJQ = this.DOM().toJQ();

			gridTabs.forEach(tab =>
			{
				tab.outerInitialize();

				tab.ParentNotificationsContainer = notificationContainer;
				tab.OuterMainParent = dialogDomJQ;
				tab.SetResourceNoteOuterMainParent(dialogDomJQ);
			});

			gridTabs[0].ViewContainerClass = this.viewContainerClass;

			this.stepsPartialViewReference = ResourceManagement.Shared.buildStepReferenceDict(gridTabs, this.LcStageStepsPartial.Steps);

			this.LcStageStepsPartial.Timeline.subscribe(this);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "BusinessDataDialog_PromptMessageCancelButton")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptMessageButtonCancel = result.text);

			DX.Loc.TextDescriptor.newByResource("Controls", "DlgYes")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptButtonYes = result.text);

			DX.Loc.TextDescriptor.newByResource("Controls", "DlgNo")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.promptButtonNo = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "BusinessDataDialog_UnsavedChangesWarning")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.unsavedChangesWarning = result.text);

            DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "DestinationMassBalanceError")
                .getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.massBalanceErrorMessage = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "SucccessfullyCreatedAssessment")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.successMessage = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "CarryProductOverToTheNextStageMessage").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
			{
				this.carryProductOverToNextStageMessage = result.text;
			});

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "MergeInputsOfCarriedOverProductMessage").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), (result) =>
			{
				this.mergeInputsOfCarriedOverProductMessage = result.text;
			});

			const interval = setInterval(() =>
            {
				const inputTab = gridTabs[0];

				if (inputTab.getIsInitialized() && !inputTab.IsGridLoading())
				{
					inputTab.ReloadGrid();
					clearInterval(interval);
				}
			}, 1000);

			// attach handler on Manage LC Stage gear icon
			this.LcStageStepsPartial.onManageStagesClick = DX.OOP.createDelegate(this, this.manageStageClickHandler);
		}

		private manageStageClickHandler()
		{
			const entityType = EntityType.LcStage;

			this._invokeController<Mvc.TypedDataAjaxResult<{ activity: JSOpenDialogActivity }>>("ManageAssessmentRaiseDialog", { identifiableString: this.identifiableString, entityType: entityType })
				.success(this, (result) =>
				{
					const activity = result.data().activity;
					activity.execute().success(this, () =>
					{
						if (activity.getButtonClicked() === 'Ok')
						{
							const closingDlgResponse = activity.getClosingDialogData().getReturnValues() as ManagementDlgResponseBase;
							if (closingDlgResponse.success)
							{
								this.showAlert(closingDlgResponse.message, "info");
							}
							else
							{
								this.showAlert(closingDlgResponse.message, "error");
								throw new Error('DIRECT LC Stage Management: ' + closingDlgResponse.message);
							}

							this._invokeController('LCStageStepsPartial', { assessmentIdent: this.identifiableString })
								.success(this, (result: DX.Mvc.ViewResult) =>
								{
									this.LcStageStepsPartial.Timeline.unsubcribe(this);

									const timelineContainer = this.$().find("#lc-stage-timeline").empty();
									DX.Ctrl.appendView(timelineContainer, result, () =>
									{
										this.lcStageStepsPartial = timelineContainer.findControlByType<DX.DIRModule.AssessmentNavigator.LCStageSteps>("DX.DIRModule.AssessmentNavigator.LCStageSteps");
                                        this.timeline = null;
                                        
                                        this.LcStageStepsPartial.Timeline.subscribe(this);
                                        this.LcStageStepsPartial.Timeline.subscribe(this.stepsPartialViewReference[this.Timeline.CurrentStageStep.StepId].ResourceNotes);
                                        this.LcStageStepsPartial.Timeline.reset();

										this._setTab();
                                        this._setDialogMenuButtons();
                                        this._reloadTabGrid();
                                        this._reloadTabNotes();

										// reattach handler
										this.LcStageStepsPartial.onManageStagesClick = DX.OOP.createDelegate(this, this.manageStageClickHandler);
									});
								});
						}
					});
				});
		}

        protected async _onMenuItemClick(eventArgs?: DX.Mvc.MenuItemClickEventArgs): Promise<void>
		{
			// (OVERRIDE)
			super._onMenuItemClick(eventArgs);

            if (eventArgs.getHandled())
                return;

            switch (eventArgs.getMenuItemKey())
            {
				case DialogButton.NextStepBtn:
					{
						if (!this._preventUnsavedChanges())
							return;

                        if (this.Timeline.NextStageStep.StageIdentifiableString != this.Timeline.CurrentStageStep.StageIdentifiableString)
                        {
                            this.showLoadingPanel();
                            const hasProductsToCarryOverAction = await this.hasProductsToCarryOver();
                            this.showLoadingPanel();

                            if (hasProductsToCarryOverAction)
                                this.showCarryProductOverToNextStagePrompt();
                            else
                                this._goToNextStage();
                        }
                        else
                        {
                            this._goToNextStage();
                        }

						eventArgs.setHandled(true);
						break;
					}
                case DialogButton.BackBtn:
					{
						if (!this._preventUnsavedChanges())
							return;

						if (typeof this.Timeline.PreviousStageStep !== 'undefined' && this.Timeline.PreviousStageStep.StageIdentifiableString !== this.Timeline.CurrentStageStep.StageIdentifiableString)
						{
							this.lcStageStepsPartial.unmarkLcStageAsCompleted(this.Timeline.PreviousStageStep.StageIdentifiableString);
							this.LcStageStepsPartial.markLcStageAsInProgress(this.Timeline.PreviousStageStep.StageIdentifiableString);
							this.LcStageStepsPartial.unmarkLcStageAsInProgress(this.Timeline.CurrentStageStep.StageIdentifiableString);
						}

                        this.Timeline.goToPreviousStep();

                        this._setDialogMenuButtons();
						this._setTab();
						this._reloadTabGrid();

                        eventArgs.setHandled(true);

                        break;
                    }
                case DialogButton.CancelBtn:
                    {
                        eventArgs.setHandled(true);

						let confirmFunc: () => void;
						if (this.cancelOrCloseDeletes)
						{
							confirmFunc = () => { this.CancelAssessmentCreation(this.identifiableString); };
						}
						else
						{
							// just close dialog
							confirmFunc = () =>
							{
								this._hideLoadingPanel();
								DX.Page.closeDialog(false);
							};
						}

						this.showPrompt(this.promptMessageButtonCancel, [
							{
								caption: this.promptButtonYes, type: 'confirm', func: confirmFunc
                            },
							{
								caption: this.promptButtonNo, type: 'cancel', func: () => {}
                            }
                        ], false);

                        break;
                    }
                case DialogButton.FinishBtn:
					{
						if (!this._preventUnsavedChanges())
							return;

						eventArgs.setHandled(true);
						this._changeStatusAndCloseDialog();
                        break;
                    }
            }
        }

        private showCarryProductOverToNextStagePrompt()
        {
            this.showPrompt(this.carryProductOverToNextStageMessage, [
                {
                    caption: this.promptButtonYes,
                    type: 'confirm',
					func: () => this.showMergeInputsPrompt()
                },
                {
                    caption: this.promptButtonNo,
                    type: 'cancel',
                    func: () =>
					{
						this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Success: boolean }>>('ClearNextLcStageSource', { nextLcStageId: this.Timeline.NextStageStep.StageId })
							.success(this, result =>
							{
								this._goToNextStage();
							});
                    }
                }
            ], false)
        }

        private showMergeInputsPrompt()
        {
            this.showPrompt(this.mergeInputsOfCarriedOverProductMessage, [
                {
                    caption: this.promptButtonYes,
                    type: 'confirm',
                    func: () => this.carryProductOverToTheNextStage(true)
                },
                {
                    caption: this.promptButtonNo,
                    type: 'cancel',
                    func: () => this.carryProductOverToTheNextStage(false)
                }
            ], false);
        }

        private async hasProductsToCarryOver()
        {
            return new Promise<boolean>((resolve) =>
            {
                this._invokeController<DX.Mvc.TypedDataAjaxResult<{ HasInputs: boolean }>>('HasProductsToCarryOver', {
                    identifiableString: this.identifiableString,
                    lcStageId: this.Timeline.CurrentStageStep.StageId,
                    nextLcStageId: this.Timeline.NextStageStep.StageId
                })
                .success(this, result =>
                {
                    resolve(result.data().HasInputs);
                });
            });
        }

        private carryProductOverToTheNextStage(mergeInputs: boolean)
		{
			const requestData = { identifiableString: this.identifiableString, lcStageId: this.Timeline.CurrentStageStep.StageId, nextLcStageId: this.Timeline.NextStageStep.StageId, mergeInputs };
			this._invokeController<DX.Mvc.TypedDataAjaxResult<{ Activity?: JSOpenDialogActivity }>>('CarryProductOverToTheNextStage', requestData)
                .success(this, (result) =>
                {
                    const response = result.data();

					if (!mergeInputs)
                    {
                        this._goToNextStage(true);
                    }
                    else
                    {
                        const activity = response.Activity;

                        activity.execute().success(this, _ =>
                        {
                            const buttonClicked = activity.getClosingDialogData().getPressedButtonKey();

                            if (buttonClicked == 'Ok')
                            {
                                this._goToNextStage(true);
                            }
                            else
                            {
                                this.carryProductOverToTheNextStage(false);
                            }
                        });
                    }
                });
        }

        private _goToNextStage(hasCarriedProduct: boolean = false)
        {
            this.Timeline.goToNextStep();

            this._setDialogMenuButtons();
            this._setTab();

            this._reloadTabGrid(hasCarriedProduct);

            if (this.Timeline.CurrentStageStep.StageIdentifiableString != this.Timeline.PreviousStageStep.StageIdentifiableString)
            {
                this.LcStageStepsPartial.markLcStageAsCompleted(this.Timeline.PreviousStageStep.StageIdentifiableString);
                this.LcStageStepsPartial.markLcStageAsInProgress(this.Timeline.CurrentStageStep.StageIdentifiableString);
            }
            else
            {
                this.LcStageStepsPartial.markLcStageAsInProgress(this.Timeline.CurrentStageStep.StageIdentifiableString);
			}
        }

		private _preventUnsavedChanges(): boolean
		{
            const gridPartialReference = this.stepsPartialViewReference[this.Timeline.CurrentStageStep.StepId];

            if (gridPartialReference.IsInEditMode)
			{
				gridPartialReference.SetErrorOutline(gridPartialReference.CurrentRowId);
				DevEx.HostPage.showAlert(this.unsavedChangesWarning, "warning", 2500);

				return false;
            }
            if (!gridPartialReference.IsValid())
            {
                DX.Notif.showNotification(gridPartialReference.ParentNotificationsContainer, new DX.Notif.Notification(DX.Notif.NotificationTypes.Error, this.massBalanceErrorMessage), true);
                return false;
            }

			return true;
		}

        private _setDialogMenuButtons()
        {
            const menu = this._getMenu();

            menu.getItem(DialogButton.BackBtn).setIsVisible(!this.Timeline.IsFirstStep);

            const isLastStep = this.Timeline.IsLastStep;

            menu.getItem(DialogButton.NextStepBtn).setIsVisible(!isLastStep);
            menu.getItem(DialogButton.FinishBtn).setIsVisible(isLastStep);
        }

        private _setTab()
		{
            this.tabStrip.selectTab(this.Timeline.CurrentStageStep.StepId);
			this._displayOnlyActiveTab();
        }

        private _displayOnlyActiveTab()
		{
            this.tabStrip.jQuerySelect('.dx-tab:not(.focused)').hide();
            this.tabStrip.jQuerySelect('.dx-tab.focused').show();
		}

        private _reloadTabGrid(hasCarriedProduct: boolean = false)
        {
            this.stepsPartialViewReference[this.Timeline.CurrentStageStep.StepId].IsProductCarried = hasCarriedProduct;
            this.stepsPartialViewReference[this.Timeline.CurrentStageStep.StepId].showLoadingPanel();
            this.stepsPartialViewReference[this.Timeline.CurrentStageStep.StepId].ReloadGrid();
        }

        private _reloadTabNotes()
        {
            this.stepsPartialViewReference[this.Timeline.CurrentStageStep.StepId].ResourceNotes.update(this.Timeline.CurrentStageStep);
        }

		private _changeStatusAndCloseDialog(): void
		{
			this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult & { Url: string }>>("ChangeStatusAndCloseDialog", { identifiableString: this.identifiableString})
				.success(this, (result: DX.Mvc.TypedDataAjaxResult<BaseActionResult & { Url: string }>) =>
				{
					this._hideLoadingPanel();
					DX.Page.closeDialog(false).setReturnValue("closeAllDialogs", true); // added

					const errorMessage = result.data().ErrorMessage;
					if (errorMessage !== "")
						DevEx.HostPage.showAlert(errorMessage, "error");
					else
					{
						DevEx.HostPage.showAlert(this.successMessage, "info");
						DevEx.HostPage.openTab(result.data().Url);
					}
				});
		}

		private CancelAssessmentCreation(identifiableString: string): void
		{
			this._invokeController<DX.Mvc.TypedDataAjaxResult<BaseActionResult>>("DeleteAssessment", { identifiableString: identifiableString })
				.success(this, (result: DX.Mvc.TypedDataAjaxResult<BaseActionResult>) =>
				{
					this._hideLoadingPanel();
					DX.Page.closeDialog(false).setReturnValue("closeAllDialogs", true);

					const errorMessage = result.data().ErrorMessage;
					if (errorMessage !== "")
						DevEx.HostPage.showAlert(errorMessage, "error");
				});
		}
    }

    enum DialogButton
    {
        BackBtn = 'BackButton',
        CancelBtn = 'CancelButton',
        NextStepBtn = 'NextButton',
        FinishBtn = 'FinishButton'
	}

	export type BaseActionResult =
	{
		ErrorMessage: string
	}

	type GridTab = ResourceManagement.Shared.GridTab;
	type DestinationsGridTab = ResourceManagement.Shared.DestinationsGridTab;

	type ManagementDlgResponseBase = DX.DIRModule.AssessmentNavigator.ManagementDlgResponseBase;
}