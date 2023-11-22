using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Supermercado_Chino
{
    public partial class FRM_Camaras : Form
    {
        public FRM_Camaras()
        {
            InitializeComponent();
            box = new List<ComboBox>() { comboBox1, comboBox2 };
            pictureBoxes = new() { pictureBox1, pictureBox2 };
            captureDevices = new() { captureDevice1, captureDevice2 };
        }
        List<ComboBox> box;
        FilterInfoCollection InfoCollection = new(FilterCategory.VideoInputDevice);
        VideoCaptureDevice captureDevice1 = new(), captureDevice2 = new();
        List<PictureBox> pictureBoxes;
        List<VideoCaptureDevice> captureDevices;
        private void FRM_Camaras_Load(object sender, EventArgs e)
        {
            
            if (InfoCollection.Count > 0)
            {
                foreach (var item in box)
                {
                    foreach (FilterInfo camara in InfoCollection)
                    {
                        item.Items.Add(camara.Name);
                    }
                }
            }
        }
        void CaptureDevice1_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox1.Image = (Bitmap)eventArgs.Frame.Clone();
        }
        void CaptureDevice2_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            pictureBox2.Image = (Bitmap)eventArgs.Frame.Clone();
        }

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            Capturar_Camara(1, comboBox1,CaptureDevice1_NewFrame);
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            Capturar_Camara(2, comboBox2, CaptureDevice2_NewFrame);
        }

        void Capturar_Camara(int captureDev, ComboBox cmbB, NewFrameEventHandler eventHandler)
        {
            if (cmbB.SelectedIndex != -1 && InfoCollection.Count > 0)
            {
                //if (captureDevices[captureDev - 1] != null && captureDevices[captureDev - 1].IsRunning) captureDevices[captureDev - 1].WaitForStop();
                if (captureDevices[captureDev - 1] == null)
                {
                    captureDevices[captureDev - 1] = new VideoCaptureDevice(InfoCollection[cmbB.SelectedIndex].MonikerString);
                    captureDevices[captureDev - 1].NewFrame += eventHandler;
                    captureDevices[captureDev - 1].Start();
                }
                //return;
            }
            else
            {
                if (captureDevices[captureDev - 1] != null)
                {
                    captureDevices[captureDev - 1].NewFrame -= eventHandler;
                    captureDevices[captureDev - 1].SignalToStop();
                    pictureBoxes[(captureDev - 1)].Image = null;
                    captureDevices[captureDev - 1] = null;
                }
            }
        }
    }
}
