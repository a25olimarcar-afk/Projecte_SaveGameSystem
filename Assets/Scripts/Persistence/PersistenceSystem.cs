using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Encargado de la escritura y lectura del archivo definitivo en formato binario.
/// La snapshot temporal llega desde BattleManager.
/// </summary>
public class PersistenceSystem
{
    private readonly IPathsManager _pathsManager;
    private readonly string basePath;

    private const string SaveFolderName = "SaveSystemExample";
    private const string SaveFileName = "savegame.bin";
    private const int FileMagic = 0x53415645; // Identificador "SAVE".
    private const int CurrentVersion = 1;

    public PersistenceSystem(IPathsManager pathsManager, string basePath)
    {
        _pathsManager = pathsManager;
        this.basePath = basePath;
    }

    // --------------------------------------------------------------------
    // Save & Load Logic
    // --------------------------------------------------------------------
    public void SaveBattle(BattleSaveSnapshot snapshot)
    {
        if (snapshot == null)
        {
            Debug.LogWarning("Cannot save: Snapshot is null.");
            return;
        }

        string folderPath = _pathsManager.GetSaveFolderPath(basePath, SaveFolderName);
        string filePath = _pathsManager.GetSaveFilePath(basePath, SaveFolderName, SaveFileName);

        try
        {
            _pathsManager.AssertDirectory(folderPath);

            // BinaryWriter escribe los valores en un orden fijo.
            // LoadBattle debe leerlos exactamente en el mismo orden.
            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(FileMagic);
                writer.Write(CurrentVersion);

                writer.Write(snapshot.hasPlayer);
                if (snapshot.hasPlayer)
                {
                    writer.Write(snapshot.player.saveId);
                    writer.Write(snapshot.player.currentHealth);
                    writer.Write(snapshot.player.positionX);
                    writer.Write(snapshot.player.positionZ);
                }

                writer.Write(snapshot.hasBarrier);
                if (snapshot.hasBarrier)
                {
                    writer.Write(snapshot.barrier.saveId);
                    writer.Write(snapshot.barrier.isOpen);
                    writer.Write(snapshot.barrier.positionY);
                }

                writer.Write(snapshot.hasBall);
                if (snapshot.hasBall)
                {
                    writer.Write(snapshot.ball.saveId);
                    writer.Write(snapshot.ball.bounces);
                }
            }

            Debug.Log($"Binary Saved successfully: {filePath}");
        }
        catch (Exception exception)
        {
            Debug.LogError($"Save failed: {exception.Message}");
        }
    }

    public BattleSaveSnapshot LoadBattle()
    {
        string filePath = _pathsManager.GetSaveFilePath(basePath, SaveFolderName, SaveFileName);

        if (!_pathsManager.AssertFile(filePath))
        {
            return null;
        }

        try
        {
            BattleSaveSnapshot snapshot = new BattleSaveSnapshot();

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                int magic = reader.ReadInt32();
                if (magic != FileMagic)
                {
                    throw new InvalidDataException("The save file has an invalid format.");
                }

                int version = reader.ReadInt32();
                if (version != CurrentVersion)
                {
                    throw new InvalidDataException($"Unsupported save version: {version}");
                }

                snapshot.version = version;

                snapshot.hasPlayer = reader.ReadBoolean();
                if (snapshot.hasPlayer)
                {
                    snapshot.player.saveId = reader.ReadInt32();
                    snapshot.player.currentHealth = reader.ReadInt32();
                    snapshot.player.positionX = reader.ReadSingle();
                    snapshot.player.positionZ = reader.ReadSingle();
                }

                snapshot.hasBarrier = reader.ReadBoolean();
                if (snapshot.hasBarrier)
                {
                    snapshot.barrier.saveId = reader.ReadInt32();
                    snapshot.barrier.isOpen = reader.ReadBoolean();
                    snapshot.barrier.positionY = reader.ReadSingle();
                }

                snapshot.hasBall = reader.ReadBoolean();
                if (snapshot.hasBall)
                {
                    snapshot.ball.saveId = reader.ReadInt32();
                    snapshot.ball.bounces = reader.ReadInt32();
                }
            }

            Debug.Log($"Loaded successfully: {filePath}");
            return snapshot;
        }
        catch (Exception exception)
        {
            Debug.LogError($"Load failed: {exception.Message}");
            return null;
        }
    }
}
