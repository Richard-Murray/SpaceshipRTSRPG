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

    public float m_physicalResistance;
    public float m_energyResistance;

    //'Fixed' variables after ship activated
    Collider2D m_collider;

    //Actively used variables
    Vector2 m_velocity; //The actual velocity applied to the transform at the end of the frame
    Vector2 m_throttleTarget; //x is the turn degree, y is the speed
    Vector2 m_throttleVelocity; //Same as the target, but to be applied to the velocity after

    float m_maxHitPoints; //These are different to the hull's HP because extenders may increase max values
    float m_currentHitPoints;
    float m_maxPower;
    float m_currentPower;
    float m_powerRegenerationRate;
    bool m_shieldAlive;
    BHEngine m_activeEngine;
    UtilityShieldGenerator m_activeShield;
    IFFGROUP m_team;

    public List<FixedHardpoint> m_hardpointList;
    //List<BaseHardpoint> m_hardpointList;

	// Use this for initialization
	void Start () {
        List<string> hardpoints = new List<string>();
        //hardpoints.Add("basicEngine");
        //hardpoints.Add("basicShield");
        //hardpoints.Add("basicTurretCannon");
        //hardpoints.Add("basicTurretCannon");
        //hardpoints.Add("basicTurretCannon2");

        //Initialise(hardpoints);
        //m_hardpointList = new List<FixedHardpoint>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < m_hardpointList.Count; ++i)
        {
            if (m_hardpointList[i].m_hardpoint != null)
            {
                m_hardpointList[i].m_hardpoint.ShipUpdate();
            }
        }

        if(Input.GetKeyDown(KeyCode.A))
        {
            ApplyDamage(new DamageInformation(100, 0, 0, 0, IFFGROUP.TEAM1));
        }

        
        ApplyMovement();
    }

    public void Initialise(List<string> a_hardpointsToLoad)
    {
        m_maxHitPoints = m_baseMaxHitPoints;
        m_maxPower = m_baseMaxPower;
        m_powerRegenerationRate = m_basePowerRegenerationRate;

        m_shieldAlive = false;

        for(int i = 0; i < a_hardpointsToLoad.Count; i++)
        {
            CreateHardpoint(a_hardpointsToLoad[i], i); //Need to know, do these immediately call start
        }

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

    public void ThrottleTurn(float a_turn)
    {

    }

    public void ThrottleSpeed(float a_speed)
    {

    }

    void ApplyMovement() //This will handle bringing speeds up to 
    {
        
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



public struct DamageInformation
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