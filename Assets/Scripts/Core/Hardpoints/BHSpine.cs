using UnityEngine;
using System.Collections;

public class BHSpine : BaseHardpoint
{
    [Header("Base Spine Attributes")]
    public Vector3 m_direction;

	// Use this for initialization
	void Start ()
    {
        m_direction = transform.up;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
