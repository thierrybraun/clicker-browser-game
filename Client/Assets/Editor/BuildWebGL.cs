using UnityEditor;
class WebGLBuilder
{
    static void build()
    {
        string[] scenes = {
            "Assets/Scenes/Login.unity",
            "Assets/Scenes/City.unity"
        };

        string pathToDeploy = "build/";

        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);
    }
}