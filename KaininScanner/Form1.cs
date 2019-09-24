using System;
using System.Drawing;
using System.Windows.Forms;

namespace KaininScanner
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public static Image Snip()
        {
            var rc = Screen.PrimaryScreen.Bounds;
            using (Bitmap bmp = new Bitmap(rc.Width, rc.Height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb))
            {
                using (Graphics gr = Graphics.FromImage(bmp))
                    gr.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                using (var snipper = new DummyForm(bmp))
                {
                    if (snipper.ShowDialog() == DialogResult.OK)
                    {
                        return snipper.Image;
                    }
                }
                return null;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (var bmp = Snip())
            {
                if (bmp != null)
                {
                    using (var image = new Bitmap(bmp))
                    {
                        using (var ocr = new Tesseract.TesseractEngine(@"./tessdata", "eng", Tesseract.EngineMode.Default))
                        {
                            ocr.SetVariable("tessedit_char_whitelist", "0123456789");
                            using (var result = ocr.Process(image, Tesseract.Rect.Empty))
                            {
                                Clipboard.SetText(result.GetText().Trim());
                            }
                        }
                    }
                }

            }
        }
    }
}
