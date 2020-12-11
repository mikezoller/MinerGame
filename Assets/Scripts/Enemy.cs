using Assets.Scripts.GameObjects;
using Assets.Scripts.Models;
using Miner.Helpers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class Enemy : NPC
	{
		private Animator animator;
		public bool isInCombat;
		public float strikeRate = 2f;
		public bool isDead;
		public float respawnRate = 10f; // seconds
		public int enemyId;
		private Canvas canvas;
		private GameObject hud;
		private GameObject go;
		public EnemyData enemyData;
		private Vector3 respawnLocation;
		private Bounds bounds;
		public Vector3 destination { get { return agent.destination; } }

		public Character opponent;
		private StatDisplay statDisplay;

		public override void Start()
		{
			base.Start();
			go = this.gameObject;
			enemyData = EnemyDatabase.GetEnemy(enemyId);
			respawnLocation = this.transform.position;
			this.hud = GameObject.Find("HUD");
			this.canvas = this.hud.GetComponent<Canvas>();

			if (statDisplay == null)
			{
				statDisplay = this.gameObject.AddComponent<StatDisplay>();
			}
			bounds = this.GetComponent<BoxCollider>().bounds;
			statDisplay.Init(new Vector3(0, bounds.size.y / gameObject.transform.localScale.y, 0));
			animator = this.GetComponent<Animator>();

		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (isInCombat)
			{
				var distance = Vector3.Distance(this.transform.position, this.opponent.transform.position);
				if (distance <= 10f)
				{
					agent.isStopped = true;
				}
			}

		}
		public bool InitiateCombat(Character opponent)
		{
			if (opponent.CanFight(this))
			{
				statDisplay.ShowHealthBar(true);
				var distance = Vector3.Distance(this.transform.position, opponent.transform.position);
				agent.stoppingDistance = 5f;
				if (distance >= 5f)
				{
					agent.ResetPath();
					agent.SetDestination(opponent.destination);
				}
				isInCombat = true;
				this.opponent = opponent;

				CancelInvoke("Strike");
				StartFight(opponent);
			}

			return isInCombat;
		}

		public void StartFight(Character character)
		{
			this.opponent = character;
			isInCombat = true;
			CancelInvoke("Strike");
			InvokeRepeating("Strike", 1, strikeRate);
		}
		public void ReceiveDamage(Character c, int amount)
		{
			if (!isInCombat)
			{
				InitiateCombat(c);
			}
			this.enemyData.CurrentHP -= amount;
			isDead = enemyData.CurrentHP <= 0;

			// Show hit text
			statDisplay.AddHit(amount);
			UpdateHealthBar();
			if (isDead)
			{
				StopFight();
				go.SetActive(false);

				// Instantiate dropped items
				foreach (Droppable drop in enemyData.Drops)
				{
					var o = (GameObject)Instantiate(Resources.Load(ItemDatabase.GetModelPath(drop.Item.item.id)), this.transform.position, Quaternion.identity);

					// TODO: handle probability
					Droppable sc = o.AddComponent(typeof(Droppable)) as Droppable;
					o.AddComponent<BoxCollider>();
					sc.Item = drop.Item;
				}

				// Start respawn timer
				Invoke("Respawn", respawnRate);
			}
		}

		public void DoDamage(int amount)
		{
			var rand = UnityEngine.Random.Range(0, 100);
			if (rand > enemyData.Defense)
			{
				opponent.ReceiveDamage(this, amount);
			}
		}

		public void Respawn()
		{
			go.transform.position = respawnLocation;
			isDead = false;
			this.enemyData.CurrentHP = this.enemyData.HP;
			UpdateHealthBar();
			go.SetActive(true);
		}
		private void UpdateHealthBar()
		{
			this.statDisplay.SetHealth(this.enemyData.HP, this.enemyData.CurrentHP);
		}

		public void Strike()
		{
			if (this.opponent != null)
			{
				// Character tried to flee
				bool isInRange = IsInRange(this.opponent.gameObject);
				if (!this.opponent.isInCombat && !isInCombat && isInRange)
				{
					InitiateCombat(this.opponent);
				}
				else if (this.opponent.isDead || !isInRange)
				{
					StopFight();
				}
				else
				{
					StartAnimation("striking");
					var oppBounds = this.opponent.GetComponent<BoxCollider>();
					var pos = oppBounds.ClosestPointOnBounds(this.gameObject.transform.position);
					Reposition(pos);

					int amount = GetHitAmount();
					this.DoDamage(amount);
				}
			}
		}

		private int GetHitAmount()
		{
			bool hit = UnityEngine.Random.Range(0, 100) < enemyData.Accuracy;
			int amount = 0;
			if (hit)
			{
				amount = (int)Math.Ceiling(UnityEngine.Random.Range(0.0f, 1.0f) * enemyData.Strength);
			}
			return amount;
		}

		private void Reposition(Vector3 target)
		{
			// If too close, move back
			var distance = Vector3.Distance(this.transform.position, target);
			if (distance < 2f)
			{
				agent.updateRotation = false;
				Vector3 away = Vector3.MoveTowards(this.transform.position, target, -5f);
				agent.ResetPath();
				agent.Warp(away);
			}
			else
			{
				agent.isStopped = false;
				agent.updateRotation = true;
				if (distance > 10f)
				{
					agent.SetDestination(target);
				}
				else
				{
					agent.ResetPath();
				}
			}
			this.transform.LookAt(this.opponent.transform);
		}

		public void StopFight()
		{
			this.isInCombat = false;
			CancelInvoke("Strike");
		}

		public bool IsInRange(GameObject go)
		{
			var thatPos = go.transform.position - new Vector3(0, go.transform.position.y, 0);
			return Vector3.Distance(this.respawnLocation, thatPos) < wanderRadius;
		}

		public bool CanFight(Character o)
		{
			return !isInCombat || (this.opponent == o);
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
	}
}