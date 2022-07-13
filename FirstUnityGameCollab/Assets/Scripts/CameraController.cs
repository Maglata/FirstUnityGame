using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float valueX = 0;
    public float valueY = 1;
    public Camera Camera;
    public int FOV = 60;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position =
            new Vector3(
                player.transform.position.x + valueX,
                player.transform.position.y + valueY,
                transform.position.z); // Camera follows the player with specified offset position

        Camera.fieldOfView = FOV;
    }
}
