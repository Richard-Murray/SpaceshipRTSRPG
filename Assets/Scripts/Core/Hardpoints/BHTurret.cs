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
    }

    // Update is called once per frame
    public override void Update()
    {
    }

    public override void ShipUpdate()
    {
        m_locationTarget = m_objectToTarget.transform.position; //DEBUG
    }

    public void FinishRotation()
    {
        transform.up = m_currentDirection;
    }

    public virtual void RotateToTarget()
    {
        Debug.Log(transform.rotation.z);

        //Quaternion.RotateTowards(



        //transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, step)
        //step is max delta in degrees

        //Eulervec, (0, 90, 0)
        //Degrees float, 90
        //Dir vec, (0, 1, 0)
        //Quaternion, (something)

        //float degrees to quat, Quaternion.Euler
        //Dirvec to quat, Quaternion.LookRotation
        //Euler to quat, Quaternion.Euler
        //float degrees to dirvec (lookvector),
        //Quaternion to degrees, eulerangles

        //Vector3 eulerVec = (0 90 0)
        //Quaternion rot = Quaternion.Euler(eulerVec)




        m_targetDirection = (m_locationTarget - transform.position).normalized;
        m_targetDirection.z = 0;

        //m_currentDirection = Quaternion.RotateTowards(Quaternion.LookRotation(m_currentDirection, Vector3.up), Quaternion.LookRotation(m_targetDirection, Vector3.up), m_turnSpeede) * transform.up;

        float fDirMod = 1.0f;
        float fPerpDot = Vector3.Dot(transform.right, m_targetDirection); //this right only works on up, forward likely needs up
        if (fPerpDot > 0) fDirMod = -1.0f;

        Quaternion rotation = Quaternion.AngleAxis(fDirMod * m_turnSpeed * Time.deltaTime, Vector3.forward);
        Vector3 vecFinalDirection = rotation * transform.up;

        Quaternion targetDirection = Quaternion.LookRotation(m_targetDirection);
        Quaternion currentDirection = Quaternion.LookRotation(transform.up);
        Quaternion finalDirection = Quaternion.LookRotation(vecFinalDirection);

        //Debug.Log(fPerpDot);
        float fCurrentAngle = Quaternion.Angle(currentDirection, targetDirection);
        float fNewAngle = Quaternion.Angle(finalDirection, targetDirection);

        if(fCurrentAngle < fNewAngle) // && Quaternion.Angle(currentDirection, transform.rotation.) 
        {
            m_currentDirection = m_targetDirection;
        }
        else
        {
            m_currentDirection = vecFinalDirection;
        }
    }

    public virtual void RotateToTargetHeader()
    {
        m_targetDirection = (m_headingTarget - transform.position).normalized;
    }
}
