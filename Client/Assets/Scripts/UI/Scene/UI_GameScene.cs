﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScene : UI_Scene
{
    enum Images
    {
        DropPanel
    }

    public UI_Stat StatUI { get; private set; }
    public UI_Inventory InvenUI { get; private set; }
    public UI_StatBar StatBarUI { get; private set; }
    public UI_ShortcutKeys ShortcutKeyUI { get; private set; }
    public UI_Skill SkillUI { get; private set; }
    public UI_QuestPanel QuestUI { get; private set; }
    public UI_CanClassUp ClassUp { get; private set; }

    public UI_DescriptionBox DescriptionBox { get; private set; }

    public override void Init()
    {
        base.Init();

        Bind<Image>(typeof(Images));
        BindEvent();

        StatUI = GetComponentInChildren<UI_Stat>();
        InvenUI = GetComponentInChildren<UI_Inventory>();
        StatBarUI = GetComponentInChildren<UI_StatBar>();
        ShortcutKeyUI = GetComponentInChildren<UI_ShortcutKeys>();
        SkillUI = GetComponentInChildren<UI_Skill>();
        QuestUI = GetComponentInChildren<UI_QuestPanel>();
        DescriptionBox = GetComponentInChildren<UI_DescriptionBox>();
        ClassUp = GetComponentInChildren<UI_CanClassUp>();

        StatUI.gameObject.SetActive(false);
        InvenUI.gameObject.SetActive(false);
        SkillUI.gameObject.SetActive(false);
        ClassUp.gameObject.SetActive(false);
    }

    void BindEvent()
    {
        // 바닥에 퀵슬롯 뿌리면 큇슬롯 비워줌
        BindEvent(Get<Image>((int)Images.DropPanel).gameObject, (e) =>
        {
            UI_ShortcutKey keyInfo = e.pointerDrag.GetComponentInParent<UI_ShortcutKey>();

            if (keyInfo != null)
                keyInfo.RemoveSlot();

        },Define.UIEvent.Drop);

        BindEvent(Get<Image>((int)Images.DropPanel).gameObject, (e) =>
        {
            UI_Inventory_Item itemInfo = e.pointerDrag.GetComponentInParent<UI_Inventory_Item>();

            if (itemInfo != null)
                Managers.Inven.TryToRemoveItem(itemInfo.ItemDbId);

        }, Define.UIEvent.Drop);
    }
}
