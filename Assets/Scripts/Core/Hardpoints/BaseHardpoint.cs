using UnityEngine;
using System.Collections;

public enum HARDPOINTMODULE
{
    BROAD_CANNON,
    BROAD_SHIELD,

    SPINE_CANNON,

    TURRET_CANNON,
    TURRET_LASER,
//  TURRET_MISSILE,
    TURRET_REPAIR_OTHER,

    UTILITY_SHIELDGENERATOR,
    UTILITY_SHIELDEXTEND,
//  UTILITY_POWERGENERATOR,
    UTILITY_POWEREXTEND,
    UTILITY_REPAIR_SELF,
    UTILITY_PARASITESHIP,

    ENGINE
}

[RequireComponent(typeof(CircleCollider2D))]
public class BaseHardpoint : MonoBehaviour {

    //These are for setting by the prefab

    [Header("Base Hardpoint Attributes")]
    public string m_id;
    public MOUNTTYPE m_mountType;
    public MOUNTSIZE m_mountSize;
    public HARDPOINTMODULE m_module;
    public float m_capacityStructureCost;
    public float m_capacityPowerCost;

    [HideInInspector]
    public BaseShip m_parent;    
    [HideInInspector]
    public Vector3 m_hullLocation;
    [HideInInspector]
    public float m_totalRotation = 360; //may be temporary

    CircleCollider2D m_collider;

    //public bool m_active;


    // Use this for initialization
    public void Start ()
    {
        m_collider = GetComponent<CircleCollider2D>();
	}
	
	// Update is called once per frame
	virtual public void Update ()
    {
	
	}

    virtual public void ShipUpdate()
    {

    }

    virtual public void PrimaryAction()
    {

    }

    virtual public void RunPrimaryAction()
    {

    }

//    void CreateHardpoint(Vector3 a_hullLocation, MOUNTTYPE a_mountType, MOUNTSIZE a_mountSize, BaseShip a_parent)
//    {
//        m_hullLocation = a_hullLocation;
//        m_mountType = a_mountType;
//        m_mountSize = a_mountSize;
//        m_parent = a_parent;
        
//        //m_active = false;
//    }
}