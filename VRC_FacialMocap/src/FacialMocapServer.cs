using System.Net;
using System.Net.Sockets;
using System.Text;
using FacialMocapTrackingModule;

namespace VRCFacialMocap
{
    class FacialMocapServer
    {
        private int _port = 49983; //port
        private FacialMocapData _trackedData = new();
        private UdpClient? _udpServer;
        public FacialMocapData FaceData { get {return _trackedData;}}

        /// <summary>
        /// Connects to the Facial Mockup server socket.
        /// </summary>
        /// <param name="ipaddress"></param>
        public void Connect(string ipaddress)
        {
            _udpServer = new(_port);
            UdpClient client = new();
            try
            {
                
                IPEndPoint dstAddr = new(IPAddress.Parse(ipaddress), _port);
                string data = "iFacialMocap_sahuasouryya9218sauhuiayeta91555dy3719|sendDataVersion=v2";
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                client.Send(bytes, bytes.Length, dstAddr);
                client.Close();

                _udpServer.Connect("localhost", _port);

            }
            catch (Exception e)
            {
                Console.WriteLine($"An exception has been caught while connecting to {ipaddress}:{_port} : {e}");
                _udpServer.Close();
                client.Close();
            }

        }

        /// <summary>
        /// Reads and parses the data recieved by the UDP Client, 
        /// storing the facial data result in the Face Data attributes.
        /// </summary>
        void ReadData()
        {
            if (_udpServer != null)
            {
                IPEndPoint RemoteIpEndPoint = new(IPAddress.Any, 0);
                byte[] receiveBytes = _udpServer.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                string[] blendData = returnData.Split('|');
                var props = typeof(FacialMocapData).GetFields();
                if (props.Length == blendData.Length)
                {
                    int i = 0;
                    while (i < props.Length - 3 && !blendData[i].Contains('=')) //While in the int attributes
                    {
                        string[] assignVal = blendData[i].Split(" & ");
                        try
                        {
                            if (assignVal[0] == props[i].Name)
                            {
                                props[i].SetValue(_trackedData, int.Parse(assignVal[1]));
                            }
                            else
                            {
                                Console.WriteLine($"Error on setting {assignVal}.");
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Bad packet. [{e}]");
                            return;
                        }
                        i++;
                    }
                    while (i < props.Length) // While in float[] attributes
                    {
                        string[] assignVal = blendData[i].Split('#');
                        try
                        {
                            if (assignVal[0] == props[i].Name)
                            {
                                string[] unparsedValues = assignVal[1].Split(',');
                                float[] values = new float[unparsedValues.Length];
                                for (int j = 0; j < unparsedValues.Length; j++)
                                {
                                    values[j] = float.Parse(unparsedValues[j]);
                                }
                                props[i].SetValue(_trackedData, values);
                            }
                            else
                            {
                                Console.WriteLine($"Error on setting {assignVal}.");
                                return;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Bad packet. [{e}]");
                            return;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"The packet is incomplete. [{blendData.Length}/{props.Length}]");
                }

            }
            else
            {
                Console.WriteLine("UDPClient wasn't initialized.");
            }
        }
    }
}