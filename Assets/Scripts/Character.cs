using Miner;
using Miner.GameObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
	// Adjust the speed for the application.
	public float speed = 50.0f;

	// The target (cylinder) position.
	public Vector3 target;
	private Animator animator;
	private int idleHash;
	private int walkingHash;

	NavMeshAgent agent;
	Vector2 smoothDeltaPosition = Vector2.zero;
	Vector2 velocity = Vector2.zero;

	GameManager gameManager;

	public Coroutine currentAction;

	// Start is called before the first frame update
	void Start()
    {
		target = transform.position;
		animator = this.GetComponent<Animator>();
		agent = GetComponent<NavMeshAgent>();
		// Don’t update position automatically
		agent.updatePosition = false;

		idleHash = Animator.StringToHash("Idle");
		walkingHash = Animator.StringToHash("walking");

		var gc = GameObject.FindGameObjectWithTag("GameController");
		gameManager = gc.GetComponent<GameManager>();
	}
	public void SetAnimBool(string trigger)
	{
		animator.SetBool(trigger, true);
	}
	public void ClearAnimBool(string trigger)
	{
		animator.SetBool(trigger, false);
	}
	public void StartAnimation(string trigger)
	{
		animator.SetTrigger(trigger);
	}

	public string activeAnimation;
	public void DoAction(Vector3 position, string animation, IEnumerator toDo, float targetRadius = 10)
	{
		if (currentAction != null)
		{
			StopCoroutine(currentAction);
			currentAction = null;
			
		}
		if (!string.IsNullOrEmpty(activeAnimation))
		{
			animator.SetBool(activeAnimation, false);
			animator.SetTrigger("Idle");
		}
		activeAnimation = animation;
		currentAction = StartCoroutine(DoIt(position, animation, toDo, targetRadius));
	}

	private IEnumerator DoIt(Vector3 position, string animation, IEnumerator toDo, float targetRadius = 10)
	{
		while (true)
		{
			var distance = Vector3.Distance(position, transform.position);
			if (distance < targetRadius)
			{
				this.transform.LookAt(position, Vector3.up);
				break;
			}
			yield return new WaitForSeconds(1);
		}
		if (!string.IsNullOrEmpty(animation))
		{
			animator.SetBool(animation, true);
		}
			yield return toDo;
		if (!string.IsNullOrEmpty(animation))
		{
			animator.SetBool(animation, false);
			animator.SetTrigger("Idle");
		}
		activeAnimation = "";
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		Vector3 worldDeltaPosition = agent.nextPosition - transform.position;

		// Map 'worldDeltaPosition' to local space
		float dx = Vector3.Dot(transform.right, worldDeltaPosition);
		float dy = Vector3.Dot(transform.forward, worldDeltaPosition);
		Vector2 deltaPosition = new Vector2(dx, dy);

		// Low-pass filter the deltaMove
		float smooth = Mathf.Min(1.0f, Time.deltaTime / 0.15f);
		smoothDeltaPosition = Vector2.Lerp(smoothDeltaPosition, deltaPosition, smooth);

		// Update velocity if time advances
		if (Time.deltaTime > 1e-5f)
			velocity = smoothDeltaPosition / Time.deltaTime;

		bool shouldMove = agent.remainingDistance > agent.radius;

		// Update animation parameters
		if (shouldMove)
		{
			animator.SetBool("walking", true);
			
		} else if (animator.GetBool("walking"))
		{
			var targetPosition = agent.pathEndPosition;
			var targetPoint = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
			var _direction = (targetPoint - transform.position).normalized;
			var _lookRotation = Quaternion.LookRotation(_direction);

			this.transform.LookAt(agent.pathEndPosition, Vector3.up);

			//transform.rotation = new Quaternion(_lookRotation.x, _lookRotation.y, _lookRotation.z, 1);
			animator.SetBool("walking", false);
		}

		//LookAt lookAt = GetComponent<LookAt>();
		//if (lookAt)
		//	lookAt.lookAtTargetPosition = agent.steeringTarget + transform.forward;
	}

	void OnAnimatorMove()
	{
		// Update position to agent position
		transform.position = agent.nextPosition;
	}
}
