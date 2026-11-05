using UnityEngine;
public enum ToulType
{
    Fist,
    Gun
}

public class ToulBase : MonoBehaviour
{
    protected ToulType toulType_;
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
}

