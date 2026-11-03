using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : PersistableObject<PlayerSaveData>
{
    [Header("Player Stats")]
    [SerializeField] private int vida = 100;
    [SerializeField] private float moveSpeed = 5f;

    [Header("UI")]
    [SerializeField] private Slider healthSlider;

    protected override int DefaultSaveId => 1;

    private void Start()
    {
        RefreshHealthUI();
    }

    // Movimiento sencillo con WASD. Solo se procesa una dirección cada vez.
    private void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        Vector3 direction = Vector3.zero;

        if (Keyboard.current.wKey.isPressed)
        {
            direction = Vector3.forward;
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            direction = Vector3.back;
        }
        else if (Keyboard.current.aKey.isPressed)
        {
            direction = Vector3.left;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            direction = Vector3.right;
        }

        if (direction != Vector3.zero)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
            NotifyStateChanged();
        }
    }

    public void TakeDamage()
    {
        SetHealth(vida - 20);
    }

    public void Heal()
    {
        SetHealth(vida + 20);
    }

    private void SetHealth(int newHealth)
    {
        vida = Mathf.Clamp(newHealth, 0, 100);
        RefreshHealthUI();
        NotifyStateChanged();
    }

    private void RefreshHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0;
            healthSlider.maxValue = 100;
            healthSlider.value = vida;
        }
    }

    // IPersistable: salud y posición mundial X/Z.
    public override void CaptureState(ref PlayerSaveData destination)
    {
        destination.saveId = SaveId;
        destination.currentHealth = vida;
        destination.positionX = transform.position.x;
        destination.positionZ = transform.position.z;
    }

    public override void RestoreState(in PlayerSaveData state)
    {
        vida = Mathf.Clamp(state.currentHealth, 0, 100);

        Vector3 position = transform.position;
        position.x = state.positionX;
        position.z = state.positionZ;
        transform.position = position;

        RefreshHealthUI();
    }

    public override void CaptureTo(BattleSaveSnapshot destination)
    {
        PlayerSaveData data = default;
        CaptureState(ref data);

        destination.player = data;
        destination.hasPlayer = true;
    }

    public override bool RestoreFrom(BattleSaveSnapshot source)
    {
        if (!source.hasPlayer || source.player.saveId != SaveId)
        {
            return false;
        }

        RestoreState(in source.player);
        return true;
    }
}
