if (window.Type && window.Type.registerNamespace) {
    Type.registerNamespace('Office.UI');
}
else {
    if (typeof Office == 'undefined') {
        Office = {
            __namespace: true
        };
    }
    if (typeof Office.UI == 'undefined') {
        Office.UI = {
            __namespace: true
        };
    }
}
Office.UI._principalSource = function() {
};
Office.UI._principalSource.prototype = {
    cache: 1,
    server: 0
};
if (Office.UI._principalSource.registerEnum)
    Office.UI._principalSource.registerEnum('Office.UI._principalSource', false);
Office.UI.PrincipalInfo = function Office_UI_PrincipalInfo() {
};
Office.UI.PeoplePickerRecord = function Office_UI_PeoplePickerRecord() {
};
Office.UI.PeoplePickerRecord.prototype = {
    isResolved: false,
    text: null,
    department: null,
    displayName: null,
    email: null,
    jobTitle: null,
    loginName: null,
    mobile: null,
    principalId: 0,
    principalType: 0,
    sipAddress: null
};
Office.UI._keyCodes = function Office_UI__keyCodes() {
};
Office.UI.PeoplePicker = function Office_UI_PeoplePicker(root, parameterObject, dataProvider) {
    this._currentTimerId$p$0 = -1;
    this.selectedItems = new Array(0);
    this._internalSelectedItems$p$0 = new Array(0);
    this.errors = new Array(0);
    this._cache$p$0 = Office.UI.PeoplePicker._mruCache.getInstance();
    if (typeof root !== 'object' || typeof parameterObject !== 'object') {
        Office.UI.Utils.errorConsole('Invalid parameters type');
        return;
    }
    Office.UI.Runtime._registerControl(root, this);
    this._root$p$0 = root;
    this._allowMultiple$p$0 = parameterObject.allowMultipleSelections;
    this._groupName$p$0 = parameterObject.groupName;
    this._onAdded$p$0 = parameterObject.onAdded;
    if (Office.UI.Utils.isNullOrUndefined(this._onAdded$p$0)) {
        this._onAdded$p$0 = Office.UI.PeoplePicker._nopAddRemove$p;
    }
    this._onRemoved$p$0 = parameterObject.onRemoved;
    if (Office.UI.Utils.isNullOrUndefined(this._onRemoved$p$0)) {
        this._onRemoved$p$0 = Office.UI.PeoplePicker._nopAddRemove$p;
    }
    this._onChange$p$0 = parameterObject.onChange;
    if (Office.UI.Utils.isNullOrUndefined(this._onChange$p$0)) {
        this._onChange$p$0 = Office.UI.PeoplePicker._nopChange$p;
    }
    if (!dataProvider) {
        this._dataProvider$p$0 = new Office.UI.PeoplePicker._searchPrincipalServerDataProvider();
    }
    else {
        this._dataProvider$p$0 = dataProvider;
    }
    if (Office.UI.Utils.isNullOrUndefined(parameterObject.displayErrors)) {
        this._showValidationErrors$p$0 = true;
    }
    else {
        this._showValidationErrors$p$0 = parameterObject.displayErrors;
    }
    if (!Office.UI.Utils.isNullOrEmptyString(parameterObject.placeholder)) {
        this._defaultTextOverride$p$0 = parameterObject.placeholder;
    }
    this._renderControl$p$0(parameterObject.inputName);
    this._autofill$p$0 = new Office.UI.PeoplePicker._autofillContainer(this);
};
Office.UI.PeoplePicker._copyToRecord$i = function Office_UI_PeoplePicker$_copyToRecord$i(record, info) {
    record.department = info.Department;
    record.displayName = info.DisplayName;
    record.email = info.Email;
    record.jobTitle = info.JobTitle;
    record.loginName = info.LoginName;
    record.mobile = info.Mobile;
    record.principalId = info.PrincipalId;
    record.principalType = info.PrincipalType;
    record.sipAddress = info.SIPAddress;
};
Office.UI.PeoplePicker._getPrincipalFromRecord$i = function Office_UI_PeoplePicker$_getPrincipalFromRecord$i(record) {
    var info = new Office.UI.PrincipalInfo();

    info.Department = record.department;
    info.DisplayName = record.displayName;
    info.Email = record.email;
    info.JobTitle = record.jobTitle;
    info.LoginName = record.loginName;
    info.Mobile = record.mobile;
    info.PrincipalId = record.principalId;
    info.PrincipalType = record.principalType;
    info.SIPAddress = record.sipAddress;
    return info;
};
Office.UI.PeoplePicker._parseUserPaste$p = function Office_UI_PeoplePicker$_parseUserPaste$p(content) {
    var openBracket = content.indexOf('<');
    var emailSep = content.indexOf('@', openBracket);
    var closeBracket = content.indexOf('>', emailSep);

    if (openBracket !== -1 && emailSep !== -1 && closeBracket !== -1) {
        return content.substring(openBracket + 1, closeBracket);
    }
    return content;
};
Office.UI.PeoplePicker._nopAddRemove$p = function Office_UI_PeoplePicker$_nopAddRemove$p(p1, p2) {
};
Office.UI.PeoplePicker._nopChange$p = function Office_UI_PeoplePicker$_nopChange$p(p1) {
};
Office.UI.PeoplePicker.create = function Office_UI_PeoplePicker$create(root, parameterObject) {
    return new Office.UI.PeoplePicker(root, parameterObject);
};
Office.UI.PeoplePicker.prototype = {
    _allowMultiple$p$0: false,
    _groupName$p$0: null,
    _defaultTextOverride$p$0: null,
    _onAdded$p$0: null,
    _onRemoved$p$0: null,
    _onChange$p$0: null,
    _dataProvider$p$0: null,
    _showValidationErrors$p$0: false,
    _actualRoot$p$0: null,
    _textInput$p$0: null,
    _inputData$p$0: null,
    _defaultText$p$0: null,
    _resolvedListRoot$p$0: null,
    _autofillElement$p$0: null,
    _errorMessageElement$p$0: null,
    _root$p$0: null,
    _alertDiv$p$0: null,
    _lastSearchQuery$p$0: '',
    _currentToken$p$0: null,
    _widthSet$p$0: false,
    _currentPrincipalsChoices$p$0: null,
    hasErrors: false,
    _errorDisplayed$p$0: null,
    _hasMultipleEntryValidationError$p$0: false,
    _hasMultipleMatchValidationError$p$0: false,
    _hasNoMatchValidationError$p$0: false,
    _autofill$p$0: null,
    remove: function Office_UI_PeoplePicker$remove(entryToRemove) {
        var record = this._internalSelectedItems$p$0;

        for (var i = 0; i < record.length; i++) {
            if (record[i].get_record() === entryToRemove) {
                record[i]._remove$i$0();
                break;
            }
        }
    },
    add: function Office_UI_PeoplePicker$add(p1, resolve) {
        if (typeof p1 === 'string') {
            this._addThroughString$p$0(p1);
        }
        else {
            if (Office.UI.Utils.isNullOrUndefined(resolve)) {
                this._addThroughRecord$p$0(p1, false);
            }
            else {
                this._addThroughRecord$p$0(p1, resolve);
            }
        }
    },
    _addThroughString$p$0: function Office_UI_PeoplePicker$_addThroughString$p$0(input) {
        if (Office.UI.Utils.isNullOrEmptyString(input)) {
            Office.UI.Utils.errorConsole('Input can\'t be null or empty string. PeoplePicker Id : ' + this._root$p$0.id);
            return;
        }
        this._addUnresolvedPrincipal$p$0(input);
    },
    _addThroughRecord$p$0: function Office_UI_PeoplePicker$_addThroughRecord$p$0(info, resolve) {
        if (resolve) {
            this._addUncertainPrincipal$p$0(info);
        }
        else {
            this._addResolvedRecord$p$0(info);
        }
    },
    _renderControl$p$0: function Office_UI_PeoplePicker$_renderControl$p$0(inputName) {
        this._root$p$0.innerHTML = Office.UI._peoplePickerTemplates.generateControlTemplate(inputName, this._allowMultiple$p$0, this._defaultTextOverride$p$0);
        if (this._root$p$0.className.length > 0) {
            this._root$p$0.className += ' ';
        }
        this._root$p$0.className += Office.UI.PeoplePicker.rootClassName;
        this._actualRoot$p$0 = this._root$p$0.querySelector('div.' + Office.UI._peoplePickerTemplates._actualControlClass$i);
        var $$t_6 = this;

        Office.UI.Utils.addEventListener(this._actualRoot$p$0, 'click', function(e) {
            return $$t_6._onPickerClick$p$0(e);
        });
        this._inputData$p$0 = this._actualRoot$p$0.querySelector('input[type=\"hidden\"]');
        this._textInput$p$0 = this._actualRoot$p$0.querySelector('input[type=\"text\"]');
        var $$t_7 = this;

        Office.UI.Utils.addEventListener(this._textInput$p$0, 'focus', function(e) {
            return $$t_7._onInputFocus$p$0(e);
        });
        var $$t_8 = this;

        Office.UI.Utils.addEventListener(this._textInput$p$0, 'blur', function(e) {
            return $$t_8._onInputBlur$p$0(e);
        });
        var $$t_9 = this;

        Office.UI.Utils.addEventListener(this._textInput$p$0, 'keydown', function(e) {
            return $$t_9._onInputKeyDown$p$0(e);
        });
        var $$t_A = this;

        Office.UI.Utils.addEventListener(this._textInput$p$0, 'keyup', function(e) {
            return $$t_A._onInputKeyUp$p$0(e);
        });
        this._defaultText$p$0 = this._actualRoot$p$0.querySelector('span.' + Office.UI._peoplePickerTemplates._defaultTextClass$i);
        this._resolvedListRoot$p$0 = this._actualRoot$p$0.querySelector('span.' + Office.UI._peoplePickerTemplates._resolvedListClass$i);
        this._autofillElement$p$0 = this._actualRoot$p$0.querySelector('.' + Office.UI._peoplePickerTemplates._autofillContainerClass$i);
        this._alertDiv$p$0 = this._actualRoot$p$0.querySelector('.' + Office.UI._peoplePickerTemplates._alertDivClass$i);
    },
    _onInputKeyDown$p$0: function Office_UI_PeoplePicker$_onInputKeyDown$p$0(e) {
        var keyEvent = Office.UI.Utils.getEvent(e);

        if (keyEvent.keyCode === Office.UI._keyCodes.tab) {
            if (this._autofill$p$0.get_isDisplayed()) {
                return true;
            }
            else {
                this._cancelLastRequest$p$0();
                this._attemptResolveInput$p$0();
                return true;
            }
        }
        else if (keyEvent.keyCode === Office.UI._keyCodes.escape) {
            this._autofill$p$0.close();
        }
        else if (keyEvent.keyCode === Office.UI._keyCodes.downArrow && this._autofill$p$0.get_isDisplayed()) {
            var firstElement = this._autofillElement$p$0.querySelector('a');

            if (firstElement) {
                firstElement.focus();
                Office.UI.Utils.cancelEvent(e);
            }
        }
        else if (keyEvent.keyCode === Office.UI._keyCodes.backspace) {
            var shouldRemove = false;

            if (!Office.UI.Utils.isNullOrUndefined(document.selection)) {
                var range = document.selection.createRange();
                var selectedText = range.text;

                range.moveStart('character', -this._textInput$p$0.value.length);
                var caretPos = range.text.length;

                if (!selectedText.length && !caretPos) {
                    shouldRemove = true;
                }
            }
            else {
                var selectionStart = this._textInput$p$0.selectionStart;
                var selectionEnd = this._textInput$p$0.selectionEnd;

                if (!selectionStart && selectionStart === selectionEnd) {
                    shouldRemove = true;
                }
            }
            if (shouldRemove && this._internalSelectedItems$p$0.length) {
                this._internalSelectedItems$p$0[this._internalSelectedItems$p$0.length - 1]._remove$i$0();
            }
        }
        else if (keyEvent.keyCode === Office.UI._keyCodes.k && keyEvent.ctrlKey || keyEvent.keyCode === Office.UI._keyCodes.semiColon) {
            this._cancelLastRequest$p$0();
            this._attemptResolveInput$p$0();
            Office.UI.Utils.cancelEvent(e);
            return false;
        }
        else if (keyEvent.keyCode === Office.UI._keyCodes.v && keyEvent.ctrlKey || keyEvent.keyCode === Office.UI._keyCodes.semiColon) {
            this._cancelLastRequest$p$0();
            var $$t_A = this;

            window.setTimeout(function() {
                $$t_A._textInput$p$0.value = Office.UI.PeoplePicker._parseUserPaste$p($$t_A._textInput$p$0.value);
                $$t_A._attemptResolveInput$p$0();
            }, 0);
            return true;
        }
        else if (keyEvent.keyCode === Office.UI._keyCodes.enter && keyEvent.shiftKey) {
            var $$t_B = this;

            this._autofill$p$0.open(function(selectedPrincipal) {
                $$t_B._addResolvedPrincipal$p$0(selectedPrincipal);
            });
        }
        else {
            this._resizeInputField$p$0();
        }
        return true;
    },
    _cancelLastRequest$p$0: function Office_UI_PeoplePicker$_cancelLastRequest$p$0() {
        window.clearTimeout(this._currentTimerId$p$0);
        if (!Office.UI.Utils.isNullOrUndefined(this._currentToken$p$0)) {
            this._hideLoadingIcon$p$0();
            this._currentToken$p$0.cancel();
            this._currentToken$p$0 = null;
        }
    },
    _onInputKeyUp$p$0: function Office_UI_PeoplePicker$_onInputKeyUp$p$0(e) {
        this._startQueryAfterDelay$p$0();
        this._resizeInputField$p$0();
        if (!this._textInput$p$0.value.length) {
            this._autofill$p$0.close();
        }
        return true;
    },
    _displayCachedEntries$p$0: function Office_UI_PeoplePicker$_displayCachedEntries$p$0() {
        var cachedEntries = this._cache$p$0.get(this._textInput$p$0.value, Office.UI.PeoplePicker._maxCacheEntries$p);

        this._autofill$p$0.setCachedEntries(cachedEntries);
        if (!cachedEntries.length && !this._autofill$p$0.get_isDisplayed()) {
            return;
        }
        var $$t_2 = this;

        this._autofill$p$0.open(function(selectedPrincipal) {
            $$t_2._addResolvedPrincipal$p$0(selectedPrincipal);
        });
    },
    _resizeInputField$p$0: function Office_UI_PeoplePicker$_resizeInputField$p$0() {
        var size = Math.max(this._textInput$p$0.value.length + 1, 1);

        this._textInput$p$0.size = size;
    },
    _clearInputField$p$0: function Office_UI_PeoplePicker$_clearInputField$p$0() {
        this._textInput$p$0.value = '';
        this._resizeInputField$p$0();
    },
    _startQueryAfterDelay$p$0: function Office_UI_PeoplePicker$_startQueryAfterDelay$p$0() {
        this._cancelLastRequest$p$0();
        var $$t_3 = this;

        this._currentTimerId$p$0 = window.setTimeout(function() {
            if ($$t_3._textInput$p$0.value !== $$t_3._lastSearchQuery$p$0) {
                $$t_3._lastSearchQuery$p$0 = $$t_3._textInput$p$0.value;
                if ($$t_3._textInput$p$0.value.length >= Office.UI.PeoplePicker._minimumNumberOfLettersToQuery$p) {
                    $$t_3._displayLoadingIcon$p$0();
                    $$t_3._removeValidationError$p$0(Office.UI.PeoplePicker.ValidationError.serverProblemName);
                    var token = new Office.UI.PeoplePicker._cancelToken();

                    $$t_3._currentToken$p$0 = token;
                    $$t_3._dataProvider$p$0.getPrincipals($$t_3._textInput$p$0.value, 15, 15, $$t_3._groupName$p$0, Office.UI.PeoplePicker._numberOfResults$p, function(principalsReceived) {
                        if (!token.get_isCanceled()) {
                            $$t_3._onDataReceived$p$0(principalsReceived);
                        }
                        else {
                            $$t_3._hideLoadingIcon$p$0();
                        }
                    }, function(error) {
                        $$t_3._onDataFetchError$p$0(error);
                    });
                }
                else {
                    $$t_3._autofill$p$0.close();
                }
                $$t_3._autofill$p$0.flushContent();
                $$t_3._displayCachedEntries$p$0();
            }
        }, Office.UI.PeoplePicker._autofillWait$p);
    },
    _onDataFetchError$p$0: function Office_UI_PeoplePicker$_onDataFetchError$p$0(message) {
        this._hideLoadingIcon$p$0();
        this._addValidationError$p$0(Office.UI.PeoplePicker.ValidationError._createServerProblemError$i());
    },
    _onDataReceived$p$0: function Office_UI_PeoplePicker$_onDataReceived$p$0(principalsReceived) {
        this._currentPrincipalsChoices$p$0 = {};
        for (var i = 0; i < principalsReceived.length; i++) {
            var principal = principalsReceived[i];

            this._currentPrincipalsChoices$p$0[principal.LoginName] = principal;
        }
        this._autofill$p$0.setServerEntries(principalsReceived);
        this._hideLoadingIcon$p$0();
        var $$t_4 = this;

        this._autofill$p$0.open(function(selectedPrincipal) {
            $$t_4._addResolvedPrincipal$p$0(selectedPrincipal);
        });
    },
    _onPickerClick$p$0: function Office_UI_PeoplePicker$_onPickerClick$p$0(e) {
        this._textInput$p$0.focus();
        e = Office.UI.Utils.getEvent(e);
        var element = Office.UI.Utils.getTarget(e);

        if (element.nodeName.toLowerCase() !== 'input') {
            this._focusToEnd$p$0();
        }
        return true;
    },
    _focusToEnd$p$0: function Office_UI_PeoplePicker$_focusToEnd$p$0() {
        var endPos = this._textInput$p$0.value.length;

        if (!Office.UI.Utils.isNullOrUndefined(this._textInput$p$0.createTextRange)) {
            var range = this._textInput$p$0.createTextRange();

            range.collapse(true);
            range.moveStart('character', endPos);
            range.moveEnd('character', endPos);
            range.select();
        }
        else {
            this._textInput$p$0.focus();
            this._textInput$p$0.setSelectionRange(endPos, endPos);
        }
    },
    _onInputFocus$p$0: function Office_UI_PeoplePicker$_onInputFocus$p$0(e) {
        this._defaultText$p$0.style.display = 'none';
        if (Office.UI.Utils.isNullOrEmptyString(this._actualRoot$p$0.className)) {
            this._actualRoot$p$0.className = Office.UI.PeoplePicker._focusClassName$i;
        }
        else {
            this._actualRoot$p$0.className += ' ' + Office.UI.PeoplePicker._focusClassName$i;
        }
        if (!this._widthSet$p$0) {
            this._setInputMaxWidth$p$0();
        }
        return true;
    },
    _setInputMaxWidth$p$0: function Office_UI_PeoplePicker$_setInputMaxWidth$p$0() {
        var maxwidth = this._actualRoot$p$0.clientWidth - 25;

        if (maxwidth <= 0) {
            maxwidth = 20;
        }
        this._textInput$p$0.style.maxWidth = maxwidth.toString() + 'px';
        this._widthSet$p$0 = true;
    },
    _onInputBlur$p$0: function Office_UI_PeoplePicker$_onInputBlur$p$0(e) {
        Office.UI.Utils.removeClass(this._actualRoot$p$0, Office.UI.PeoplePicker._focusClassName$i);
        if (this._textInput$p$0.value.length > 0) {
            return true;
        }
        if (this.selectedItems.length > 0) {
            return true;
        }
        this._defaultText$p$0.style.display = 'inline';
        return true;
    },
    _onDataSelected$p$0: function Office_UI_PeoplePicker$_onDataSelected$p$0(selectedPrincipal) {
        this._lastSearchQuery$p$0 = '';
        this._validateMultipleEntryAllowed$p$0();
        this._clearInputField$p$0();
        this._refreshInputField$p$0();
        this._onAdded$p$0(this, selectedPrincipal);
        this._onChange$p$0(this);
    },
    _onDataRemoved$p$0: function Office_UI_PeoplePicker$_onDataRemoved$p$0(selectedPrincipal) {
        this.selectedItems.splice(this.selectedItems.indexOf(selectedPrincipal), 1);
        this._refreshInputField$p$0();
        this._validateMultipleMatchError$p$0();
        this._validateMultipleEntryAllowed$p$0();
        this._validateNoMatchError$p$0();
        this._onRemoved$p$0(this, selectedPrincipal);
        this._onChange$p$0(this);
    },
    _addToCache$p$0: function Office_UI_PeoplePicker$_addToCache$p$0(entry) {
        if (!this._cache$p$0.isCacheAvailable) {
            return;
        }
        this._cache$p$0.set(entry);
    },
    _refreshInputField$p$0: function Office_UI_PeoplePicker$_refreshInputField$p$0() {
        this._inputData$p$0.value = Office.UI.Utils.serializeJSON(this.selectedItems);
    },
    _changeAlertMessage$p$0: function Office_UI_PeoplePicker$_changeAlertMessage$p$0(message) {
        this._alertDiv$p$0.innerHTML = Office.UI.Utils.htmlEncode(message);
    },
    _displayLoadingIcon$p$0: function Office_UI_PeoplePicker$_displayLoadingIcon$p$0() {
        this._actualRoot$p$0.style.backgroundPosition = (this._actualRoot$p$0.clientWidth - 20).toString() + 'px';
        Office.UI.Utils.addClass(this._actualRoot$p$0, Office.UI._peoplePickerTemplates._loadingDataClass$i);
        this._changeAlertMessage$p$0(Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_Searching));
    },
    _hideLoadingIcon$p$0: function Office_UI_PeoplePicker$_hideLoadingIcon$p$0() {
        Office.UI.Utils.removeClass(this._actualRoot$p$0, Office.UI._peoplePickerTemplates._loadingDataClass$i);
    },
    _attemptResolveInput$p$0: function Office_UI_PeoplePicker$_attemptResolveInput$p$0() {
        this._autofill$p$0.close();
        if (this._textInput$p$0.value.length > 0) {
            this._lastSearchQuery$p$0 = '';
            this._addUnresolvedPrincipal$p$0(this._textInput$p$0.value);
            this._clearInputField$p$0();
        }
    },
    _onDataReceivedForResolve$p$0: function Office_UI_PeoplePicker$_onDataReceivedForResolve$p$0(principalsReceived, internalRecordToResolve) {
        this._hideLoadingIcon$p$0();
        if (principalsReceived.length === 1) {
            internalRecordToResolve._resolveTo$i$0(principalsReceived[0]);
        }
        else {
            internalRecordToResolve._setResolveOptions$i$0(principalsReceived);
        }
        this._refreshInputField$p$0();
        this._onAdded$p$0(this, internalRecordToResolve.get_record());
        this._onChange$p$0(this);
    },
    _onDataReceivedForStalenessCheck$p$0: function Office_UI_PeoplePicker$_onDataReceivedForStalenessCheck$p$0(principalsReceived, internalRecordToCheck) {
        if (principalsReceived.length === 1) {
            internalRecordToCheck._refresh$i$0(principalsReceived[0]);
        }
        else {
            internalRecordToCheck._unresolve$i$0();
            internalRecordToCheck._setResolveOptions$i$0(principalsReceived);
        }
        this._refreshInputField$p$0();
        this._onAdded$p$0(this, internalRecordToCheck.get_record());
        this._onChange$p$0(this);
    },
    _addResolvedPrincipal$p$0: function Office_UI_PeoplePicker$_addResolvedPrincipal$p$0(principal) {
        var record = new Office.UI.PeoplePickerRecord();

        Office.UI.PeoplePicker._copyToRecord$i(record, principal);
        record.text = principal.DisplayName;
        record.isResolved = true;
        this.selectedItems.push(record);
        var internalRecord = new Office.UI.PeoplePicker._internalPeoplePickerRecord(this, record);

        internalRecord._add$i$0();
        this._internalSelectedItems$p$0.push(internalRecord);
        this._onDataSelected$p$0(record);
        this._addToCache$p$0(principal);
        this._currentPrincipalsChoices$p$0 = null;
        this._autofill$p$0.close();
    },
    _addResolvedRecord$p$0: function Office_UI_PeoplePicker$_addResolvedRecord$p$0(record) {
        this.selectedItems.push(record);
        var internalRecord = new Office.UI.PeoplePicker._internalPeoplePickerRecord(this, record);

        internalRecord._add$i$0();
        this._internalSelectedItems$p$0.push(internalRecord);
        this._onDataSelected$p$0(record);
        this._currentPrincipalsChoices$p$0 = null;
    },
    _addUncertainPrincipal$p$0: function Office_UI_PeoplePicker$_addUncertainPrincipal$p$0(record) {
        this.selectedItems.push(record);
        var internalRecord = new Office.UI.PeoplePicker._internalPeoplePickerRecord(this, record);

        internalRecord._add$i$0();
        this._internalSelectedItems$p$0.push(internalRecord);
        var $$t_4 = this, $$t_5 = this;

        this._dataProvider$p$0.getPrincipals(record.email, 15, 15, this._groupName$p$0, Office.UI.PeoplePicker._numberOfResults$p, function(ps) {
            $$t_4._onDataReceivedForStalenessCheck$p$0(ps, internalRecord);
        }, function(message) {
            $$t_5._onDataFetchError$p$0(message);
        });
        this._validateMultipleEntryAllowed$p$0();
    },
    _addUnresolvedPrincipal$p$0: function Office_UI_PeoplePicker$_addUnresolvedPrincipal$p$0(input) {
        var record = new Office.UI.PeoplePickerRecord();

        record.text = input;
        record.isResolved = false;
        var internalRecord = new Office.UI.PeoplePicker._internalPeoplePickerRecord(this, record);

        internalRecord._add$i$0();
        this.selectedItems.push(record);
        this._internalSelectedItems$p$0.push(internalRecord);
        this._displayLoadingIcon$p$0();
        var $$t_5 = this, $$t_6 = this;

        this._dataProvider$p$0.getPrincipals(input, 15, 15, this._groupName$p$0, Office.UI.PeoplePicker._numberOfResults$p, function(ps) {
            $$t_5._onDataReceivedForResolve$p$0(ps, internalRecord);
        }, function(message) {
            $$t_6._onDataFetchError$p$0(message);
        });
        this._validateMultipleEntryAllowed$p$0();
    },
    _addValidationError$p$0: function Office_UI_PeoplePicker$_addValidationError$p$0(err) {
        this.hasErrors = true;
        this.errors.push(err);
        this._displayValidationErrors$p$0();
    },
    _removeValidationError$p$0: function Office_UI_PeoplePicker$_removeValidationError$p$0(errorName) {
        for (var i = 0; i < this.errors.length; i++) {
            if (this.errors[i].errorName === errorName) {
                this.errors.splice(i, 1);
                break;
            }
        }
        if (!this.errors.length) {
            this.hasErrors = false;
        }
        this._displayValidationErrors$p$0();
    },
    _validateMultipleEntryAllowed$p$0: function Office_UI_PeoplePicker$_validateMultipleEntryAllowed$p$0() {
        if (!this._allowMultiple$p$0) {
            if (this.selectedItems.length > 1) {
                if (!this._hasMultipleEntryValidationError$p$0) {
                    this._addValidationError$p$0(Office.UI.PeoplePicker.ValidationError._createMultipleEntryError$i());
                    this._hasMultipleEntryValidationError$p$0 = true;
                }
            }
            else if (this._hasMultipleEntryValidationError$p$0) {
                this._removeValidationError$p$0(Office.UI.PeoplePicker.ValidationError.multipleEntryName);
                this._hasMultipleEntryValidationError$p$0 = false;
            }
        }
    },
    _validateMultipleMatchError$p$0: function Office_UI_PeoplePicker$_validateMultipleMatchError$p$0() {
        var oldStatus = this._hasMultipleMatchValidationError$p$0;
        var newStatus = false;

        for (var i = 0; i < this._internalSelectedItems$p$0.length; i++) {
            if (!Office.UI.Utils.isNullOrUndefined(this._internalSelectedItems$p$0[i]._optionsList$i$0) && this._internalSelectedItems$p$0[i]._optionsList$i$0.length > 0) {
                newStatus = true;
                break;
            }
        }
        if (!oldStatus && newStatus) {
            this._addValidationError$p$0(Office.UI.PeoplePicker.ValidationError._createMultipleMatchError$i());
        }
        if (oldStatus && !newStatus) {
            this._removeValidationError$p$0(Office.UI.PeoplePicker.ValidationError.multipleMatchName);
        }
        this._hasMultipleMatchValidationError$p$0 = newStatus;
    },
    _validateNoMatchError$p$0: function Office_UI_PeoplePicker$_validateNoMatchError$p$0() {
        var oldStatus = this._hasNoMatchValidationError$p$0;
        var newStatus = false;

        for (var i = 0; i < this._internalSelectedItems$p$0.length; i++) {
            if (!Office.UI.Utils.isNullOrUndefined(this._internalSelectedItems$p$0[i]._optionsList$i$0) && !this._internalSelectedItems$p$0[i]._optionsList$i$0.length) {
                newStatus = true;
                break;
            }
        }
        if (!oldStatus && newStatus) {
            this._addValidationError$p$0(Office.UI.PeoplePicker.ValidationError._createNoMatchError$i());
        }
        if (oldStatus && !newStatus) {
            this._removeValidationError$p$0(Office.UI.PeoplePicker.ValidationError.noMatchName);
        }
        this._hasNoMatchValidationError$p$0 = newStatus;
    },
    _displayValidationErrors$p$0: function Office_UI_PeoplePicker$_displayValidationErrors$p$0() {
        if (!this._showValidationErrors$p$0) {
            return;
        }
        if (!this.errors.length) {
            if (!Office.UI.Utils.isNullOrUndefined(this._errorMessageElement$p$0)) {
                this._errorMessageElement$p$0.parentNode.removeChild(this._errorMessageElement$p$0);
                this._errorMessageElement$p$0 = null;
                this._errorDisplayed$p$0 = null;
            }
        }
        else {
            if (this._errorDisplayed$p$0 !== this.errors[0]) {
                if (!Office.UI.Utils.isNullOrUndefined(this._errorMessageElement$p$0)) {
                    this._errorMessageElement$p$0.parentNode.removeChild(this._errorMessageElement$p$0);
                }
                var holderDiv = document.createElement('div');

                holderDiv.innerHTML = Office.UI._peoplePickerTemplates.generateErrorTemplate(this.errors[0].localizedErrorMessage);
                this._errorMessageElement$p$0 = holderDiv.firstChild;
                this._root$p$0.appendChild(this._errorMessageElement$p$0);
                this._errorDisplayed$p$0 = this.errors[0];
            }
        }
    },
    setDataProvider: function Office_UI_PeoplePicker$setDataProvider(newProvider) {
        this._dataProvider$p$0 = newProvider;
    }
};
Office.UI.PeoplePicker._internalPeoplePickerRecord = function Office_UI_PeoplePicker__internalPeoplePickerRecord(parent, record) {
    this._parent$i$0 = parent;
    this.set_record(record);
};
Office.UI.PeoplePicker._internalPeoplePickerRecord.prototype = {
    _$$pf_Record$p$0: null,
    get_record: function Office_UI_PeoplePicker__internalPeoplePickerRecord$get_record() {
        return this._$$pf_Record$p$0;
    },
    set_record: function Office_UI_PeoplePicker__internalPeoplePickerRecord$set_record(value) {
        this._$$pf_Record$p$0 = value;
        return value;
    },
    _principalOptions$i$0: null,
    _optionsList$i$0: null,
    _$$pf_Node$p$0: null,
    get_node: function Office_UI_PeoplePicker__internalPeoplePickerRecord$get_node() {
        return this._$$pf_Node$p$0;
    },
    set_node: function Office_UI_PeoplePicker__internalPeoplePickerRecord$set_node(value) {
        this._$$pf_Node$p$0 = value;
        return value;
    },
    _parent$i$0: null,
    _onRecordRemovalClick$p$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_onRecordRemovalClick$p$0(e) {
        var recordRemovalEvent = Office.UI.Utils.getEvent(e);
        var target = Office.UI.Utils.getTarget(recordRemovalEvent);

        this._remove$i$0();
        Office.UI.Utils.cancelEvent(e);
        this._parent$i$0._autofill$p$0.close();
        return false;
    },
    _onRecordRemovalKeyDown$p$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_onRecordRemovalKeyDown$p$0(e) {
        var recordRemovalEvent = Office.UI.Utils.getEvent(e);
        var target = Office.UI.Utils.getTarget(recordRemovalEvent);

        if (recordRemovalEvent.keyCode === Office.UI._keyCodes.backspace || recordRemovalEvent.keyCode === Office.UI._keyCodes.enter || recordRemovalEvent.keyCode === Office.UI._keyCodes.deleteKey) {
            this._remove$i$0();
            Office.UI.Utils.cancelEvent(e);
            this._parent$i$0._autofill$p$0.close();
        }
        return false;
    },
    _add$i$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_add$i$0() {
        var holderDiv = document.createElement('div');

        holderDiv.innerHTML = Office.UI._peoplePickerTemplates.generateRecordTemplate(this.get_record());
        var recordElement = holderDiv.firstChild;
        var removeButtonElement = recordElement.querySelector('a.' + Office.UI._peoplePickerTemplates._recordRemoverClass$i);
        var $$t_5 = this;

        Office.UI.Utils.addEventListener(removeButtonElement, 'click', function(e) {
            return $$t_5._onRecordRemovalClick$p$0(e);
        });
        var $$t_6 = this;

        Office.UI.Utils.addEventListener(removeButtonElement, 'keydown', function(e) {
            return $$t_6._onRecordRemovalKeyDown$p$0(e);
        });
        this._ensureNoBiggerThanParent$p$0(recordElement.firstChild);
        this._parent$i$0._resolvedListRoot$p$0.appendChild(recordElement);
        this._parent$i$0._defaultText$p$0.style.display = 'none';
        this.set_node(recordElement);
    },
    _remove$i$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_remove$i$0() {
        this._parent$i$0._resolvedListRoot$p$0.removeChild(this.get_node());
        this._parent$i$0._textInput$p$0.focus();
        for (var i = 0; i < this._parent$i$0._internalSelectedItems$p$0.length; i++) {
            if (this._parent$i$0._internalSelectedItems$p$0[i] === this) {
                this._parent$i$0._internalSelectedItems$p$0.splice(i, 1);
            }
        }
        this._parent$i$0._focusToEnd$p$0();
        this._parent$i$0._onDataRemoved$p$0(this.get_record());
    },
    _ensureNoBiggerThanParent$p$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_ensureNoBiggerThanParent$p$0(userLabel) {
        userLabel.style.maxWidth = (this._parent$i$0._actualRoot$p$0.clientWidth - 36).toString() + 'px';
    },
    _setResolveOptions$i$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_setResolveOptions$i$0(options) {
        this._optionsList$i$0 = options;
        this._principalOptions$i$0 = {};
        for (var i = 0; i < options.length; i++) {
            this._principalOptions$i$0[options[i].LoginName] = options[i];
        }
        var $$t_3 = this;

        Office.UI.Utils.addEventListener((this.get_node()).querySelector('a.' + Office.UI._peoplePickerTemplates._unresolvedUserClass$i), 'click', function(e) {
            return $$t_3._onUnresolvedUserClick$i$0(e);
        });
        this._parent$i$0._validateMultipleMatchError$p$0();
        this._parent$i$0._validateNoMatchError$p$0();
    },
    _onUnresolvedUserClick$i$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_onUnresolvedUserClick$i$0(e) {
        e = Office.UI.Utils.getEvent(e);
        this._parent$i$0._autofill$p$0.flushContent();
        this._parent$i$0._autofill$p$0.setServerEntries(this._optionsList$i$0);
        var $$t_2 = this;

        this._parent$i$0._autofill$p$0.open(function(selectedPrincipal) {
            $$t_2._onAutofillClick$i$0(selectedPrincipal);
        });
        this._parent$i$0._autofill$p$0.focusOnFirstElement();
        Office.UI.Utils.cancelEvent(e);
        return false;
    },
    _resolveTo$i$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_resolveTo$i$0(principal) {
        Office.UI.PeoplePicker._copyToRecord$i(this.get_record(), principal);
        (this.get_record()).text = principal.DisplayName;
        (this.get_record()).isResolved = true;
        this._parent$i$0._addToCache$p$0(principal);
        var linkNode = (this.get_node()).querySelector('a.' + Office.UI._peoplePickerTemplates._unresolvedUserClass$i);
        var newSpan = document.createElement('span');

        newSpan.className = Office.UI._peoplePickerTemplates._resolvedUserClass$i;
        this._updateHoverText$p$0(newSpan);
        this._ensureNoBiggerThanParent$p$0(newSpan);
        newSpan.innerHTML = Office.UI.Utils.htmlEncode(principal.DisplayName);
        linkNode.parentNode.insertBefore(newSpan, linkNode);
        linkNode.parentNode.removeChild(linkNode);
    },
    _refresh$i$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_refresh$i$0(principal) {
        Office.UI.PeoplePicker._copyToRecord$i(this.get_record(), principal);
        (this.get_record()).text = principal.DisplayName;
        var spanNode = (this.get_node()).querySelector('span.' + Office.UI._peoplePickerTemplates._resolvedUserClass$i);

        spanNode.innerHTML = Office.UI.Utils.htmlEncode(principal.DisplayName);
    },
    _unresolve$i$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_unresolve$i$0() {
        (this.get_record()).isResolved = false;
        var spanNode = (this.get_node()).querySelector('span.' + Office.UI._peoplePickerTemplates._resolvedUserClass$i);
        var newLink = document.createElement('a');

        newLink.className = Office.UI._peoplePickerTemplates._unresolvedUserClass$i;
        this._ensureNoBiggerThanParent$p$0(newLink);
        this._updateHoverText$p$0(newLink);
        newLink.innerHTML = Office.UI.Utils.htmlEncode((this.get_record()).text);
        spanNode.parentNode.insertBefore(newLink, spanNode);
        spanNode.parentNode.removeChild(spanNode);
    },
    _updateHoverText$p$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_updateHoverText$p$0(userLabel) {
        userLabel.title = Office.UI.Utils.htmlEncode((this.get_record()).text);
        ((this.get_node()).querySelector('a.' + Office.UI._peoplePickerTemplates._recordRemoverClass$i)).title = Office.UI.Utils.formatString(Office.UI._peoplePickerTemplates.getString(Office.UI.Utils.htmlEncode(Office.UI._peoplePickerResourcesStrings.pP_RemovePerson)), (this.get_record()).text);
    },
    _onAutofillClick$i$0: function Office_UI_PeoplePicker__internalPeoplePickerRecord$_onAutofillClick$i$0(selectedPrincipal) {
        this._parent$i$0._onRemoved$p$0(this._parent$i$0, this.get_record());
        this._resolveTo$i$0(selectedPrincipal);
        this._parent$i$0._refreshInputField$p$0();
        this._principalOptions$i$0 = null;
        this._optionsList$i$0 = null;
        this._parent$i$0._addToCache$p$0(selectedPrincipal);
        this._parent$i$0._validateMultipleMatchError$p$0();
        this._parent$i$0._autofill$p$0.close();
        this._parent$i$0._onAdded$p$0(this._parent$i$0, this.get_record());
        this._parent$i$0._onChange$p$0(this._parent$i$0);
    }
};
Office.UI.PeoplePicker._autofillContainer = function Office_UI_PeoplePicker__autofillContainer(parent) {
    this._entries$p$0 = {};
    this._cachedEntries$p$0 = new Array(0);
    this._serverEntries$p$0 = new Array(0);
    this._parent$p$0 = parent;
    this._root$p$0 = parent._autofillElement$p$0;
    if (!Office.UI.PeoplePicker._autofillContainer._boolBodyHandlerAdded$p) {
        var $$t_2 = this;

        Office.UI.Utils.addEventListener(document.body, 'click', function(e) {
            return Office.UI.PeoplePicker._autofillContainer._bodyOnClick$p(e);
        });
        Office.UI.PeoplePicker._autofillContainer._boolBodyHandlerAdded$p = true;
    }
};
Office.UI.PeoplePicker._autofillContainer._getControlRootFromSubElement$p = function Office_UI_PeoplePicker__autofillContainer$_getControlRootFromSubElement$p(element) {
    while (element && element.nodeName.toLowerCase() !== 'body') {
        if (element.className.indexOf(Office.UI.PeoplePicker.rootClassName) !== -1) {
            return element;
        }
        element = element.parentNode;
    }
    return null;
};
Office.UI.PeoplePicker._autofillContainer._bodyOnClick$p = function Office_UI_PeoplePicker__autofillContainer$_bodyOnClick$p(e) {
    if (!Office.UI.PeoplePicker._autofillContainer._currentOpened$p) {
        return true;
    }
    var click = Office.UI.Utils.getEvent(e);
    var target = Office.UI.Utils.getTarget(click);
    var controlRoot = Office.UI.PeoplePicker._autofillContainer._getControlRootFromSubElement$p(target);

    if (!target || controlRoot !== Office.UI.PeoplePicker._autofillContainer._currentOpened$p._parent$p$0._root$p$0) {
        Office.UI.PeoplePicker._autofillContainer._currentOpened$p.close();
    }
    return true;
};
Office.UI.PeoplePicker._autofillContainer.prototype = {
    _parent$p$0: null,
    _root$p$0: null,
    _$$pf_IsDisplayed$p$0: false,
    get_isDisplayed: function Office_UI_PeoplePicker__autofillContainer$get_isDisplayed() {
        return this._$$pf_IsDisplayed$p$0;
    },
    set_isDisplayed: function Office_UI_PeoplePicker__autofillContainer$set_isDisplayed(value) {
        this._$$pf_IsDisplayed$p$0 = value;
        return value;
    },
    setCachedEntries: function Office_UI_PeoplePicker__autofillContainer$setCachedEntries(entries) {
        this._cachedEntries$p$0 = entries;
        this._entries$p$0 = {};
        var length = entries.length;

        for (var i = 0; i < length; i++) {
            this._entries$p$0[entries[i].LoginName] = entries[i];
        }
    },
    setServerEntries: function Office_UI_PeoplePicker__autofillContainer$setServerEntries(entries) {
        var newServerEntries = new Array(0);
        var length = entries.length;

        for (var i = 0; i < length; i++) {
            var currentEntry = entries[i];

            if (Office.UI.Utils.isNullOrUndefined(this._entries$p$0[currentEntry.LoginName])) {
                this._entries$p$0[entries[i].LoginName] = entries[i];
                newServerEntries.push(currentEntry);
            }
        }
        this._serverEntries$p$0 = newServerEntries;
    },
    _renderList$p$0: function Office_UI_PeoplePicker__autofillContainer$_renderList$p$0(handler) {
        this._root$p$0.innerHTML = Office.UI._peoplePickerTemplates.generateAutofillListTemplate(this._cachedEntries$p$0, this._serverEntries$p$0, Office.UI.PeoplePicker._numberOfResults$p);
        var autofillElementsLinkTags = this._root$p$0.querySelectorAll('a');

        for (var i = 0; i < autofillElementsLinkTags.length; i++) {
            var link = autofillElementsLinkTags[i];
            var $$t_8 = this;

            Office.UI.Utils.addEventListener(link, 'click', function(e) {
                return $$t_8._onEntryClick$p$0(e, handler);
            });
            var $$t_9 = this;

            Office.UI.Utils.addEventListener(link, 'keydown', function(e) {
                return $$t_9._onKeyDown$p$0(e);
            });
            var $$t_A = this;

            Office.UI.Utils.addEventListener(link, 'focus', function(e) {
                return $$t_A._onEntryFocus$p$0(e);
            });
            var $$t_B = this;

            Office.UI.Utils.addEventListener(link, 'blur', function(e) {
                return $$t_B._onEntryBlur$p$0(e);
            });
        }
    },
    flushContent: function Office_UI_PeoplePicker__autofillContainer$flushContent() {
        var entry = this._root$p$0.querySelectorAll('li');

        for (var i = 0; i < entry.length; i++) {
            this._root$p$0.removeChild(entry[i]);
        }
        this._entries$p$0 = {};
        this._serverEntries$p$0 = new Array(0);
        this._cachedEntries$p$0 = new Array(0);
    },
    open: function Office_UI_PeoplePicker__autofillContainer$open(handler) {
        this._root$p$0.style.top = (this._parent$p$0._actualRoot$p$0.clientHeight + 2).toString() + 'px';
        this._renderList$p$0(handler);
        this.set_isDisplayed(true);
        Office.UI.PeoplePicker._autofillContainer._currentOpened$p = this;
        if (!Office.UI.Utils.containClass(this._parent$p$0._actualRoot$p$0, Office.UI._peoplePickerTemplates._autofillOpenedClass$i)) {
            Office.UI.Utils.addClass(this._parent$p$0._actualRoot$p$0, Office.UI._peoplePickerTemplates._autofillOpenedClass$i);
        }
        if (!Office.UI.Utils.containClass(this._parent$p$0._actualRoot$p$0, Office.UI._peoplePickerTemplates._loadingDataClass$i)) {
            if (this._cachedEntries$p$0.length + this._serverEntries$p$0.length > 0) {
                this._parent$p$0._changeAlertMessage$p$0(Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_SuggestionsAvailable));
            }
            else {
                this._parent$p$0._changeAlertMessage$p$0(Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_NoSuggestionsAvailable));
            }
        }
    },
    close: function Office_UI_PeoplePicker__autofillContainer$close() {
        this.set_isDisplayed(false);
        Office.UI.Utils.removeClass(this._parent$p$0._actualRoot$p$0, Office.UI._peoplePickerTemplates._autofillOpenedClass$i);
    },
    _onEntryClick$p$0: function Office_UI_PeoplePicker__autofillContainer$_onEntryClick$p$0(e, handler) {
        var click = Office.UI.Utils.getEvent(e);
        var target = Office.UI.Utils.getTarget(click);

        target = this._getParentListItem$p$0(target);
        var loginName = this._getLoginNameFromListElement$p$0(target);

        handler(this._entries$p$0[loginName]);
        this.flushContent();
        return true;
    },
    focusOnFirstElement: function Office_UI_PeoplePicker__autofillContainer$focusOnFirstElement() {
        var first = this._root$p$0.querySelector('li.' + Office.UI._peoplePickerTemplates._autofillItemClass$i);

        if (!Office.UI.Utils.isNullOrUndefined(first)) {
            first.firstChild.focus();
        }
    },
    _onKeyDown$p$0: function Office_UI_PeoplePicker__autofillContainer$_onKeyDown$p$0(e) {
        var key = Office.UI.Utils.getEvent(e);
        var target = Office.UI.Utils.getTarget(key);

        if (key.keyCode === Office.UI._keyCodes.upArrow || key.keyCode === Office.UI._keyCodes.tab && key.shiftKey) {
            var previous = target.parentNode.previousSibling;

            if (!previous) {
                this._parent$p$0._focusToEnd$p$0();
            }
            else {
                if (previous.firstChild.tagName.toLowerCase() !== 'a') {
                    previous = previous.previousSibling;
                }
                previous.firstChild.focus();
            }
            Office.UI.Utils.cancelEvent(e);
            return false;
        }
        else if (key.keyCode === Office.UI._keyCodes.downArrow) {
            var next = target.parentNode.nextSibling;

            if (next) {
                if (next.firstChild.tagName.toLowerCase() !== 'a') {
                    next = next.nextSibling;
                    if (next) {
                        next.firstChild.focus();
                    }
                }
                else {
                    next.firstChild.focus();
                }
            }
        }
        else if (key.keyCode === Office.UI._keyCodes.escape) {
            this.close();
        }
        if (key.keyCode !== Office.UI._keyCodes.tab && key.keyCode !== Office.UI._keyCodes.enter) {
            Office.UI.Utils.cancelEvent(key);
        }
        return false;
    },
    _getLoginNameFromListElement$p$0: function Office_UI_PeoplePicker__autofillContainer$_getLoginNameFromListElement$p$0(listElement) {
        return (listElement.attributes.getNamedItem(Office.UI._peoplePickerTemplates._autofillItemDataAttribute$i)).value;
    },
    _getParentListItem$p$0: function Office_UI_PeoplePicker__autofillContainer$_getParentListItem$p$0(element) {
        while (element && element.nodeName.toLowerCase() !== 'li') {
            element = element.parentNode;
        }
        return element;
    },
    _onEntryFocus$p$0: function Office_UI_PeoplePicker__autofillContainer$_onEntryFocus$p$0(e) {
        var target = Office.UI.Utils.getTarget(e);

        target = this._getParentListItem$p$0(target);
        if (!Office.UI.Utils.containClass(target, Office.UI.PeoplePicker._autofillContainer._focusClassName$p)) {
            Office.UI.Utils.addClass(target, Office.UI.PeoplePicker._autofillContainer._focusClassName$p);
        }
        return false;
    },
    _onEntryBlur$p$0: function Office_UI_PeoplePicker__autofillContainer$_onEntryBlur$p$0(e) {
        var target = Office.UI.Utils.getTarget(e);

        target = this._getParentListItem$p$0(target);
        Office.UI.Utils.removeClass(target, Office.UI.PeoplePicker._autofillContainer._focusClassName$p);
        return false;
    }
};
Office.UI.PeoplePicker.Parameters = function Office_UI_PeoplePicker_Parameters() {
};
Office.UI.PeoplePicker.ISearchPrincipalDataProvider = function() {
};
if (Office.UI.PeoplePicker.ISearchPrincipalDataProvider.registerInterface)
    Office.UI.PeoplePicker.ISearchPrincipalDataProvider.registerInterface('Office.UI.PeoplePicker.ISearchPrincipalDataProvider');
