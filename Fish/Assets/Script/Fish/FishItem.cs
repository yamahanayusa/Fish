using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class FishItem : ToulBase
{
    [Header("データソース")]
    [SerializeField] private FishData fishData;

    void Start()
    {
        toulType_ = ToulType.Fish;
    }

    public override bool Use()

    {
        return Eat();
    }

    public override void ToulReset() { Initialize(fishData); }

    private bool Eat()

    {

        return true;

    }

    /// <summary>/// アイテムの初期化/// </summary>
    private void Initialize(FishData data)
    {

        fishData = data;

    }

}