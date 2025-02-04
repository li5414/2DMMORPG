﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginScene : BaseScene
{
    UI_LoginScene _sceneUI;

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;

        Managers.Web.BaseUrl = "https://localhost:5001/api";

        Screen.SetResolution(1440, 810, false);

        _sceneUI = Managers.UI.ShowSceneUI<UI_LoginScene>();

        UI_LoginScene loginSceneUI = Managers.UI.SceneUI as UI_LoginScene;
        loginSceneUI.ChangeUI.ArrivedRoom();
    }

    public override void Clear()
    {
        
    }

}
