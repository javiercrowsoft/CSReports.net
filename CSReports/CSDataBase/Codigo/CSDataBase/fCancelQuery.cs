using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CSKernelClient;

namespace CSDataBase
{
    public partial class fCancelQuery : Form
    {
        private bool m_cancel = false;
        private Timer m_timer;
        private int m_minutes = 0;
        private int m_seconds = 0;

        public fCancelQuery()
        {
            InitializeComponent();

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            picIcon.Image = new Bitmap(assembly.GetManifestResourceStream(assembly.GetName().Name + ".Resources.Database.png"));
            m_timer = new Timer();
            m_timer.Tick += new EventHandler(timer_tick);
            m_timer.Interval = 1000;
            m_timer.Start();
        }

        public string descript 
        {
            set {
                lbTask.Text = value;
            }
        }

        public Boolean cancel 
        {
            get {
                return m_cancel;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_cancel = true;
            Hide();
        }

        private void timer_tick(object sender, EventArgs e) 
        {
            lbTime.Text = m_minutes.ToString("00") + ":" + m_seconds.ToString("00");
            m_seconds++;
            m_minutes = m_minutes + m_seconds / 60;
            m_seconds = m_seconds % 60;            
        }

        private void fCancelQuery_Load(object sender, EventArgs e)
        {
            cWindow.centerForm(this);
        }
    }
}
