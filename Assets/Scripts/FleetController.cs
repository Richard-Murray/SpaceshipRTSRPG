using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetController : MonoBehaviour {

    public bool m_player;
    public IFFGROUP m_team;
    
    List<ShipController> m_shipList;
    List<ShipFormation> m_formationList;

    void Awake()
    {
        m_shipList = new List<ShipController>();
        m_formationList = new List<ShipFormation>();
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
    }

    public void RequestDirectControl()
    {

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