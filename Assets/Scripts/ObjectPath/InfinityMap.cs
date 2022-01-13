using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityMap : MonoBehaviour
{
    Transform tf;
    public Transform cam;

    public float speed;


    private void Start()
    {
        tf = gameObject.transform;
    }
    private void Update()
    {
        tf.position = new Vector3(tf.position.x - speed * Time.deltaTime, tf.position.y, tf.position.z);
        if (tf.position.x < cam.transform.position.x -100)
            tf.position = new Vector3(tf.position.x + 307.2f, tf.position.y, tf.position.z);
    }
}
