using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mantiene una única snapshot temporal en memoria.
/// Los objetos registran aquí sus cambios; el volcado a disco se hace bajo petición.
/// </summary>
public class BattleSnapshotBuffer
{
    public BattleSaveSnapshot Snapshot { get; } = new BattleSaveSnapshot();

    private readonly Dictionary<int, IPersistableObject> _registeredObjects =
        new Dictionary<int, IPersistableObject>();

    public void RegisterObject(IPersistableObject persistable)
    {
        if (persistable == null)
        {
            return;
        }

        if (_registeredObjects.TryGetValue(persistable.SaveId, out IPersistableObject existingObject) &&
            !ReferenceEquals(existingObject, persistable))
        {
            Debug.LogWarning($"Duplicate SaveId detected: {persistable.SaveId}");
        }

        _registeredObjects[persistable.SaveId] = persistable;

        // Si hay datos guardados, se restauran al registrar el objeto.
        // Si no los hay, capturamos su estado inicial.
        if (!persistable.RestoreFrom(Snapshot))
        {
            CaptureObject(persistable);
        }
    }

    public void UnregisterObject(IPersistableObject persistable)
    {
        if (persistable == null)
        {
            return;
        }

        if (_registeredObjects.TryGetValue(persistable.SaveId, out IPersistableObject registeredObject) &&
            ReferenceEquals(registeredObject, persistable))
        {
            _registeredObjects.Remove(persistable.SaveId);
        }
    }

    // Recibe el aviso de un objeto cuando cambia una variable persistible.
    public void NotifyObjectChanged(IPersistableObject persistable)
    {
        if (persistable == null)
        {
            return;
        }

        if (_registeredObjects.TryGetValue(persistable.SaveId, out IPersistableObject registeredObject) &&
            ReferenceEquals(registeredObject, persistable))
        {
            CaptureObject(persistable);
        }
    }

    public void CaptureAll()
    {
        foreach (IPersistableObject persistable in _registeredObjects.Values)
        {
            CaptureObject(persistable);
        }
    }

    private void CaptureObject(IPersistableObject persistable)
    {
        persistable.CaptureTo(Snapshot);
    }

    // La snapshot cargada sustituye los datos temporales y actualiza los objetos registrados.
    public void RestoreSnapshot(BattleSaveSnapshot loadedSnapshot)
    {
        if (loadedSnapshot == null)
        {
            Debug.LogWarning("Cannot restore: Loaded snapshot is null.");
            return;
        }

        Snapshot.CopyFrom(loadedSnapshot);

        foreach (IPersistableObject persistable in _registeredObjects.Values)
        {
            if (!persistable.RestoreFrom(Snapshot))
            {
                CaptureObject(persistable);
            }
        }
    }

    // Se utiliza al arrancar, antes de que los objetos de la escena se registren.
    public void PreloadSnapshot(BattleSaveSnapshot loadedSnapshot)
    {
        if (loadedSnapshot != null)
        {
            Snapshot.CopyFrom(loadedSnapshot);
        }
    }

    public void Clear()
    {
        _registeredObjects.Clear();
        Snapshot.Clear();
    }
}
