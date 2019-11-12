using Windows.Foundation.Metadata;

//This project type is RuntimeComponent(Universal Windows)
//This project type is RuntimeComponent(Universal Windows)
//This project type is RuntimeComponent(Universal Windows)
namespace AllowForWebRuntimeComponent
{
    //this class must be sealed, and must have attribuite [Windows.Foundation.Metadata.AllowForWeb]
    [AllowForWeb]
    public sealed class AllowFromWebExample
    {
        public int GetPlusResult(int param1, int param2)
        {
            return param1 + param2;
        }
    }
}
