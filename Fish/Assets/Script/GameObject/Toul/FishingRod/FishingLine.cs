using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class FishingLine : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private FishingRod fishingRod_;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;

        fishingRod_ = GetComponent<FishingRod>();
    }

    private void Update()
    {
        Transform rodTip = fishingRod_.GetRodTip();
        GameObject lure = fishingRod_.GetCurrentLure();

        if (rodTip != null && lure != null)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, rodTip.position);
            lineRenderer.SetPosition(1, lure.transform.position);
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}