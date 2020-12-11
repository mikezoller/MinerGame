using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
	// Adjust the speed for the application.
	public float speed = 50.0f;
	public float wanderRadius = 10.0f;
	public float wanderRate = 10.0f;
	private Vector3 startingLocation;

	// The target (cylinder) position.
	public Vector3 target;
	private Animator animator;
	protected NavMeshAgent agent;


	// Start is called before the first frame update
	public virtual void Start()
    {
		startingLocation = target = transform.position;
		animator = this.GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		// Don’t update position automatically
		agent.updatePosition = false;

		InvokeRepeating("Wander", 1, wanderRate);
	}

	void Wander()
	{
		if (ShouldWander())
		{
			var x = Random.Range(startingLocation.x - wanderRadius, startingLocation.x + wanderRadius);
			var z = Random.Range(startingLocation.z - wanderRadius, startingLocation.z + wanderRadius);
			Vector3 pos = new Vector3(x, startingLocation.y, z);
			NavMeshHit myNavHit;
			if (NavMesh.SamplePosition(pos, out myNavHit, 1, NavMesh.AllAreas))
			{
				agent.SetDestination(myNavHit.position);
			}
		}
	}

	private bool ShouldWander()
	{
		var fightable = this.GetComponent<Enemy>();
		return this.gameObject.activeSelf &&
			(fightable == null || !fightable.isInCombat);
	}

	public virtual void  FixedUpdate()
	{
		bool shouldMove = agent.remainingDistance > agent.radius;

		// Update animation parameters
		if (shouldMove)
		{
			animator.SetBool("walking", true);
			
		} else if (animator.GetBool("walking"))
		{
			this.transform.LookAt(agent.pathEndPosition, Vector3.up);

			animator.SetBool("walking", false);
		}
	}

	void OnAnimatorMove()
	{
		// Update position to agent position
		transform.position = agent.nextPosition;
	}
}
