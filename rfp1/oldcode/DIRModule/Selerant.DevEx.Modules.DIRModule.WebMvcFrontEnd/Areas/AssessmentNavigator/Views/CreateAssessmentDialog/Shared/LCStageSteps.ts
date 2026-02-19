namespace DX.DIRModule.AssessmentNavigator
{
    export class LCStageSteps extends DX.Mvc.ViewControl
    {
        static $$className = 'DX.DIRModule.AssessmentNavigator.LCStageSteps';

        public readonly Steps: string[] = ['TabInputs', 'TabDestinations', 'TabNonFoodDestinations', 'TabOutputs', 'TabBusinessCosts'];

        private stagesContainer: JQuery = null;
		private viewLcStages: LcStage[] = [];
        private lcStageElements: LcStageHTMLLIElement[] = [];
        private viewMode: ViewMode;
		private timeline: Timeline = null;
		
        private get StagesContainer(): JQuery
        {
            if (this.stagesContainer === null)
                this.stagesContainer = DX.DOM.findJQElements('#DIR-lc-stages-list');

            return this.stagesContainer;
        }

        public get ViewLcStages(): LcStage[]
        {
            return this.viewLcStages;
        }

		public get ViewMode()
		{
			return this.viewMode;
		}

		public get Timeline(): Timeline
        {
            if (this.timeline === null)
            {
                if (this.viewMode === ViewMode.Dialog)
					this.timeline = new Timeline(this.ViewLcStages, this.Steps);
                else
					this.timeline = new Timeline(this.ViewLcStages);
            }

            return this.timeline;
        }

        private get LcStageElements(): LcStageHTMLLIElement[]
        {
            return this.lcStageElements;
		}

        public __applyJSONData()
        {
            super.__applyJSONData();

			this.viewLcStages = this._getJSONData<LcStage[]>('lcs', []);
            this.viewMode = this._getJSONData<ViewMode>('vmode');
        }

        protected _initialize()
        {
			super._initialize();
			this.viewLcStages.forEach(stage =>
			{
                const listElement = this._createLcStageHTMLLIElement(stage);

                this.StagesContainer.append(listElement);
                this.lcStageElements.push(listElement);
            });

            if (this.lcStageElements.length)
            {
                this.lcStageElements[0].classList.add(this.viewMode === ViewMode.Navigator ? 'lc-stage--selected' : 'lc-stage--inprogress');
                
                if (this.lcStageElements.length < 3)
                    this.StagesContainer.css('width', `${this.lcStageElements.length * 33}%`);
			}

			this.$().find('#ManageLcStages').on('click', () =>
			{
				if (typeof this.onManageStagesClick !== 'undefined' && typeof this.onManageStagesClick === 'function')
				{
					this.onManageStagesClick();
				}
					
			});
        }

		public onManageStagesClick: () => void;

        private _createLcStageHTMLLIElement(stage: LcStage)
        {
            const listElement = document.createElement('li') as LcStageHTMLLIElement;

            listElement.__data__ = stage;

			const anchor = document.createElement('a') as LcStageHTMLAnchorElement;
			anchor.text = stage.Title;
            anchor.setAttribute("data-stage-id", stage.Id.toString());
            anchor.setAttribute("data-stage-name", stage.Title);
			anchor.__data__ = stage;

			$(listElement).append(anchor);
			listElement.classList.add('lc-stage');

            if (this.viewMode === ViewMode.Navigator)
            {
                const self = this;

				anchor.addEventListener('click', function (event) 
				{
					const stage = (event.target as LcStageHTMLAnchorElement).__data__;
					const previousLcStageStep = self.LcStageElements.filter(x => x.classList.contains('lc-stage--selected'))[0].__data__;

					$(self.get_element()).find('li.lc-stage').removeClass('lc-stage--selected');
                    
                    $(this).parent().addClass('lc-stage--selected');

					self.Timeline.notify(new LcStageStep(stage), new LcStageStep(previousLcStageStep));
                });
            }

            return listElement;
        }

        public markLcStageAsCompleted(lcStageIdentifiableString: string)
        {
            this._setLcStageCompletionStatus(lcStageIdentifiableString, true);
        }

        public unmarkLcStageAsCompleted(lcStageIdentifiableString: string)
        {
            this._setLcStageCompletionStatus(lcStageIdentifiableString, false);
        }

        private _setLcStageCompletionStatus(lcStageIdentifiableString: string, isCompleted: boolean)
        {
            const lcStage = this.LcStageElements.filter(x => x.__data__.IdentifiableString === lcStageIdentifiableString)[0];

            const lcStageCompletedCssClass = 'lc-stage--completed';

			if (isCompleted)
			{
				const lcStageInProgressCssClass = 'lc-stage--inprogress';
				lcStage.classList.remove(lcStageInProgressCssClass)
				lcStage.classList.add(lcStageCompletedCssClass);
			}
			else
			{
				lcStage.classList.remove(lcStageCompletedCssClass);
			}
		}

		public markLcStageAsInProgress(lcStageIdentifiableString: string)
		{
			this._setLcStageInProgressStatus(lcStageIdentifiableString, true);
		}

		public unmarkLcStageAsInProgress(lcStageIdentifiableString: string)
		{
			this._setLcStageInProgressStatus(lcStageIdentifiableString, false);
		}

        public getSelectedLcStage(): LcStage
        {
            const selectedLcStage = document.querySelector('.lc-stage--selected') as LcStageHTMLAnchorElement;

            return selectedLcStage.__data__;
		}

		public getLcStageElement(stageId: number): LcStageHTMLLIElement
		{
			return this.LcStageElements.filter(lcStageElement => $(lcStageElement).find("a").attr("data-stage-id") === stageId.toString())[0];
		}

		public revertLcStage(currentLcStageId: number, previousLcStageId?: number)
		{
			// if first from left it stays on same LC Stage, or if clicking on same stage
			if (previousLcStageId && currentLcStageId !== previousLcStageId) 
			{
				const previousLcStageElement = this.getLcStageElement(previousLcStageId);
				const currentLcStageElement = this.getLcStageElement(currentLcStageId);

				$(previousLcStageElement).addClass("lc-stage--selected");
				$(currentLcStageElement).removeClass("lc-stage--selected");
			}
		}

		private _setLcStageInProgressStatus(lcStageIdentifiableString: string, isInProgress: boolean)
		{
			const lcStage = this.LcStageElements.filter(x => x.__data__.IdentifiableString === lcStageIdentifiableString)[0];
			const lcStageInProgressCssClass = 'lc-stage--inprogress';

			if (isInProgress)
				lcStage.classList.add(lcStageInProgressCssClass);
			else
				lcStage.classList.remove(lcStageInProgressCssClass);
		}
    }

	export class Timeline
	{
		static $$className = 'DX.DIRModule.AssessmentNavigator.Timeline';

		private stageSteps: LcStageStep[] = [];
		private currentStageStep: LcStageStep = null;
		private previousStageStep: LcStageStep = null;
		private stageStepIndex = 0;
		private observers: IObserver[] = [];

		public get StageSteps()
		{
			return this.stageSteps;
		}

		public get CurrentStageStep()
		{
			return this.currentStageStep;
		}

		public get PreviousStageStep()
		{
			return this.previousStageStep;
		}

		public get NextStageStep()
		{
			return this.StageSteps[this.stageStepIndex + 1];
		}

		public get IsFirstStep()
		{
			return this.stageStepIndex == 0;
		}

		public get IsLastStep()
		{
			return this.stageStepIndex == this.StageSteps.length - 1;
		}

		constructor(stages: LcStage[], steps?: string[])
		{
			if (typeof stages === 'undefined' || stages.length === 0)
				throw new Error('stages must be defined');

			if (typeof steps !== 'undefined' && steps.length !== 0)
			{
				stages.forEach(stage => steps.forEach(step => this.StageSteps.push(new LcStageStep(stage, step))));
			}
			else
			{
				stages.forEach(stage => this.StageSteps.push(new LcStageStep(stage)));
			}

			this.currentStageStep = this.StageSteps[this.stageStepIndex];
		}

		public subscribe(observer: IObserver)
		{
			this.observers.push(observer);
		}

		public unsubcribe(observer: IObserver)
		{
			const index = this.observers.indexOf(observer);

			if (index > -1)
				this.observers.splice(index, 1);
		}

		public notify(currentLcStageStep: LcStageStep, previousLcStageStep?: LcStageStep)
		{
			this.observers.forEach(o => o.update(currentLcStageStep, previousLcStageStep));
		}

		public goToNextStep()
		{
			this._setStageStepCompletionStatus(true);
			this.previousStageStep = this.CurrentStageStep;
			this.currentStageStep = this.StageSteps[++this.stageStepIndex];

			if (this.PreviousStageStep.StageIdentifiableString !== this.CurrentStageStep.StageIdentifiableString)
				this.notify(this.CurrentStageStep);
		}

		public goToPreviousStep()
		{
			if (this.StageSteps[this.stageStepIndex].StageIdentifiableString !== this.StageSteps[this.stageStepIndex - 1].StageIdentifiableString)
				this.notify(this.StageSteps[this.stageStepIndex - 1]);

			this.stageStepIndex--;
			this._setStageStepCompletionStatus(false);

			this.currentStageStep = this.StageSteps[this.stageStepIndex];
			this.previousStageStep = this.StageSteps[this.stageStepIndex - 1];
		}

		public reset()
		{
			this.stageStepIndex = 0;
			this.currentStageStep = this.StageSteps[this.stageStepIndex];
			this.previousStageStep = null;

			this._uncompleteAllStageSteps();

			this.notify(this.currentStageStep);
		}

		private _setStageStepCompletionStatus(isCompleted: boolean) 
		{
			this.stageSteps[this.stageStepIndex].IsCompleted = isCompleted;
		}

		private _uncompleteAllStageSteps()
		{
			this.stageSteps.forEach(stegeStep => stegeStep.IsCompleted = false);
		}
    }

    export class LcStageStep
    {
        static $$className = 'DX.DIRModule.AssessmentNavigator.LcStageStep';

        private isCompleted: boolean = false;

        public get StageId()
        {
            return this.stage.Id;
        }

        public get StageIdentifiableString()
        {
            return this.stage.IdentifiableString;
        }

        public get StepId()
        {
            return this.step;
        }

        public get IsCompleted(): boolean
        {
            return this.isCompleted;
        }

        public set IsCompleted(value: boolean)
        {
            this.isCompleted = value;
        }

        constructor(private stage: LcStage, private step?: string)
        {

        }
    }

    export interface IObserver
    {
		update(currentLcStageStep: LcStageStep, previousLcStageStep?: LcStageStep): void;
    }

	type LcStageHTMLLIElement = HTMLLIElement & { __data__: LcStage }

    export enum ViewMode
    {
        Navigator,
        Dialog
    }

    export type LcStage = {
        IdentifiableString: string,
        Id: number,
        Title: string
	}

	export enum ContainerType
	{
		Panel,
		Dialog
	}

	export enum EntityType
	{
		LcStage = 0,
		Destinations = 1,
	}

	type LcStageHTMLAnchorElement = HTMLAnchorElement & { __data__: LcStage }

}