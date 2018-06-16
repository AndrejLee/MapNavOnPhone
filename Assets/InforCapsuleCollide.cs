using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InforCapsuleCollide : MonoBehaviour {

    private static Vector3 DefaultPos;

	// Use this for initialization
	void Start () {
        DefaultPos = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "GetImagePlane")
        {
            DisableRigidbody();
            BackToDefaulPos();
        }
    }

    public void BackToDefaulPos()
    {
        this.transform.position = DefaultPos;
    }

    public void EnableRigidbody()
    {
        this.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void DisableRigidbody()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
    }
}
