namespace DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared
{
	export class ResourceNote extends DX.Mvc.ViewControl implements IObserver
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.ResourceManagement.Shared.ResourceNote";

		private readonly BUSINESS_COST: string = "BUSINESS_COST";
		private readonly NAVIGATOR: string = "Navigator";

		private resourceType: string = null;
		private lcStageId: number = null;
		private assessmentCode: string = null;
		private notes: DX.Mvc.TextControl = null;
		private businessCostOther?: DX.Mvc.TextControl = null;
		private viewMode: string = null;
		private topOuterParentElement: HTMLElement;

		public set LcStageId(value: number)
		{
			this.lcStageId = value;
		}

		public set BusinessCostOther(value: string)
		{
			this.businessCostOther.setValue(value);
		}

		public set TopOuterParentElement(value: HTMLElement)
		{
			this.topOuterParentElement = value;
		}

		constructor(element: HTMLElement)
		{
			super(element);
		}

		public __applyJSONData()
		{
			// (OVERRIDE)
			super.__applyJSONData();
			this.resourceType = this._getJSONData<string>('resourceType');
			this.lcStageId = this._getJSONData<number>('lcStageId');
			this.assessmentCode = this._getJSONData<string>('assessmentCode');
			this.viewMode = this._getJSONData<string>('viewMode');
		}

		protected _initialize(): void
		{
			// (OVERRIDE)
			super._initialize();
			this.notes = this.$$("txbNotes");

			this.SetControlsForIObserver();

			if (this.resourceType === this.BUSINESS_COST)
			{
				this.businessCostOther = this.$$("txbBusinessCostOther");
			}

			this.$$<DX.Mvc.ButtonControl>("btnSave").add_click(() =>
			{
				this.SaveResourceNotes();
			});
		}

		public showLoadingPanel(): void
		{
			// (OVERRIDE)
			LP.showLoadingPanel(this.topOuterParentElement);
		}

		public hideLoadingPanel(): void
		{
			// (OVERRIDE)
			LP.hideLoadingPanel(this.topOuterParentElement);
		}

		update(currentLcStageStep: LcStageStep, previousLcStageStep?: LcStageStep): void
		{
			this.lcStageId = currentLcStageStep.StageId;

			this.__invokeController<Mvc.TypedDataAjaxResult<{ Notes?: string, BusinessCostOther?: string, HasBusinessCostOther: boolean }>>("GetNotes", { "resourceType": this.resourceType, "lcStageId": this.lcStageId, "assessmentCode": this.assessmentCode })
				.success((result) =>
				{
					let response = result.getData<{ Notes?: string, BusinessCostOther?: string, HasBusinessCostOther: boolean }>();

					this.$$("txbNotes").setValue(response.Notes);

					response.HasBusinessCostOther ? this.$().find("#BusinessCostOtherNotes").show() : this.$().find("#BusinessCostOtherNotes").hide();
					response.HasBusinessCostOther ? this.$$("txbBusinessCostOther").setValue(response.BusinessCostOther) : this.$$("txbBusinessCostOther").setValue("");
				});
		}

		private SaveResourceNotes()
		{
			let otherCost = this.businessCostOther == null ? "" : this.businessCostOther.getValue();

			this.showLoadingPanel();
			this._invokeController("SaveResourceNote", { "resourceType": this.resourceType, "lcStageId": this.lcStageId, "notes": this.notes.getValue(), "assessmentCode": this.assessmentCode, "businessCostOther": otherCost })
				.success((result) =>
				{
					this.hideLoadingPanel();
					let message = (result as DX.Mvc.DataAjaxResult).getDataValue<string>("message");
					DevEx.HostPage.showAlert(message, "info", 3000);
				});
		}

		private SetControlsForIObserver()
		{
			let parent: DX.Ctrl.BaseControl;

			if (this.viewMode === this.NAVIGATOR)
			{
				parent = DX.Ctrl.findParentControlOfType(this.get_element(), DX.Mvc.NavigatorBase) as DX.Mvc.NavigatorBase;
			}
			else
			{
				parent = DX.Ctrl.findParentControlOfType(this.get_element(), DX.DIRModule.AssessmentNavigator.BusinessDataDialog);
			}

			const lcStageStepsPartial = DX.Ctrl.findControlOfType(parent.get_element(), DX.DIRModule.AssessmentNavigator.LCStageSteps);
			lcStageStepsPartial.Timeline.subscribe(this);

		}
	}
}