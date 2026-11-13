using UnityEngine;

public class SellableItem : MonoBehaviour
{
    // 価格の決定方法を分けるためのモード
    public enum PriceType
    {
        Fixed, // 武器など
        FishData // 魚
    }

    [Header("売却設定")]
    [SerializeField] private PriceType priceType = PriceType.Fixed;

    [Header("【固定価格の場合】")]
    [SerializeField] private int fixedPrice = 10;

    [Header("【魚データの場合】")]
    [SerializeField] private FishData fishData;

    /// <summary>
    /// アイテムに応じた売値を返す
    /// </summary>
    public int GetSellPrice()
    {
        switch (priceType)
        {
            case PriceType.Fixed:
                return fixedPrice;

            case PriceType.FishData:
                if (fishData != null)
                {
                    return fishData.sellPrice;
                }
                return fixedPrice; // データがない場合の保険

            default:
                return 10;
        }
    }

    /// <summary>
    /// 外部から魚データなどをセットするための初期化用
    /// </summary>
    public void InitializeFish(FishData data)
    {
        priceType = PriceType.FishData;
        fishData = data;
    }

    /// <summary>
    /// 売却されたときの処理
    /// </summary>
    public void Sell()
    {
        int price = GetSellPrice();
        string itemName = fishData != null ? fishData.fishName : gameObject.name;

        // Walletを使ってお金を増やす
        if (Wallet.Instance != null)
        {
            Wallet.Instance.AddGold(price);
        }
        else
        {
            Debug.LogWarning("シーン内にWalletが見つかりませんでした！");
        }

        Debug.Log($"【売却】{itemName} を {price}G で売却しました！");

        // オブジェクトを消去
        Destroy(gameObject);
    }
}