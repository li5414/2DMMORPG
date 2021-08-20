﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
    static Managers s_instance;
    static Managers Instance { get { Init(); return s_instance; } }

    #region Contents
    InventoryManager _inven = new InventoryManager();
    NetworkManager _network = new NetworkManager();
    MapManager _map = new MapManager();
    ObjectManager _obj = new ObjectManager();


    public static InventoryManager Inven { get { return Instance._inven; } }
    public static NetworkManager Network { get { return Instance._network; } }
    public static MapManager Map { get { return Instance._map; } }
    public static ObjectManager Object { get { return Instance._obj; } }
    #endregion

    #region Core
    DataManager _data = new DataManager();
    ResourceManager _resource = new ResourceManager();
    UIManager _ui = new UIManager();


    public static DataManager Data { get { return Instance._data; } }
    public static ResourceManager Resource { get { return Instance._resource; } }
    public static UIManager UI { get { return Instance._ui; } }
    #endregion

    void Start()
    {
        Init();
    }

    void Update()
    {
        _network.Update();
    }

    static void Init()
    {
        if(s_instance == null)
        {
            GameObject go = GameObject.Find("@Managers");
            if (go == null)
            {
                go = new GameObject { name = "@Managers" };
                go.AddComponent<Managers>();
            }

            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._network.Init();
            Data.LoadData();
        }
    }

    public static void Clear()
    {
        UI.Clear();
    }
}
