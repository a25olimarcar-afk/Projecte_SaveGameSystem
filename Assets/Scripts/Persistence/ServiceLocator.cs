using UnityEngine;

/// <summary>
/// Punto de acceso global a los servicios del sistema de guardado.
/// Se inicializa antes de cargar la primera escena.
/// </summary>
public static class ServiceLocator
{
    public static PersistenceSystem Persistence { get; private set; }
    public static BattleManager BattleManager { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        IPathsManager pathsManager = new PathsManager();
        Persistence = new PersistenceSystem(pathsManager, Application.persistentDataPath);
        BattleManager = new BattleManager(Persistence);

        // La lectura del archivo termina antes de que los objetos ejecuten su Awake.
        // Al registrarse, cada objeto aplicará los datos que correspondan.
        BattleManager.PreloadSave();

        Debug.Log("<color=green>[ServiceLocator]</color> Save system initialized.");
    }
}
