
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Puente entre los botones de la interfaz y los sistemas del juego.
/// Gestiona las referencias a los botones y registra sus listeners.
/// </summary>
public class SaveGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button buttonSave;
    [SerializeField] private Button buttonLoad;
    [SerializeField] private Button buttonDamage;
    [SerializeField] private Button buttonHeal;
    [SerializeField] private Button buttonOpen;
    [SerializeField] private Button buttonClose;

    [Header("References")]
    [SerializeField] private AutoSaveTimer autoSaveTimer;
    [SerializeField] private Player player;
    [SerializeField] private Barrier barrier;

    private void OnEnable()
    {
        // Save & Load
        if (buttonSave != null)
            buttonSave.onClick.AddListener(SaveGame);

        if (buttonLoad != null)
            buttonLoad.onClick.AddListener(LoadGame);

        // Player
        if (buttonDamage != null)
            buttonDamage.onClick.AddListener(DamagePlayer);

        if (buttonHeal != null)
            buttonHeal.onClick.AddListener(HealPlayer);

        // Barrier
        if (buttonOpen != null)
            buttonOpen.onClick.AddListener(OpenBarrier);

        if (buttonClose != null)
            buttonClose.onClick.AddListener(CloseBarrier);
    }

    private void OnDisable()
    {
        // Eliminamos los listeners para evitar suscripciones duplicadas.
        if (buttonSave != null)
            buttonSave.onClick.RemoveListener(SaveGame);

        if (buttonLoad != null)
            buttonLoad.onClick.RemoveListener(LoadGame);

        if (buttonDamage != null)
            buttonDamage.onClick.RemoveListener(DamagePlayer);

        if (buttonHeal != null)
            buttonHeal.onClick.RemoveListener(HealPlayer);

        if (buttonOpen != null)
            buttonOpen.onClick.RemoveListener(OpenBarrier);

        if (buttonClose != null)
            buttonClose.onClick.RemoveListener(CloseBarrier);
    }

    // --------------------------------------------------------------------
    // Save & Load
    // --------------------------------------------------------------------

    private void SaveGame()
    {
        if (ServiceLocator.BattleManager == null)
        {
            Debug.LogWarning("Save system is not initialized.");
            return;
        }

        ServiceLocator.BattleManager.SaveCurrentBattle();

        if (autoSaveTimer != null)
        {
            autoSaveTimer.NotifyManualSaved();
        }
    }

    private void LoadGame()
    {
        if (ServiceLocator.BattleManager == null)
        {
            Debug.LogWarning("Save system is not initialized.");
            return;
        }

        ServiceLocator.BattleManager.LoadBattle();
    }

    // --------------------------------------------------------------------
    // Player
    // --------------------------------------------------------------------

    private void DamagePlayer()
    {
        if (player != null)
            player.TakeDamage();
    }

    private void HealPlayer()
    {
        if (player != null)
            player.Heal();
    }

    // --------------------------------------------------------------------
    // Barrier
    // --------------------------------------------------------------------

    private void OpenBarrier()
    {
        if (barrier != null)
            barrier.Open();
    }

    private void CloseBarrier()
    {
        if (barrier != null)
            barrier.Close();
    }
}
