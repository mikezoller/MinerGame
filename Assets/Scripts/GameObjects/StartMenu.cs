using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Miner.GameObjects
{
    public class StartMenu : MonoBehaviour
    {
        public delegate void StartGameEvent(GameObject o);
        public event StartGameEvent StartGame;
        public void StartGameClicked()
        {
            if (StartGame != null)
            {
                StartGame.Invoke(gameObject);
            }
        }
    }
}