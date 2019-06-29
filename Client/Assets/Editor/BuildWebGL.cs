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

        PHPClassWriter.WriteAll();
        BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);
    }
}