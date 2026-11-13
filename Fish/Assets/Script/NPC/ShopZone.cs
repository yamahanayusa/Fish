using UnityEngine;

public class ShopZone : MonoBehaviour
{
    private bool isPlayerInZone = false;
    private SellableItem targetItem; // エリア内にある売却可能なアイテム

    private void OnTriggerEnter(Collider other)
    {
        // エリアに入ってきたオブジェクトから SellableItem コンポーネントを探す
        SellableItem item = other.GetComponent<SellableItem>();

        // もし売却可能なアイテムだったら、即座に売却を実行！
        if (item != null)
        {
            string itemName = other.gameObject.name;
            Debug.Log($"【ショップ受付】 {itemName} を検知しました。売却処理を実行します。");

            // 売却処理を呼ぶ
            item.Sell();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // プレイヤーがエリアから出た場合
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            Debug.Log("ショップエリアから出ました。");
        }

        // アイテムがエリアから出た場合
        SellableItem item = other.GetComponent<SellableItem>();
        if (item != null && targetItem == item)
        {
            targetItem = null;
        }
    }

    private void Update()
    {
        // プレイヤーがエリア内にいて、売却対象のアイテムがあり、Qキーが押されたとき
        if (isPlayerInZone && Input.GetKeyDown(KeyCode.Q))
        {
            if (targetItem != null)
            {
                // 売却を実行！
                targetItem.Sell();
                targetItem = null; // 売ったら参照をリセット
            }
            else
            {
                Debug.Log("売却できるアイテムが近くにありません。");
            }
        }
    }
}