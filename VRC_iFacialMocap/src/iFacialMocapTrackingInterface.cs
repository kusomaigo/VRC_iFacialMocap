using VRCFaceTracking;
using VRCFaceTracking.Core.Library;
using VRCFaceTracking.Core.Params.Expressions;

public class iFacialMocapTrackingInterface : ExtTrackingModule
{
    // What your interface is able to send as tracking data.
    public override (bool SupportsEye, bool SupportsExpression) Supported => (true, true);

    // This is the first function ran by VRCFaceTracking. Make sure to completely initialize 
    // your tracking interface or the data to be accepted by VRCFaceTracking here. This will let 
    // VRCFaceTracking know what data is available to be sent from your tracking interface at initialization.
    public override (bool eyeSuccess, bool expressionSuccess) Initialize(bool eyeAvailable, bool expressionAvailable)
    {
        var state = (eyeAvailable, expressionAvailable);

        ModuleInformation.Name = "iFacialMocap";

        // Example of an embedded image stream being referenced as a stream
        var stream = 
            GetType()
            .Assembly
            .GetManifestResourceStream("VRC_iFacialMocap.Assets.Assets.logo.png");

        // Setting the stream to be referenced by VRCFaceTracking.
        ModuleInformation.StaticImages = 
            stream != null ? new List<Stream> { stream } : ModuleInformation.StaticImages;

        //... Initializing module. Modify state tuple as needed (or use bool contexts to determine what should be initialized).

        return state;
    }

    // Polls data from the tracking interface.
    // VRCFaceTracking will run this function in a separate thread;
    public override void Update()
    {
        // Get latest tracking data from interface and transform to VRCFaceTracking data.

        if (Status == ModuleState.Active) // Module Status validation
        {
            // ... Execute update cycle.
            //UnifiedTracking.Data.Eye.Left.Openness = ExampleTracker.LeftEye.Openness;
            //UnifiedTracking.Data.Shapes[(int)UnifiedExpressions.JawOpen] = ExampleTracker.Mouth.JawOpen;
        }

        // Add a delay or halt for the next update cycle for performance. eg: 
        Thread.Sleep(10);
    }

    // Called when the module is unloaded or VRCFaceTracking itself tears down.
    public override void Teardown()
    {
        //... Deinitialize tracking interface; dispose any data created with the module.
    }
}