using UnityEngine;
using System.Collections;

public class UtilityShieldGenerator : BHUtility {


    [Header("Shield Generator Attributes")]
    public float m_baseShieldMaxHitPoints;
    public float m_baseShieldRegenerationRate;
    public float m_baseShieldRegenerationDelay;

    public float m_physicalResistance;
    public float m_energyResistance;

    float m_shieldMaxHitPoints;
    float m_shieldCurrentHitPoints; //debug
    float m_shieldRegenerationRate;
    float m_shieldRegenerationDelay;

    bool m_shieldAlive;

    // Use this for initialization
    public void Start () {
        base.Start();
        m_module = HARDPOINTMODULE.UTILITY_SHIELDGENERATOR;

        m_shieldMaxHitPoints = m_baseShieldMaxHitPoints;
        m_shieldRegenerationRate = m_baseShieldRegenerationRate;
        m_shieldRegenerationDelay = m_baseShieldRegenerationDelay;
        m_shieldCurrentHitPoints = m_shieldMaxHitPoints;

        m_shieldAlive = true;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public override void ShipUpdate()
    {
        m_shieldCurrentHitPoints += m_shieldRegenerationRate * Time.deltaTime;
        if(m_shieldCurrentHitPoints > m_shieldMaxHitPoints)
        {
            m_shieldCurrentHitPoints = m_shieldMaxHitPoints;
        }

        if(m_shieldCurrentHitPoints >= m_shieldMaxHitPoints * 0.1f)
        {
            m_shieldAlive = true;
        }
    }

    public void LinkExtender(UtilityShieldExtender a_shieldExtender)
    {
        m_shieldMaxHitPoints += a_shieldExtender.m_hitPointIncrease;
        m_shieldRegenerationRate += a_shieldExtender.m_regenerationRateIncrease;
    }

    public DamageInformation ApplyDamage(DamageInformation a_damageInformation)
    {
        DamageInformation damageInfo = a_damageInformation;
        if (m_shieldAlive)
        {
            float damageDealt = m_shieldCurrentHitPoints;
            float damageReduction = Mathf.Abs(1 - Mathf.Clamp((m_energyResistance - damageInfo.m_energyPierceModifier) / 100, 0, 1)); //calculate the damage the standard damage will deal
            //damageReduction = Mathf.Clamp(damageReduction, 0, 1);

            m_shieldCurrentHitPoints -= damageInfo.m_physicalDamageMagnitude * damageReduction + damageInfo.m_energyDamageMagnitude; //Apply damagereduced armour damage + bonus damage vs shield
            damageDealt -= m_shieldCurrentHitPoints;

            //if(damageInfo.m_physicalDamageMagnitude > damageDealt) //May remove this. Prevents the base damage then done to the armour underneath from exceeding the damage that was just done to the shield
            //{
            //    damageInfo.m_physicalDamageMagnitude = damageDealt;
            //}
            damageReduction = Mathf.Abs(1 - Mathf.Clamp((m_physicalResistance - damageInfo.m_energyPierceModifier) / 100, 0, 1)); //Calculate the damage that will then be applied to the base ship behind the shield. PAY ATTENTION TO THE MODIFIER, IT SHOULD BE ENERGY

            damageInfo.m_physicalDamageMagnitude *= damageReduction;

            if (m_shieldCurrentHitPoints <= 0)
            {
                m_shieldAlive = false;
            }
        }

        return damageInfo;
    }
}
