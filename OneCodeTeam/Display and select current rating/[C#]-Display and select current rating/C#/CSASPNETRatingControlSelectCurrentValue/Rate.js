/****************************** Module Header ******************************\
* Module Name:    Rate.js
* Project:        CSASPNETRatingControlSelectCurrentValue
* Copyright (c) Microsoft Corporation
*
* Rate.js
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL.
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\*****************************************************************************/
Sys.Extended.UI.RatingBehavior.prototype._onStarClick = function(item) {
    if (this._readOnly) {
        return;
    }
    this.set_Rating(this._currentRating);
    __doPostBack('btnSubmit', '');

}; 