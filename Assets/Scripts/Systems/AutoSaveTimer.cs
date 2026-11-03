using System.Collections;
using UnityEngine;

// Temporizador de autoguardado. El guardado real siempre lo coordina BattleManager.
public class AutoSaveTimer : MonoBehaviour
{
    [Header("Auto Save")]
    [SerializeField] private float saveInterval = 60f; //guarda cada 1 minuto

    private Coroutine saveCoroutine;
    private WaitForSecondsRealtime waitObject;

    private void Awake()
    {
        // Cacheamos el objeto de espera para reutilizarlo en cada ciclo.
        waitObject = new WaitForSecondsRealtime(saveInterval);
    }

    private void OnEnable()
    {
        if (waitObject == null)
        {
            waitObject = new WaitForSecondsRealtime(saveInterval);
        }
        else
        {
            waitObject.Reset();
        }

        saveCoroutine = StartCoroutine(AutoSaveRoutine());
    }

    private void OnDisable()
    {
        if (saveCoroutine != null)
        {
            StopCoroutine(saveCoroutine);
            saveCoroutine = null;
        }
    }

    private IEnumerator AutoSaveRoutine()
    {
        while (true)
        {
            yield return waitObject;
            SaveAutomatically();
        }
    }

    private void SaveAutomatically()
    {
        if (ServiceLocator.BattleManager != null)
        {
            ServiceLocator.BattleManager.SaveCurrentBattle();
            Debug.Log("Automatic save completed.");
        }
    }

    // Reinicia el contador después de un guardado manual.
    public void NotifyManualSaved()
    {
        if (saveCoroutine != null)
        {
            StopCoroutine(saveCoroutine);
            saveCoroutine = null;
        }

        if (isActiveAndEnabled)
        {
            waitObject.waitTime = saveInterval;
            waitObject.Reset();
            saveCoroutine = StartCoroutine(AutoSaveRoutine());
        }
    }

    // Permite cambiar el intervalo desde otros scripts. El valor recibido está en minutos.
    public void ChangeSaveInterval(float newMinutes)
    {
        saveInterval = Mathf.Max(0.1f, newMinutes * 60f);

        if (waitObject == null)
        {
            waitObject = new WaitForSecondsRealtime(saveInterval);
        }
        else
        {
            waitObject.waitTime = saveInterval;
        }

        NotifyManualSaved();
    }
}
