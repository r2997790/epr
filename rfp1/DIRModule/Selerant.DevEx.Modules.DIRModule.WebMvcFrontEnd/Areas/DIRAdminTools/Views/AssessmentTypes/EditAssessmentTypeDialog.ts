namespace DX.DIRModule.DIRAdminTools.AssessmentTypes
{
    export class EditAssessmentTypeDialog extends DX.Mvc.DialogViewControl
    {
        static $$className = "DX.DIRModule.DIRAdminTools.AssessmentTypes.EditAssessmentTypeDialog";
        private _identifiableString: string = null;
        protected _localization: Dictionary<string>;
        protected _editMode: string;
        protected _autoGenString: string;
        protected _canEditCode: boolean;
        protected _currentName: string;
        protected _isNew: boolean;

        private _codeTxtControl: DX.Mvc.TextControl = null;
        private _descriptionTxtControl: DX.Mvc.TextControl = null;
        private _autoGenerateBoolControl: DX.Mvc.BooleanControl = null;
		private checkRedHighlightedMsg: string;

        constructor(element: HTMLElement) {
            super(element);
            this._localization = null;
            this._editMode = null;
            this._autoGenString = null;
            this._canEditCode = true;
            this._currentName = null;
            this._isNew = null;
        }
        __applyJSONData(): void {
            // (OVERRIDE)
            super.__applyJSONData();
            this._editMode = this._getDOMData<string>("editMode", null);
            this._autoGenString = this._getDOMData<string>("autogeneratestr");
            this._canEditCode = this._getDOMData<boolean>("canEditCode", null);
            this._isNew = this._getDOMData<boolean>("isNew", null);
        }
        private _initLocalizationResources() {
            var self = this;
            DX.Loc.TextDescriptor.newByResource("Global", "Invalid Value").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), function (result) {
                self._localization.invalidValue = result.text;
            });
        }
        protected _initialize(): void {
            var self = this;
            if (!self._editMode || !self._canEditCode) {
                self.onBeforeBtnClick();
			}

			DX.Loc.TextDescriptor.newByResource("Global", "Invalid Value").getSolvedText(DX.Loc.getCurrentUserCultureInfoDescriptor(), function (result) 
			{
				self.checkRedHighlightedMsg = result.text;
			});

            self._currentName = <string>self.$$<DX.Mvc.TextControl>("AssessmentTypeDescription").getValue();
            self._codeTxtControl = <DX.Mvc.TextControl>self.$("AssessmentTypeCode").getDevExControl();   
            self._descriptionTxtControl = <DX.Mvc.TextControl>self.$("AssessmentTypeDescription").getDevExControl();   
            self._autoGenerateBoolControl = <DX.Mvc.BooleanControl>self.$("codeAutoGen").getDevExControl();

            // (OVERRIDE)
            super._initialize();
            if (this._isNew)
                self._codeTxtControl.setValue(self._autoGenString);

            self.$$<DX.Mvc.BooleanControl>("codeAutoGen").add_valueChanged((handler) => {
                if (!self._editMode || !self._canEditCode)
                    return;
                const codeControl: DX.Mvc.TextControl = self.$$("AssessmentTypeCode");
                codeControl.ValidationAfterSetValueOff();
                var newValue = handler.getValue();
                codeControl.setReadOnly(newValue);
                codeControl.setValue(newValue ? self._autoGenString : null);
            });
            if (!self._editMode)
                $(".control-info-message").hide();
            self._initLocalizationResources();
        }
        protected _getNotificationsContainer(): JQuery {
            // (OVERRIDE)
            var panel = $("#ErrorsNotificationPanel");
            if (panel.length > 0)
                return panel;
            else
                return super._getNotificationsContainer();
        }
        protected _validateForm() {
            var self = this;
            var validationResult = self.$$("AssessmentTypeDescription").validate() && self.$$<DX.Mvc.TextControl>("AssessmentTypeDescription").getValue().trim().length > 0;
            validationResult = validationResult && this.$$("codeAutoGen").getValue() ? true : self.$$("AssessmentTypeCode").validate();
            return validationResult;
        }
        protected _onMenuItemClick(eventArgs: DX.Mvc.MenuItemClickEventArgs): void {
            // (VIRTUAL)
            super._onMenuItemClick(eventArgs);
            var self = this;

            if (eventArgs.getHandled())
                return;
                        
            var menuItem = eventArgs.getMenuItem();
            if (self._getNotificationsContainer())
                self._getNotificationsContainer().empty();

            switch (menuItem.getKey()) {

                case "CancelButton":
                    {
                        eventArgs.setHandled(true);
                        DX.Page.closeDialog();
                    }
                    break;

                case "SaveButton":
                    {
						eventArgs.setHandled(true);
						if (!self._codeTxtControl.validate() || !self._descriptionTxtControl.validate())
						{
							DX.Notif.showNotifications(self._getNotificationsContainer(), [new DX.Notif.Notification(DX.Notif.NotificationTypes.Error, self.checkRedHighlightedMsg)], false);
							return;
						}

                        self._createNewAssessmentType(self._getJSONFormData());    
                    }
                    break;

                default:
                    break;
            }
        }

        protected _createNewAssessmentType(jsonFormData: string) {
            var _self = this;
            _self.showLoadingPanel();
            _self._invokeController("SaveAssessmentType", { formData: jsonFormData, mode: _self._editMode, identifiableString: _self._identifiableString })
                .success((result) => {
                    if ((<DX.Mvc.DataAjaxResult>result).getDataValue("validated") != null && (<DX.Mvc.DataAjaxResult>result).getDataValue("validated") === "0")
                    {
                        //validation issue
                        //const messages = (<DX.Mvc.DataAjaxResult>result).getDataValue<string>("error");
                        //const invalidFields = (<DX.Mvc.DataAjaxResult>result).getDataValue<string>("invalidFields");
                        //_self._setValidation(messages.split("|"), invalidFields.split("|"), DX.Notif.NotificationTypes.Error);
                        _self.hideLoadingPanel();
                    }
                    else {
                        // finally succeed
                        _self.hideLoadingPanel();
                        _self._closeDialog(DX.Dialog.OK_BUTTON_KEY);
                    }

                });
        }   

        onBeforeBtnClick() : void {
            if (!this._canEditCode || !this._editMode) {
                self.$("codeAutoGen").css("pointer-events", "none !important;");
            }
            else {
                self.$("codeAutoGen").removeAttr("style");
            }
        }

        protected _getJSONFormData() {
            var _self = this;
            var jsonFormData: string = JSON.stringify({
                "Code": _self._codeTxtControl.getValue(),
                "Description": _self._descriptionTxtControl.getValue(),
                "IsAutoGeneratedCode": _self._autoGenerateBoolControl.getValue(),
                "IsNew": _self._isNew,
                "IsEditMode": _self._canEditCode
            });
            return jsonFormData;
        }
    }
}