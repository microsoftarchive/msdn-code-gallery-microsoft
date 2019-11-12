/****************************** Module Header ******************************\
* Module Name: WebGridExtension.cs
* Project:     CSASPNETWebGrid
* Copyright (c) Microsoft Corporation.
* 
* This sample will show how to add checkbox to MVC web grid.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
* All other rights reserved.
* 
* THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
* EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
* WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;

namespace System.Web.Helpers
{
    public static class WebGridExtension
    {
        public static IHtmlString GetHtmlWithSelectAllCheckBox(this WebGrid webGrid, string tableStyle = null,
            string headerStyle = null, string footerStyle = null, string rowStyle = null,
            string alternatingRowStyle = null, string selectedRowStyle = null,
            string caption = null, bool displayHeader = true, bool fillEmptyRows = false,
            string emptyRowCellValue = null, IEnumerable<WebGridColumn> columns = null,
            IEnumerable<string> exclusions = null, WebGridPagerModes mode = WebGridPagerModes.All,
            string firstText = null, string previousText = null, string nextText = null,
            string lastText = null, int numericLinksCount = 5, object htmlAttributes = null,
            string checkBoxValue = "ID")
        {
            var newColumn = webGrid.Column(header: "{}",
                format: item => new HelperResult(writer =>
                {
                    writer.Write("<input class=\"singleCheckBox\" name=\"selectedRows\" value=\""
                    + item.Value.GetType().GetProperty(checkBoxValue).GetValue(item.Value, null).ToString()
                    + "\" type=\"checkbox\" />"
                    );
                }));
            var newColumns = columns.ToList();
            newColumns.Insert(0, newColumn);
            var script = @"<script>
                if (typeof jQuery == 'undefined')
                {
                    document.write(
                        unescape(
                        ""%3Cscript src='http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.9.0.min.js'%3E%3C/script%3E""
                        )
                     );
                }
                (function(){
                    window.setTimeout(function() { initializeCheckBoxes();  }, 1000);
                    function initializeCheckBoxes(){   
                        $(function () {
                            $('#allCheckBox').live('click',function () {
                                var isChecked = $(this).attr('checked');                       
                                $('.singleCheckBox').attr('checked', isChecked  ? true: false);
                                $('.singleCheckBox').closest('tr').addClass(isChecked  ? 'selected-row': 'not-selected-row');
                                $('.singleCheckBox').closest('tr').removeClass(isChecked  ? 'not-selected-row': 'selected-row');
                            });
                            $('.singleCheckBox').live('click',function () {
                                var isChecked = $(this).attr('checked');
                                $(this).closest('tr').addClass(isChecked  ? 'selected-row': 'not-selected-row');
                                $(this).closest('tr').removeClass(isChecked  ? 'not-selected-row': 'selected-row');
                                if(isChecked && $('.singleCheckBox').length == $('.selected-row').length)
                                     $('#allCheckBox').attr('checked',true);
                                else
                                    $('#allCheckBox').attr('checked',false);
                            });
                        });
                    }
                })();
            </script>";
            var html = webGrid.GetHtml(tableStyle, headerStyle, footerStyle, rowStyle,
                                          alternatingRowStyle, selectedRowStyle, caption,
                                          displayHeader, fillEmptyRows, emptyRowCellValue,
                                          newColumns, exclusions, mode, firstText,
                                          previousText, nextText, lastText,
                                          numericLinksCount, htmlAttributes
                                          );

            return MvcHtmlString.Create(html.ToString().Replace("{}",
                                        "<input type='checkbox' id='allCheckBox'/>") + script);
        }  
    }
}