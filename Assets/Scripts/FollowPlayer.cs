using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Vector3 offset;
    [SerializeField] float smoothSpeed = 0.125f;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        Vector3 followPos = player.position + offset;
        Vector3 smoothedSpeed = Vector3.Lerp(transform.position, followPos, smoothSpeed * Time.deltaTime);
        transform.position = smoothedSpeed;
    }
}
