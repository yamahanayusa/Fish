using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追従設定")]
    [Tooltip("追いかける対象（プレイヤー）")]
    [SerializeField] private Transform target;

    [Tooltip("対象からの距離と高さ（X, Y, Z）")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 5f, -10f);

    [Tooltip("カメラが追従する滑らかさ（小さいほど遅れてついてくる）")]
    [SerializeField] private float smoothSpeed = 5f;

    private void LateUpdate()
    {
        if (target == null) return;

        // 目標となるカメラの位置を計算
        Vector3 desiredPosition = target.position + offset;

        // 現在位置から目標位置へ滑らかに移動させる
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 常にプレイヤーの方を向かせたい場合は以下のコメントアウトを外す
        // transform.LookAt(target);
    }
}