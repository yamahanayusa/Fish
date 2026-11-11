using UnityEngine;

public class BoatWaterInteraction : MonoBehaviour
{
    public float waterLevel = 0f;      // 海の面の高さ
    public float floatStrength = 10f;  // 水面から押し戻される浮力の強さ
    public float waterDrag = 3f;       // 水中でのブレーキ

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // ボートの現在のY座標が、水面より下にあるか計算
        float depth = waterLevel - transform.position.y;

        if (depth > 0f)
        {
            // 水面より沈んでいる深さに応じて、上向きの力を加える
            Vector3 buoyantForce = Vector3.up * depth * floatStrength;
            rb.AddForce(buoyantForce, ForceMode.Acceleration);

            // 水中にいるときはスピードを適度に落として安定させる
            rb.linearVelocity *= (1f - waterDrag * Time.fixedDeltaTime);
        }
    }
}