using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class itween_demo : MonoBehaviour {
  //  Vector3[] path;
   public Transform t;
   public Transform s;
   public Transform b;
    // Use this for initialization
    void Start () {
       
	}

    // Update is called once per frame
    void Update() {
       
        
    }
     void OnDrawGizmos() {
        iTween.DrawPath( new Vector3[]  { t.position, s.position , b.position } , Color.magenta);
    }
}
