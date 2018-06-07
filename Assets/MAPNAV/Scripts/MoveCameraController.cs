using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MoveCameraController : MonoBehaviour {

    public static bool isLoaded = false;
    private new GameObject camera;
    private const int frame = 60;
    private int i = 0;
    private int move = 0;
    public Text text;
    // Use this for initialization
    void Start () {
        camera = GameObject.Find("Camera");
    }
	
	// Update is called once per frame
	void Update () {
        if (isLoaded)
        {
            Vector2 receivedData = Client.getReceivedCoord();
            text.text = receivedData.x + "\t" + receivedData.y;
            MoveCamera(receivedData.x, receivedData.y, -12f);
        }
	}

    private void MoveCamera(float x, float y, float z)
    {
        Vector3 newPos = new Vector3(x, y, z);
        //camera.transform.localPosition = Vector3.Lerp(camera.transform.localPosition, newPos, Time.deltaTime * 10f);
        camera.transform.localPosition = newPos;
    }
}
