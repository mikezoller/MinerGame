using Assets.Scripts.Models.Quests;
using Miner.Communication;
using Miner.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Miner.Communication.PlayersApi;

namespace Assets.Scripts.Helpers
{
	public static class QuestManager
	{

		public static IEnumerator UpdateQuest(int questId, QuestProgress progress, Action<bool, UpdateQuestResponse, string> done = null)
		{
			return PlayersApi.UpdateQuestProgress("mwnzoller", progress, done);
		}

		public static QuestProgress GetQuestProgress(Character character, int questId)
		{
			var progress = character.playerData.Progress.QuestProgress;
			var baseQuest = QuestDatabase.GetQuest(questId);
			var qp = progress.Find(x => x.QuestId == questId);

			if (qp == null)
			{
				qp = new QuestProgress()
				{
					Checkpoints = new List<CheckpointProgress>(),
					QuestId = questId,
				};
				foreach (var qcp in baseQuest.Checkpoints)
				{
					qp.Checkpoints.Add(new CheckpointProgress() { QuestId = baseQuest.Id, Number = qcp.Number });
				}
				progress.Add(qp);
			}

			return qp;
		}
	}
}
