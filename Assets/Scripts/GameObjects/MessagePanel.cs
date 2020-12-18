using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour
{
	private Action accept;
	private Action cancel;

	public Button btnYes;
	public Button btnNo;
	public Button btnOk;
	public enum MessageType
	{
		OK,
		YesNo
	}
    // Start is called before the first frame update
    void Start()
    {
	}

    // Update is called once per frame
    void Update()
    {
        
    }


	public void ShowMessage(string message, MessageType type, Action accept = null, Action cancel = null)
	{
		var txt = this.gameObject.transform.Find("textMessage");
		var text = txt.GetComponent<Text>();
		text.text = message;

		switch (type)
		{
			case MessageType.YesNo:
				btnYes.gameObject.SetActive(true);
				btnNo.gameObject.SetActive(true);
				btnOk.gameObject.SetActive(false);
				break;
			case MessageType.OK:
			default:
				btnYes.gameObject.SetActive(false);
				btnNo.gameObject.SetActive(false);
				btnOk.gameObject.SetActive(true);

				break;
		}

		this.accept = accept;
		this.cancel = cancel;
		this.gameObject.SetActive(true);
	}

	public void YesClick()
	{
		this.accept?.Invoke();
		HideMessage();
	}

	public void NoClick()
	{
		this.cancel?.Invoke();
		HideMessage();
	}

	public void OKClick()
	{
		this.accept?.Invoke();
		HideMessage();
	}

	public void HideMessage()
	{
		this.gameObject.SetActive(false);
	}
}
