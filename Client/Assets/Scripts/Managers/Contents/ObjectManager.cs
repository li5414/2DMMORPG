﻿using Data;
using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ObjectManager
{
    public MyPlayerController MyPlayer { get; set; }
    Dictionary<int, GameObject> _objects = new Dictionary<int, GameObject>();
    Dictionary<Vector2Int, List<ItemController>> _items = new Dictionary<Vector2Int, List<ItemController>>(); // item 여부
    Dictionary<int, GameObject> _npcs = new Dictionary<int, GameObject>(); // Npc만 빠르게 서칭
    private Dictionary<int, Obstacle> _obstacles = new Dictionary<int, Obstacle>();

    public GameObject ItemRoot
    {
        get
        {
            GameObject root = GameObject.Find("@ItemRoot");
            if (root == null)
            {
                root = new GameObject { name = "@ItemRoot" };
                root.AddComponent<SortingGroup>().sortingOrder = 5;
            }

            return root;
        }
    }


    public ItemController FindItemFromGround(Vector3Int cellPos)
    {
        Vector2Int pos = new Vector2Int(cellPos.x, cellPos.y);

        List<ItemController> _groundItems = null;
        if (_items.TryGetValue(pos, out _groundItems) == false)
            return null;

        if (_groundItems.Count <= 0)
            return null;

        // 해당 좌표의 제일 마지막에 떨어진 아이템 가져옴
        ItemController item = _groundItems[_groundItems.Count - 1];
        if (item == null)
            return null;

        // 리스트에서 지우는건 서버에서 허락맡고

        return item;
    }

    public void Add(ObjectInfo info, bool myPlayer = false)
    {
        if (MyPlayer != null && MyPlayer.Id == info.ObjectId)
            return;
        if (_objects.ContainsKey(info.ObjectId))
            return;

        GameObjectType type = GetObjectTypeById(info.ObjectId);

        switch (type)
        {
            case GameObjectType.Player:
                {
                    if (myPlayer)
                    {
                        GameObject go = Managers.Resource.Instantiate("Creature/MyPlayer");
                        go.name = info.Name;
                        _objects.Add(info.ObjectId, go);

                        MyPlayer = go.GetComponent<MyPlayerController>();
                        MyPlayer.Name = info.Name;
                        MyPlayer.Id = info.ObjectId;
                        MyPlayer.PosInfo = info.PosInfo;
                        MyPlayer.Stat = info.StatInfo;
                        MyPlayer.SyncPos();

                        // 퀘스트 여부 체크
                        Managers.Quest.CheckCondition();
                        Managers.Inven.Gold = info.Gold;
                        // 내 플레이어가 입장 했으면 이제 검은색 화면 풀어준다
                        UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
                        gameSceneUI.ChangeUI.ArrivedRoom();
                    }
                    else
                    {
                        GameObject go = Managers.Resource.Instantiate("Creature/Player");
                        go.name = info.Name;
                        _objects.Add(info.ObjectId, go);

                        PlayerController pc = go.GetComponent<PlayerController>();
                        pc.Name = info.Name;
                        pc.Id = info.ObjectId;
                        pc.PosInfo = info.PosInfo;
                        pc.Stat = info.StatInfo;
                        MyPlayer.SyncPos();

                        // 컨디션
                        if(info.ConditionInfos.Count > 0)
                        {
                            Condition condition = pc.GetComponent<Condition>();
                            foreach (ConditionInfo conditionInfo in info.ConditionInfos)
                            {
                                condition.UpdateCondition(conditionInfo.ConditionType, info.ObjectId, conditionInfo.Id, conditionInfo.Time,
                                    conditionInfo.AttackSpeedValue, conditionInfo.MoveSpeedValue);
                            }
                        }


                    }
                }
                break;
            case GameObjectType.Monster:
                {
                    MonsterData monsterData = null;
                    Managers.Data.MonsterDict.TryGetValue(info.TemplateId, out monsterData);

                    GameObject go = Managers.Resource.Instantiate($"Creature/{monsterData.prefabPath}");
                    go.name = "Monster";
                    _objects.Add(info.ObjectId, go);

                    MonsterController mc = go.GetComponent<MonsterController>();
                    mc.Name = monsterData.name;
                    mc.PosInfo = info.PosInfo;
                    mc.Stat = info.StatInfo;
                    mc.SyncPos();

                    // 컨디션
                    if (info.ConditionInfos.Count > 0)
                    {
                        Condition condition = mc.GetComponent<Condition>();
                        foreach (ConditionInfo conditionInfo in info.ConditionInfos)
                        {
                            condition.UpdateCondition(conditionInfo.ConditionType, info.ObjectId, conditionInfo.Id, conditionInfo.Time,
                                conditionInfo.AttackSpeedValue, conditionInfo.MoveSpeedValue);
                        }
                    }

                }
                break;
            case GameObjectType.Projectile:
                {
                    if(info.TemplateId == 1002)
                    {
                        Skill skillData = null;
                        Managers.Data.SkillDict.TryGetValue(1002, out skillData);

                        GameObject go = Managers.Resource.Instantiate(skillData.projectile.prefab);
                        go.name = "Arrow";
                        _objects.Add(info.ObjectId, go);

                        ArrowController ac = go.GetComponent<ArrowController>();
                        ac.PosInfo = info.PosInfo;
                        ac.Stat = info.StatInfo;
                        ac.SyncPos();
                    }
                    else if(info.TemplateId == 2001)
                    {
                        Skill skillData = null;
                        Managers.Data.SkillDict.TryGetValue(2001, out skillData);

                        GameObject go = Managers.Resource.Instantiate(skillData.projectile.prefab);
                        go.name = "IceBall";
                        _objects.Add(info.ObjectId, go);

                        ArrowController ac = go.GetComponent<ArrowController>();
                        ac.PosInfo = info.PosInfo;
                        ac.Stat = info.StatInfo;
                        ac.SyncPos();
                    }

                    
                }
                break;
            case GameObjectType.Summoning:
                {
                    if (info.TemplateId == 2003)
                    {
                        Skill skillData = null;
                        Managers.Data.SkillDict.TryGetValue(2003, out skillData);

                        GameObject go = Managers.Resource.Instantiate(skillData.summoning.prefab);
                        go.name = "PoisonSmoke";

                        // 이펙트 크기 조정
                        int skillLevel = info.StatInfo.Level;
                        int radian = skillData.summoning.summoningPointInfos[skillLevel].radian;
                        go.transform.localScale = new Vector3(
                            radian,
                            radian,
                            1);

                        _objects.Add(info.ObjectId, go);

                        SummoningController sc = go.GetComponent<SummoningController>();
                        sc.PosInfo = info.PosInfo;
                        sc.Stat = info.StatInfo;
                        sc.SyncPos();
                    }
                    else if (info.TemplateId == 2005)
                    {
                        Skill skillData = null;
                        Managers.Data.SkillDict.TryGetValue(2005, out skillData);

                        GameObject go = Managers.Resource.Instantiate(skillData.summoning.prefab);
                        go.name = "HealZone";

                        // 이펙트 크기 조정
                        int skillLevel = info.StatInfo.Level;
                        int radian = skillData.summoning.summoningPointInfos[skillLevel].radian;
                        go.transform.localScale = new Vector3(
                            radian,
                            radian,
                            1);

                        _objects.Add(info.ObjectId, go);

                        SummoningController sc = go.GetComponent<SummoningController>();
                        sc.PosInfo = info.PosInfo;
                        sc.Stat = info.StatInfo;
                        sc.SyncPos();
                    }

                }
                break;
            case GameObjectType.Item:
                {
                    ItemData itemData = null;
                    Managers.Data.ItemDict.TryGetValue(info.ItemInfo.TemplateId, out itemData);
                    if (itemData == null)
                        return;

                    GameObject go = Managers.Resource.Instantiate(itemData.iconPath, ItemRoot.transform);
                    go.name = itemData.name;


                    ItemController ic = go.GetComponent<ItemController>();
                    ic.itemInfo = info.ItemInfo;
                    ic.PosInfo = info.PosInfo;
                    ic.Id = info.ObjectId;
                    ic.SyncPos();

                    _objects.Add(info.ObjectId, go);

                    List<ItemController> itemList = new List<ItemController>();
                    
                    Vector2Int pos = new Vector2Int(ic.PosInfo.PosX, ic.PosInfo.PosY);

                    if (!_items.ContainsKey(pos))
                        _items.Add(pos, itemList);
                    if (_items.TryGetValue(pos, out itemList) == false)
                        _items.Add(pos, itemList);

                    itemList.Add(ic);
                }
                break;
        }        
    }

    public void Remove(int id)
    {
        //if (MyPlayer != null && MyPlayer.Id == id)
        //    return;
        if (_objects.ContainsKey(id) == false)
            return;

        GameObject go = FindById(id);
        if (go == null)
            return;

        _objects.Remove(id);

        ItemController item = go.GetComponent<ItemController>();
        if (item != null)
        {
            Vector2Int pos = new Vector2Int(item.CellPos.x, item.CellPos.y);
            List<ItemController> groundItems = null;
            if (_items.TryGetValue(pos, out groundItems) == false)
                return;

            for (int i = 0; i < groundItems.Count; i++)
            {
                ItemController groundItem = groundItems[i];
                if (groundItem.Id == id)
                    groundItems.Remove(groundItem);
            }

        }

        BanBanController bc = go.GetComponent<BanBanController>();
        if(bc != null)
        {
            UI_GameScene gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
            gameSceneUI.BossHpUI.gameObject.SetActive(false);
        }

        Managers.Resource.Destroy(go);
    }

    public GameObject FindById(int id)
    {
        GameObject go = null;
        _objects.TryGetValue(id, out go);
        return go;
    }

    public GameObject FindCollsion(Vector3Int cellPos)
    {
        foreach (GameObject obj in _objects.Values)
        {
            BaseController bc = obj.GetComponent<BaseController>();
            if (bc == null)
                continue;

            if (bc.CanCollision == false)
                continue;

            if (bc.CellPos == cellPos)
                return obj;
        }

        return null;
    }

    public GameObject FindNpc(Vector3Int cellPos)
    {
        foreach (GameObject obj in _npcs.Values)
        {
            NpcController nc = obj.GetComponent<NpcController>();
            if (nc == null)
                continue;

            if (nc.CellPos == cellPos)
                return obj;
        }

        return null;
    }

    public GameObject FindNpcWithId(int id)
    {
        GameObject npc = null;
        _npcs.TryGetValue(id, out npc);

        return npc;
    }

    public GameObject Find(Func<GameObject, bool> condition)
    {
        foreach(GameObject obj in _objects.Values)
        {
            if (condition.Invoke(obj))
                return obj;
        }

        return null;
    }

    public void Clear()
    {
        foreach (GameObject obj in _objects.Values)
        {
            Managers.Resource.Destroy(obj);
        }
            
        _objects.Clear();
        _npcs.Clear();
        MyPlayer = null;
        Managers.Quest.Clear();

        // 장애물
        foreach (Obstacle obstacle in _obstacles.Values)
            Managers.Resource.Destroy(obstacle.gameObject);
        _obstacles.Clear();
        Managers.Map.RemoveObstacleAll();

    }

    public void SpawnNpc(ObjectInfo info)
    {
        // Npc프리팹을 찾고
        GameObject go = Managers.Resource.Instantiate($"Creature/Npc/Npc_{info.ObjectId}");
        go.name = $"Npc_{info.ObjectId}";
        _objects.Add(info.ObjectId, go);
        _npcs.Add(info.ObjectId, go);

        NpcData npcData = null;
        Managers.Data.NpcDict.TryGetValue(info.ObjectId, out npcData);

        // 퀘스트 담기
        QuestGiver questGiver = go.GetComponent<QuestGiver>();
        questGiver.NpcId = info.ObjectId;
        questGiver.Init(npcData);
        Managers.Quest.InitQuests(questGiver);

        // 위치를 설정
        NpcController nc = go.GetComponent<NpcController>();
        nc.Name = npcData.name;
        nc.PosInfo = info.PosInfo;
        nc.State = CreatureState.Idle;
        nc.SyncPos();

    }

    public void SpawnObstacle(int templateId)
    {
        ObstacleData obstacleData = null;
        Managers.Data.ObstacleDict.TryGetValue(templateId, out obstacleData);

        GameObject go = Managers.Resource.Instantiate($"Map/Obstacle/Obstacle_{templateId}");
        Obstacle obstacle = go.GetComponent<Obstacle>();
        obstacle.Init(obstacleData);
        go.transform.position = new Vector3(obstacle.SpawnPos.x + 0.5f, obstacle.SpawnPos.y + 1f, 0);
        
        Managers.Map.AddObstacle(obstacle);
        _obstacles.Add(obstacle.TemplateId, obstacle);
    }

    public void DespawnObstacle(int templateId)
    {
        Obstacle obstacle = null;
        _obstacles.TryGetValue(templateId, out obstacle);
        _obstacles.Remove(templateId);

        Managers.Map.RemoveObstacle(obstacle);

        Managers.Resource.Destroy(obstacle.gameObject);
    }

    public static GameObjectType GetObjectTypeById(int id)
    {
        int type = (id >> 24) & 0x7F;
        return (GameObjectType)type;
    }
        
}
