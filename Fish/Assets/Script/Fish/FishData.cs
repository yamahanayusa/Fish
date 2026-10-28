using UnityEngine;

[CreateAssetMenu(fileName = "NewFishData", menuName = "Fishing/Fish Data")]
public class FishData : ScriptableObject
{
    [Header("基本情報")]
    [Tooltip("魚の名前")]
    public string fishName = "新しい魚";

    [Tooltip("見た目（スプライトまたはモデルPrefab）")]
    public Sprite fishIcon;

    [Header("基本ステータス")]
    [Tooltip("体力")]
    public float maxHealth = 100f;

    [Tooltip("ジャンプ力")]
    public float jumpPower = 5f;

    [Tooltip("移動方向（プレイヤーに向かう / 海に向かう）")]
    public MoveDirection defaultMoveDirection = MoveDirection.TowardsSea;

    [Tooltip("攻撃力（プレイヤーへのダメージや糸への負荷）")]
    public float attackPower = 10f;

    [Tooltip("売却額")]
    public int sellPrice = 100;
}