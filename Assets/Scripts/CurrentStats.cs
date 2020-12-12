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
		public int Strength = 4; // 10 - 100
		public int Accuracy = 4; // 10 - 100
		public int Defense = 4; // 10 - 100
		public int Attack = 4; // 10 - 100
		public int AttackSpeed = 4; // 10 - 100

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
			AttackSpeed = 2;
			if (weaponData != null)
			{
				Attack += weaponData.AttackBoost;
				Strength +=  weaponData.StrengthBoost;
				Accuracy +=  weaponData.AccuracyBoost;
				AttackSpeed = weaponData.Speed;
			}
		}
	}
}
