using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectFactory : MonoBehaviour {

    public static ObjectFactory Instance { get; private set; }

    int m_numOfShipsDebug = 0;

    void Awake()
    {        
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this); //Not sure if this will work
        }
    }

	// Use this for initialization
	void Start () {        
        //debug
        string hull = "basicShip";
        List<string> hardpoints = new List<string>();
        hardpoints.Add("basicEngine");
        hardpoints.Add("basicShield");
        hardpoints.Add("basicTurretCannon");
        hardpoints.Add("basicTurretCannon");
        hardpoints.Add("basicTurretCannon2");
        List<int> controlGroups = new List<int>();
        controlGroups.Add(0);
        controlGroups.Add(0);
        controlGroups.Add(0);
        controlGroups.Add(0);
        controlGroups.Add(0);
        float cost = 100.0f;
        ShipBuild tempBuild = new ShipBuild(hull, hardpoints, controlGroups, cost);

        CreateShip(tempBuild, IFFGROUP.TEAM1, new Vector3(-5, 0, 0));
        CreateShip(tempBuild, IFFGROUP.TEAM1, new Vector3(-5, 7, 0));
        CreateShip(tempBuild, IFFGROUP.TEAM2, new Vector3(5, 0, 0));
        CreateShip(tempBuild, IFFGROUP.TEAM2, new Vector3(5, -7, 0));
	}
	
	// Update is called once per frame
    void Update()
    {
        //Debug.Log(m_numOfShipsDebug);

        if(Input.GetKey(KeyCode.Space))
        {
            string hull = "basicShip";
            List<string> hardpoints = new List<string>();
            hardpoints.Add("basicEngine");
            hardpoints.Add("basicShield");
            hardpoints.Add("basicTurretCannon");
            hardpoints.Add("basicTurretCannon");
            hardpoints.Add("basicTurretCannon2");
            List<int> controlGroups = new List<int>();
            controlGroups.Add(0);
            controlGroups.Add(0);
            controlGroups.Add(0);
            controlGroups.Add(0);
            controlGroups.Add(0);
            float cost = 100.0f;
            ShipBuild tempBuild = new ShipBuild(hull, hardpoints, controlGroups, cost);

            CreateShip(tempBuild, IFFGROUP.TEAM1, new Vector3(Random.value * 100, 0, 0));

            m_numOfShipsDebug++;
        }
    }

    void CreateShip(ShipBuild a_build, IFFGROUP a_team, Vector3 a_position)
    {
        GameObject ship = Instantiate(DataManager.Instance.GetPrefab(a_build.m_hull));
        BaseShip shipScript = ship.GetComponent<BaseShip>();
        shipScript.Initialise(a_build.m_hardpoints);
        ship.transform.position = a_position;
        ship.GetComponent<ShipController>().SetIFF(a_team);

        //add ship to lists
        ObjectManager.Instance.AddShip(shipScript);
    }
}

public struct ShipBuild
{
    public string m_hull; //the hull id
    public List<string> m_hardpoints; //the strings of the hardpoint ids
    public List<int> m_controlGroups; //corresponds to the m_hardpoints index, 0 is no control group, '0, 1, 1' refers to the first 3 items where items 2 and 3 are part of control group 1
    public float m_cost;

    public ShipBuild(string a_hull, List<string> a_hardpoints, List<int> a_controlGroups, float a_cost)
    {
        m_hull = a_hull;
        m_hardpoints = a_hardpoints;
        m_controlGroups = a_controlGroups;
        m_cost = a_cost;
    }

}