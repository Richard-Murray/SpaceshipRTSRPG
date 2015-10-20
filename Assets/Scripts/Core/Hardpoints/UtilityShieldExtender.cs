using UnityEngine;
using System.Collections;

public class UtilityShieldExtender : BHUtility {

    [Header("Shield Extender Attributes")]
    public float m_hitPointIncrease; //Percentage versions may be a good idea
    public float m_regenerationRateIncrease;
    //public float m_regenerationDelayDecrease;


	// Use this for initialization
	void Start () {
        base.Start();
        m_module = HARDPOINTMODULE.UTILITY_SHIELDEXTEND;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
