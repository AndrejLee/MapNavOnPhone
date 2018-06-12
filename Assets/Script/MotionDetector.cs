﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MotionDetector : MonoBehaviour
{
    private GameObject socketController;
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
    float accelerationZThreshold = 1.5f;

    float lowPassFilterFactor;
    Vector3 lowPassValue;
    bool isFlipped = false;
    bool isBarChartShown = false;
    void Start()
    {
        socketController = GameObject.Find("SocketReceiver");
        socket = socketController.GetComponent<Client>();

        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        shakeDetectionThreshold *= shakeDetectionThreshold;
        lowPassValue = Input.acceleration;
    }

    void Update()
    {
        Vector3 acceleration = Input.acceleration;
        lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - lowPassValue;
        // For Zooming (rejected)
        //float accelerationZ = Input.acceleration.x;
        /*if (Math.Abs(accelerationZ) > accelerationZThreshold)
        {
            warningDebug.text = "Zoom event detected at value " + accelerationZ;
            Debug.Log("Zoom event detected at time " + Time.time);
            float zoomVal = 0;
            if (accelerationZ > 0 ? true : false)
            {
                zoomVal = accelerationZ;
                ZoomMap(zoomVal);
            } else
            {
                zoomVal = accelerationZ;
                ZoomMap(zoomVal);
            }
        } else */
        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            // Perform your "shaking actions" here. If necessary, add suitable
            // guards in the if check above to avoid redundant handling during
            // the same shake (e.g. a minimum refractory period).
            warningDebug.text = "Shake event detected at value " + deltaAcceleration.sqrMagnitude;
            Debug.Log("Shake event detected at time " + Time.time);
            if (Client.isSocketReady)
            {
                socket.SendData(Constant.TOKEN_BEGIN_SHAKE + Constant.TOKEN_END);
                StatusSendSocket.text = "Send shake complete";
            } else
            {
                StatusSendSocket.text = "Send shake fail";
            }
        }

        switch (Input.deviceOrientation)
        {
            case DeviceOrientation.FaceUp:
                isFlipped = true;
                isBarChartShown = false;
                break;
            case DeviceOrientation.FaceDown:
                if (isFlipped)
                {
                    ChangeMapType();
                    isFlipped = false;
                }
                break;
            case DeviceOrientation.Portrait:
                if (!isBarChartShown)
                {
                    showBarChart();
                    isBarChartShown = true;
                }                
                break;
        }
    }

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
        throw new NotImplementedException();
    }

    private void ChangeMapType()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_FLIP + Constant.TOKEN_END);
            StatusSendSocket.text = "Send flip complete";
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
}