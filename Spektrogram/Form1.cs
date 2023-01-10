using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Spectrogram;
using FftSharp;
using NAudio;

namespace Spektrogram
{
    public partial class Form1 : Form
    {
        private ComboBox comboBox1; // Wysuwany panel z oknami
        private ComboBox comboBox2;
        private Button save_file;
        private double[] audio;
        private double[] windowed;
        private int sampleRate;
        int fftsize = 1024;
        int step = 1024;
        int intensity = 1;
        NumericUpDown intens;
        SpectrogramGenerator sg;
        private PictureBox spec; // PictureBox ze spektrogramem 
        Bitmap btmp; // Bitmapa wygenerowanego spektrogramu
        OpenFileDialog openFileDialog1;
        NumericUpDown nud;
        PictureBox vScale;
        (double[] audio, int sampleRate) ReadMono(string filePath, double multiplier = 16_000)
        {

            var afr = new NAudio.Wave.AudioFileReader(filePath);
            int sampleRate = afr.WaveFormat.SampleRate;
            int bytesPerSample = afr.WaveFormat.BitsPerSample / 8;
            int sampleCount = (int)(afr.Length / bytesPerSample);
            int channelCount = afr.WaveFormat.Channels;
            var audio = new List<double>(sampleCount);
            var buffer = new float[sampleRate * channelCount];
            int samplesRead = 0;
            while ((samplesRead = afr.Read(buffer, 0, buffer.Length)) > 0)
                audio.AddRange(buffer.Take(samplesRead).Select(x => x * multiplier));
            return (audio.ToArray(), sampleRate);
        }
        public Form1()
        {
            this.Size = new Size(950, 500);
            //l1 = new Label();
            //l1.Text = "Selected Item: ";
            //l1.Location = new Point(10, 40);
            //l1.Size = new Size(x, l1.Size.Height);
            //this.Controls.Add(l1);

            openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "WAV Files|*.WAV;";

            Button load_file = new Button();
            load_file.Size = new Size(100, 25);
            load_file.Location = new Point(10, 10);
            load_file.Click += new EventHandler(SelectButton_Click);
            load_file.Text = "Load File";
            this.Controls.Add(load_file);

            save_file = new Button();
            save_file.Size = new Size(100, 25);
            save_file.Location = new Point(10, 40);
            save_file.Click += new EventHandler(SelectButton2_Click);
            save_file.Text = "Save File";
            this.Controls.Add(save_file);

            comboBox1 = new ComboBox();
            comboBox1.Location = new Point(120, 40);
            comboBox1.Items.Add("Default");
            comboBox1.Items.Add("Hanning");
            comboBox1.Items.Add("Hamming");
            comboBox1.Items.Add("Bartlett");
            comboBox1.Items.Add("Blackman");
            comboBox1.Items.Add("Cosine");
            comboBox1.Items.Add("FlatTop");
            comboBox1.Items.Add("Kaiser");
            comboBox1.Items.Add("Tukey");
            comboBox1.Items.Add("Welch");
            comboBox1.SelectedItem = "Default";
            comboBox1.SelectedIndexChanged += new EventHandler(ComboBox1_SelectedIndexChanged);
            this.Controls.Add(comboBox1);
            Label l1 = new Label();
            l1.Text = "Select Window";
            l1.Location = new Point(120, 20);
            l1.Size = new Size(comboBox1.Width, comboBox1.Height);
            this.Controls.Add(l1);

            nud = new NumericUpDown();
            nud.Value = 50;
            nud.Maximum = 100;
            nud.Minimum = 1;
            nud.Location = new Point(250, 40);
            nud.Size = new Size(50, 50);
            nud.ValueChanged += new EventHandler(nud_ValueChanged);
            this.Controls.Add(nud);
            Label l2 = new Label();
            l2.Text = "Overlap";
            l2.Location = new Point(250, 20);
            l2.Size = new Size(50, l2.Height);
            this.Controls.Add(l2);

            intens = new NumericUpDown();
            intens.Value = 1;
            intens.Maximum = 100;
            intens.Minimum = 1;
            intens.Location = new Point(310, 40);
            intens.Size = new Size(50, 50);
            intens.ValueChanged += new EventHandler(intens_ValueChanged);
            this.Controls.Add(intens);
            Label l3 = new Label();
            l3.Text = "Intensity";
            l3.Location = new Point(310, 20);
            l3.Size = new Size(50, l3.Height);
            this.Controls.Add(l3);

            comboBox2 = new ComboBox();
            comboBox2.Location = new Point(370, 40);
            comboBox2.Items.Add("Narrow");
            comboBox2.Items.Add("Wide");
            comboBox2.SelectedItem = "Narrow";
            comboBox2.SelectedIndexChanged += new EventHandler(ComboBox2_SelectedIndexChanged);
            this.Controls.Add(comboBox2);
            Label l4 = new Label();
            l4.Text = "Type";
            l4.Location = new Point(370, 20);
            l4.Size = new Size(50, l4.Height);
            this.Controls.Add(l4);

            spec = new PictureBox();
            spec.Location = new Point(100, 100);
            spec.Size = new Size(700, 300);
            this.Controls.Add(spec);

            vScale = new PictureBox();
            vScale.Size = new Size(100, spec.Height);
            vScale.Location = new Point(0, 100);
            this.Controls.Add(vScale);

            save_file.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            intens.Enabled = false;
            nud.Enabled = false;

            InitializeComponent();
        }
        private void intens_ValueChanged(object sender, EventArgs e)
        {
            intensity = Convert.ToInt32(intens.Value);
            int steps = Convert.ToInt32(step * (1 - nud.Value / 100));
            if (steps >= fftsize)
            {
                steps = fftsize - 1;
            }
            sg = new SpectrogramGenerator(sampleRate, fftSize: fftsize, stepSize: steps);
            sg.Add(windowed);
            btmp = sg.GetBitmap(intensity: intensity);
            Bitmap btmp2 = new Bitmap(btmp, spec.Width, spec.Height);
            spec.Image = btmp2;
        }
        private void nud_ValueChanged(object sender, EventArgs e)
        {
            int steps = Convert.ToInt32(step * (1 - nud.Value / 100));
            if (steps >= fftsize)
            {
                steps = fftsize - 1;
            }
            sg = new SpectrogramGenerator(sampleRate, fftSize: fftsize, stepSize: steps);
            sg.Add(windowed);
            btmp = sg.GetBitmap(intensity: intensity);
            Bitmap btmp2 = new Bitmap(btmp, spec.Width, spec.Height);
            spec.Image = btmp2;
        }
        private void SelectButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                (audio, sampleRate) = ReadMono(openFileDialog1.FileName);
                MessageBox.Show(sampleRate.ToString());
                windowed = audio;
                int steps = Convert.ToInt32(step * (1 - nud.Value / 100));
                if (steps >= fftsize)
                {
                    steps = fftsize - 1;
                }
                sg = new SpectrogramGenerator(sampleRate, fftSize: fftsize, stepSize: steps);
                sg.Add(windowed);
                btmp = sg.GetBitmap(intensity: intensity);
                Bitmap btmp2 = new Bitmap(btmp, spec.Width, spec.Height);
                spec.Image = btmp2;
                generate_scale();
                save_file.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                intens.Enabled = true;
                nud.Enabled = true;
            }
        }
        private void SelectButton2_Click(object sender, EventArgs e)
        {
            btmp.Save("Spectrogram.png");
        }
        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            windowed = new double[1];
            Window window;
            switch (comboBox1.Text)
            {
                case "Default":
                    windowed = audio;
                    break;
                case "Hanning":
                    window = new FftSharp.Windows.Hanning();
                    windowed = window.Apply(audio);
                    break;
                case "Hamming":
                    window = new FftSharp.Windows.Hamming();
                    windowed = window.Apply(audio);
                    break;
                case "Bartlett":
                    window = new FftSharp.Windows.Bartlett();
                    windowed = window.Apply(audio);
                    break;
                case "Blackman":
                    window = new FftSharp.Windows.Blackman();
                    windowed = window.Apply(audio);
                    break;
                case "Cosine":
                    window = new FftSharp.Windows.Cosine();
                    windowed = window.Apply(audio);
                    break;
                case "FlatTop":
                    window = new FftSharp.Windows.FlatTop();
                    windowed = window.Apply(audio);
                    break;
                case "Kaiser":
                    window = new FftSharp.Windows.Kaiser();
                    windowed = window.Apply(audio);
                    break;
                case "Tukey":
                    window = new FftSharp.Windows.Tukey();
                    windowed = window.Apply(audio);
                    break;
                case "Welch":
                    window = new FftSharp.Windows.Welch();
                    windowed = window.Apply(audio);
                    break;
            }
            int steps = Convert.ToInt32(step * (1 - nud.Value / 100));
            if (steps >= fftsize)
            {
                steps = fftsize - 1;
            }
            sg = new SpectrogramGenerator(sampleRate, fftSize: fftsize, stepSize: steps);
            sg.Add(windowed);
            btmp = sg.GetBitmap(intensity: intensity);
            Bitmap btmp2 = new Bitmap(btmp, spec.Width, spec.Height);
            spec.Image = btmp2;
        }
        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.Text == "Narrow")
                fftsize = 1024;
            else
                fftsize = 256;
            step = fftsize;
            int steps = Convert.ToInt32(step * (1 - nud.Value / 100));
            if (steps >= fftsize)
            {
                steps = fftsize - 1;
            }
            sg = new SpectrogramGenerator(sampleRate, fftSize: fftsize, stepSize: steps);
            sg.Add(windowed);
            btmp = sg.GetBitmap(intensity: intensity);
            Bitmap btmp2 = new Bitmap(btmp, spec.Width, spec.Height);
            spec.Image = btmp2;
        }
        void generate_scale()
        {
            var temp = new SpectrogramGenerator(sampleRate, fftSize: 1024, stepSize: 1024);
            Bitmap sscale = temp.GetVerticalScale(vScale.Width);
            sscale.Save("scale.png");
            sscale = new Bitmap(sscale, vScale.Width, vScale.Height);
            vScale.Image = sscale;
        }
    }
}
