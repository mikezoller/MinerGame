using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Miner.Models
{
    [System.Serializable]
    public class Item
    {
        public int id;
        public string title;
        public string description;
        public bool stackable = true;
        public Item() { }
        public Item(int id, string title, string description, bool stackable)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.stackable = stackable;
        }

        public Item(Item item)
        {
            this.id = item.id;
            this.title = item.title;
            this.description = item.description;
        }
    }
}