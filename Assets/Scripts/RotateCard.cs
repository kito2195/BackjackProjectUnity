using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCard : MonoBehaviour {

    public Vector3 rotation;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.Rotate(rotation * Time.deltaTime);
    }
}
