using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class AIController : MonoBehaviour
{
    public StateMachine StateMachine { get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public Animator Animator { get; private set; }
    public Transform[] Waypoints;
    public Transform Player;
    public float SightRange = 10f;
    public float maxAngle = 45.0f;
    public float AttackRange = 2f;
    public LayerMask PlayerLayer;
    public LayerMask obstacleMask;
    public Transform raycastOrigin;

    void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        StateMachine = new StateMachine();
        StateMachine.AddState(new IdleState(this));
        StateMachine.AddState(new PatrolState(this));
        StateMachine.AddState(new ChaseState(this));
        StateMachine.AddState(new AttackState(this));

        StateMachine.TransitionToState(StateType.Idle);
    }

    void Update()
    {
        StateMachine.Update();
        Animator.SetFloat("CharacterSpeed", Agent.velocity.magnitude);
    }

    public bool CanSeePlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, Player.position);
        if (distanceToPlayer <= SightRange)
        {
            Vector3 directionToPlayer = (Player.position - transform.position).normalized;
            float angle = Mathf.Acos(Vector3.Dot(transform.forward, directionToPlayer));
            if (angle < maxAngle)
            {
                if (!Physics.Raycast(raycastOrigin.position, directionToPlayer, SightRange))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, Player.position) <= AttackRange;
    }

    public void HitPlayer()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
