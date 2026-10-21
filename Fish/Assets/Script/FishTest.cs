using UnityEngine;

public class FishTest : MonoBehaviour
{
    private Fish fish;

    [Header("自動ジャンプ設定")]
    [SerializeField] private float minJumpInterval = 1.0f;
    [SerializeField] private float maxJumpInterval = 2.5f;

    private float timer = 0f;
    private float nextJumpTime = 0f;

    private void Awake()
    {
        fish = GetComponent<Fish>();
    }

    private void Start()
    {
        SetNextJumpTime();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextJumpTime)
        {
            if (fish != null)
            {
                // 中心から外側へ向けて跳ねる関数を呼ぶ
                fish.JumpAwayFromCenter();
            }

            timer = 0f;
            SetNextJumpTime();
        }
    }

    private void SetNextJumpTime()
    {
        nextJumpTime = Random.Range(minJumpInterval, maxJumpInterval);
    }
}