using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MOUNTTYPE
{
    BROADSIDE,
    SPINAL,
    TURRET,
    UTILITY,
    ENGINE,
}

public enum MOUNTSIZE
{
    S,
    M,
    L,
    XL,
}

public enum IFFGROUP
{
    TEAM1,
    TEAM2,
    TEAM3,
    TEAM4,
    TEAM5,
    TEAM6,
    TEAM7,
    TEAM8
}

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(ShipController))]

public class BaseShip : MonoBehaviour {

    public string m_id;

    [Header("Base Ship Attributes")]
    //Statistics that will be used for all ship prefabs
    public float m_baseCapacityStructure; //In KG
    public float m_baseCapacityPowerGrid;
    public float m_baseWeight; //In KG
    public float m_baseMaxPower;
    public float m_baseMaxHitPoints;
    public float m_basePowerRegenerationRate;
    public float m_signatureRadius;

    public float m_physicalResistance;
    public float m_energyResistance;

    //'Fixed' variables after ship activated
    Collider2D m_collider;

    //Actively used variables
    Vector3 m_velocity; //The actual velocity applied to the transform at the end of the frame

    float m_throttleRateSpeed; //from -1 to 1
    float m_throttleRateYaw; //from -1 to 1
    float m_throttleTargetSpeed; //the actual speed target
    float m_currentSpeed; //the current speed, which is continually adjusted to reach targetspeed
    float m_throttleTargetYaw; //probably the same as current yaw
    float m_currentYaw; //unused

    float m_maxHitPoints; //These are different to the hull's HP because extenders may increase max values
    float m_currentHitPoints;
    float m_maxPower;
    float m_currentPower;
    float m_powerRegenerationRate;
    bool m_shieldAlive;
    bool m_boosting;
    bool m_powerDisabled;
    BHEngine m_activeEngine;
    UtilityShieldGenerator m_activeShield;
    IFFGROUP m_team;

    public List<FixedHardpoint> m_hardpointList;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update ()
    {
        //Debug.Log(m_currentHitPoints);
        //Debug.Log(m_currentPower);
        Debug.Log(GetIFF());

        for (int i = 0; i < m_hardpointList.Count; ++i)
        {
            if (m_hardpointList[i].m_hardpoint != null)
            {
                m_hardpointList[i].m_hardpoint.ShipUpdate();
            }
        }
        //if(Input.GetKeyDown(KeyCode.A))
        //{
        //    ApplyDamage(new DamageInformation(100, 0, 0, 0, IFFGROUP.TEAM1));
        //}
                
        ApplyMovement();

        ApplyPower(m_powerRegenerationRate);
    }

    public void Initialise(List<string> a_hardpointsToLoad)
    {
        m_maxHitPoints = m_baseMaxHitPoints;
        m_maxPower = m_baseMaxPower;
        m_powerRegenerationRate = m_basePowerRegenerationRate;

        m_shieldAlive = false;
        m_boosting = false;
        m_powerDisabled = false;

        for(int i = 0; i < a_hardpointsToLoad.Count; i++)
        {
            CreateHardpoint(a_hardpointsToLoad[i], i); //Need to know, do these immediately call start
        }

        m_collider = GetComponent<PolygonCollider2D>();
        m_velocity = new Vector3(0, 0, 0);

        m_throttleRateSpeed = 0.0f;
        m_throttleRateYaw = 0.0f;
        m_throttleTargetSpeed = 0.0f;
        m_currentSpeed = 0.0f;
        m_throttleTargetYaw = 0.0f;
        m_currentYaw = 0.0f;

        //Linking process, changes stats to reflect extenders
        LinkAndUpdateHardpoints();

        m_currentHitPoints = m_maxHitPoints;
        m_currentPower = m_baseMaxPower;
    }    

    void LinkAndUpdateHardpoints()
    {
        m_maxHitPoints = m_baseMaxHitPoints; //These are different to the hull's HP because extenders may increase max values
        m_maxPower = m_baseMaxPower;
        m_powerRegenerationRate = m_basePowerRegenerationRate;

        LinkEngine();
        LinkShield();

        for (int i = 0; i < m_hardpointList.Count; i++)
        {
            if (m_hardpointList[i].m_hardpoint.m_module == HARDPOINTMODULE.UTILITY_SHIELDEXTEND)
            {
                m_activeShield.LinkExtender((UtilityShieldExtender)m_hardpointList[i].m_hardpoint);
            }
            if (m_hardpointList[i].m_hardpoint.m_module == HARDPOINTMODULE.UTILITY_POWEREXTEND)
            {
                UtilityPowerExtender powerExtend = (UtilityPowerExtender)m_hardpointList[i].m_hardpoint;
                m_maxPower += powerExtend.m_maxPowerIncrease;
                m_powerRegenerationRate += powerExtend.m_powerRegenerationRateIncrease;
            }
        }
    }

