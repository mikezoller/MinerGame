using Assets.Scripts;
using Miner.Models;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
#endif

namespace Miner.GameObjects
{
	public class InteractionObject : MonoBehaviour, IClickable
	{
		public GameObject uiToShow;
		public Player playerData;
		public IEnumerator DoWork(System.Action<GameObject> callback = null)
		{
			callback(uiToShow);
			yield return null;
		}


		public virtual void Clicked()
		{
			var player = GameObject.FindGameObjectWithTag("Player");
			var character = player.GetComponent<Character>();

			var gc = GameObject.FindGameObjectWithTag("GameController");
			var gameManager = gc.GetComponent<GameManager>();
			NavMeshHit myNavHit;
			if (NavMesh.SamplePosition(transform.position, out myNavHit, 100, -1))
			{
				character.GetComponent<NavMeshAgent>().ResetPath();
				character.GetComponent<NavMeshAgent>().SetDestination(myNavHit.position);

				var bounds = this.GetComponent<Collider>().bounds;
				character.DoAction(myNavHit.position, null, DoWork((ui) => ui.SetActive(true)), Math.Max(bounds.size.x / 2.0f, bounds.size.z / 2.0f) + Vector3.Distance(myNavHit.position, this.transform.position));
			}
		}

		public IEnumerator ToDo()
		{
			uiToShow.SetActive(true);
			yield return null;
		}
	}
}