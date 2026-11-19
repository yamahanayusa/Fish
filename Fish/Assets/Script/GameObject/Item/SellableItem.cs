using UnityEngine;

public class SellableItem : MonoBehaviour
{
    // 価格の決定方法を分けるためのモード
    public enum PriceType
    {
        enFixed, // 武器など
        enFish // 魚
    }

    [Header("売却設定")]
    [SerializeField] private PriceType priceType;

    [Header("【固定価格の場合】")]
    [SerializeField] private int fixedPrice;


    private float doneness = 1;

    private bool burnt = false;

    /// <summary>
    /// アイテムに応じた売値を返す
    /// </summary>
    public int GetSellPrice()
    {
        return fixedPrice;
    }

    public float GetDoneness()
    {
        return doneness;
    }

    public PriceType GetPriceType()
    {
        return priceType;
    }

    private void OnCollisionStay(Collision collision)

    {
        if (collision.gameObject.tag.Contains("Grill"))
        {
            if (burnt)
            {
                doneness -= 0.01f;
            }
            else
            {
                doneness += 0.01f;
                if (doneness > 1.5f)
                {
                    burnt = true;
                }
            }
        }
    }

    /// <summary>
    /// 外部から魚データなどをセットするための初期化用
    /// </summary>
    public void InitializeFish(FishData data)
    {
        Debug.Log(data.sellPrice);
        priceType = PriceType.enFish;
        fixedPrice = data.sellPrice;
    }

}