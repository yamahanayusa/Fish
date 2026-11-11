using UnityEngine;

public class Ammo : MonoBehaviour
{
    private float lifeCounter_ = 0; 
    private float lifeTime_ = 10;
    private int ammoDamege_ = 0;

    void Start()
    {
        
    }

    void Update()
    {
        if(lifeCounter_ > lifeTime_)
        {
            Destroy(gameObject);
        }

    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Contains("Fish_"))
        {
            Health hp = collision.gameObject.GetComponent<Health>();

            if (hp)
            {
                hp.AddDamege(ammoDamege_);
            }
        }

        Destroy(gameObject);
    }

    public void SetAmmoDamege(int damege)
    {
        ammoDamege_ = damege;
    }
}
