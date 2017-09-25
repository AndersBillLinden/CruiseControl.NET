using System;
using System.Windows.Forms;
using service.testrunner.Util;

namespace service.testrunner
{
    public partial class Form1 : Form
    {
        private ServiceWrapper service = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (service == null)
                service = new ServiceWrapper();

            service.StartService();
            button1.Enabled = false;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            service.StopService();
            button1.Enabled = true;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            service.PauseService();
            button3.Enabled = false;
            button4.Enabled = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            service.ResumeService();
            button3.Enabled = true;
            button4.Enabled = false;
        }
    }
}