Office.UI.PeoplePicker._cancelToken = function Office_UI_PeoplePicker__cancelToken() {
    this.set_isCanceled(false);
};
Office.UI.PeoplePicker._cancelToken.prototype = {
    _$$pf_IsCanceled$p$0: false,
    get_isCanceled: function Office_UI_PeoplePicker__cancelToken$get_isCanceled() {
        return this._$$pf_IsCanceled$p$0;
    },
    set_isCanceled: function Office_UI_PeoplePicker__cancelToken$set_isCanceled(value) {
        this._$$pf_IsCanceled$p$0 = value;
        return value;
    },
    cancel: function Office_UI_PeoplePicker__cancelToken$cancel() {
        this.set_isCanceled(true);
    }
};
Office.UI.PeoplePicker._searchPrincipalServerDataProvider = function Office_UI_PeoplePicker__searchPrincipalServerDataProvider() {
    this._requestExecutor$p$0 = Office.UI.Runtime.context.getRequestExecutor();
};
Office.UI.PeoplePicker._searchPrincipalServerDataProvider._deserializePrincipalsFromResponse$p = function Office_UI_PeoplePicker__searchPrincipalServerDataProvider$_deserializePrincipalsFromResponse$p(response) {
    return (Office.UI.Utils.deserializeJSON(response.body)).d.SearchPrincipalsUsingContextWeb.results;
};
Office.UI.PeoplePicker._searchPrincipalServerDataProvider.prototype = {
    _requestExecutor$p$0: null,
    getPrincipals: function Office_UI_PeoplePicker__searchPrincipalServerDataProvider$getPrincipals(input, scopes, sources, groupName, maxCount, callback, errorCallback) {
        var requestInfos = new SP.RequestInfo();

        requestInfos.headers = {};
        requestInfos.headers['Accept'] = Office.UI.PeoplePicker._searchPrincipalServerDataProvider._oDataJSONAcceptHeader$p;
        requestInfos.method = 'GET';
        var $$t_C = this;

        requestInfos.success = function(infos) {
            if (infos.statusCode === 200) {
                callback(Office.UI.PeoplePicker._searchPrincipalServerDataProvider._deserializePrincipalsFromResponse$p(infos));
            }
            else {
                Office.UI.Utils.errorConsole('Bad error code returned by the server : ' + infos.statusCode.toString());
            }
        };
        var $$t_D = this;

        requestInfos.error = function(infos, code, errorMessage) {
            errorCallback(errorMessage);
            Office.UI.Utils.errorConsole('Error trying to reach the server : ' + errorMessage);
        };
        requestInfos.url = this._buildSearchRequestUrl$p$0(input, scopes, sources, groupName, maxCount);
        this._requestExecutor$p$0.executeAsync(requestInfos);
    },
    _buildResolveRequestUrl$p$0: function Office_UI_PeoplePicker__searchPrincipalServerDataProvider$_buildResolveRequestUrl$p$0(input, scopes, sources) {
        var url = '_api/';

        url += Office.UI.PeoplePicker._searchPrincipalServerDataProvider._resolveEndpointPath$p;
        var queryString = '?';

        queryString += 'input=\'' + encodeURIComponent(input) + '\'';
        queryString += '&scopes=' + scopes.toString();
        queryString += '&sources=' + sources.toString();
        return url + queryString;
    },
    _buildSearchRequestUrl$p$0: function Office_UI_PeoplePicker__searchPrincipalServerDataProvider$_buildSearchRequestUrl$p$0(input, scopes, sources, groupName, maxCount) {
        var url = '_api/';

        url += Office.UI.PeoplePicker._searchPrincipalServerDataProvider._searchEndpointPath$p;
        var queryString = '?';

        queryString += 'input=\'' + encodeURIComponent(input) + '\'';
        queryString += '&scopes=' + scopes.toString();
        queryString += '&sources=' + sources.toString();
        if (!Office.UI.Utils.isNullOrEmptyString(groupName)) {
            queryString += '&groupName=\'' + encodeURIComponent(groupName) + '\'';
        }
        queryString += '&maxCount=' + maxCount.toString();
        return url + queryString;
    }
};
Office.UI.PeoplePicker.ValidationError = function Office_UI_PeoplePicker_ValidationError() {
};
Office.UI.PeoplePicker.ValidationError._createMultipleMatchError$i = function Office_UI_PeoplePicker_ValidationError$_createMultipleMatchError$i() {
    var err = new Office.UI.PeoplePicker.ValidationError();

    err.errorName = Office.UI.PeoplePicker.ValidationError.multipleMatchName;
    err.localizedErrorMessage = Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_MultipleMatch);
    return err;
};
Office.UI.PeoplePicker.ValidationError._createMultipleEntryError$i = function Office_UI_PeoplePicker_ValidationError$_createMultipleEntryError$i() {
    var err = new Office.UI.PeoplePicker.ValidationError();

    err.errorName = Office.UI.PeoplePicker.ValidationError.multipleEntryName;
    err.localizedErrorMessage = Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_MultipleEntry);
    return err;
};
Office.UI.PeoplePicker.ValidationError._createNoMatchError$i = function Office_UI_PeoplePicker_ValidationError$_createNoMatchError$i() {
    var err = new Office.UI.PeoplePicker.ValidationError();

    err.errorName = Office.UI.PeoplePicker.ValidationError.noMatchName;
    err.localizedErrorMessage = Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_NoMatch);
    return err;
};
Office.UI.PeoplePicker.ValidationError._createServerProblemError$i = function Office_UI_PeoplePicker_ValidationError$_createServerProblemError$i() {
    var err = new Office.UI.PeoplePicker.ValidationError();

    err.errorName = Office.UI.PeoplePicker.ValidationError.serverProblemName;
    err.localizedErrorMessage = Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_ServerProblem);
    return err;
};
Office.UI.PeoplePicker.ValidationError.prototype = {
    errorName: null,
    localizedErrorMessage: null
};
Office.UI.PeoplePicker._mruCache = function Office_UI_PeoplePicker__mruCache() {
    this.isCacheAvailable = this._checkCacheAvailability$p$0();
    if (!this.isCacheAvailable) {
        return;
    }
    this._initializeCache$p$0();
};
Office.UI.PeoplePicker._mruCache.getInstance = function Office_UI_PeoplePicker__mruCache$getInstance() {
    if (!Office.UI.PeoplePicker._mruCache._instance$p) {
        Office.UI.PeoplePicker._mruCache._instance$p = new Office.UI.PeoplePicker._mruCache();
    }
    return Office.UI.PeoplePicker._mruCache._instance$p;
};
Office.UI.PeoplePicker._mruCache.prototype = {
    isCacheAvailable: false,
    _localStorage$p$0: null,
    _dataObject$p$0: null,
    get: function Office_UI_PeoplePicker__mruCache$get(key, maxResults) {
        if (Office.UI.Utils.isNullOrUndefined(maxResults) || !maxResults) {
            maxResults = Number.maxValue;
        }
        var numberOfResults = 0;
        var results = new Array(0);
        var cache = this._dataObject$p$0.cacheMapping[Office.UI.Runtime.context.sharePointHostUrl];
        var cacheLength = cache.length;

        for (var i = cacheLength; i > 0 && numberOfResults < maxResults; i--) {
            var candidate = cache[i - 1];

            if (this._entityMatches$p$0(candidate, key)) {
                results.push(candidate);
                numberOfResults += 1;
            }
        }
        return results;
    },
    set: function Office_UI_PeoplePicker__mruCache$set(entry) {
        var cache = this._dataObject$p$0.cacheMapping[Office.UI.Runtime.context.sharePointHostUrl];
        var cacheSize = cache.length;
        var alreadyThere = false;

        for (var i = 0; i < cacheSize; i++) {
            var cacheEntry = cache[i];

            if (cacheEntry.LoginName === entry.LoginName) {
                cache.splice(i, 1);
                alreadyThere = true;
                break;
            }
        }
        if (!alreadyThere) {
            if (cacheSize >= Office.UI.PeoplePicker._mruCache._maxCacheItem$p) {
                cache.splice(0, 1);
            }
        }
        cache.push(entry);
        this._cacheWrite$p$0(Office.UI.PeoplePicker._mruCache._localStorageKey$p, Office.UI.Utils.serializeJSON(this._dataObject$p$0));
    },
    _entityMatches$p$0: function Office_UI_PeoplePicker__mruCache$_entityMatches$p$0(candidate, key) {
        if (Office.UI.Utils.isNullOrEmptyString(key) || Office.UI.Utils.isNullOrUndefined(candidate)) {
            return false;
        }
        key = key.toLowerCase();
        var userNameKey = candidate.LoginName;

        if (Office.UI.Utils.isNullOrUndefined(userNameKey)) {
            userNameKey = '';
        }
        var divideIndex = userNameKey.indexOf('\\');

        if (divideIndex !== -1 && divideIndex !== userNameKey.length - 1) {
            userNameKey = userNameKey.substr(divideIndex + 1);
        }
        var emailKey = candidate.Email;

        if (Office.UI.Utils.isNullOrUndefined(emailKey)) {
            emailKey = '';
        }
        var atSignIndex = emailKey.indexOf('@');

        if (atSignIndex !== -1) {
            emailKey = emailKey.substr(0, atSignIndex);
        }
        if (Office.UI.Utils.isNullOrUndefined(candidate.DisplayName)) {
            candidate.DisplayName = '';
        }
        if (!(userNameKey.toLowerCase()).indexOf(key) || !(emailKey.toLowerCase()).indexOf(key) || !(candidate.DisplayName.toLowerCase()).indexOf(key)) {
            return true;
        }
        return false;
    },
    _initializeCache$p$0: function Office_UI_PeoplePicker__mruCache$_initializeCache$p$0() {
        var cacheData = this._cacheRetreive$p$0(Office.UI.PeoplePicker._mruCache._localStorageKey$p);

        if (Office.UI.Utils.isNullOrEmptyString(cacheData)) {
            this._dataObject$p$0 = new Office.UI.PeoplePicker._mruCache._mruData();
        }
        else {
            var datas = Office.UI.Utils.deserializeJSON(cacheData);

            if (datas.cacheVersion !== Office.UI.PeoplePicker._mruCache._currentVersion$p) {
                this._dataObject$p$0 = new Office.UI.PeoplePicker._mruCache._mruData();
                this._cacheDelete$p$0(Office.UI.PeoplePicker._mruCache._localStorageKey$p);
            }
            else {
                this._dataObject$p$0 = datas;
            }
        }
        if (Office.UI.Utils.isNullOrUndefined(this._dataObject$p$0.cacheMapping[Office.UI.Runtime.context.sharePointHostUrl])) {
            this._dataObject$p$0.cacheMapping[Office.UI.Runtime.context.sharePointHostUrl] = new Array(0);
        }
    },
    _checkCacheAvailability$p$0: function Office_UI_PeoplePicker__mruCache$_checkCacheAvailability$p$0() {
        this._localStorage$p$0 = window.self.localStorage;
        if (Office.UI.Utils.isNullOrUndefined(this._localStorage$p$0)) {
            return false;
        }
        return true;
    },
    _cacheRetreive$p$0: function Office_UI_PeoplePicker__mruCache$_cacheRetreive$p$0(key) {
        return this._localStorage$p$0.getItem(key);
    },
    _cacheWrite$p$0: function Office_UI_PeoplePicker__mruCache$_cacheWrite$p$0(key, value) {
        this._localStorage$p$0.setItem(key, value);
    },
    _cacheDelete$p$0: function Office_UI_PeoplePicker__mruCache$_cacheDelete$p$0(key) {
        this._localStorage$p$0.removeItem(key);
    }
};
Office.UI.PeoplePicker._mruCache._mruData = function Office_UI_PeoplePicker__mruCache__mruData() {
    this.cacheMapping = {};
    this.cacheVersion = Office.UI.PeoplePicker._mruCache._currentVersion$p;
    this.sharePointHost = Office.UI.Runtime.context.sharePointHostUrl;
};
Office.UI._peoplePickerTemplates = function Office_UI__peoplePickerTemplates() {
};
Office.UI._peoplePickerTemplates.getString = function Office_UI__peoplePickerTemplates$getString(stringName) {
    return Office.UI.Utils.getStringFromResource('PeoplePicker', stringName);
};
Office.UI._peoplePickerTemplates._getDefaultText$i = function Office_UI__peoplePickerTemplates$_getDefaultText$i(allowMultiple) {
    if (allowMultiple) {
        return Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_DefaultMessagePlural);
    }
    else {
        return Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_DefaultMessage);
    }
};
Office.UI._peoplePickerTemplates.generateControlTemplate = function Office_UI__peoplePickerTemplates$generateControlTemplate(inputName, allowMultiple, defaultTextOverride) {
    var defaultText;

    if (Office.UI.Utils.isNullOrEmptyString(defaultTextOverride)) {
        defaultText = Office.UI.Utils.htmlEncode(Office.UI._peoplePickerTemplates._getDefaultText$i(allowMultiple));
    }
    else {
        defaultText = Office.UI.Utils.htmlEncode(defaultTextOverride);
    }
    var body = '<div class=\"' + Office.UI._peoplePickerTemplates._actualControlClass$i + '\" title=\"' + defaultText + '\">';

    body += '<input type=\"hidden\"';
    if (!Office.UI.Utils.isNullOrEmptyString(inputName)) {
        body += ' name=\"' + Office.UI.Utils.htmlEncode(inputName) + '\"';
    }
    body += '/>';
    body += '<span class=\"' + Office.UI._peoplePickerTemplates._defaultTextClass$i + ' ' + Office.UI._peoplePickerTemplates._helperTextClass$i + '\">' + defaultText + '</span>';
    body += '<span class=\"' + Office.UI._peoplePickerTemplates._resolvedListClass$i + '\"></span>';
    body += '<input type=\"text\" class=\"' + Office.UI._peoplePickerTemplates._inputClass$i + '\" size=\"1\" autocorrect=\"off\" autocomplete=\"off\" autocapitalize=\"off\" title=\"' + defaultText + '\"/>';
    body += '<ul class=\"' + Office.UI._peoplePickerTemplates._autofillContainerClass$i + '\"></ul>';
    body += Office.UI._peoplePickerTemplates.generateAlertNode();
    body += '</div>';
    return body;
};
Office.UI._peoplePickerTemplates.generateErrorTemplate = function Office_UI__peoplePickerTemplates$generateErrorTemplate(ErrorMessage) {
    var innerHtml = '<span class=\"' + Office.UI._peoplePickerTemplates._errorMessageClass$i + ' ' + Office.UI._peoplePickerTemplates._controlErrorClass$i + '\">';

    innerHtml += Office.UI.Utils.htmlEncode(ErrorMessage);
    innerHtml += '</span>';
    return innerHtml;
};
Office.UI._peoplePickerTemplates.generateAutofillListItemTemplate = function Office_UI__peoplePickerTemplates$generateAutofillListItemTemplate(principal, source) {
    var elementClass = Office.UI._peoplePickerTemplates._autofillItemClass$i + ' ' + (source === Office.UI._principalSource.cache ? Office.UI._peoplePickerTemplates._autofillMRUClass$i : Office.UI._peoplePickerTemplates._autofillServerClass$i);
    var titleText = Office.UI.Utils.htmlEncode(Office.UI.Utils.isNullOrEmptyString(principal.Email) ? '' : principal.Email);
    var itemHtml = '<li class=\"' + elementClass + '\" ' + Office.UI._peoplePickerTemplates._autofillItemDataAttribute$i + '=\"' + Office.UI.Utils.htmlEncode(principal.LoginName) + '\" title=\"' + titleText + '\">';

    itemHtml += '<a onclick=\"return false;\" href=\"#\">';
    itemHtml += '<div class=\"' + Office.UI._peoplePickerTemplates._autofillMenuLabelClass$i + '\" unselectable=\"on\">' + Office.UI.Utils.htmlEncode(principal.DisplayName) + '</div>';
    if (!Office.UI.Utils.isNullOrEmptyString(principal.JobTitle)) {
        itemHtml += '<div class=\"' + Office.UI._peoplePickerTemplates._autofillMenuSublabelClass$i + '\" unselectable=\"on\">' + Office.UI.Utils.htmlEncode(principal.JobTitle) + '</div>';
    }
    itemHtml += '</a></li>';
    return itemHtml;
};
Office.UI._peoplePickerTemplates.generateAutofillListTemplate = function Office_UI__peoplePickerTemplates$generateAutofillListTemplate(cachedEntries, serverEntries, maxCount) {
    var html = '';

    if (Office.UI.Utils.isNullOrUndefined(cachedEntries)) {
        cachedEntries = new Array(0);
    }
    if (Office.UI.Utils.isNullOrUndefined(serverEntries)) {
        serverEntries = new Array(0);
    }
    html += Office.UI._peoplePickerTemplates._generateAutofillListTemplatePartial$p(cachedEntries, Office.UI._principalSource.cache);
    if (cachedEntries.length > 0) {
        html += Office.UI._peoplePickerTemplates._autofillListSeparator$p;
    }
    html += Office.UI._peoplePickerTemplates._generateAutofillListTemplatePartial$p(serverEntries, 0);
    if (serverEntries.length > 0) {
        html += Office.UI._peoplePickerTemplates._autofillListSeparator$p;
    }
    html += Office.UI._peoplePickerTemplates.generateAutofillFooterTemplate(cachedEntries.length + serverEntries.length, maxCount);
    return html;
};
Office.UI._peoplePickerTemplates._generateAutofillListTemplatePartial$p = function Office_UI__peoplePickerTemplates$_generateAutofillListTemplatePartial$p(principals, source) {
    var listHtml = '';

    for (var i = 0; i < principals.length; i++) {
        listHtml += Office.UI._peoplePickerTemplates.generateAutofillListItemTemplate(principals[i], source);
    }
    return listHtml;
};
Office.UI._peoplePickerTemplates.generateAutofillFooterTemplate = function Office_UI__peoplePickerTemplates$generateAutofillFooterTemplate(count, maxCount) {
    var footerHtml = '<li class=\"' + Office.UI._peoplePickerTemplates._autofillMenuFooterClass$i + '\">';
    var footerText;

    if (count >= maxCount) {
        footerText = Office.UI.Utils.formatString(Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_ShowingTopNumberOfResults), count.toString());
    }
    else {
        footerText = Office.UI.Utils.formatString(Office.UI.Utils.getLocalizedCountValue(Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_Results), Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_ResultsIntervals), count), count.toString());
    }
    footerText = Office.UI.Utils.htmlEncode(footerText);
    footerHtml += footerText;
    footerHtml += '</li>';
    footerHtml += '<li class=\"' + Office.UI._peoplePickerTemplates._autofillLoadingClass$i + '\"></li>';
    return footerHtml;
};
Office.UI._peoplePickerTemplates.generateRecordTemplate = function Office_UI__peoplePickerTemplates$generateRecordTemplate(record) {
    var recordHtml = '<span class=\"' + Office.UI._peoplePickerTemplates._userRecordClass$i + '\">';

    if (record.isResolved) {
        recordHtml += '<span class=\"' + Office.UI._peoplePickerTemplates._resolvedUserClass$i + '\" title=\"' + Office.UI.Utils.htmlEncode(record.text) + '\">' + Office.UI.Utils.htmlEncode(record.displayName) + '</span>';
    }
    else {
        recordHtml += '<a class=\"' + Office.UI._peoplePickerTemplates._unresolvedUserClass$i + '\" onclick=\"return false;\" href=\"#\" title=\"' + Office.UI.Utils.htmlEncode(record.text) + '\">' + Office.UI.Utils.htmlEncode(record.text) + '</a>';
    }
    recordHtml += '<a class=\"' + Office.UI._peoplePickerTemplates._recordRemoverClass$i + '\" onclick=\"return false;\" href=\"#\" title=\"' + Office.UI.Utils.formatString(Office.UI._peoplePickerTemplates.getString(Office.UI._peoplePickerResourcesStrings.pP_RemovePerson), Office.UI.Utils.htmlEncode(record.text)) + '\">' + 'x' + '</a>';
    recordHtml += '</span>';
    return recordHtml;
};
Office.UI._peoplePickerTemplates.generateAlertNode = function Office_UI__peoplePickerTemplates$generateAlertNode() {
    var alertHtml = '<div role=\"alert\" class=\"' + Office.UI._peoplePickerTemplates._alertDivClass$i + '\">';

    alertHtml += '</div>';
    return alertHtml;
};
Office.UI.PeoplePickerResourcesDefaults = function Office_UI_PeoplePickerResourcesDefaults() {
};
Office.UI._peoplePickerResourcesStrings = function Office_UI__peoplePickerResourcesStrings() {
};
if (Office.UI.PrincipalInfo.registerClass)
    Office.UI.PrincipalInfo.registerClass('Office.UI.PrincipalInfo');
