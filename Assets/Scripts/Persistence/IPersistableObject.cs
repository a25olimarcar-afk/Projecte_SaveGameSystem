/// <summary>
/// Interfaz no genérica que permite al sistema central gestionar objetos
/// persistibles de diferentes tipos dentro de una misma colección.
/// </summary>
public interface IPersistableObject
{
    int SaveId { get; }

    // Copia los datos del objeto a la snapshot temporal.
    void CaptureTo(BattleSaveSnapshot destination);

    // Restaura los datos desde la snapshot. Devuelve false si no hay datos para este objeto.
    bool RestoreFrom(BattleSaveSnapshot source);
}
