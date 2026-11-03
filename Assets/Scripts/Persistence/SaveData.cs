using System;

// ==========================================
// ESTRUCTURAS DE DATOS DE CADA OBJETO
// ==========================================

[Serializable]
public struct PlayerSaveData
{
    public int saveId;
    public int currentHealth;
    public float positionX;
    public float positionZ;
}

[Serializable]
public struct BarrierSaveData
{
    public int saveId;
    public bool isOpen;
    public float positionY;
}

[Serializable]
public struct BallSaveData
{
    public int saveId;
    public int bounces;
}

// ==========================================
// SNAPSHOT GENERAL DE LA PARTIDA
// ==========================================
/// <summary>
/// Contiene una captura temporal de los datos de los objetos del ejemplo.
/// La snapshot se mantiene en memoria y se escribe en disco cuando se ordena guardar.
/// </summary>
[Serializable]
public class BattleSaveSnapshot
{
    public int version = 1;

    public bool hasPlayer;
    public PlayerSaveData player;

    public bool hasBarrier;
    public BarrierSaveData barrier;

    public bool hasBall;
    public BallSaveData ball;

    public void CopyFrom(BattleSaveSnapshot source)
    {
        if (source == null)
        {
            return;
        }

        version = source.version;

        hasPlayer = source.hasPlayer;
        player = source.player;

        hasBarrier = source.hasBarrier;
        barrier = source.barrier;

        hasBall = source.hasBall;
        ball = source.ball;
    }

    public void Clear()
    {
        version = 1;

        hasPlayer = false;
        player = default;

        hasBarrier = false;
        barrier = default;

        hasBall = false;
        ball = default;
    }
}
