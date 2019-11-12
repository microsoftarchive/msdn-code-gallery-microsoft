using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace CSASPNETImageEditUpload
{
    /// <summary>
    /// Connected to the DataBase,
    /// If the specific record has image,read out the specific row's 
    /// image byte collection as well as image type, output it.
    /// If not, output the default image instead.
    /// </summary>
    public class ImageHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = new SqlConnection(
                    ConfigurationManager.ConnectionStrings["db_PersonsConnectionString"].ConnectionString);
                cmd.Connection.Open();
                cmd.CommandText = "select PersonImage,PersonImageType from tb_personInfo" +
                    " where id=" + context.Request.QueryString["id"];

                SqlDataReader reader = cmd.ExecuteReader(
                    CommandBehavior.CloseConnection | CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    byte[] imgbytes = null;
                    string imgtype = null;

                    if (reader.GetValue(0) != DBNull.Value)
                    {
                        imgbytes = (byte[])reader.GetValue(0);
                        imgtype = reader.GetString(1);

                        // If bmp, convert to jpg and show because of the different formation type.
                        if (imgtype.Equals("image/bmp", StringComparison.OrdinalIgnoreCase))
                        {
                            using (MemoryStream ms = new MemoryStream(imgbytes))
                            {
                                using (Bitmap bm = new Bitmap(Image.FromStream(ms)))
                                {
                                    bm.Save(context.Response.OutputStream, ImageFormat.Jpeg);
                                }
                            }
                        }
                        else
                        {
                            context.Response.ContentType = imgtype;
                            context.Response.BinaryWrite(imgbytes);
                        }
                    }
                    else
                    {
                        imgbytes = File.ReadAllBytes(context.Server.MapPath
                            ("~/DefaultImage/DefaultImage.JPG"));
                        imgtype = "image/pjpeg";
                        context.Response.ContentType = imgtype;
                        context.Response.BinaryWrite(imgbytes);
                    }
                }
                reader.Close();
                context.Response.End();
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}