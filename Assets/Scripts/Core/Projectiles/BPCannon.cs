using UnityEngine;
using System.Collections;

public class BPCannon : BaseProjectile {

    float m_projectileSpeed;
    float m_lifeTime;
    Collider2D m_collider;
    GameObject m_missileSeekingTarget;
    bool m_explosiveBehaviour = false;

	// Use this for initialization
	void Start () {
        m_collider = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.rotation * new Vector3(0, m_projectileSpeed * Time.deltaTime, 0); //add turns for 

        m_lifeTime -= Time.deltaTime;
        if(m_lifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("Collision");
        //Debug.Log(collision.gameObject);
        BaseShip ship = collider.GetComponent<BaseShip>();
        if (ship.gameObject != m_originObject)
        {
            if (ship != null)
            {
                if (!m_explosiveBehaviour)
                {
                    ship.ApplyDamage(m_damageInformation);
                }
            }

            Destroy(this.gameObject);
        }

    }

    public void Initialise(DamageInformation a_damageInformation, IFFGROUP a_team, GameObject a_originObject, float a_projectileSpeed, Vector3 a_direction, float a_lifeTime = 100)
    {
        m_damageInformation = a_damageInformation;
        m_team = a_team;
        m_collider = GetComponent<Collider2D>();
        m_originObject = a_originObject;
        m_projectileSpeed = a_projectileSpeed;
        transform.up = a_direction;
        m_lifeTime = a_lifeTime;
    }
}
