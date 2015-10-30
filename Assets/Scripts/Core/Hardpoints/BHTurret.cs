using UnityEngine;
using System.Collections;

public class BHTurret : BaseHardpoint
{
    protected GameObject m_objectToTarget; //Non-null object that is the currently selected target
    protected Vector3 m_locationTarget; //The 'actual' location of the target recorded, also used for calculating velocity
    protected Vector3 m_headingTarget; //The lead indicator

    protected Vector3 m_currentDirection; //The current rotation of the turret
    protected Vector3 m_targetDirection;  //Calculated angle for the turret to point the headingtarget, or the regular target if needed

    [Header("Base Turret Attributes")]
    public float m_turnSpeed; //Tracking speed of the turret
    public float m_maxRange; //Detection range for targets

    //public float m_initialRotation; //for placing turrets, these will go into base hardpoint stuff
    //public float m_rotationLock;

    // Use this for initialization
    public void Start()
    {
        m_currentDirection = transform.up;
        m_targetDirection = new Vector3(0, 1, 0);
    }

    // Update is called once per frame
    public override void Update()
    {
    }

    public override void ShipUpdate()
    {

    }

    public void FinishRotation()
    {
        //transform.up = m_currentDirection;
    }

    public virtual void RotateToTarget()
    {                
        m_targetDirection = (m_locationTarget - transform.position).normalized;
        m_targetDirection.z = 0;

        var qTargetDirection = Quaternion.FromToRotation(Vector3.up, m_targetDirection);
        float fNewAngle = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, qTargetDirection.eulerAngles.z, m_turnSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, fNewAngle);
    }

    public virtual void RotateToTargetHeader()
    {
        m_targetDirection = (m_headingTarget - transform.position).normalized;
        m_targetDirection.z = 0;

        var qTargetDirection = Quaternion.FromToRotation(Vector3.up, m_targetDirection);
        float fNewAngle = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.z, qTargetDirection.eulerAngles.z, m_turnSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(0, 0, fNewAngle);
    }

    public virtual void RotateToVectorTarget(Vector3 a_target)
    {
        m_targetDirection = (a_target - transform.position).normalized;
        m_targetDirection.z = 0;

        var qTargetDirection = Quaternion.FromToRotation(Vector3.up, m_targetDirection);
        float fNewAngle = Mathf.MoveTowardsAngle(transform.localRotation.eulerAngles.z, qTargetDirection.eulerAngles.z, m_turnSpeed * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(0, 0, fNewAngle);
    }
}
