namespace DX.DIRModule.AssessmentNavigator
{
	export class ManageAssessmentDialog extends DX.Mvc.DialogViewControl
	{
		static $$className = "DX.DIRModule.AssessmentNavigator.ManageAssessmentDialog";

		private entityType: EntityType;
		private asmtIdentifiableString: string;
		private currentlcStageId: number = null;
		private redHighlightedMsg: string = null;
		private dialogPromptMsg: string = null;

		constructor(element: HTMLElement)
		{
			super(element);
		}

		public __applyJSONData()
		{
			super.__applyJSONData();
			this.entityType = this._getJSONData<EntityType>("entityType");
			this.asmtIdentifiableString = this._getJSONData<string>("asmtIdentifiableString");
			this.currentlcStageId = this._getJSONData<number>("currentlcStageId", null);
		}

		private _initLocalizationResources()
		{
			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "UsedCannotBeEmpty")
				.getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.redHighlightedMsg = result.text);

			DX.Loc.TextDescriptor.newByResource("DIR_AssessmentManager", "ManageDialogPromptMessage").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), result => this.dialogPromptMsg = result.text);
		}

		protected _initialize(): void
		{
			// (OVERRIDE)	
			super._initialize();

			this._initLocalizationResources();
			this._initializeEvents();
		}

		private _initializeEvents(): void
		{
			const usedListboxId = "#UsedListbox";
			const availableListboxId = "#AvailableListbox";

			this.$$<DX.Mvc.ButtonControl>("btnAdd").add_click(() =>
			{
				this.swapItems(availableListboxId, usedListboxId);
			});

			this.$$<DX.Mvc.ButtonControl>("btnRemove").add_click(() =>
			{
				this.swapItems(usedListboxId, availableListboxId);
			});
		}

		protected _onMenuItemClick(eventArgs: DX.Mvc.MenuItemClickEventArgs): void
		{
			// (VIRTUAL)
			super._onMenuItemClick(eventArgs);

			if (eventArgs.getHandled())
				return;

			const menuItem = eventArgs.getMenuItem();

			switch (menuItem.getKey())
			{
				case DX.Dialog.OK_BUTTON_KEY:
					{
						this.Save(eventArgs);
					}
					break;

				case DX.Dialog.CANCEL_BUTTON_KEY:
					{
						eventArgs.setHandled(true);
						this._closeDialog(DX.Dialog.CANCEL_BUTTON_KEY);
					}
					break;
			}
		}

		private IsDestinationManagement(): boolean
		{
			return (this.entityType as number) !== 0;
		}
		 
		private Save(eventArgs: DX.Mvc.MenuItemClickEventArgs): void
		{
			let listboxUsed = this.getListBoxItems($("#UsedListbox"));
			let listboxAvailable = this.getListBoxItems($("#AvailableListbox"));

			if (listboxUsed.length === 0 && !this.IsDestinationManagement())
			{
				$("#UsedListbox").css("border-color", "red");
				DX.Notif.showNotifications(this._getNotificationsContainer(), [new DX.Notif.Notification(DX.Notif.NotificationTypes.Error, this.redHighlightedMsg)], false);
				return;
			}

			const requestData: RequestData = {
				entityType: this.entityType,
				asmtIdentifiableString: this.asmtIdentifiableString,
				removedEntities: listboxAvailable,
				addedEntities: listboxUsed
			};

			if (this.IsDestinationManagement())
				requestData.lcStageId = this.currentlcStageId;

			this.showPrompt(this.dialogPromptMsg, () =>
			{
				this._invokeController<Mvc.TypedDataAjaxResult<ResponseData>>("Manage", { requestData: JSON.stringify(requestData) })
					.success(this, (result) =>
					{
						const response = result.data();
						eventArgs.setHandled(true);

						this._closeDialog(DX.Dialog.OK_BUTTON_KEY).setReturnValues(response);
					})
					.error(() => eventArgs.setHandled(true));
			});

		}

		private swapItems(controlFrom: string , controlTo: string)
		{
			$(controlFrom).find(":selected").appendTo(controlTo);
			$(controlTo).val([]);
		}

		protected getListBoxItems(listbox: JQuery): string[]
		{
			var listboxItems = listbox.find("option");
			var items: string[] = [];

			listboxItems.each(function ()
			{
				items.push(this.value);
			});

			return items;
		}

	}

	type EntityType = ResourceManagement.Shared.EntityType;

	type RequestData = {
		entityType: EntityType,
		asmtIdentifiableString: string,
		lcStageId?: number,
		removedEntities?: string[],
		addedEntities?: string[]
	}

	export type ManagementDlgResponseBase = {
		success: boolean,
		message: string
	}

	export type DestinationManagementDlgResponse = ManagementDlgResponseBase & {
		removedFood: string[],
		addedFood: Dictionary<number>,
		removedNonFood: string[],
		addedNonFood: Dictionary<number>
	}

	type ResponseData = ManagementDlgResponseBase & Partial<DestinationManagementDlgResponse>
}