using UnityEngine;
using System.Collections;

public class UtilityPowerExtender : BHUtility {

    [Header("Power Extender Attributes")]
    public float m_maxPowerIncrease;
    public float m_powerRegenerationRateIncrease;

	// Use this for initialization
	void Start () {
        base.Start();
        m_module = HARDPOINTMODULE.UTILITY_POWEREXTEND;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
