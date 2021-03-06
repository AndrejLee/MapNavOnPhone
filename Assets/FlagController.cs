﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlagController : MonoBehaviour
{
    private GameObject socketController;
    Client socket;
    List<Vector3> listFlagPosition = new List<Vector3>();
    List<GameObject> listFlags = new List<GameObject>();
    GameObject FlagObject;
    //Vector2 curPos;
    GameObject myCamera;
    GameObject ClearFlagBtn;

    // Use this for initialization
    void Start()
    {
        myCamera = GameObject.Find("Camera");
        FlagObject = GameObject.Find("flagobject");
        socketController = GameObject.Find("SocketReceiver");
        ClearFlagBtn = GameObject.Find("ClearFlag");
        socket = socketController.GetComponent<Client>();
        FlagObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (listFlagPosition.Count > 0)
        {
            ClearFlagBtn.SetActive(true);
        } else
        {
            ClearFlagBtn.SetActive(false);
        }

        for (int i = 0; i < listFlagPosition.Count; i++)
        {
            if (listFlagPosition[i].z == 1)
            {
                continue;
            }
            listFlagPosition[i] = new Vector3(listFlagPosition[i].x, listFlagPosition[i].y, 1);
            listFlags[i].SetActive(true);
            listFlags[i].transform.localPosition = new Vector2(listFlagPosition[i].x, listFlagPosition[i].y);
        }
    }

    public void AddFlag()
    {
        Vector2 curPos = Client.getReceivedCoord();
        listFlagPosition.Add(new Vector3(curPos.x, curPos.y, 0));

        GameObject newFlag = cloneFlag();
        newFlag.SetActive(false);
        listFlags.Add(newFlag);
    }

    private GameObject cloneFlag()
    {
        GameObject flag = UnityEngine.Object.Instantiate(FlagObject);
        flag.transform.SetParent(transform);
        flag.SetActive(true);
        flag.transform.localRotation = Quaternion.Euler(0, 90, 0);
        flag.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

        return flag;
    }

    public void ClearFlag()
    {
        listFlagPosition.Clear();
        foreach(GameObject obj in listFlags)
        {
            GameObject.Destroy(obj);
        }
        listFlags.Clear();

        if (Client.isSocketReady)
        {
            socket.SendData(Constant.TOKEN_BEGIN_CLEAR_FLAG + Constant.TOKEN_END);
        }
    }
}
