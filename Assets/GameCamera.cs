using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
	public int xOffset;
	public int zOffset;
	public Transform followTransform;
	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	void FixedUpdate()
	{
		this.transform.position = new Vector3(followTransform.position.x + xOffset, this.transform.position.y, followTransform.position.z + zOffset);
	}
}
