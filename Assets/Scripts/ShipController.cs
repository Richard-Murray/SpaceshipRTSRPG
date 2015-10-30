using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CONTROLTYPE
{
    DIRECT,
    AI,
    NONE
}

public class ShipController : MonoBehaviour {

    BaseShip m_baseShip;
    IFFGROUP m_team;
    //AIController m_ai;
    public CONTROLTYPE m_currentControlMethod;

    Vector3 m_waypoint;

    //AIinfo

    void Awake()
    {
        m_baseShip = GetComponent<BaseShip>();
    }

	// Use this for initialization
	void Start () {
        m_currentControlMethod = CONTROLTYPE.DIRECT;
	}
	
	// Update is called once per frame
	void Update () {
	    
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
        //run AI's control
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
