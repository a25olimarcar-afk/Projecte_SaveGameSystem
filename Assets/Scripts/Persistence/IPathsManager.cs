public interface IPathsManager
{
    string GetSaveFolderPath(string basePath, string saveFolderName);
    string GetSaveFilePath(string basePath, string saveFolderName, string saveFileName);
    void AssertDirectory(string path);
    bool AssertFile(string path);
}
