﻿using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Shop_Shoper_Item : UI_Base
{
    enum Texts 
    {
        NameText,
        GoldText,
        CountText
    }

    enum Images
    {
        ItemImage
    }

    enum Buttons
    {
        BuyButton
    }

    UI_DescriptionBox _descriptionBox = null;
    ItemData _itemData = null;

    bool _isInit = false;
    public override void Init()
    {
        if (_isInit)
            return;

        _descriptionBox = transform.root.GetComponentInChildren<UI_DescriptionBox>();


        Bind<Text>(typeof(Texts));
        Bind<Image>(typeof(Images));
        Bind<Button>(typeof(Buttons));

        BindEvent();
        _isInit = true;
    }

    private void BindEvent()
    {
        // 아이템 설명
        BindEvent(Get<Image>((int)Images.ItemImage).gameObject, (e) =>
        {
            if (_itemData == null)
                return;

            _descriptionBox.WriteNameText(_itemData.name);
            _descriptionBox.WriteDescriptionText(_itemData.description);
            _descriptionBox.ModifyPosition(e);

        }, Define.UIEvent.Enter);
        BindEvent(Get<Image>((int)Images.ItemImage).gameObject, (e) =>
        {
            if (_itemData == null)
                return;

            _descriptionBox.ClosePosition();

        }, Define.UIEvent.Exit);


        // 아이템 사기
        BindEvent(Get<Button>((int)Buttons.BuyButton).gameObject, (e) =>
        {
            if (_itemData == null)
                return;

            C_BuyItem buyItemPacket = new C_BuyItem();
            buyItemPacket.ItemId = _itemData.id;

            int maxCount = 0;
            if (_itemData.itemType == ItemType.Consumable)
            {
                ConsumableData consumableData = _itemData as ConsumableData;
                maxCount = consumableData.maxCount;
            }
            else if (_itemData.itemType == ItemType.Collection)
            {
                CollectionData collectionData = _itemData as CollectionData;
                maxCount = collectionData.maxCount;
            }

            if (maxCount == 0)
            {
                buyItemPacket.Count = 0;
                buyItemPacket.Stackable = false;
            }
            else
            {
                // 몇개를 팔거냐
                buyItemPacket.Count = 1;
                buyItemPacket.Stackable = true;
            }
                

            Managers.Network.Send(buyItemPacket);

        }, Define.UIEvent.LeftClick);

    }

    public void SetInfo(ItemData data)
    {
        Init();

        // 초기화
        _itemData = null;

        Get<Image>((int)Images.ItemImage).sprite = null;
        Get<Text>((int)Texts.NameText).text = "";
        Get<Text>((int)Texts.GoldText).text = "골드: ";
        Get<Text>((int)Texts.CountText).text = "x";

        // 셋팅
        _itemData = data;

        Sprite icon = Managers.Resource.Load<Sprite>(data.iconPath);
        Get<Image>((int)Images.ItemImage).sprite = icon;

        Get<Text>((int)Texts.NameText).text = data.name;
        Get<Text>((int)Texts.GoldText).text = "골드: " + data.gold;

        Get<Text>((int)Texts.CountText).gameObject.SetActive(false);
    }

}
