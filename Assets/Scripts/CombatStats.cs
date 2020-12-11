using Miner.Models;
using Newtonsoft.Json;
using UnityEngine;
using static Character;

namespace Assets.Scripts
{
	public class CurrentStats : MonoBehaviour
	{
		public Progress playerProgress;

		public int health = 4; // 10 - 100
		public int strength = 4; // 10 - 100
		public int accuracy = 4; // 10 - 100
		public int defense = 4; // 10 - 100
		public int attack = 4; // 10 - 100
		public int attackSpeed = 4; // 10 - 100

		public void AddHealth(int amount)
		{
			health += amount;
			int fullHealth = playerProgress.GetFullHeath();
			if (health > fullHealth) health = fullHealth;
		}

		public void UpdateEquipmentStats(EquippedItems equipped)
		{
			if (equipped.Weapon != null)
			{
				var weaponData = JsonConvert.DeserializeObject<WeaponData>(equipped.Weapon.ItemData);
				SetWeapon(weaponData);
			}

			defense = playerProgress.GetDefense() + equipped.GetTotalDefense();
		}

		private void SetWeapon(WeaponData weaponData)
		{
			attack = playerProgress.GetAttack();
			strength = playerProgress.GetStrength();
			accuracy = playerProgress.GetAccuracy();
			attackSpeed = 2;
			if (weaponData != null)
			{
				attack += weaponData.AttackBoost;
				strength +=  weaponData.StrengthBoost;
				accuracy +=  weaponData.AccuracyBoost;
				attackSpeed = weaponData.Speed;
			}
		}
	}
}
