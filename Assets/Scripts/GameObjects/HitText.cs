using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameObjects
{
	public class HitText : MonoBehaviour
	{
		private int floatDir = 1;
		private float floatPos = 0;
		private Vector3 initialPos;
		public void Start()
		{

		}

		public void Init(int amount)
		{
			TextMesh textMesh = gameObject.AddComponent<TextMesh>();
			textMesh.text = amount.ToString();
			initialPos = gameObject.transform.position;
		}
		private void FixedUpdate()
		{
			var transform = Camera.main.transform;
			gameObject.transform.LookAt(transform);
			gameObject.transform.Rotate(0, 180, 0);

			float floatAmount = 0.1f;
			float floatRadius = 1.0f;
			floatPos = floatAmount * floatDir;
			if (Math.Abs(gameObject.transform.position.x - initialPos.x) > floatRadius)
			{
				floatDir = -floatDir;
			}
			floatPos = floatAmount * floatDir;
			gameObject.transform.position += new Vector3(floatPos, 0.1f, floatPos);
		}
	}
}
