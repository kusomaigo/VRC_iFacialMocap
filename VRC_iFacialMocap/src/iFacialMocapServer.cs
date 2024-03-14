using System.Net;
using System.Net.Sockets;
using System.Text;

namespace iFacialMocapTrackingModule
{
    class iFacialMocapServer
    {
        private int _port = 49983; //port
        private FacialMocapData _trackedData = new();
        private UdpClient? _udpListener, _udpClient;
        public FacialMocapData FaceData { get { return _trackedData; } }

        /// <summary>
        /// Stops and disposes the clients
        /// </summary>
        public void Stop()
        {
            if (_udpClient != null) { _udpClient.Close(); _udpClient.Dispose(); }
            if (_udpListener != null) { _udpListener.Close(); _udpListener.Dispose(); }
        }

        /// <summary>
        /// Connects to the Facial Mockup server socket.
        /// </summary>
        /// <param name="ipaddress"></param>
        public void Connect(string ipaddress = "255.255.255.255")
        {
            _udpListener = new(_port);
            _udpClient = new();
            try
            {

                IPEndPoint dstAddr = new(IPAddress.Parse(ipaddress), _port);
                string data = "iFacialMocap_sahuasouryya9218sauhuiayeta91555dy3719|sendDataVersion=v2";
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                _udpClient.Send(bytes, bytes.Length, dstAddr);
                _udpClient.Close();

                _udpListener.Connect("", _port);
                _udpListener.Client.ReceiveTimeout = 1000;

            }
            catch (Exception e)
            {
                Console.WriteLine($"An exception has been caught while connecting to {ipaddress}:{_port} : {e}");
                _udpListener.Close();
                _udpClient.Close();
            }

        }

        /// <summary>
        /// Reads and parses the data recieved by the UDP Client, 
        /// storing the facial data result in the Face Data attributes.
        /// </summary>
        void ReadData()
        {
            if (_udpListener != null)
            {
                IPEndPoint RemoteIpEndPoint = new(IPAddress.Any, 0);
                byte[] receiveBytes = _udpListener.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                string[] blendData = returnData.Split('|')[1..^1];
                var props = typeof(FacialMocapData).GetFields();
                int i = 0;
                while (i < props.Length) //While in the int attributes
                {
                    HandleChange(blendData[i]);
                    i++;
                }

            }
            else
            {
                Console.WriteLine("UDPClient wasn't initialized.");
            }
        }

        void HandleChange(string blend)
        {
            if (blend.Contains('&'))
            {

                string[] assignVal = blend.Split('&');
                try
                {
                    _trackedData.blends[assignVal[0]] = int.Parse(assignVal[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Invalid assignation. [{e};;{blend}]");
                    return;
                }
            }
            else if (blend.Contains('#'))
            {
                string[] assignVal = blend.Split('#');
                try
                {
                    string[] unparsedValues = assignVal[1].Split(',');
                    float[] values = new float[unparsedValues.Length];
                    for (int j = 0; j < unparsedValues.Length; j++)
                    {
                        values[j] = float.Parse(unparsedValues[j]);
                    }
                    if (assignVal[0] == "=head")
                    {
                        _trackedData.head = values;
                    }else if (assignVal[0]=="rightEye")
                    {
                        _trackedData.rightEye = values;
                    }
                    else if (assignVal[0]=="leftEye")
                    {
                        _trackedData.leftEye = values;
                    }
                    else
                    {
                        Console.WriteLine($"Error on setting {assignVal}.");
                        return;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Invalid assignation. [{e};;{blend}]");
                    return;
                }

            }
            else
            {
                Console.WriteLine($"Data cropped.");
            }

        }
    }
}