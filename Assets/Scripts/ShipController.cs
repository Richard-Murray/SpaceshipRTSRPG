using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CONTROLTYPE
{
    DIRECT,
    AI,
    NONE
}

public enum BEHAVIOUROVERRIDE //independant of AI or direct controls
{
    NONE,
    WAYPOINT, //a single or possibly multiple set of waypoints for the ship to steer to
    FORMATION //this ship currently belongs to a formation
}

public enum AIBEHAVIOURSTATE
{
    NONE,
    ENGAGE_BROADSIDE, //the AI is currently moving to engage with their broadside
    ENGAGE_SPINE, //the AI is currently moving to engage with their spinal weapon
    NOT_ENGAGING //the AI may fire freely with their turrets, but won't move to engage with turrets
}

public class ShipController : MonoBehaviour {

    BaseShip m_baseShip;
    IFFGROUP m_team;
    //FleetController m_fleetController; //new
    //AIController m_ai;
    public CONTROLTYPE m_currentControlMethod;
    public BEHAVIOUROVERRIDE m_currentOverrideBehaviour = BEHAVIOUROVERRIDE.NONE;

    Vector3 m_waypoint; //used for all movement functions

    //AIinfo

    void Awake()
    {
        m_baseShip = GetComponent<BaseShip>();
    }

	// Use this for initialization
	void Start () {
        m_currentControlMethod = CONTROLTYPE.AI;
	}
	
	// Update is called once per frame
	void Update () 
    {

        switch(m_currentControlMethod)
        {
            case CONTROLTYPE.DIRECT:
                {
                    UpdateDirectControl();
                    break;
                }
            case CONTROLTYPE.AI:
                {
                    UpdateAIControl();
                    break;
                }
            case CONTROLTYPE.NONE:
                {
                    //nothing
                    break;
                }
        }
	}

    void UpdateDirectControl()
    {
        if(Input.GetKey(KeyCode.W))
        {
            m_baseShip.SetThrottleSpeed(1);
        }
        else if(Input.GetKey(KeyCode.S))
        {
            m_baseShip.SetThrottleSpeed(-1);
        }
        else
        {
            m_baseShip.SetThrottleSpeed(0);
        }
        if(Input.GetKey(KeyCode.A))
        {
            m_baseShip.SetThrottleYaw(1);
        }
        else if(Input.GetKey(KeyCode.D))
        {
            m_baseShip.SetThrottleYaw(-1);
        }
        else
        {
            m_baseShip.SetThrottleYaw(0); //note - this favours A over D)
        }
        if(Input.GetKey(KeyCode.LeftShift))
        {
            m_baseShip.SetBoostOn();
        }
        else
        {
            m_baseShip.SetBoostOff();
        }
    }

    void UpdateAIControl()
    {
        m_baseShip.SetThrottleSpeed(0);
        m_baseShip.SetThrottleYaw(0);
        m_baseShip.SetBoostOff();
        //run AI's control
        //get decision from AI component
    }

    void OnMouseDown()
    {
        ObjectManager.Instance.GetFleetController(m_team).RequestDirectControl(this);
    }

    public void SetControlType(CONTROLTYPE a_controlType)
    {
        m_currentControlMethod = a_controlType;
    }

    public void ChangeThrottleYaw(float a_yaw)
    {
        m_baseShip.ChangeThrottleYaw(a_yaw);
    }

    public void ChangeThrottleSpeed(float a_speed)
    {
        m_baseShip.ChangeThrottleSpeed(a_speed);
    }

    public void SetThrottleYaw(float a_yaw)
    {
        m_baseShip.SetThrottleYaw(a_yaw);
    }

    public void SetThrottleSpeed(float a_speed)
    {
        m_baseShip.SetThrottleSpeed(a_speed);
    }

    public void SetBoostOn()
    {
        m_baseShip.SetBoostOn();
    }

    public void SetBoostOff()
    {
        m_baseShip.SetBoostOff();
    }

    public BaseShip GetBaseShip()
    {
        return m_baseShip;
    }

    public void SetIFF(IFFGROUP a_team)
    {
        m_team = a_team;
        m_baseShip.SetIFF(a_team);
    }
}
