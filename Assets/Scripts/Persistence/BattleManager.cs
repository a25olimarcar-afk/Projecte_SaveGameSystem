using System;
using UnityEngine;

/// <summary>
/// Coordinador principal del guardado y la carga.
/// Recibe los avisos de los objetos y decide cuándo enviar la snapshot a disco.
/// </summary>
public class BattleManager : IDisposable
{
    private readonly PersistenceSystem _persistence;
    private readonly BattleSnapshotBuffer _snapshotBuffer;

    public BattleManager(PersistenceSystem persistence)
    {
        _persistence = persistence;
        _snapshotBuffer = new BattleSnapshotBuffer();
    }

    // Se ejecuta desde ServiceLocator antes de cargar la escena.
    public void PreloadSave()
    {
        BattleSaveSnapshot snapshot = _persistence.LoadBattle();

        if (snapshot != null)
        {
            _snapshotBuffer.PreloadSnapshot(snapshot);
        }
        else
        {
            Debug.Log("No previous save found. Starting a new example session.");
        }
    }

    public void RegisterObject(IPersistableObject persistable)
    {
        _snapshotBuffer.RegisterObject(persistable);
    }

    public void UnregisterObject(IPersistableObject persistable)
    {
        _snapshotBuffer.UnregisterObject(persistable);
    }

    // Los objetos llaman a este método cada vez que cambia un dato persistible.
    public void NotifyObjectChanged(IPersistableObject persistable)
    {
        _snapshotBuffer.NotifyObjectChanged(persistable);
    }

    // Se conserva este método para permitir capturas completas cuando sea necesario.
    public void CaptureTurnState()
    {
        _snapshotBuffer.CaptureAll();
    }

    public void SaveCurrentBattle()
    {
        if (_snapshotBuffer != null && _snapshotBuffer.Snapshot != null)
        {
            // Aseguramos una captura completa justo antes de escribir el archivo.
            _snapshotBuffer.CaptureAll();
            _persistence.SaveBattle(_snapshotBuffer.Snapshot);
        }
        else
        {
            Debug.LogWarning("Cannot save: Battle snapshot is null.");
        }
    }

    public void LoadBattle()
    {
        BattleSaveSnapshot snapshot = _persistence.LoadBattle();

        if (snapshot != null)
        {
            _snapshotBuffer.RestoreSnapshot(snapshot);
        }
        else
        {
            Debug.LogWarning("Cannot load: Save file is missing or invalid.");
        }
    }

    public void Dispose()
    {
        _snapshotBuffer.Clear();
    }
}
