using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CropPicture
{
    public partial class Form1 : Form
    {
        Image img;
        Boolean mouseClicked;
        Point startPoint = new Point();
        Point endPoint = new Point();
        Rectangle rectCropArea;

        public Form1()
        {
            InitializeComponent();
            mouseClicked = false;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            loadPrimaryImage();
        }

        private void loadPrimaryImage()
        {
            img = Image.FromFile(@"..\..\images.jpg");
            pictureBox1.Image = img;
            //pictureBox1.Height = img.Height;
            //pictureBox1.Width = img.Width;
        }

        private void PicBox_MouseUp(object sender, MouseEventArgs e)
        {
            mouseClicked = false;

            if (endPoint.X != -1)
            {
                Point currentPoint = new Point(e.X, e.Y);
                // Display coordinates
                X2.Text = e.X.ToString();
                Y2.Text = e.Y.ToString();

            }
            endPoint.X = -1;
            endPoint.Y = -1;
            startPoint.X = -1;
            startPoint.Y = -1;
        }


        private void PicBox_MouseDown(object sender, MouseEventArgs e)
        {
            mouseClicked = true;

            startPoint.X = e.X;
            startPoint.Y = e.Y;
            // Display coordinates
            X1.Text = startPoint.X.ToString();
            Y1.Text = startPoint.Y.ToString();

            endPoint.X = -1;
            endPoint.Y = -1;

            rectCropArea = new Rectangle(new Point(e.X, e.Y), new Size());
        }


        private void PicBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point ptCurrent = new Point(e.X, e.Y);

            if (mouseClicked)
            {
                if (endPoint.X != -1)
                {
                    // Display Coordinates
                    X1.Text = startPoint.X.ToString();
                    Y1.Text = startPoint.Y.ToString();
                    X2.Text = e.X.ToString();
                    Y2.Text = e.Y.ToString();
                }

                endPoint = ptCurrent;

                if (e.X > startPoint.X && e.Y > startPoint.Y)
                {
                    rectCropArea.Width = e.X - startPoint.X;
                    rectCropArea.Height = e.Y - startPoint.Y;
                }
                else if (e.X < startPoint.X && e.Y > startPoint.Y)
                {
                    rectCropArea.Width = startPoint.X - e.X;
                    rectCropArea.Height = e.Y - startPoint.Y;
                    rectCropArea.X = e.X;
                    rectCropArea.Y = startPoint.Y;
                }
                else if (e.X > startPoint.X && e.Y < startPoint.Y)
                {
                    rectCropArea.Width = e.X - startPoint.X;
                    rectCropArea.Height = startPoint.Y - e.Y;
                    rectCropArea.X = startPoint.X;
                    rectCropArea.Y = e.Y;
                }
                else
                {
                    rectCropArea.Width = startPoint.X - e.X;
                    rectCropArea.Height = startPoint.Y - e.Y;
                    rectCropArea.X = e.X;
                    rectCropArea.Y = e.Y;
                }
                pictureBox1.Refresh();
            }
        }

        private void PicBox_Paint(object sender, PaintEventArgs e)
        {
            Pen drawLine = new Pen(Color.Red);
            drawLine.DashStyle = DashStyle.Dash;
            e.Graphics.DrawRectangle(drawLine, rectCropArea);
        }

        private void btnCrop_Click(object sender, EventArgs e)
        {
            pictureBox2.Refresh();
            
            Bitmap sourceBitmap = new Bitmap(pictureBox1.Image, pictureBox1.Width, pictureBox1.Height);
            Graphics g = pictureBox2.CreateGraphics();

            if (!checkBox1.Checked)
            {
                g.DrawImage(sourceBitmap, new Rectangle(0, 0, pictureBox2.Width, pictureBox2.Height), rectCropArea, GraphicsUnit.Pixel);
                sourceBitmap.Dispose();
            }
            else
            {
                
                int x1, x2, y1, y2;
                Int32.TryParse(CX1.Text,out x1);
                Int32.TryParse(CX2.Text, out x2);
                Int32.TryParse(CY1.Text, out y1);
                Int32.TryParse(CY2.Text, out y2);

                if ((x1 < x2 && y1 < y2))
                {
                    rectCropArea = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                }
                else if (x2 < x1 && y2 > y1)
                {
                    rectCropArea = new Rectangle(x2, y1, x1 - x2, y2 - y1);
                }
                else if (x2 > x1 && y2 < y1)
                {
                    rectCropArea = new Rectangle(x1, y2, x2 - x1, y1 - y2);
                }
                else
                {
                    rectCropArea = new Rectangle(x2, y2, x1 - x2, y1 - y2);
                }

                pictureBox1.Refresh(); // This repositions the dashed box to new location as per coordinates entered.

                g.DrawImage(sourceBitmap, new Rectangle(0, 0, pictureBox2.Width, pictureBox2.Height), rectCropArea, GraphicsUnit.Pixel);
                sourceBitmap.Dispose();
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            // to remove the dashes
            pictureBox1.Refresh();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                CX1.Visible = true; label10.Visible = true;
                CY1.Visible = true; label9.Visible = true;
                CX2.Visible = true; label8.Visible = true;
                CY2.Visible = true; label7.Visible = true;
                X1.Text = "0"; X2.Text = "0"; Y1.Text = "0"; Y2.Text = "0";
            }
            else
            {
                CX1.Visible = false; label10.Visible = false;
                CY1.Visible = false; label9.Visible = false;
                CX2.Visible = false; label8.Visible = false;
                CY2.Visible = false; label7.Visible = false;
            }
        }

    }
}
