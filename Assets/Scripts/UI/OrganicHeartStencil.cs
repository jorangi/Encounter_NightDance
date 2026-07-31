using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OrganicHeartSweep : MonoBehaviour
{
    [SerializeField] private RectTransform heartRectTransform;
    [SerializeField] private Image heartImage;

    [Range(0f, 1f)] public float currentHpRatio = 1.0f; // 체력 비율

    private Material heartMaterial;
    private Sequence beatSequence;
    private float lastBeatTime = 0f;

    private static readonly int ProgressID = Shader.PropertyToID("_Progress");

    void Start()
    {
        if (heartImage != null)
        {
            heartMaterial = heartImage.material;
        }
    }


    private void TriggerBeat()
{
    beatSequence?.Kill();
    heartRectTransform.localScale = Vector3.one;

    float duration = Mathf.Lerp(0.1f, 0.35f, currentHpRatio)*3;

    float punchStrength = Mathf.Lerp(0.3f, 0.08f, currentHpRatio);

    beatSequence = DOTween.Sequence();

    beatSequence.Join(
        heartRectTransform.DOPunchScale(new Vector3(punchStrength, punchStrength, 0), duration, 5, 0.5f)
    );

    heartMaterial.SetFloat(ProgressID, -0.3f);
    beatSequence.Join(
        heartMaterial.DOFloat(1.3f, ProgressID, duration)
            .SetEase(Ease.OutQuad) // 빠른 연출을 위해 OutQuad 사용
    );
}

void Update()
{
    if (heartMaterial == null) return;
    float beatInterval = Mathf.Lerp(0.2f, 1.0f, currentHpRatio);

    if (Time.time - lastBeatTime >= beatInterval)
    {
        TriggerBeat();
        lastBeatTime = Time.time;
    }
}

    private void OnDestroy()
    {
        beatSequence?.Kill();
    }
}