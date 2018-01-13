using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuestViewer
{

    public partial class Form2 : Form
    {
        
        public delegate void GetValue(string name, string level, int type);
        public GetValue getValue;
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = null;
            string level = null;
            if (checkBox2.Checked)
                name = textBox2.Text;
            if (checkBox1.Checked)
                level = textBox2.Text;
            getValue(textBox2.Text, textBox1.Text, 0);
            this.Close();
        }
    }
}
