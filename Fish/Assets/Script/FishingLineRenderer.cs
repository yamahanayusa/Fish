using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class FishingLineRenderer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
    }

    private void Update()
    {
        if (FishingManager.Instance == null) return;

        Transform rodTip = FishingManager.Instance.GetRodTip();
        GameObject lure = FishingManager.Instance.GetCurrentLure();

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