if (Office.UI.PeoplePickerRecord.registerClass)
    Office.UI.PeoplePickerRecord.registerClass('Office.UI.PeoplePickerRecord');
if (Office.UI._keyCodes.registerClass)
    Office.UI._keyCodes.registerClass('Office.UI._keyCodes');
if (Office.UI.PeoplePicker.registerClass)
    Office.UI.PeoplePicker.registerClass('Office.UI.PeoplePicker');
if (Office.UI.PeoplePicker._internalPeoplePickerRecord.registerClass)
    Office.UI.PeoplePicker._internalPeoplePickerRecord.registerClass('Office.UI.PeoplePicker._internalPeoplePickerRecord');
if (Office.UI.PeoplePicker._autofillContainer.registerClass)
    Office.UI.PeoplePicker._autofillContainer.registerClass('Office.UI.PeoplePicker._autofillContainer');
if (Office.UI.PeoplePicker.Parameters.registerClass)
    Office.UI.PeoplePicker.Parameters.registerClass('Office.UI.PeoplePicker.Parameters');
if (Office.UI.PeoplePicker._cancelToken.registerClass)
    Office.UI.PeoplePicker._cancelToken.registerClass('Office.UI.PeoplePicker._cancelToken');
if (Office.UI.PeoplePicker._searchPrincipalServerDataProvider.registerClass)
    Office.UI.PeoplePicker._searchPrincipalServerDataProvider.registerClass('Office.UI.PeoplePicker._searchPrincipalServerDataProvider', null, Office.UI.PeoplePicker.ISearchPrincipalDataProvider);
