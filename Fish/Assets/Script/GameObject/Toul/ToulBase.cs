using UnityEngine;
public enum ToulType
{
    Fist,
    Gun,
    Fish
}

public class ToulBase : MonoBehaviour
{
    protected ToulType toulType_;
    [SerializeField] protected GameObject rHand_;
    [SerializeField] protected GameObject lHand_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual bool Use() { return false; }
    public virtual void ToulReset() {}

    public ToulType GetToulType()
    {
        return toulType_;
    }

    public GameObject GetRHand() { return rHand_; }
    public GameObject GetLHand() { return lHand_; }
}

