using UnityEngine;
using System.Collections;


public class BaseState : MonoBehaviour {

    protected bool m_activeInput;
    protected bool m_run;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    virtual public void EnableInput(bool a_active)
    {
        //will be true if the state is selected
        //otherwise, false should reset player inputs (the currently selected ship, allowing the AI to take control, and the cursor placement on the options screen)
    }

    virtual public void Run()
    {

    }
}
