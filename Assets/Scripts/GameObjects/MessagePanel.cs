using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagePanel : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ShowMessage(string message)
	{
		this.gameObject.SetActive(true);
		var txt = GameObject.Find("textMessage");
		var text = txt.GetComponent<Text>();
		text.text = message;
	}

	public void HideMessage()
	{
		this.gameObject.SetActive(false);
	}
}
