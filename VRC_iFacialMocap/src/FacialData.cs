using System.Collections.Generic;

namespace iFacialMocapTrackingModule
{
    //
    public class FacialMocapData{
        public Dictionary<string,int> blends = new();
        public float[] head = new float[6];
        public float[] rightEye = new float[3];
        public float[] leftEye = new float[3];
        
    }

    //mouthSmile_R;eyeLookOut_L;mouthUpperUp_L;eyeWide_R;mouthClose;mouthPucker;mouthRollLower;eyeBlink_R;eyeLookDown_L;cheekSquint_R;eyeBlink_L;tongueOut;jawRight;eyeLookIn_R;cheekSquint_L;mouthDimple_L;mouthPress_L;eyeSquint_L;mouthRight;mouthShrugLower;eyeLookUp_R;eyeLookOut_R;mouthPress_R;cheekPuff;jawForward;mouthLowerDown_L;mouthFrown_L;mouthShrugUpper;browOuterUp_L;browInnerUp;mouthDimple_R;browDown_R;mouthUpperUp_R;mouthRollUpper;mouthFunnel;mouthStretch_R;mouthFrown_R;eyeLookDown_R;jawOpen;jawLeft;browDown_L;mouthSmile_L;noseSneer_R;mouthLowerDown_R;noseSneer_L;eyeWide_L;mouthStretch_L;browOuterUp_R;eyeLookIn_L;eyeSquint_R;eyeLookUp_L;mouthLeft;
}

