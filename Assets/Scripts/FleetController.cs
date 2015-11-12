using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetController : MonoBehaviour {

    public bool m_player;
    public IFFGROUP m_team;
    
    List<ShipController> m_shipList;
    List<ShipFormation> m_formationList;

    ShipController m_currentControlledShip;

    void Awake()
    {
        m_shipList = new List<ShipController>();
        m_formationList = new List<ShipFormation>();
        m_currentControlledShip = null;
    }

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	    if(m_player)
        {
            ProcessPlayerInput();
        }
	}

    public void AddShip(ShipController a_shipController)
    {
        m_shipList.Add(a_shipController);
    }

    public void SetTeam(int a_team)
    {
        m_team = (IFFGROUP)a_team;
    }

    void ProcessPlayerInput()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            //var ray = new Ray()
        }

        if(Input.GetKeyDown(KeyCode.P))
        {
            if(Time.timeScale == 1)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }
    }

    public void RequestDirectControl(ShipController a_controller)
    {
        if (m_player)
        {
            if (m_currentControlledShip)
            {
                m_currentControlledShip.SetControlType(CONTROLTYPE.AI);
            }
            m_currentControlledShip = a_controller;
            a_controller.SetControlType(CONTROLTYPE.DIRECT);
        }
    }
}

class ShipFormation
{
    ShipController m_leadingShip;
    List<ShipController> m_formedShips;
    List<Vector3> m_formedShipPositions;
    
    ShipFormation()
    {
        m_formedShips = new List<ShipController>();
        m_formedShipPositions = new List<Vector3>();
    }

    void InformShipPositions()
    {
        foreach(ShipController ship in m_formedShips)
        {

        }
    }

    public void DebugPositions()
    {

    }

}