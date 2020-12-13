using Assets.Scripts;
using Miner;
using Miner.Communication;
using Miner.Helpers;
using Miner.Models;
using System;
using System.Collections;
using UnityEngine;

public class Startup : MonoBehaviour
{
	private bool characterLoaded;
	public GameManager gameManager;
	public LoadingPanel panelLoading;
	public Character character;
	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine(InitializeData());
	}
	private float GetProgress()
	{
		float progress = 0;
		float step = 1f / 5.0f;

		if (ItemDatabase.initialized) progress += step;
		if (EnemyDatabase.initialized) progress += step;
		if (RecipeDatabase.initialized) progress += step;
		if (ResourceActionDatabase.initialized) progress += step;
		if (characterLoaded) progress += step;

		return progress;
	}
	private IEnumerator InitializeData()
	{
		bool failed = false;
		StartCoroutine(ItemDatabase.BuildItemDatabase(() => failed = true));
		StartCoroutine(EnemyDatabase.Initialize(() => failed = true));
		StartCoroutine(RecipeDatabase.Initialize(() => failed = true));
		StartCoroutine(ResourceActionDatabase.Initialize(() => failed = true));
		LoadPlayerData(() => failed = true);

		yield return new WaitUntil(() =>
		{
			panelLoading.SetProgress(GetProgress());
			return (ItemDatabase.initialized
&& EnemyDatabase.initialized
&& RecipeDatabase.initialized
&& ResourceActionDatabase.initialized
&& characterLoaded)
|| failed;
		});

		if (failed)
		{
			panelLoading.ShowError("Error connecting to server!");
		}
		else
		{
			panelLoading.SetProgress(1f);
			gameManager.gameObject.SetActive(true);
			panelLoading.gameObject.SetActive(false);
		}
	}
	public void Retry()
	{
		StartCoroutine(InitializeData());
	}
	public void LoadPlayerData(Action fail = null)
	{
		StartCoroutine(PlayersApi.Get("mwnzoller", (user, err) =>
		{
			if (err != null)
			{
				Debug.LogError(err);
				fail?.Invoke();
			}
			else
			{
				character.playerData = user;
				character.initialPosition = new Vector3((float)user.LastLocation.X, (float)user.LastLocation.Y, (float)user.LastLocation.Z);
				gameManager.character = character;

				characterLoaded = true;
			}
		}));
	}
}
