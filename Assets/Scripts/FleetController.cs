using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FleetController : MonoBehaviour {

    public bool m_player;
    public IFFGROUP m_team;
    
    List<ShipController> m_shipList;

    void Awake()
    {
        m_shipList = new List<ShipController>();
    }

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void AddShip(ShipController a_shipController)
    {
        m_shipList.Add(a_shipController);
    }
}
