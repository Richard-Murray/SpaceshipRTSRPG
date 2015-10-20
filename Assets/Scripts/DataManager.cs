using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MonoBehaviour {

    public static DataManager Instance { get; private set; }

    //Dictionary<string, string> m_prefabPathDictionary; //First string is the ID, second is the directory
    Dictionary<string, GameObject> m_prefabLoadedDictionary;

	// Use this for initialization
	void Awake () {
        if(!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this); //Not sure if this will work
        }
        
        Initialise();


	}

    void Initialise()
    {
        m_prefabLoadedDictionary = new Dictionary<string, GameObject>();

        foreach (GameObject gObject in Resources.LoadAll("Prefabs/Hardpoints", typeof(GameObject)))
        {
            m_prefabLoadedDictionary.Add(gObject.GetComponent<BaseHardpoint>().m_id, gObject);
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public GameObject GetPrefab(string a_id)
    {
        return m_prefabLoadedDictionary[a_id];
    }
}