using iFacialMocapTrackingModule;

namespace VRCiFacialMocap
{
    class Program
    {
        static void Main(string[] args)
        {
            var myPropertyInfo = typeof(FacialMocapData).GetFields();
            Console.WriteLine("Properties of System.Type are:");
            for (int i = 0; i < myPropertyInfo.Length; i++)
            {
                Console.WriteLine(myPropertyInfo[i].ToString());
            }
        }
    }
}