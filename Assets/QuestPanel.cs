using Assets.Scripts.GameObjects;
using Assets.Scripts.Helpers;
using Assets.Scripts.Models.Quests;
using Miner.GameObjects;
using Miner.Helpers;
using Miner.Scripts;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
	public ListCreator list;
	public List<QuestProgress> progress;
	public Character character;
    // Start is called before the first frame update
    void Start()
    {
		var player = GameObject.FindGameObjectWithTag("Player");
		character = player.GetComponent<Character>();
		Refresh();
	}


	public void Refresh()
	{
		progress = character.playerData.Progress.QuestProgress;
		var allQuests = QuestDatabase.Quests;

		List<ListItemInfo> infos = new List<ListItemInfo>();
		foreach (var quest in allQuests)
		{
			var pqp = QuestManager.GetQuestProgress(character, quest.Id);
			infos.Add(new ListItemInfo()
			{
				Text = quest.Name,
				FontColor = pqp.Complete ? Color.green : Color.yellow
			});
		}
		list.SetItems(infos);
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
