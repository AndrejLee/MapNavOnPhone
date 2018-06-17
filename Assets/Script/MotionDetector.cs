using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotionDetector : MonoBehaviour
{
    private GameObject socketController;
    public GameObject flagController;
    Client socket;
    public Text warningDebug;
    public Text StatusSendSocket;
    float accelerometerUpdateInterval = 1.0f / 60.0f;
    // The greater the value of LowPassKernelWidthInSeconds, the slower the
    // filtered value will converge towards current input sample (and vice versa).
    float lowPassKernelWidthInSeconds = 1.0f;
    // This next parameter is initialized to 2.0 per Apple's recommendation,
    // or at least according to Brady! ;)
    float shakeDetectionThreshold = 2.0f;
    //float accelerationZThreshold = 2.0f;

    float lowPassFilterFactor;
    Vector3 lowPassValue;
    bool isFlipped = false;
    bool isBarChartShown = false;
    //bool isUp = false;
    //bool isDown = false;
    GameObject scroller;
    void Start()
    {
        socketController = GameObject.Find("SocketReceiver");
        socket = socketController.GetComponent<Client>();
        scroller = GameObject.Find("Scroller");
        scroller.SetActive(false);
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        lowPassValue = Input.acceleration;
    }

    void Update()
    {
        //Vector3 acceleration = Input.acceleration;
        //lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        //Vector3 deltaAcceleration = acceleration - lowPassValue;
        // For Zooming (rejected)

        float accelerationX = Input.acceleration.x;
        if (Math.Abs(accelerationX) >= shakeDetectionThreshold)
        {
            // Perform your "shaking actions" here. If necessary, add suitable
            // guards in the if check above to avoid redundant handling during
            // the same shake (e.g. a minimum refractory period).
            warningDebug.text = "Shake event detected at value " + accelerationX;
            Debug.Log("Shake event detected at time " + Time.time);
            if (Client.isSocketReady)
            {
                socket.SendData(Constant.TOKEN_BEGIN_SHAKE + Constant.TOKEN_END);
                StatusSendSocket.text = "Send shake complete";
            }
            else
            {
                StatusSendSocket.text = "Send shake fail";
            }
        }

        switch (Input.deviceOrientation)
        {
            case DeviceOrientation.FaceUp:
                isFlipped = true;
                isBarChartShown = false;
                //isUp = true;
                //isDown = false;
                break;
            case DeviceOrientation.FaceDown:
                //isDown = true;
                //isUp = false;
                if (isFlipped)
                {
                    ChangeMapType();
                    isFlipped = false;
                }
                break;
            case DeviceOrientation.Portrait:
                //isUp = false;
                //isDown = false;
                if (!isBarChartShown)
                {
                    showBarChart();
                    isBarChartShown = true;
                }
                break;
        }

        /*float accelerationZ = Input.acceleration.z;
        if (Math.Abs(accelerationZ) > accelerationZThreshold)
        {
            if (isUp)
            {
                warningDebug.text = "Get Image event detected at value " + accelerationZ;
                Debug.Log("Get Image event detected at value " + accelerationZ);
                GetImageFromTable();
            }
            else if (isDown)
            {
                warningDebug.text = "Drop Image event detected at value " + accelerationZ;
                Debug.Log("Drop Image event detected at value " + accelerationZ);
                DropImageToTable();
            }
        }*/
    }

    public void CloseScroller()
    {
        scroller.SetActive(false);
    }

    /*private void DropImageToTable()
    {
        
    }

    private void GetImageFromTable()
    {
        
    }*/

    private void ZoomMap(float zoomVal)
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_ZOOM + ":" + zoomVal + Constant.TOKEN_END);
            StatusSendSocket.text = "Send zoom complete";
        }
        else
        {
            StatusSendSocket.text = "Send zoom fail";
        }
    }

    private void ZoomMapDefault()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_ZOOMDEFAULT + Constant.TOKEN_END);
            StatusSendSocket.text = "Send zoom default complete";
        }
        else
        {
            StatusSendSocket.text = "Send zoom default fail";
        }
    }

    private void showBarChart()
    {
        scroller.SetActive(true);
    }

    private void ChangeMapType()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_FLIP + Constant.TOKEN_END);
            StatusSendSocket.text = "Send flip complete";

            Debug.Log("Changemaptype");
            GameObject map = GameObject.Find("Map");
            map.SendMessage("ChangeMapType");
        }
        else
        {
            StatusSendSocket.text = "Send flip fail";
        }
    }

    public void FreezeData()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_FREEZE + Constant.TOKEN_END);
            StatusSendSocket.text = "Send freeze complete";
        }
        else
        {
            StatusSendSocket.text = "Send freeze fail";
        }
    }

    public void SetFlag()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_SET_FLAG + Constant.TOKEN_END);
            flagController.GetComponent<FlagController>().AddFlag();
            StatusSendSocket.text = "Send flag complete";
        }
        else
        {
            StatusSendSocket.text = "Send flag fail";
        }
    }

    public void Analyze()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_ANALYZE + Constant.TOKEN_END);
            StatusSendSocket.text = "Send analyze complete";
        }
        else
        {
            StatusSendSocket.text = "Send analyze fail";
        }
    }
}
