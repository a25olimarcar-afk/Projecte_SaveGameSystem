using TMPro;
using UnityEngine;

public class Ball : PersistableObject<BallSaveData>
{
    [Header("Ball State")]
    [SerializeField] private int bounces = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text textBounces;

    protected override int DefaultSaveId => 3;

    private void Start()
    {
        RefreshBouncesUI();
    }

    // Cada colisión incrementa el contador.
    private void OnCollisionEnter(Collision collision)
    {
        bounces++;
        RefreshBouncesUI();
        NotifyStateChanged();
    }

    private void RefreshBouncesUI()
    {
        if (textBounces != null)
        {
            textBounces.text = $"Bounces: {bounces}";
        }
    }

    // IPersistable: contador de colisiones.
    public override void CaptureState(ref BallSaveData destination)
    {
        destination.saveId = SaveId;
        destination.bounces = bounces;
    }

    public override void RestoreState(in BallSaveData state)
    {
        bounces = state.bounces;
        RefreshBouncesUI();
    }

    public override void CaptureTo(BattleSaveSnapshot destination)
    {
        BallSaveData data = default;
        CaptureState(ref data);

        destination.ball = data;
        destination.hasBall = true;
    }

    public override bool RestoreFrom(BattleSaveSnapshot source)
    {
        if (!source.hasBall || source.ball.saveId != SaveId)
        {
            return false;
        }

        RestoreState(in source.ball);
        return true;
    }
}
