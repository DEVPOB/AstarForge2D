using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamControl : MonoBehaviour
{
    Camera cam;
    GameObject player;
    public Vector3 offset;
    float defaultCamSize;
    private void Start() {
        cam = Camera.main;
        defaultCamSize = cam.orthographicSize;
    }
    private void Update() {
        player = GameObject.FindGameObjectWithTag("Player");
        Vector3 newoffset = player.transform.position + offset;
        this.transform.position = Vector3.Lerp(this.transform.position, newoffset, 1);
        if (Input.GetKey(KeyCode.Space)){
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, 75, 8 * Time.deltaTime);
        }else{
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, defaultCamSize, 8 * Time.deltaTime);

        }
    }
}
