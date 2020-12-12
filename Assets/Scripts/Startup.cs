using Miner;
using Miner.Communication;
using Miner.Helpers;
using Miner.Models;
using System.Collections;
using UnityEngine;

public class Startup : MonoBehaviour
{
	private bool characterLoaded;
	public GameManager gameManager;
	public GameObject panelLoading;
	public Character character;
	// Start is called before the first frame update
	void Start()
	{
		StartCoroutine(InitializeData());
	}

	private IEnumerator InitializeData()
	{
		StartCoroutine(ItemDatabase.BuildItemDatabase());
		StartCoroutine(EnemyDatabase.Initialize());
		StartCoroutine(RecipeDatabase.Initialize());
		StartCoroutine(ResourceActionDatabase.Initialize());
		LoadPlayerData();

		yield return new WaitUntil(() => ItemDatabase.initialized
			&& EnemyDatabase.initialized
			&& RecipeDatabase.initialized
			&& ResourceActionDatabase.initialized
			&& characterLoaded);

		gameManager.gameObject.SetActive(true);
		panelLoading.SetActive(false);
	}

	public void LoadPlayerData()
	{
		StartCoroutine(PlayersApi.Get("mwnzoller", (user, err) =>
		{
			if (err != null)
			{
				Debug.LogError(err);
				// TODO: Show error message
			}
			else
			{
				character.playerData = user;
				character.playerData.CurrentStats.Health = 10;
				character.initialPosition = new Vector3((float)user.LastLocation.X, (float)user.LastLocation.Y, (float)user.LastLocation.Z);
				gameManager.character = character;

				characterLoaded = true;				
			}
		}));
	}
}
