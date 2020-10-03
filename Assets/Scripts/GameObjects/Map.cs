using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Miner.GameObjects
{
    public class Map : MonoBehaviour
    {
		public Tilemap environment;
        // Start is called before the first frame update
        void Start()
        {
			environment.RefreshAllTiles();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}