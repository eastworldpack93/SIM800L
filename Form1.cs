using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Timers;

namespace GSM_AT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
            panel2.Enabled = false;
            groupBox2.Enabled = false;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        public bool btn1_click = false;
        private void button1_Click(object sender, EventArgs e)
        {
            btn1_click = !btn1_click;
            if (btn1_click)
            {
                serialPort1.PortName = comboBox1.Text;
                try
                {
                    serialPort1.Open();
                }
                catch (Exception ex)
                {
                    btn1_click = false;
                    MessageBox.Show("Error 1: Serial port name is not selected\nor this port is not available!\n\n" + ex.ToString());
                    return;
                } 
                button1.Text = "CLOSE";
                comboBox1.Enabled = false;
                groupBox2.Enabled = true;
                timer1.Interval = 1000;
                timer1.Enabled = false;
                panel2.Enabled = true;
            }
            else
            {
                button1.Text = "OPEN";
                comboBox1.Enabled = true;
                serialPort1.Close();
                timer1.Enabled = false;
                groupBox2.Enabled = false;
                panel2.Enabled = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                textBox1.Text = "0,16384,32768,65535,1024, 1";
                checkBox3.Text = "OFF ECHO MODE";
            }
            else
            {
                textBox1.Text = "0,0,0,0,0,0";
                checkBox3.Text = "NORMAL ECHO";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text += "1";    
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox2.Text += "2";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBox2.Text += "3";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBox2.Text += "4";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBox2.Text += "5";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox2.Text += "6";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            textBox2.Text += "7";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            textBox2.Text += "8";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            textBox2.Text += "9";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            textBox2.Text += "*";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox2.Text += "#";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0)
            {
                textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1);
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string s = serialPort1.ReadLine();
            Invoke(new serial_received(addDataRchTB), s);
        }

        public delegate void serial_received(String str);
        public void addDataRchTB(String str)
        {
            richTextBox1.Text += str;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
            if (str[0] == '+' && str[1] == 'C' && str[2] == 'B' && str[3] == 'C') 
            {
                string batt = str.Substring(8, 3);
                if (batt[2] == ',')
                {
                    batt = batt.Substring(0, 2);
                }
                progressBar2.Value = Int32.Parse(batt);
            }   
            else if (str[0] == '+' && str[1] == 'C' && str[2] == 'S' && str[3] == 'Q')
            {
                string ant = str.Substring(6, 2);
                progressBar1.Value = Int32.Parse(ant);
            }
            if (str.CompareTo("RING") >= 0)
            {
                inCall = true;
            }
            if (str[0] == '+' && str[1] == 'C' && str[2] == 'L' && str[3] == 'I' && str[4] == 'P')
            {
                textBox2.Text = str.Substring(11, 9);
            }
   
        }

        private void serialPort1_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            MessageBox.Show("ERROR 2: Receive data from this port is incorrectly!");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("AT+ECHO=" + textBox1.Text + "\n");
            if (checkBox1.Checked)
            {
                serialPort1.WriteLine("ATS0=1\n");
            }
            else
            {
                serialPort1.WriteLine("ATS0=0\n");
            }
            serialPort1.WriteLine("AT&W\n");
        }

        public bool timer_tick = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer_tick = !timer_tick;
            if (timer_tick)
            {
                serialPort1.WriteLine("AT+CSQ");
            }
            else
            {
                serialPort1.WriteLine("AT+CBC");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        public DateTime dt;
        private void button10_MouseDown(object sender, MouseEventArgs e)
        {
            dt = DateTime.Now;
        }

        private void button10_MouseUp(object sender, MouseEventArgs e)
        {
            TimeSpan ts = DateTime.Now - dt;
            if (ts.TotalSeconds > 2)
            {
                textBox2.Text += "+";
            }
            else
            {
                textBox2.Text += "0";
            }
        }

        public bool inCall = false;
        private void button16_Click(object sender, EventArgs e)
        {
            if (!inCall)
            {
                serialPort1.WriteLine("ATD" + textBox2.Text + ";");
            }
            else
            {
                serialPort1.WriteLine("ATA");
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("ATH");
            inCall = false;
        }

        private void button18_Click(object sender, EventArgs e)
        {
            serialPort1.WriteLine("AT+CMGF=1");
            serialPort1.WriteLine("AT+CMGS=\"+998" + textBox2.Text + '"');
            serialPort1.WriteLine(richTextBox2.Text);
            serialPort1.WriteLine(((char)26).ToString());
        }

    }
}