    void LinkEngine()
    {
        m_activeEngine = null;
        for (int i = 0; i < m_hardpointList.Count; i++)
        {
            if (m_hardpointList[i].m_hardpoint.m_module == HARDPOINTMODULE.ENGINE)
            {
                m_activeEngine = (BHEngine)m_hardpointList[i].m_hardpoint;
                break;
            }
        }
    }

    void LinkShield()
    {
        m_activeShield = null;
        for (int i = 0; i < m_hardpointList.Count; i++)
        {
            if (m_hardpointList[i].m_hardpoint.m_module == HARDPOINTMODULE.UTILITY_SHIELDGENERATOR)
            {
                m_activeShield = (UtilityShieldGenerator)m_hardpointList[i].m_hardpoint;
                m_shieldAlive = true;
                break;
            }
        }
    }

    void CreateHardpoint(string a_hardpointID, int a_selectedHardpointLocation)
    {
        GameObject hardpoint = Instantiate(DataManager.Instance.GetPrefab(a_hardpointID));
        hardpoint.transform.SetParent(this.transform);
        hardpoint.transform.up = m_hardpointList[a_selectedHardpointLocation].m_initialRotation; //UP OR FORWARD
        ApplyHardpoint(hardpoint.GetComponent<BaseHardpoint>(), m_hardpointList[a_selectedHardpointLocation]);
    }

    bool ApplyHardpoint(Component a_hardpoint, FixedHardpoint a_selectedFixedHardpoint)
    {
        BaseHardpoint baseHardpoint = (BaseHardpoint)a_hardpoint;
        if( baseHardpoint.m_mountType == a_selectedFixedHardpoint.m_mountType &&
            baseHardpoint.m_mountSize == a_selectedFixedHardpoint.m_mountSize)
        {
            a_selectedFixedHardpoint.ApplyHardpoint((BaseHardpoint)a_hardpoint, this);
            return true;
        }

        return false;
    }

    public void ApplyDamage(DamageInformation a_damageInformation)
    {
        DamageInformation damageInfo = a_damageInformation;

        damageInfo = m_activeShield.ApplyDamage(damageInfo);

        float damageReduction = Mathf.Abs(1 - Mathf.Clamp((m_physicalResistance - damageInfo.m_physicalPierceModifier) / 100, 0, 1));
        m_currentHitPoints -= damageInfo.m_physicalDamageMagnitude * damageReduction;
    }

    public void ApplyPower(float a_power)
    {
        m_currentPower = Mathf.Clamp(m_currentPower + a_power * Time.deltaTime, -999999, m_maxPower);
        if(m_currentPower < 0)
        {
            m_powerDisabled = true;
        }
        else
        {
            m_powerDisabled = false;
        }
    }


    //Inputs
    public void ChangeThrottleYaw(float a_yaw)
    {
        m_throttleRateYaw += a_yaw;
        m_throttleRateYaw = Mathf.Clamp(m_throttleRateYaw, -1, 1);
    }

    public void ChangeThrottleSpeed(float a_speed)
    {
        m_throttleRateSpeed += a_speed;
        m_throttleRateSpeed = Mathf.Clamp(m_throttleRateSpeed, -1, 1);
    }

    public void SetThrottleYaw(float a_yaw)
    {
        m_throttleRateYaw = a_yaw;
    }

    public void SetThrottleSpeed(float a_speed)
    {
        m_throttleRateSpeed = a_speed;
    }

    public void SetBoostOn()
    {
        if (!m_powerDisabled)
        {
            m_boosting = true;
            ApplyPower(-m_activeEngine.m_boostPowerDrain);
        }
    }

    public void SetBoostOff()
    {
        m_boosting = false;
    }



