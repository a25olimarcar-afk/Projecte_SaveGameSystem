/// <summary>
/// Define el contrato para cualquier objeto capaz de capturar y restaurar su estado.
/// Cada objeto utiliza su propio struct para mantener los datos fuertemente tipados.
/// </summary>
public interface IPersistable<TState> where TState : struct
{
    int SaveId { get; }

    void CaptureState(ref TState destination);

    void RestoreState(in TState state);
}
