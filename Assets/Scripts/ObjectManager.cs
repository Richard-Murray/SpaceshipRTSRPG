using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TARGETTYPE
{
    STRAFECRAFT,
    SMALLSHIP,
    LARGESHIP,
    FLAGSHIP,
    TURRET
}

public enum TARGETINGAI
{
    DEFAULT, //targets anything and focuses down
    PRIORITISE, //prioritises a target type
    EXCLUSIVE //only targets one type
}

public class ObjectManager : MonoBehaviour {

    public static ObjectManager Instance { get; private set; }

    GameObject m_baseFleetController;

    List<FleetController> m_fleetControllerList;
    List<ShipController> m_shipList;

    void Awake()
    {
        if (!Instance)
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
        m_baseFleetController = (GameObject)Resources.Load("Prefabs/FleetController", typeof(GameObject));

	    m_fleetControllerList = new List<FleetController>();
        m_shipList = new List<ShipController>();

        Initialise(2);
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    void Initialise(int a_numOfTeams)
    {
        for(int i = 0; i < a_numOfTeams; i++)
        {
            GameObject fleetControllerObject = Instantiate(m_baseFleetController);
            fleetControllerObject.transform.SetParent(this.transform);
            m_fleetControllerList.Add(fleetControllerObject.GetComponent<FleetController>());
            fleetControllerObject.name = "FleetController" + (i + 1);
            m_fleetControllerList[i].SetTeam(i);
        }
    }

    public void AddShip(BaseShip a_shipScript)
    {
        m_shipList.Add(a_shipScript.GetComponent<ShipController>());
        m_fleetControllerList[(int)a_shipScript.GetIFF()].AddShip(a_shipScript.GetComponent<ShipController>());
    }

    public GameObject GetTargetWithinRange(Vector3 a_origin, IFFGROUP a_originTeam, /*TARGETINGAI a_aiMethod,*/ float a_maxRange = 1000.0f)
    {
        GameObject target;

        //may use if to switch between turrets and ships
        var possibleTargetList = new List<BaseShip>();

        for (int i = 0; i < m_shipList.Count; i++)
        {
            var baseShip = m_shipList[i].GetBaseShip();
            //Debug.Log(baseShip.GetIFF());
            //Debug.Log(a_originTeam);
            //Debug.Log((a_origin - baseShip.transform.position).magnitude);
            //Debug.Log((a_maxRange + baseShip.m_signatureRadius));
            if(baseShip.GetIFF() != a_originTeam && (a_origin - baseShip.transform.position).magnitude < (a_maxRange + baseShip.m_signatureRadius)) //maybe this would be better optimised if I nested ifs
            {
                possibleTargetList.Add(baseShip);
            }
        }

        if(possibleTargetList.Count > 0)
        {
            target = possibleTargetList[0].gameObject;

            return target;
        }
        else
        {
            return null;
        }

    }

    public FleetController GetFleetController(IFFGROUP a_team)
    {
        return m_fleetControllerList[(int)a_team];
    }
}
