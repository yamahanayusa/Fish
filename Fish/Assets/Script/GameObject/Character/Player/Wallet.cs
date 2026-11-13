using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance;

    [Header("Š‹à")]
    public int currentGold = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"Š‹à‚ª•Ï“®‚µ‚Ü‚µ‚½: +{amount}G (Œ»İ: {currentGold}G)");
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            Debug.Log($"‚¨‹à‚ğx•¥‚¢‚Ü‚µ‚½: -{amount}G (c‚è: {currentGold}G)");
            return true;
        }
        Debug.Log("‚¨‹à‚ª‘«‚è‚Ü‚¹‚ñI");
        return false;
    }
}