using UnityEngine;

/// <summary>
/// Clase base de los objetos que pueden guardar su estado.
/// Mantiene la identidad y conecta los cambios del objeto con el sistema central.
/// </summary>
public abstract class PersistableObject<TState> : MonoBehaviour, IPersistable<TState>, IPersistableObject
    where TState : struct
{
    [Header("Save Settings")]
    [SerializeField] private int saveId;

    // Cada objeto define un ID por defecto para evitar colisiones si no se configura en el Inspector.
    protected abstract int DefaultSaveId { get; }

    public int SaveId => saveId;

    protected virtual void Awake()
    {
        if (saveId <= 0)
        {
            saveId = DefaultSaveId;
        }

        // Los objetos se registran después de que ServiceLocator haya preparado la snapshot.
        if (ServiceLocator.BattleManager != null)
        {
            ServiceLocator.BattleManager.RegisterObject(this);
        }
    }

    protected virtual void OnDestroy()
    {
        if (ServiceLocator.BattleManager != null)
        {
            ServiceLocator.BattleManager.UnregisterObject(this);
        }
    }

    // Las clases hijas definen cómo capturar y restaurar sus propios datos.
    public abstract void CaptureState(ref TState destination);
    public abstract void RestoreState(in TState state);
    public abstract void CaptureTo(BattleSaveSnapshot destination);
    public abstract bool RestoreFrom(BattleSaveSnapshot source);

    // Llamar a este método siempre que cambie un dato que deba guardarse.
    protected void NotifyStateChanged()
    {
        if (ServiceLocator.BattleManager != null)
        {
            ServiceLocator.BattleManager.NotifyObjectChanged(this);
        }
    }
}
