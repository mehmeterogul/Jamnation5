using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    Transform player;
    [SerializeField] float catchSpeed = 2;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 smoothedSpeed = Vector3.Lerp(transform.position, player.position, catchSpeed * Time.deltaTime);
        Vector3 newPos = new Vector3(smoothedSpeed.x, transform.position.y, transform.position.z);
        transform.position = newPos;
    }
}
