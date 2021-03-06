﻿using UnityEngine;
using System.Collections;

public class BHEngine : BaseHardpoint {

    [Header("Engine Attributes")]
    public float m_maxSpeed;
    public float m_acceleration;
    public float m_steerAgility;

    public float m_boostMaxSpeedMod;
    public float m_boostAccelerationMod;
    public float m_boostSteerAgilityMod;
    public float m_boostPowerDrain;

    //[HideInInspector]
    //public bool m_boosting;

	// Use this for initialization
	void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //public override void ShipUpdate()
    //{
    //    m_boosting = false;
    //}

    //public override void PrimaryAction()
    //{
    //    m_boosting = true;
    //}
}
