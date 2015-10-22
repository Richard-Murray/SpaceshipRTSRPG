using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TurretCannon : BHTurret
{

    [Header("Cannon Attributes")]
    public float m_firingAngle; //The accuracy in degrees of each projectile. 0 is perfectly accurate
    public List<Vector3> m_projectileOffset; //Where the bullet comes out relative to the gun, if needed can create a second point and bullets can pick a point between those to fire from, like broadsides
                                             //by default there is only one, but each one will be fired through in sequence

    public float m_cooldown; //Cooldown between clicks
    public float m_frequency; //Number of shots after one 'click'
    public float m_firingTime; //If more than one shot, they will be spread out along the firing time

    [Header("Projectile Attributes")]
    public GameObject m_projectile;
    public float m_armourDamageMagnitude;
    public float m_energyDamageMagnitude;
    public float m_armourPierceModifier;
    public float m_energyPierceModifier;
    public float m_projectileSpeed; //Speed of the projectile

    [Header("Explosion Attributes")]
    public bool m_useExplosionBehaviour; //The explosion MUST overwrite projectile's standard damage application function so it is applied only once
    public float m_explosionRadius;

    [Header("Flak Attributes")]
    public bool m_useFlakBehaviour;
    public float m_flakTriggerAccuracy; //The percentage deviance for the bullet's lifetime

    [Header("Missile Attributes")]
    public bool m_useMissileBehaviour;
    public float m_maximumSeekingRotation;

    float m_firing; //The space between shots, calculated after runtime

    float m_cooldownTimer;
    float m_firingTimer;
    int m_projectileOffsetIndex; //used to spin through all possible locations projectiles can exit from
    int m_frequencyCount; //The current number of shots fired

    bool m_activated;

    // Use this for initialization
    public void Start()
    {
        base.Start();
        m_module = HARDPOINTMODULE.TURRET_CANNON;

        m_firing = m_firingTime / (m_frequency + 1); //This is probably screwed up, 4 poles 3 fences concept
        m_cooldownTimer = 0;
        m_frequencyCount = 0;
        m_projectileOffsetIndex = 0;
        m_activated = false;

        //debug
        m_objectToTarget = GameObject.Find("DebugTarget");
        m_locationTarget = m_objectToTarget.transform.position;
        m_headingTarget = m_objectToTarget.transform.position;
    }

    // Update is called once per frame
    public override void Update()
    {

    }

    public override void ShipUpdate()
    {
        base.ShipUpdate(); //One base should refer to BHturret

        if (m_objectToTarget != null)
        {
            m_headingTarget = m_objectToTarget.transform.position + (m_objectToTarget.transform.position - m_locationTarget) * 10;
            m_locationTarget = m_objectToTarget.transform.position;

            RotateToTarget();
        }

        FinishRotation();

        PrimaryAction(); //debug

        if(m_activated)
        {
            RunPrimaryAction();
        }
        
        m_cooldownTimer -= Time.deltaTime;
        m_firingTimer -= Time.deltaTime;
        if(m_cooldownTimer <= 0)
        {
            m_cooldownTimer = 0;
        }
    }

    public override void PrimaryAction()
    {
        if(m_cooldownTimer <= 0)
        {
            m_activated = true;
            m_cooldownTimer = m_cooldown;
            m_firingTimer = m_firing;
        }
    }

    public override void RunPrimaryAction()
    {
        if (m_firingTimer <= 0)
        {
            CreateProjectile();
            m_firingTimer += m_firing;
            m_frequencyCount++;
            if(m_frequencyCount > m_frequency)
            {
                m_activated = false;
                m_frequencyCount = 0;
                m_projectileOffsetIndex = 0;
                
            }
        }
    }

    public void CreateProjectile()
    {
        DamageInformation damageInfo = new DamageInformation(m_armourDamageMagnitude, m_energyDamageMagnitude, m_armourPierceModifier, m_energyPierceModifier, m_parent.GetIFF());
        GameObject projectile = Instantiate(m_projectile);
        BPCannon projectileScript = projectile.GetComponent<BPCannon>();

        projectileScript.Initialise(damageInfo, m_parent.GetIFF(), m_parent.gameObject, m_projectileSpeed, transform.up);

        if (m_projectileOffsetIndex > m_projectileOffset.Count - 1)
        {
            m_projectileOffsetIndex = 0;
        }
        projectile.transform.position = transform.position + (transform.rotation * m_projectileOffset[m_projectileOffsetIndex]);
        m_projectileOffsetIndex++;
        
    }
}
