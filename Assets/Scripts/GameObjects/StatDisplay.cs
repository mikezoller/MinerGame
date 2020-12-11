using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.GameObjects
{
	public class StatDisplay : MonoBehaviour
	{
		private GameObject go;
		private GameObject healthBar;
		private Mesh tileMeshRed;
		private Mesh tileMeshGreen;

		private static Material healthBarRed;
		private static Material healthBarGreen;

		private const float HEALTH_BAR_WIDTH = 6f;
		private const float HEALTH_BAR_HEIGHT = .6f;

		public void Init(Vector3 pos)
		{
			go = new GameObject("StatDisplay");
			go.tag = "StatDisplay";
			go.transform.parent = gameObject.transform;
			go.transform.localPosition = pos;
			AddHealthBar();
		}

		private void AddHealthBar()
		{
			healthBar = new GameObject("HealthBar");
			var greenBar = new GameObject("GreenBar");
			var meshFilterRed = healthBar.AddComponent<MeshFilter>();
			var meshFilterGreen = greenBar.AddComponent<MeshFilter>();
			healthBar.AddComponent<MeshRenderer>();
			greenBar.AddComponent<MeshRenderer>();
			this.tileMeshRed = new Mesh();
			this.tileMeshGreen = new Mesh();
			Vector3[] vertices = new Vector3[]
			{
				 new Vector3(-HEALTH_BAR_WIDTH / 2, - HEALTH_BAR_HEIGHT / 2, 0),
				 new Vector3(HEALTH_BAR_WIDTH / 2, - HEALTH_BAR_HEIGHT / 2, 0),
				 new Vector3( -HEALTH_BAR_WIDTH / 2,  HEALTH_BAR_HEIGHT / 2, 0),
				 new Vector3( HEALTH_BAR_WIDTH / 2,  HEALTH_BAR_HEIGHT / 2, 0)
			};

			Vector2[] uv = new Vector2[]
			{
			new Vector2(0 ,0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1)
			};

			int[] tris = new int[]
			{
			 2,1,0,1,2,3
			};
			this.tileMeshRed.vertices = vertices;
			this.tileMeshRed.uv = uv;
			this.tileMeshRed.triangles = tris;
			tileMeshGreen.vertices = vertices;
			tileMeshGreen.uv = uv;
			tileMeshGreen.triangles = tris;

			if (healthBarRed == null)
			{
				healthBarRed = UnityEngine.Resources.Load<Material>("Materials\\HealthBarRed");
				healthBarGreen = UnityEngine.Resources.Load<Material>("Materials\\HealthBarGreen");
			}
			healthBar.GetComponent<MeshFilter>().mesh = this.tileMeshRed;
			healthBar.GetComponent<MeshRenderer>().material = healthBarRed;
			greenBar.GetComponent<MeshFilter>().mesh = tileMeshGreen;
			greenBar.GetComponent<MeshRenderer>().material = healthBarGreen;
			greenBar.transform.parent = healthBar.transform;
			healthBar.transform.parent = go.transform;
			healthBar.transform.localPosition = new Vector3(0, 0, 0);

			healthBar.SetActive(false);
		}

		public void ShowHealthBar(bool show)
		{
			healthBar.SetActive(show);
		}

		public void SetHealth(float max, float amount)
		{
			float percent = amount / max;
			tileMeshGreen.vertices = new Vector3[]
			{
				 new Vector3(-HEALTH_BAR_WIDTH / 2, - HEALTH_BAR_HEIGHT / 2, 0),
				 new Vector3(HEALTH_BAR_WIDTH / 2 - (HEALTH_BAR_WIDTH - HEALTH_BAR_WIDTH * percent), - HEALTH_BAR_HEIGHT / 2, 0),
				 new Vector3( -HEALTH_BAR_WIDTH / 2,  HEALTH_BAR_HEIGHT / 2, 0),
				 new Vector3( HEALTH_BAR_WIDTH / 2 - (HEALTH_BAR_WIDTH - HEALTH_BAR_WIDTH * percent),  HEALTH_BAR_HEIGHT / 2, 0)
			};
		}

		public void AddHit(int amount)
		{
			var hitTextGo = new GameObject("HitText");
			hitTextGo.tag = "HitText";

			HitText hitText = hitTextGo.AddComponent<HitText>();
			hitText.Init(amount);
			hitTextGo.transform.parent = go.transform;
			hitTextGo.transform.localPosition = new Vector3(0, 0, 0);

			Destroy(hitTextGo, 2);
		}
		public void Start()
		{

		}

		private void FixedUpdate()
		{
			var transform = Camera.main.transform;
			var cameraCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, Camera.main.nearClipPlane));
			transform.position = transform.position + (cameraCenter - gameObject.transform.position);
			go.transform.LookAt(transform, Vector3.up);
			go.transform.Rotate(0, 180, 0);
		}
	}
}
