using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    Transform playerTransform;
    Vector3 offset;
    // Start is called before the first frame update
    private void Awake()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        offset = transform.position - playerTransform.position;
    }

    void Start()
    {

    }

    private void LateUpdate()
    {
        transform.position = playerTransform.position + offset;
    }
}
