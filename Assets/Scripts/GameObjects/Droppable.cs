﻿using Miner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.GameObjects
{
	public class Droppable
	{
		public InventoryItem Item { get; set; }
		public int Probability { get; set; }
	}
}
