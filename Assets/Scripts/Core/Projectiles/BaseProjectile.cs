using UnityEngine;
using System.Collections;

public class BaseProjectile : MonoBehaviour {

    protected DamageInformation m_damageInformation;
    protected IFFGROUP m_team; //only used in the case of friendly fire
    protected GameObject m_originObject; //used to ignore collisions with

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
