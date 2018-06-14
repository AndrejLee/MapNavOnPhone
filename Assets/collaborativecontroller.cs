﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class collaborativecontroller : MonoBehaviour {

    private GameObject socketController;
    Client socket;
    GameObject ScrollerContent;
    GameObject GetImageContent;
    GameObject ImageContent;
    ScrollSnapRect ScrollRect;
    bool isDropMode = true;
    bool isUp = false;
    bool isDown = false;
    float accelerationZThreshold = 1.5f;
    List<Texture> TextureList = new List<Texture>();
    string[] FileNameList = { "boku_no_hero", "gintama", "kuroko", "nanatsu_taizai", "one_punch_man" };
    public Text MyText;

	// Use this for initialization
	void Start () {
        socketController = GameObject.Find("SocketReceiver");
        socket = socketController.GetComponent<Client>();
        ScrollerContent = GameObject.Find("ScrollerDrop");
        GetImageContent = GameObject.Find("ImageContentGet");
        ImageContent = GameObject.Find("ImageContent");
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
        }
    }

    private void DropImageToTable()
    {
        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_DROP + ":" + ScrollRect._currentPage + Constant.TOKEN_END);
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
}