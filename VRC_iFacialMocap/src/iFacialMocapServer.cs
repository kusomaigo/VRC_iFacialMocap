using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;


namespace iFacialMocapTrackingModule
{
    class iFacialMocapServer
    {
        static private int _port = 49983; //port
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
            FaceData.blends.Clear();
        }

        /// <summary>
        /// Connects to the Facial Mockup server socket.
        /// </summary>
        /// <param name="ipaddress"></param>
        public void Connect(ref ILogger logger, string ipaddress = "127.0.0.1")
        {
            _udpListener = new(_port);
            _udpClient = new();
            try
            {

                IPEndPoint dstAddr = new(IPAddress.Parse(ipaddress), _port);
                string data = "iFacialMocap_sahuasouryya9218sauhuiayeta91555dy3719|sendDataVersion=v2";
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                _udpClient.Send(bytes, bytes.Length, dstAddr);

                _udpListener.Client.ReceiveTimeout = 1000;
                logger.LogInformation($"Connecting to {ipaddress}:{_port}");
            }
            catch (Exception e)
            {
                logger.LogError($"Unable to establish connection. {e}");
                _udpListener.Close();
                _udpClient.Close();
            }

        }

        /// <summary>
        /// Reads and parses the data recieved by the UDP Client, 
        /// storing the facial data result in the Face Data attributes.
        /// </summary>
        public void ReadData(ref ILogger logger)
        {
            if (_udpListener != null)
            {
                IPEndPoint? RemoteIpEndPoint = null;
                byte[] receiveBytes = _udpListener.Receive(ref RemoteIpEndPoint);
                string returnData = Encoding.ASCII.GetString(receiveBytes);
                string[] blendData = returnData.Split('|')[1..^1];
                int i = 0;
                while (i < blendData.Length) //While in the int attributes
                {
                    HandleChange(blendData[i], ref logger);
                    i++;
                }

            }
            else
            {
                logger.LogError("UDPClient wasn't initialized.");
            }
        }

        /*public void Test(string data)
        {

            string[] blendData = data.Split('|')[1..^1];
            int i = 0;
            while (i < blendData.Length) //While in the int attributes
            {
                HandleChange(blendData[i]);
                i++;
            }
        }*/
        /// <summary>
        /// Changes the facial data depending of the assignation recieved.
        /// </summary>
        /// <param name="blend"></param>
        void HandleChange(string blend, ref ILogger logger)
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
                    logger.LogWarning($"Invalid assignation. [{e};;{blend}]");
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
                        values[j] = float.Parse(unparsedValues[j],CultureInfo.InvariantCulture.NumberFormat);
                    }
                    if (assignVal[0] == "=head")
                    {
                        if(values.Length == 6)
                            _trackedData.head = values;
                        else
                            logger.LogWarning("Insuficient data to assign head's position");
                    }else if (assignVal[0]=="rightEye")
                    {
                        if(values.Length == 3)
                            _trackedData.rightEye = values;
                        else
                            logger.LogWarning("Insuficient data to assign right eye's position");
                    }
                    else if (assignVal[0]=="leftEye")
                    {
                        if(values.Length == 3)
                            _trackedData.leftEye = values;
                        else
                            logger.LogWarning("Insuficient data to assign left eye's position");
                    }
                    else
                    {
                        logger.LogWarning($"Error on setting {assignVal}.");
                        return;
                    }
                }
                catch (Exception e)
                {
                    logger.LogWarning($"Invalid assignation. [{e};;{blend}]");
                    return;
                }

            }
            else
            {
                logger.LogWarning($"Data cropped.");
            }

        }
    }
}