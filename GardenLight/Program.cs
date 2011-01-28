using System;
using System.Threading;
using System.Text;
using System.IO.Ports;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;


using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace GardenLight
{
    public class Program
    {

        public static SerialPort Radio = null;
        public static int read_count = 0;
        public static byte[] rx_data = new byte[256];
        public static byte[] tx_data;
        public static byte[] str = new byte[256];
        public static string input = ""; 
        public static int i = 0;

        static BlinkM light;
        static Random rand;

        public static void Main()
        {
            Radio = new SerialPort("COM1", 9600);
            Radio.Open();

            light = new BlinkM();
            rand = new Random();

            Radio.DataReceived += new SerialDataReceivedEventHandler(Radio_DataReceived);

            while (true)
            {
                Thread.Sleep(100);
            }
        }

        static void Radio_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

             read_count = Radio.Read(rx_data, 0, Radio.BytesToRead);
            try{
                    if (read_count > 0)
                    {
                        for (int k = 0; k < read_count; k++ )
                        {
                            if (rx_data[k] == '$')
                                i = 0;
                            else if (rx_data[k] == '*')
                            {
                                Radio.Flush();
                                for(int j = 0; j < i; j++)
                                {
                                    input += (char)str[j];
                                }
                                ParseCommand(input);
                                input = "";
                                i = 0;
                                read_count = 0;
                                
                            }
                            else
                            {
                                str[i] = rx_data[k];
                                i++;
                                if (i > str.Length) i = 0;
                            }
                        }
                    }
                }
                
                catch
                {
                }

        }

        private static void ParseCommand(string cmd)
        {

            string[] strings = cmd.Split(new char[] { ',' });
            if (strings.Length == 3)
            {
                light.Red = byte.Parse(strings[0]);
                light.Blue = byte.Parse(strings[1]);
                light.Green = byte.Parse(strings[2]);
            }
        }

    }
}
