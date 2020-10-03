using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Miner.Helpers
{
    public class CameraDrag : MonoBehaviour
    {
        public float dragSpeed = 2;
        private Vector3 dragOrigin;
        public bool Paused;
        void Update()
        {
            if (!Paused)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    dragOrigin = Input.mousePosition;
                    return;
                }

                if (!Input.GetMouseButton(0)) return;
                dragSpeed = Camera.main.GetComponent<Camera>().orthographicSize / 10.0f;
                Vector3 pos = this.GetComponent<Camera>().ScreenToViewportPoint(Input.mousePosition - dragOrigin);
                Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
                var newPos = transform.position - move;

                if (newPos.x > 4)
                {
                    newPos.x = 4;
                }
                else if (newPos.x < -10)
                {
                    newPos.x = -10;
                }
                if (newPos.y > 0.5f)
                {
                    newPos.y = 0.5f;
                }
                else if (newPos.y < -6)
                {
                    newPos.y = -6;
                }
                transform.position = newPos;
            }
        }
    }
}