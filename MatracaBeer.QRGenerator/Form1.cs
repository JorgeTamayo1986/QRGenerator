using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace MatracaBeer.QRGenerator
{
    public partial class frmGenerateQR : Form
    {
        public frmGenerateQR()
        {
            InitializeComponent();
            LoadAndClean();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                bool generate = true;
                if (string.IsNullOrWhiteSpace(txtInfo.Text))
                {
                    MessageBox.Show(this, "No se encontró información para generar el QR", "Error al generar", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    generate = false;
                }

                if (generate)
                {
                    QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                    QrCode qrCode = new QrCode();
                    qrEncoder.TryEncode(txtInfo.Text.Trim(), out qrCode);

                    GraphicsRenderer renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), Brushes.Black, Brushes.White);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
                        var tmpImage = new Bitmap(ms);
                        var image = new Bitmap(tmpImage, new Size(new Point(200, 200)));
                        QRImageBox.BackgroundImage = image;
                        btnSave.Enabled = true;
                        btnClean.Enabled = true;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(this, $"Se presentó un error al generar el código {ex.InnerException}", "Error al generar", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (Image image = (Image)QRImageBox.BackgroundImage.Clone())
                {
                    saveFileDialog1.AddExtension = true;
                    saveFileDialog1.Filter = "Image PNG (*.png)|*.png";
                    saveFileDialog1.ShowDialog();
                    if (!string.IsNullOrWhiteSpace(saveFileDialog1.FileName))
                        image.Save(saveFileDialog1.FileName, ImageFormat.Png);
                    else
                    {
                        MessageBox.Show("Debe seleccionar una ruta válidad para guardar la imagen");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,$"Se presentó un error al guardar la imagen {ex.InnerException}", "Error al guardar",MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Metodo para cargar y limpiar los controles al iniciar o al grabar
        /// </summary>
        void LoadAndClean()
        {
            btnSave.Enabled = false;
            btnClean.Enabled = false;
            txtInfo.Text = string.Empty;
            if (QRImageBox.BackgroundImage != null)
                QRImageBox.BackgroundImage = null;
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            LoadAndClean();
        }
    }
}
