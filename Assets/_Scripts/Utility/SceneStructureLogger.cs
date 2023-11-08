#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SceneStructureLogger
{
    [MenuItem("Tools/Log Scene Structure")]
    private static void LogSceneStructure()
    {
        string sceneStructure = GetSceneStructure();
        Debug.Log(sceneStructure);
    }

    private static string GetSceneStructure()
    {
        string structure = "Scene Structure:\n";
        foreach (GameObject rootObject in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            structure += GetObjectStructure(rootObject.transform, "- ");
        }
        return structure;
    }

    private static string GetObjectStructure(Transform transform, string indentation)
    {
        string structure = indentation + transform.name + "\n";
        foreach (Transform child in transform)
        {
            structure += GetObjectStructure(child, indentation + "- ");
        }
        return structure;
    }
}
#endif