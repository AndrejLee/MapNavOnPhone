﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class collaborativecontroller : MonoBehaviour {

    private GameObject socketController;
    Client socket;
    GameObject ScrollerContent;
    GameObject GetImageContent;
    GameObject ImageContent;
    ScrollSnapRect ScrollRect;
    InputField Message;
    bool isDropMode = true;
    bool isUp = false;
    bool isDown = false;
    float accelerationZThreshold = 1.8f;
    List<Texture> TextureList = new List<Texture>();
    string[] FileNameList = { "hcmus2", "nha_i", "nhadieuhanh", "nguoi-hoc"};
    public Text MyText;
    public float durationThreshold = 1.5f;

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;

    // Use this for initialization
    void Start () {
        socketController = GameObject.Find("SocketReceiver");
        socket = socketController.GetComponent<Client>();
        ScrollerContent = GameObject.Find("ScrollerDrop");
        GetImageContent = GameObject.Find("ImageContentGet");
        ImageContent = GameObject.Find("ImageContent");
        Message = GameObject.Find("Message").GetComponent<InputField>();
        ScrollRect = ScrollerContent.GetComponent<ScrollSnapRect>();
        ScrollerContent.SetActive(true);
        GetImageContent.SetActive(false);
        isDropMode = true;
        LoadTexture();
	}

    private void LoadTexture()
    {
        for (int i = 0; i < FileNameList.Length; i++)
        {
            TextureList.Add(Resources.Load(FileNameList[i]) as Texture);
        }
    }

    // Update is called once per frame
    void Update () {

        if (Input.touchCount == 1 && isUp && isDropMode)
        {
            Touch touch = Input.GetTouch(0);

            // Handle finger movements based on touch phase.
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    timePressStarted = Time.time;
                    isPointerDown = true;
                    break;
                case TouchPhase.Ended:
                    isPointerDown = false;
                    break;
            }


            if (isPointerDown && Time.time - timePressStarted > durationThreshold)
            {
                DropImageToTable();
                isPointerDown = false;
            }
        }

        if (Input.touchCount == 2 && isUp && !isDropMode)
        {
            // Store both touches.
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            // Find the position in the previous frame of each touch.
            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            if (deltaMagnitudeDiff > 20)
            {
                GetImageFromTable();
            }
        }

        switch (Input.deviceOrientation)
        {
            case DeviceOrientation.FaceUp:
                isUp = true;
                isDown = false;
                break;
            case DeviceOrientation.FaceDown:
                isDown = true;
                isUp = false;
                break;
        }

        float accelerationZ = Input.acceleration.z;
        if (Math.Abs(accelerationZ) > accelerationZThreshold)
        {
            if (isUp && !isDropMode && accelerationZ < 0)
            {
                Debug.Log("Get Image event detected at value " + accelerationZ);
                MyText.text = "Get Image event detected at value " + accelerationZ;
                GetImageFromTable();
            }
            else if (isDown && isDropMode && accelerationZ > 0)
            {
                Debug.Log("Drop Image event detected at value " + accelerationZ);
                MyText.text = "Drop Image event detected at value " + accelerationZ;
                DropImageToTable();
            }
        }

        if (!isDropMode && Client.isImageReceived && Client.ImageNumber != -1)
        {
            ImageContent.GetComponent<RawImage>().texture = TextureList[Client.ImageNumber];
            Client.isImageReceived = false;
            Client.ImageNumber = -1;
            GameObject.Find("Capsule").GetComponent<InforCapsuleCollide>().EnableRigidbody();
        }
    }

    private void DropImageToTable()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_DROP + ":" + ScrollRect._currentPage + "|" + Message.text + Constant.TOKEN_END);
            Debug.Log("Drop event detected at image " + Time.time);
        }
        else
        {
            Debug.Log("Drop Image fail");
        }
    }

    private void GetImageFromTable()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_GET + Constant.TOKEN_END);
            Debug.Log("Get Image event detected");
        }
        else
        {
            Debug.Log("Get Image fail");
        }
    }

    public void onClickDrop()
    {
        isDropMode = true;
        ScrollerContent.SetActive(true);
        GetImageContent.SetActive(false);
    } 

    public void onClickGet()
    {
        isDropMode = false;
        ScrollerContent.SetActive(false);
        GetImageContent.SetActive(true);
    }

    public void showImage()
    {

    }
}
