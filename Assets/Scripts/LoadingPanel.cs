using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
	public class LoadingPanel : MonoBehaviour
	{
		public TextMeshProUGUI errorText;
		public TextMeshProUGUI loadingText;
		public Slider loadingSlider;
		public Startup startup;
		public Button btnRetry;
		public void Start()
		{

		}

		public void SetProgress(float progress)
		{
			loadingSlider.value = progress;
		}

		public void ShowError(string message)
		{
			this.errorText.text = message;
			this.errorText.gameObject.SetActive(true);
			this.loadingSlider.gameObject.SetActive(false);
			this.loadingText.gameObject.SetActive(false);
			this.btnRetry.gameObject.SetActive(true);
		}

		public void RetryLoad()
		{
			this.errorText.gameObject.SetActive(false);
			this.btnRetry.gameObject.SetActive(false);
			this.loadingSlider.value = 0;
			this.loadingText.gameObject.SetActive(true);
			this.loadingSlider.gameObject.SetActive(true);
			startup.Retry();
		}
		public void Update()
		{

		}
	}
}
