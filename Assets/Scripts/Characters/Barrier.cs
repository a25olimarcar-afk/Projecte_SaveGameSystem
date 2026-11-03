using UnityEngine;

public class Barrier : PersistableObject<BarrierSaveData>
{
    [Header("Barrier State")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private float closedY = 2f;
    [SerializeField] private float openY = -2f;
    [SerializeField] private float movementSpeed = 2f;

    private float targetY = 2f;

    protected override int DefaultSaveId => 2;

    protected override void Awake()
    {
        // En una partida nueva, la barrera empieza cerrada en Y = 2.
        Vector3 position = transform.position;
        position.y = isOpen ? openY : closedY;
        transform.position = position;
        targetY = isOpen ? openY : closedY;

        // Si existe una partida guardada, la clase base restaurará su posición después.
        base.Awake();
    }

    private void Update()
    {
        Vector3 position = transform.position;
        float newY = Mathf.MoveTowards(position.y, targetY, movementSpeed * Time.deltaTime);

        if (!Mathf.Approximately(position.y, newY))
        {
            position.y = newY;
            transform.position = position;

            // Mientras se mueve, actualizamos también la snapshot temporal.
            NotifyStateChanged();
        }
    }

    public void Open()
    {
        isOpen = true;
        targetY = openY;
        NotifyStateChanged();
    }

    public void Close()
    {
        isOpen = false;
        targetY = closedY;
        NotifyStateChanged();
    }

    // IPersistable: estado abierto/cerrado y posición mundial Y.
    public override void CaptureState(ref BarrierSaveData destination)
    {
        destination.saveId = SaveId;
        destination.isOpen = isOpen;
        destination.positionY = transform.position.y;
    }

    public override void RestoreState(in BarrierSaveData state)
    {
        isOpen = state.isOpen;
        targetY = isOpen ? openY : closedY;

        Vector3 position = transform.position;
        position.y = state.positionY;
        transform.position = position;
    }

    public override void CaptureTo(BattleSaveSnapshot destination)
    {
        BarrierSaveData data = default;
        CaptureState(ref data);

        destination.barrier = data;
        destination.hasBarrier = true;
    }

    public override bool RestoreFrom(BattleSaveSnapshot source)
    {
        if (!source.hasBarrier || source.barrier.saveId != SaveId)
        {
            return false;
        }

        RestoreState(in source.barrier);
        return true;
    }
}
