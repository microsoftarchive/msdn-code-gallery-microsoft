/****************************** Module Header ******************************\
* Module Name:    Default.aspx.cs
* Project:        CSASPNETImageEditUpload
* Copyright (c) Microsoft Corporation
*
* The project shows up how to insert,edit or update an image and store
* it into Sql database.
* 
* This source is subject to the Microsoft Public License.
* See http://www.microsoft.com/opensource/licenses.mspx#Ms-PL.
* All other rights reserved.
\***************************************************************************/

using System.Collections.Generic;
using System.Web.UI.WebControls;
using System;
using System.Data.SqlClient;
using System.IO;


namespace CSASPNETImageEditUpload
{
    public partial class _Default : System.Web.UI.Page
    {
        // Static types of common images for checking.
        private static List<string> imgytpes = new List<string>()
        {
            ".BMP",".GIF",".JPG",".PNG"
        };


        /// <summary>
        /// Read all records into GridView.
        /// If has records, select the first record to be shown in the FormView
        /// as default; otherwise, change the formview to insert mode so as to let
        /// data to be inserted.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvPersonOverView.DataBind();

                if (gvPersonOverView.Rows.Count > 0)
                {
                    gvPersonOverView.SelectedIndex = 0;
                    fvPersonDetails.ChangeMode(FormViewMode.ReadOnly);
                    fvPersonDetails.DefaultMode = FormViewMode.ReadOnly;
                }
                else
                {
                    fvPersonDetails.ChangeMode(FormViewMode.Insert);
                    fvPersonDetails.DefaultMode = FormViewMode.Insert;
                }
            }
        }


        /// <summary>
        /// Validate whether data satisfies the type of image.
        /// </summary>
        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (args.Value != null && args.Value != "")
            {
                args.IsValid = imgytpes.IndexOf(System.IO.Path.GetExtension(args.Value).ToUpper()) >= 0;
            }
        }


        /// <summary>
        /// After checking the validation of the image type,
        /// assign the image type and the image byte collection through
        /// the e.Values argument and do the insert.
        /// </summary>
        protected void fvPersonDetails_ItemInserting(object sender, FormViewInsertEventArgs e)
        {
            object obj = Session["insertstate"];

            if (obj == null || (bool)obj)
            {
                CustomValidator cv = fvPersonDetails.FindControl("cmvImageType") as CustomValidator;

                cv.Validate();
                e.Cancel = !cv.IsValid;

                FileUpload fup = (FileUpload)fvPersonDetails.FindControl("fupInsertImage");

                if (cv.IsValid && fup.PostedFile.FileName.Trim() != "")
                {
                    e.Values["PersonImage"] = File.ReadAllBytes(fup.PostedFile.FileName);
                    e.Values["PersonImageType"] = fup.PostedFile.ContentType;
                }

            }
            else
            {
                e.Cancel = true;
                gvPersonOverView.DataBind();
                fvPersonDetails.ChangeMode(FormViewMode.ReadOnly);
                fvPersonDetails.DefaultMode = FormViewMode.ReadOnly;
            }
        }


        /// <summary>
        /// After checking the validation of the image type,
        /// assign the image type and the image byte collection through
        /// the e.Values argument and do the update.
        /// </summary>
        protected void fvPersonDetails_ItemUpdating(object sender, FormViewUpdateEventArgs e)
        {
            CustomValidator cv = fvPersonDetails.FindControl("cmvImageType") as CustomValidator;

            cv.Validate();
            e.Cancel = !cv.IsValid;

            FileUpload fup = (FileUpload)fvPersonDetails.FindControl("fupEditImage");

            if (cv.IsValid && fup.PostedFile.FileName.Trim() != "")
            {
                e.NewValues["PersonImage"] = File.ReadAllBytes(fup.PostedFile.FileName);
                e.NewValues["PersonImageType"] = fup.PostedFile.ContentType;
            }
        }


        /// <summary>
        /// After updated, re-databind data and select the first one as default.
        /// </summary>
        protected void fvPersonDetails_ItemUpdated(object sender, FormViewUpdatedEventArgs e)
        {
            gvPersonOverView.DataBind();
            gvPersonOverView.SelectedIndex = gvPersonOverView.SelectedRow.RowIndex;
        }


        /// <summary>
        /// After inserted successfully, re-databind data,select the first one as default,
        /// Change the FormView mode to ReadOnly (for viewing).
        /// </summary>
        protected void fvPersonDetails_ItemInserted(object sender, FormViewInsertedEventArgs e)
        {
            gvPersonOverView.DataBind();
            gvPersonOverView.SelectedIndex = gvPersonOverView.Rows.Count - 1;
            fvPersonDetails.ChangeMode(FormViewMode.ReadOnly);
            fvPersonDetails.DefaultMode = FormViewMode.ReadOnly;
        }


        /// <summary>
        /// After deleted successfully, re-databind data.
        /// </summary>
        protected void fvPersonDetails_ItemDeleted(object sender, FormViewDeletedEventArgs e)
        {
            gvPersonOverView.DataBind();

            // If still has records
            if (gvPersonOverView.Rows.Count > 0)
            {
                // Take out the index of row to be deleted
                int delindex = (int)ViewState["delindex"];

                // If the first record is deleted,Move to its next record.
                if (delindex == 0)
                {
                    gvPersonOverView.SelectedIndex = 0;
                }

                // If the last record is deleted, Move to its previous one.
                else if (delindex == gvPersonOverView.Rows.Count)
                {
                    gvPersonOverView.SelectedIndex = gvPersonOverView.Rows.Count - 1;
                }

                // Otherwise, move to the next record after itself is deleted.
                else
                {
                    gvPersonOverView.SelectedIndex = delindex;
                }

            }

            // If has no records, change to insert mode for insering new records.
            else
            {
                fvPersonDetails.ChangeMode(FormViewMode.Insert);
                fvPersonDetails.DefaultMode = FormViewMode.Insert;
            }
        }


        /// <summary>
        /// To show detail image and information in the FormView when GridView's
        /// SelectedRowIndex Changed.
        /// </summary>
        protected void gvPersonOverView_SelectedIndexChanged(object sender, EventArgs e)
        {
            fvPersonDetails.ChangeMode(FormViewMode.ReadOnly);
            fvPersonDetails.DefaultMode = FormViewMode.ReadOnly;
        }


        /// <summary>
        /// Keep the row index into ViewState for the usage of Item_Deleted.
        /// </summary>
        protected void fvPersonDetails_ItemDeleting(object sender, FormViewDeleteEventArgs e)
        {
            ViewState["delindex"] = gvPersonOverView.SelectedIndex;
        }


        /// <summary>
        /// Keep the insertState into Session to avoid the duplicated inserting
        /// after refreshing page.
        /// </summary>
        protected void fvPersonDetails_ModeChanging(object sender, FormViewModeEventArgs e)
        {
            Session["insertstate"] = (e.NewMode == FormViewMode.Insert);

        }
    }
}