    void ApplyMovement() //This will handle bringing speeds up to 
    {
        float boostMod;
        if(m_boosting == true)
        {
            boostMod = 1;
        }
        else
        {
            boostMod = 0;
        }

        m_throttleTargetSpeed = (m_activeEngine.m_maxSpeed + m_activeEngine.m_boostMaxSpeedMod * boostMod) * m_throttleRateSpeed;
        m_throttleTargetYaw = (m_activeEngine.m_steerAgility + m_activeEngine.m_boostSteerAgilityMod * boostMod) * m_throttleRateYaw;

        if(m_currentSpeed < m_throttleTargetSpeed)
        {
            m_currentSpeed = Mathf.Clamp(m_currentSpeed + ((m_activeEngine.m_acceleration + m_activeEngine.m_boostAccelerationMod * boostMod) * Time.deltaTime), -m_activeEngine.m_maxSpeed, m_throttleTargetSpeed);
        }
        else if(m_currentSpeed > m_throttleTargetSpeed)
        {
            m_currentSpeed = Mathf.Clamp(m_currentSpeed - ((m_activeEngine.m_acceleration + m_activeEngine.m_boostAccelerationMod * boostMod) * Time.deltaTime), m_throttleTargetSpeed, m_activeEngine.m_maxSpeed);
        }

        transform.rotation = Quaternion.Euler(0, 0, transform.localRotation.eulerAngles.z + m_throttleTargetYaw * Time.deltaTime); //PROBLEM, TURRETS DO NOT NOTICE
        m_velocity = transform.up * m_currentSpeed; //not the 'real' velocity
        //GetComponent<Rigidbody2D>().AddForce(new Vector2(m_velocity.x, m_velocity.y));
        //Debug.Log(GetComponent<Rigidbody2D>().inertia);
        transform.position += m_velocity * Time.deltaTime;

        
        
    }

    //void AddBaseHardpoint(Vector3 a_hullLocation, MOUNTTYPE a_mountType, MOUNTSIZE a_mountSize, Vector3 a_initialRotation) //Currently unneeded
    //{
    //    var fixedHardpoint = new FixedHardpoint(a_hullLocation, a_mountType, a_mountSize, a_initialRotation);
    //    m_hardpointList.Add(fixedHardpoint);
    //}

    //void ClearHardpoints() //Currently obsolete
    //{
    //    for(int i = 0; i < m_hardpointList.Count; ++i)
    //    {
    //        if(m_hardpointList[i].m_hardpoint != null)
    //        {
    //            Destroy(m_hardpointList[i].m_hardpoint.transform.gameObject); //Needed specifically because fixedhardpoint contains a gameobject
    //        }
    //    }
    //    m_hardpointList.Clear();
    //}

    public IFFGROUP GetIFF()
    {
        return m_team;
    }

    public void SetIFF(IFFGROUP a_team)
    {
        m_team = a_team;
    }
}

[System.Serializable]
public class FixedHardpoint
{
    [HideInInspector]
    public BaseHardpoint m_hardpoint;

    public Vector3 m_hullLocation;
    public MOUNTTYPE m_mountType;
    public MOUNTSIZE m_mountSize;

    public Vector3 m_initialRotation;
    public float m_totalRotation = 360; //360 or 0 should be fine
    //bool m_active;

    public FixedHardpoint(Vector3 a_hullLocation, MOUNTTYPE a_mountType, MOUNTSIZE a_mountSize, Vector3 a_initialRotation, float a_totalRotation = 360)
    {
        m_hullLocation = a_hullLocation;
        m_mountType = a_mountType;
        m_mountSize = a_mountSize;
        m_initialRotation = a_initialRotation;
        m_totalRotation = a_totalRotation;
        //m_active = false;
    }

    ~FixedHardpoint()
    {

    }

    public void ApplyHardpoint(BaseHardpoint a_hardpoint, BaseShip a_parent)
    {
        m_hardpoint = a_hardpoint;
        m_hardpoint.transform.localPosition = m_hullLocation;
        m_hardpoint.m_totalRotation = m_totalRotation;
        m_hardpoint.m_parent = a_parent;
    }
}



public class DamageInformation
{
    public float m_physicalDamageMagnitude; //Damage in HP
    public float m_energyDamageMagnitude;
    public float m_physicalPierceModifier; //Percentage of physical damage reduction ignored
    public float m_energyPierceModifier; //Percentage of shield damage reduction ignored

    public IFFGROUP m_team;
    //heat modifier?

    public DamageInformation(float a_physicalDamageMagnitude, float a_energyDamageMagnitude, float a_physicalPierceModifier, float a_energyPierceModifier, IFFGROUP a_team)
    {
        m_physicalDamageMagnitude = a_physicalDamageMagnitude;
        m_energyDamageMagnitude = a_energyDamageMagnitude;
        m_physicalPierceModifier = a_physicalPierceModifier;
        m_energyPierceModifier = a_energyPierceModifier;
        m_team = a_team;
    }
}