using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace SIM900
{
    public partial class Form1 : Form
    {
        private SerialPort port;
        public Form1()
        {
            InitializeComponent();
        }

        private void inputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13 || (e.Control && e.KeyCode == Keys.Z) )
            {
                outputBox.SelectionStart = outputBox.TextLength;
                outputBox.SelectionLength = 0;
                outputBox.SelectionColor = Color.Red;
                outputBox.AppendText(inputBox.Text + (e.KeyValue == 13 ? "" : "\x1A") + "\r");
                outputBox.SelectionColor = outputBox.ForeColor;

                SendCommand(inputBox.Text, (e.KeyValue == 13 ? 13 : 26) );

                inputBox.Text = "";
            }
        }

        private void DataReceivedHandler(
                        object sender,
                        SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            this.Invoke((MethodInvoker)delegate()
            {
                outputBox.SelectionStart = outputBox.TextLength;
                outputBox.SelectionLength = 0;
                outputBox.SelectionColor = Color.Black;
                outputBox.AppendText(data);
                outputBox.SelectionColor = outputBox.ForeColor;

                outputBox.SelectionStart = outputBox.TextLength;
                outputBox.ScrollToCaret();
            });
        }

        private void SendCommand(string command, int end=13)
        {
            string comm = command.Replace("\n", "");
            comm = comm.Replace("\r", "");

            port.Write(comm + (char) end);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblStatus.BackColor = Color.Red;
            lblStatus.Text = "COM3: Closed";

            port = new SerialPort("COM3");
            port.BaudRate = 9600;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            port.Open();

            if (port.IsOpen)
            {
                lblStatus.BackColor = Color.LawnGreen;
                lblStatus.Text = "COM3: Open";
            }
        }
    }
}