if (Office.UI.PeoplePicker.ValidationError.registerClass)
    Office.UI.PeoplePicker.ValidationError.registerClass('Office.UI.PeoplePicker.ValidationError');
if (Office.UI.PeoplePicker._mruCache.registerClass)
    Office.UI.PeoplePicker._mruCache.registerClass('Office.UI.PeoplePicker._mruCache');
if (Office.UI.PeoplePicker._mruCache._mruData.registerClass)
    Office.UI.PeoplePicker._mruCache._mruData.registerClass('Office.UI.PeoplePicker._mruCache._mruData');
if (Office.UI._peoplePickerTemplates.registerClass)
    Office.UI._peoplePickerTemplates.registerClass('Office.UI._peoplePickerTemplates');
if (Office.UI.PeoplePickerResourcesDefaults.registerClass)
    Office.UI.PeoplePickerResourcesDefaults.registerClass('Office.UI.PeoplePickerResourcesDefaults');
if (Office.UI._peoplePickerResourcesStrings.registerClass)
    Office.UI._peoplePickerResourcesStrings.registerClass('Office.UI._peoplePickerResourcesStrings');
Office.UI._keyCodes.backspace = 8;
Office.UI._keyCodes.tab = 9;
Office.UI._keyCodes.escape = 27;
Office.UI._keyCodes.upArrow = 38;
Office.UI._keyCodes.downArrow = 40;
Office.UI._keyCodes.enter = 13;
Office.UI._keyCodes.deleteKey = 46;
Office.UI._keyCodes.k = 75;
Office.UI._keyCodes.v = 86;
Office.UI._keyCodes.semiColon = 186;
Office.UI.PeoplePicker.rootClassName = 'office office-peoplepicker';
Office.UI.PeoplePicker._focusClassName$i = 'office-peoplepicker-focus';
Office.UI.PeoplePicker._numberOfResults$p = 30;
Office.UI.PeoplePicker._autofillWait$p = 250;
Office.UI.PeoplePicker._minimumNumberOfLettersToQuery$p = 3;
Office.UI.PeoplePicker._maxCacheEntries$p = 5;
Office.UI.PeoplePicker._autofillContainer._currentOpened$p = null;
Office.UI.PeoplePicker._autofillContainer._boolBodyHandlerAdded$p = false;
Office.UI.PeoplePicker._autofillContainer._focusClassName$p = 'office-peoplepicker-autofill-focus';
Office.UI.PeoplePicker._searchPrincipalServerDataProvider._searchEndpointPath$p = 'SP.Utilities.Utility.SearchPrincipalsUsingContextWeb';
Office.UI.PeoplePicker._searchPrincipalServerDataProvider._resolveEndpointPath$p = 'SP.Utilities.Utility.ResolvePrincipalInCurrentContext';
Office.UI.PeoplePicker._searchPrincipalServerDataProvider._oDataJSONAcceptHeader$p = 'application/json;odata=verbose';
Office.UI.PeoplePicker.ValidationError.multipleMatchName = 'MultipleMatch';
Office.UI.PeoplePicker.ValidationError.multipleEntryName = 'MultipleEntry';
Office.UI.PeoplePicker.ValidationError.noMatchName = 'NoMatch';
Office.UI.PeoplePicker.ValidationError.serverProblemName = 'ServerProblem';
Office.UI.PeoplePicker._mruCache._instance$p = null;
Office.UI.PeoplePicker._mruCache._localStorageKey$p = 'Office.PeoplePicker.Cache';
Office.UI.PeoplePicker._mruCache._maxCacheItem$p = 200;
Office.UI.PeoplePicker._mruCache._currentVersion$p = 0;
Office.UI._peoplePickerTemplates._actualControlClass$i = 'office-peoplepicker-main';
Office.UI._peoplePickerTemplates._helperTextClass$i = 'office-helper';
Office.UI._peoplePickerTemplates._defaultTextClass$i = 'office-peoplepicker-default';
Office.UI._peoplePickerTemplates._autofillContainerClass$i = 'office-peoplepicker-menu';
Office.UI._peoplePickerTemplates._resolvedListClass$i = 'office-peoplepicker-recordList';
Office.UI._peoplePickerTemplates._inputClass$i = 'office-peoplepicker-input';
Office.UI._peoplePickerTemplates._loadingDataClass$i = 'office-peoplepicker-loading';
Office.UI._peoplePickerTemplates._errorMessageClass$i = 'office-peoplepicker-error';
Office.UI._peoplePickerTemplates._controlErrorClass$i = 'office-error';
Office.UI._peoplePickerTemplates._autofillOpenedClass$i = 'office-peoplepicker-autofillopened';
Office.UI._peoplePickerTemplates._autofillItemClass$i = 'office-peoplepicker-menu-item';
Office.UI._peoplePickerTemplates._autofillMRUClass$i = 'office-peoplepicker-autofill-mru';
Office.UI._peoplePickerTemplates._autofillServerClass$i = 'office-peoplepicker-autofill-Server';
Office.UI._peoplePickerTemplates._autofillItemDataAttribute$i = 'data-office-peoplepicker-value';
Office.UI._peoplePickerTemplates._autofillMenuLabelClass$i = 'office-menu-label';
Office.UI._peoplePickerTemplates._autofillMenuSublabelClass$i = 'office-menu-sublabel';
Office.UI._peoplePickerTemplates._autofillMenuFooterClass$i = 'office-menu-footer';
Office.UI._peoplePickerTemplates._autofillLoadingClass$i = 'office-peoplepicker-autofill-loading';
Office.UI._peoplePickerTemplates._userRecordClass$i = 'office-peoplepicker-record';
Office.UI._peoplePickerTemplates._resolvedUserClass$i = 'office-peoplepicker-resolved';
Office.UI._peoplePickerTemplates._unresolvedUserClass$i = 'office-peoplepicker-unresolved';
Office.UI._peoplePickerTemplates._recordRemoverClass$i = 'office-peoplepicker-deleterecord';
Office.UI._peoplePickerTemplates._alertDivClass$i = 'office-peoplepicker-alert';
Office.UI._peoplePickerTemplates._autofillListSeparator$p = '<li><hr></li>';
Office.UI.PeoplePickerResourcesDefaults.PP_SuggestionsAvailable = 'Suggestions Available';
Office.UI.PeoplePickerResourcesDefaults.PP_NoMatch = 'We couldn\'t find an exact match.';
Office.UI.PeoplePickerResourcesDefaults.PP_ShowingTopNumberOfResults = 'Showing the top {0} results';
Office.UI.PeoplePickerResourcesDefaults.PP_ServerProblem = 'Sorry, we\'re having trouble reaching the server.';
Office.UI.PeoplePickerResourcesDefaults.PP_DefaultMessagePlural = 'Enter names or email addresses...';
Office.UI.PeoplePickerResourcesDefaults.PP_MultipleMatch = 'Multiple entries matched, please click to resolve.';
Office.UI.PeoplePickerResourcesDefaults.PP_Results = 'No results found||Showing {0} result||Showing {0} results';
Office.UI.PeoplePickerResourcesDefaults.PP_Searching = 'Searching';
Office.UI.PeoplePickerResourcesDefaults.PP_ResultsIntervals = '0||1||2-';
Office.UI.PeoplePickerResourcesDefaults.PP_NoSuggestionsAvailable = 'No Suggestions Available';
Office.UI.PeoplePickerResourcesDefaults.PP_RemovePerson = 'Remove person or group {0}';
Office.UI.PeoplePickerResourcesDefaults.PP_DefaultMessage = 'Enter a name or email address...';
Office.UI.PeoplePickerResourcesDefaults.PP_MultipleEntry = 'You can only enter one name.';
Office.UI._peoplePickerResourcesStrings.pP_DefaultMessage = 'PP_DefaultMessage';
Office.UI._peoplePickerResourcesStrings.pP_DefaultMessagePlural = 'PP_DefaultMessagePlural';
Office.UI._peoplePickerResourcesStrings.pP_MultipleEntry = 'PP_MultipleEntry';
Office.UI._peoplePickerResourcesStrings.pP_MultipleMatch = 'PP_MultipleMatch';
Office.UI._peoplePickerResourcesStrings.pP_NoMatch = 'PP_NoMatch';
Office.UI._peoplePickerResourcesStrings.pP_NoSuggestionsAvailable = 'PP_NoSuggestionsAvailable';
Office.UI._peoplePickerResourcesStrings.pP_RemovePerson = 'PP_RemovePerson';
Office.UI._peoplePickerResourcesStrings.pP_Results = 'PP_Results';
Office.UI._peoplePickerResourcesStrings.pP_ResultsIntervals = 'PP_ResultsIntervals';
Office.UI._peoplePickerResourcesStrings.pP_Searching = 'PP_Searching';
Office.UI._peoplePickerResourcesStrings.pP_ServerProblem = 'PP_ServerProblem';
Office.UI._peoplePickerResourcesStrings.pP_ShowingTopNumberOfResults = 'PP_ShowingTopNumberOfResults';
Office.UI._peoplePickerResourcesStrings.pP_SuggestionsAvailable = 'PP_SuggestionsAvailable';
