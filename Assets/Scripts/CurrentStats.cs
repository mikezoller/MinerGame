using Miner.Models;
using Newtonsoft.Json;
using UnityEngine;
using static Character;

namespace Assets.Scripts
{
	public class CurrentStats : Component
	{
		public Progress playerProgress;

		public int Health = 4; // 10 - 100
		public float Strength = 4; // 10 - 100
		public float Accuracy = 4; // 10 - 100
		public float Defense = 4; // 10 - 100
		public float Attack = 4; // 10 - 100

		public void AddHealth(int amount)
		{
			Health += amount;
			int fullHealth = playerProgress.GetFullHeath();
			if (Health > fullHealth) Health = fullHealth;
		}

		public void UpdateEquipmentStats(EquippedItems equipped)
		{
			if (equipped.Weapon != null)
			{
				var weaponData = JsonConvert.DeserializeObject<WeaponData>(equipped.Weapon.ItemData);
				SetWeapon(weaponData);
			}

			Defense = playerProgress.GetDefense() + equipped.GetTotalDefense();
		}

		private void SetWeapon(WeaponData weaponData)
		{
			Attack = playerProgress.GetAttack();
			Strength = playerProgress.GetStrength();
			Accuracy = playerProgress.GetAccuracy();
			if (weaponData != null)
			{
				Attack += weaponData.AttackBuff;
				Strength +=  weaponData.StrengthBuff;
				Accuracy +=  weaponData.AccuracyBuff;
			}
		}
	}
}
