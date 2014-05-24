using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace RS_Goal_Tracker.Interface
{
    public class XButton:Control
    {
        private const string imageName = "RS_Goal_Tracker.Interface.img.XButton";
        private Image backgroundImage, hoverBackgroundImage, pressedBackgroundImage;
        private bool pressed = false;
        private bool hovering = false;
        
        public Rectangle Dimensions
        {
            get
            {
                if (backgroundImage != null)
                    return new Rectangle(0, 0, this.backgroundImage.Width, this.backgroundImage.Height);
                else
                    return new Rectangle(0, 0, this.Size.Width, this.Size.Height);
            }
        }

        public XButton():base()
        {        
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BackColor = System.Drawing.Color.Maroon; 
            this.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.Name = "closeTabButton";
            this.Size = new Size(24, 19); // default width and height

            //this.Margin = new System.Windows.Forms.Padding(0);
            //this.Padding = new System.Windows.Forms.Padding(0);
            //this.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            //this.UseVisualStyleBackColor = false;   
            //this.FlatAppearance.BorderSize = 0;
            //this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            //this.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));

            try 
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream imageStream;

                imageStream= assembly.GetManifestResourceStream(imageName + ".bmp");
                this.backgroundImage = new Bitmap(imageStream);

                imageStream = assembly.GetManifestResourceStream(imageName + "_Hover.bmp");
                this.hoverBackgroundImage = new Bitmap(imageStream);

                imageStream = assembly.GetManifestResourceStream(imageName + "_Click.bmp");
                this.pressedBackgroundImage = new Bitmap(imageStream);

                imageStream.Close();
                imageStream.Dispose();

                this.Text = "";
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Failed to create XButton image.");
                System.Diagnostics.Debug.WriteLine(" > " + e.Message);
                System.Diagnostics.Debug.WriteLine(" > " + e.StackTrace);
                this.Text = "x";
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            this.pressed = true;
            this.Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnMouseHover(EventArgs e)
        {
            this.hovering = true;
            this.Invalidate();
            base.OnMouseHover(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            this.hovering = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            this.pressed = false;
            this.Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {            
            /*
            if (this.pressed && this.pressedBackgroundImage != null)
                e.Graphics.DrawImage(this.pressedBackgroundImage, scaledDimensions, imageDimensions, GraphicsUnit.Pixel);
            else if (this.hovering && this.hoverBackgroundImage != null)
                e.Graphics.DrawImage(this.hoverBackgroundImage, scaledDimensions, imageDimensions, GraphicsUnit.Pixel);
            else
                e.Graphics.DrawImage(this.backgroundImage, scaledDimensions, imageDimensions, GraphicsUnit.Pixel);
             */
            Rectangle scaledDimensions = new Rectangle(0, 0, this.Size.Width, this.Size.Height);

            if (this.pressed && this.pressedBackgroundImage != null)
                e.Graphics.DrawImage(this.pressedBackgroundImage, scaledDimensions, this.Dimensions, GraphicsUnit.Pixel);
            else if (this.hovering && this.hoverBackgroundImage != null)
                e.Graphics.DrawImage(this.hoverBackgroundImage, scaledDimensions, this.Dimensions, GraphicsUnit.Pixel);
            else if (this.backgroundImage != null)
                e.Graphics.DrawImage(this.backgroundImage, scaledDimensions, this.Dimensions, GraphicsUnit.Pixel);
            else
                base.OnPaint(e);
        }
    }
}
