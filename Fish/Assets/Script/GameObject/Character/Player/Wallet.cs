using UnityEngine;

public class Wallet : MonoBehaviour
{
    [Header("Š‹à")]
    public int currentGold = 0;

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

    public int GetWallet()
    {
        return currentGold;
    }
}