using UnityEngine;

public class BoatBobbing : MonoBehaviour
{
    public float bobbingSpeed = 2f;   // 揺れる速さ
    public float bobbingHeight = 0.2f; // 揺れる高さ
    public float tiltAmount = 2f;     // 波で少し傾く角度

    private Vector3 startPosition;

    void Start()
    {
        // 初期位置を記憶しておく
        startPosition = transform.localPosition;
    }

    void Update()
    {
        // 時間経過でサイン波を使って滑らかな上下移動を計算
        float time = Time.time * bobbingSpeed;
        float newY = startPosition.y + Mathf.Sin(time) * bobbingHeight;

        // わずかに前後に傾く動き
        float tiltX = Mathf.Sin(time * 0.7f) * tiltAmount;
        float tiltZ = Mathf.Cos(time * 0.5f) * tiltAmount;

        // 位置と回転を適用
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
        transform.localRotation = Quaternion.Euler(tiltX, transform.localEulerAngles.y, tiltZ);
    }
}