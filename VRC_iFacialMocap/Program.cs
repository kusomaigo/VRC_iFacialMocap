using iFacialMocapTrackingModule;

namespace VRCiFacialMocap
{
    class Program
    {
        static void Main(string[] args)
        {
            iFacialMocapTrackingInterface testInterface = new();
            testInterface.Initialize(true,true);
            Thread.Sleep(1000);
            while(true){
                testInterface.Update();
                
            }
        }
    }
}