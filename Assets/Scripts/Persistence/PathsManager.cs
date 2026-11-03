using System.IO;
using UnityEngine;

// Paths and Validators
public class PathsManager : IPathsManager
{
    // Paths
    public string GetSaveFolderPath(string basePath, string saveFolderName)
    {
        return Path.Combine(basePath, saveFolderName);
    }

    public string GetSaveFilePath(string basePath, string saveFolderName, string saveFileName)
    {
        return Path.Combine(GetSaveFolderPath(basePath, saveFolderName), saveFileName);
    }

    // Validators
    public void AssertDirectory(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log($"Directory created: {path}");
        }
    }

    public bool AssertFile(string path)
    {
        return File.Exists(path);
    }
}
