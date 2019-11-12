/****************************** Module Header ******************************\
*Module Name:  Global.asax.cs
*Project:      CSAzureReceiveSalesMessageViaTopic
*Copyright (c) Microsoft Corporation.
* 
*In contrast to queues, in which each message is processed by a single consumer,
*topics and subscriptions provide a one-to-many form of communication, in a publish/subscribe pattern.
*
*This project will automatically receive messages that the sales department send when the sales order is built.
*
*This source is subject to the Microsoft Public License.
*See http://www.microsoft.com/en-us/openness/licenses.aspx#MPL
*All other rights reserved.
* 
*THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
*EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
*WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
\***************************************************************************/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CSAzureReceiveSalesMessageViaTopic
{
    public partial class ProductFootWearInfo : System.Web.UI.Page
    {
        DALClass dalClassHelper;

        public DALClass DalClassHelper
        {
            get
            {
                if (dalClassHelper == null)
                {
                    dalClassHelper = new DALClass();
                }
                return dalClassHelper;
            }

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Session["menu"] = 2;
            if (!IsPostBack)
            {

            }
            else
            {
                if (ViewState["FromDate"] != null || ViewState["ToDate"] != null)
                {

                    BindOrders();
                }
            }


        }

        /// <summary>
        /// Gets the data of the production order based on the from date value input and the to date value input.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            if (ViewState["FromDate"] != null)
            {
                ViewState["FromDate"] = txt_FromDate.Text.Trim();
            }
            else
            {
                ViewState.Add("FromDate", txt_FromDate.Text.Trim());
            }
            if (ViewState["ToDate"] != null)
            {
                ViewState["ToDate"] = txt_ToDate.Text.Trim();
            }
            else
            {
                ViewState.Add("ToDate", txt_ToDate.Text.Trim());
            }

            BindOrders();
        }

        /// <summary>
        /// Gets the data of the production order based on the from date value input and the to date value input.
        /// </summary>
        private void BindOrders()
        {
            if (tbl_FootWearOrder.Rows.Count > 0)
            {
                try
                {
                    if (tbl_FootWearOrder.Rows.Count > 2)
                    {
                        int intRowCount = tbl_FootWearOrder.Rows.Count;
                        for (int i = 2; i < intRowCount; i++)
                        {
                            tbl_FootWearOrder.Rows.RemoveAt(2);
                        }
                    }

                    DataTable dtOrder = DalClassHelper.GetProductFootWearOrder(txt_FromDate.Text.Trim(), txt_ToDate.Text.Trim());

                    for (int i = 0; i < dtOrder.Rows.Count; i++)
                    {
                        #region //Initializes a new instance of the TableRow
                        TableRow tableRow = new TableRow();
                        tableRow.CssClass = "TableRow";
                        tableRow.ID = i.ToString() + "_" + dtOrder.Rows[i]["ProductFootWearOrderId"].ToString();

                        TableCell cellProductOrderNo = new TableCell();
                        cellProductOrderNo.CssClass = "TableCell1";
                        cellProductOrderNo.Text = dtOrder.Rows[i]["ProductFootWearOrderNo"].ToString();

                        TableCell cellSaleOrderNo = new TableCell();
                        cellSaleOrderNo.CssClass = "TableCell1";
                        cellSaleOrderNo.Text = dtOrder.Rows[i]["SaleOrderNo"].ToString();

                        TableCell cellCreatedDate = new TableCell();
                        cellCreatedDate.CssClass = "TableCell1";
                        cellCreatedDate.Text = FormatString(dtOrder.Rows[i]["CreatedDate"].ToString());

                        TableCell cellCustmerName = new TableCell();
                        cellCustmerName.CssClass = "TableCell1";
                        cellCustmerName.Text = dtOrder.Rows[i]["CustomerName"].ToString();

                        TableCell cellSalesManName = new TableCell();
                        cellSalesManName.CssClass = "TableCell1";
                        cellSalesManName.Text = dtOrder.Rows[i]["SalesManName"].ToString();

                        TableCell cellDeliveryDate = new TableCell();
                        cellDeliveryDate.CssClass = "TableCell1";
                        cellDeliveryDate.Text = FormatString(dtOrder.Rows[i]["DeliveryProductDate"].ToString());

                        TableCell cellShippingAddress = new TableCell();
                        cellShippingAddress.CssClass = "TableCell1";
                        cellShippingAddress.Text = dtOrder.Rows[i]["CustomerAddress"].ToString();

                        TableCell cellDetails = new TableCell();
                        cellDetails.CssClass = "TableCell1";
                        cellDetails.ID = i.ToString() + "_Details" + dtOrder.Rows[i]["ProductFootWearOrderId"].ToString();
                        Button btn_cellDetails = new Button();
                        btn_cellDetails.ID = i.ToString() + "DetailButton_" + dtOrder.Rows[i]["ProductFootWearOrderId"].ToString();
                        btn_cellDetails.Text = "";
                        btn_cellDetails.BorderStyle = BorderStyle.None;
                        btn_cellDetails.CssClass = "DetailsButton";
                        btn_cellDetails.Click += new EventHandler(Btn_ViewDetailsClick);
                        cellDetails.Controls.Add(btn_cellDetails);
                       
                        tableRow.Cells.Add(cellProductOrderNo);
                        tableRow.Cells.Add(cellSaleOrderNo);
                        tableRow.Cells.Add(cellCreatedDate);
                        tableRow.Cells.Add(cellCustmerName);
                        tableRow.Cells.Add(cellSalesManName);
                        tableRow.Cells.Add(cellDeliveryDate);
                        tableRow.Cells.Add(cellShippingAddress);
                        tableRow.Cells.Add(cellDetails);
                        tbl_FootWearOrder.Rows.Add(tableRow);
                      
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        /// <summary>
        /// Coverts the string to the specified string  using the specified format. 
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        private string FormatString(string strContent)
        {
            string strRet = string.Empty;
            try
            {
                DateTime dt = Convert.ToDateTime(strContent);

                strRet = dt.ToString("yyyy/MM/dd");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRet;
        }

        /// <summary>
        ///   Views all the product data of the order selected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_ViewDetailsClick(object sender, EventArgs e)
        {
            Button btn_edit = (Button)sender;
            string strId = btn_edit.ID;
            string[] Arra = strId.Split('_');
            if (Arra.Length == 2)
            {
                string stProductClothesOrderId = Arra[1];

                if (ViewState["SelectedDetalis"] != null)
                {
                    ViewState["SelectedDetalis"] = stProductClothesOrderId;
                }
                else
                {
                    ViewState.Add("SelectedDetalis", stProductClothesOrderId);
                }

                try
                {
                    BindProductInfo(stProductClothesOrderId);
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Error Message:" + ex.Message + "');</script>");
                }
            }
        }

        /// <summary>
        ///  Binds the data of the product to the Control table.
        /// </summary>
        /// <param name="stProductClothesOrderId"></param>
        private void BindProductInfo(string stProductClothesOrderId)
        {
            if (tbl_ProductionDetails.Rows.Count > 0)
            {
                try
                {
                    if (tbl_ProductionDetails.Rows.Count > 2)
                    {
                        int intRowCount = tbl_FootWearOrder.Rows.Count;
                        for (int i = 2; i < intRowCount; i++)
                        {
                            tbl_ProductionDetails.Rows.RemoveAt(2);
                        }
                    }

                    DataTable dtDetailsOrder = DalClassHelper.GetProductFootWearDetailsOrder(stProductClothesOrderId);

                    for (int i = 0; i < dtDetailsOrder.Rows.Count; i++)
                    {
                        #region //Initializes a new instance of the TableRow
                        TableRow tableRow = new TableRow();
                        tableRow.CssClass = "TableRow";
                        tableRow.ID = i.ToString() + "_Details" + dtDetailsOrder.Rows[i]["ProductFootWearOrderId"].ToString();

                        TableCell cellProductClothesOrderId = new TableCell();
                        cellProductClothesOrderId.CssClass = "TableCell1";
                        cellProductClothesOrderId.Text = dtDetailsOrder.Rows[i]["ProductFootWearOrderId"].ToString();
                       
                        TableCell cellProductNo = new TableCell();
                        cellProductNo.CssClass = "TableCell1";
                        cellProductNo.Text = dtDetailsOrder.Rows[i]["ProductNo"].ToString();

                        TableCell cellProductName = new TableCell();
                        cellProductName.CssClass = "TableCell1";
                        cellProductName.Text = dtDetailsOrder.Rows[i]["ProductName"].ToString();

                        TableCell cellProductColor = new TableCell();
                        cellProductColor.CssClass = "TableCell1";
                        cellProductColor.Text = dtDetailsOrder.Rows[i]["ProductColor"].ToString();

                        TableCell cellProductSize = new TableCell();
                        cellProductSize.CssClass = "TableCell1";
                        cellProductSize.Text = dtDetailsOrder.Rows[i]["ProductSize"].ToString();

                        TableCell cellProductPrice = new TableCell();
                        cellProductPrice.CssClass = "TableCell1";
                        cellProductPrice.Text = dtDetailsOrder.Rows[i]["ProductPrice"].ToString();

                        TableCell cellNumber = new TableCell();
                        cellNumber.CssClass = "TableCell1";
                        cellNumber.Text = dtDetailsOrder.Rows[i]["ProductNumber"].ToString();

                        TableCell cellUnit = new TableCell();
                        cellUnit.CssClass = "TableCell1";
                        cellUnit.Text = dtDetailsOrder.Rows[i]["ProductUnit"].ToString();
                        tableRow.Cells.Add(cellProductClothesOrderId);
                        tableRow.Cells.Add(cellProductNo);
                        tableRow.Cells.Add(cellProductName);
                        tableRow.Cells.Add(cellProductColor);
                        tableRow.Cells.Add(cellProductSize);
                        tableRow.Cells.Add(cellProductPrice);
                        tableRow.Cells.Add(cellNumber);
                        tableRow.Cells.Add(cellUnit);


                        tbl_ProductionDetails.Rows.Add(tableRow);
                    
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected void img_Btn_FromDate_Click(object sender, ImageClickEventArgs e)
        {
            calendarFrom.Visible = !calendarFrom.Visible;
        }

        protected void img_Btn_ToDate_Click(object sender, ImageClickEventArgs e)
        {
            calendarTo.Visible = !calendarTo.Visible;
        }

        protected void RequestedFromDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            txt_FromDate.Text = RequestedFromDateCalendar.SelectedDate.ToShortDateString();
            calendarFrom.Visible = false;
            txt_FromDate.Focus();
        }

        protected void RequestedToDateCalendar_SelectionChanged(object sender, EventArgs e)
        {
            txt_ToDate.Text = requestedToDateCalendar.SelectedDate.ToShortDateString();
            calendarTo.Visible = false;
            txt_ToDate.Focus();
        }
    }
}