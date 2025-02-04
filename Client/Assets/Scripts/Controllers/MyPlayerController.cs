﻿using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MyPlayerController : PlayerController
{
	UI_GameScene _gameSceneUI = null;
	bool _moveKeyPressed = false;

    public override StatInfo Stat 
	{
		get { return base._stat; }
		set 
		{ 
			base._stat = value;

			UpdateMpBar();
			UpdateHpBar();
			RefreshAdditionanlStat();
			AddExBar();
			InitLevelUI();

			if (_gameSceneUI == null)
				return;
			if (Stat.CanUpClass)
				_gameSceneUI.ClassUp.gameObject.SetActive(true);
			else
				_gameSceneUI.ClassUp.gameObject.SetActive(false);
		}
	}

    public int Exp
    {
        get { return Stat.TotalExp; }
		set { Stat.TotalExp = value; }
    }

	private int LevelUpExp
	{
		get { return Level * 25; }
	}

	public int WeaponDamage { get; private set; }
	public int ArmorDefence { get; private set; }

	protected override void Init()
    {
        base.Init();

		_gameSceneUI = Managers.UI.SceneUI as UI_GameScene;
		RefreshAdditionanlStat();
		AddExBar();
		InitLevelUI();

		if (Stat.CanUpClass)
			_gameSceneUI.ClassUp.gameObject.SetActive(true);
		else
			_gameSceneUI.ClassUp.gameObject.SetActive(false);
	}

    protected override void AddHpBar()
    {
		UpdateHpBar();
	}

    protected override void UpdateHpBar()
    {
		float ratio = 0.0f;
		if (Stat.MaxHp > 0)
		{
			ratio = ((float)Hp / Stat.MaxHp);
		}

		UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
		if (gameScene == null)
			return;

		gameScene.StatBarUI.SetHpBar(ratio);
	}

	protected override void AddMpBar()
	{
		UpdateMpBar();
	}

	protected override void UpdateMpBar()
	{
		float ratio = 0.0f;
		if (Stat.MaxMp > 0)
		{
			ratio = ((float)Mp / Stat.MaxMp);
		}

		UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
		if (gameScene == null)
			return;

		gameScene.StatBarUI.SetMpBar(ratio);
	}

	private void AddExBar()
    {
		UpdateExBar();
	}

	public void UpdateExBar()
    {
		float ratio = 0.0f;
		if (LevelUpExp > 0)
		{
			ratio = ((float)Exp / LevelUpExp);
		}

		UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
		if (gameScene == null)
			return;

		gameScene.StatBarUI.SetExBar(ratio);
	}

	private void InitLevelUI()
	{
		UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
		if (gameScene == null)
			return;

		gameScene.StatBarUI.SetLevel(Level);
	}

    public override void LevelUp(int level)
    {
        base.LevelUp(level);

		UI_GameScene gameScene = Managers.UI.SceneUI as UI_GameScene;
		if (gameScene == null)
			return;

		gameScene.StatBarUI.SetLevel(level);

		// 퀘스트 여부 체크
		Managers.Quest.CheckCondition();
	}

	// 맵핵 사용 테스트
	public Vector3Int TestPos;
	public bool TestMapDo = false;

    protected override void UpdateController()
    {
		// 맵핵 사용 테스트
		if (TestMapDo)
        {
			CellPos = TestPos;
        }

		if (State == CreatureState.Dead &&
			State == CreatureState.Skill &&
			State == CreatureState.Stun &&
			State == CreatureState.Cutscene)
			return;

		GetUIKeyInput();
		GetQuickSlotInput();

		switch (State)
		{
			case CreatureState.Idle:
				GetDirInput();
				GetKeyInput();
				break;
			case CreatureState.Moving:
				GetDirInput();
				GetKeyInput();
				break;
		}

		base.UpdateController();
    }

	protected override void UpdateIdle()
	{
		// 이동 상태로 갈지 확인
		if (_moveKeyPressed)
		{
			State = CreatureState.Moving;
			return;
		}
	}

	// 키보드 입력
	void GetDirInput()
	{
		_moveKeyPressed = true;

		if (Input.GetKey(KeyCode.UpArrow))
		{
			Dir = MoveDir.Up;
		}
		else if (Input.GetKey(KeyCode.DownArrow))
		{
			Dir = MoveDir.Down;
		}
		else if (Input.GetKey(KeyCode.LeftArrow))
		{
			Dir = MoveDir.Left;
		}
		else if (Input.GetKey(KeyCode.RightArrow))
		{
			Dir = MoveDir.Right;
		}
		else
		{
			_moveKeyPressed = false;
		}
	}

	void GetKeyInput()
    {
		// 엔터치면 채팅 서버 전송
		if (Input.GetKeyDown(KeyCode.Return))
		{
			_gameSceneUI.ChatInputBoxUI.GetInputField.Select();
			Managers.Chat.SendChatToServer();
		}
		// 채팅을 치고 있을 땐 다른 키입력 x
		if (_gameSceneUI.ChatInputBoxUI.GetInputField.isFocused)
			return;

		// 아이템 먹기
		if (Input.GetKeyDown(KeyCode.Z))
        {
			ItemController item = Managers.Object.FindItemFromGround(CellPos);

			if (item != null)
			{
				C_GetDropItem dropItemPacket = new C_GetDropItem()
				{
					PosInfo = new PositionInfo(),
					ItemInfo = new ItemInfo()
				};
				dropItemPacket.PosInfo.PosX = CellPos.x;
				dropItemPacket.PosInfo.PosY = CellPos.y;
				dropItemPacket.ItemInfo.MergeFrom(item.itemInfo);

				Managers.Network.Send(dropItemPacket);
			}
        }
		
		// 바로 앞 오브젝트와 상호작용
		if(Input.GetKeyDown(KeyCode.Space))
        {
			GameObject go = Managers.Object.FindCollsion(GetFrontCellPos());
			if (go == null)
				return;

			if(go.GetComponent<NpcController>())
            {
				NpcController nc = go.GetComponent<NpcController>();
				Managers.Quest.ViewQuest(nc);
			}
            else if (go.GetComponent<PlayerController>())
            {
				_gameSceneUI.InteractionUI.SetPlayerInfo(go.GetComponent<PlayerController>());
				
				if(_gameSceneUI.InteractionUI.gameObject.activeSelf == false)
					_gameSceneUI.InteractionUI.gameObject.SetActive(true);
			}
        }

	}

	void GetUIKeyInput()
    {
		// 채팅을 치고 있을 땐 다른 키입력 x
		if (_gameSceneUI.ChatInputBoxUI.GetInputField.isFocused)
			return;

		if (Input.GetKeyDown(KeyCode.I))
        {
			UI_Inventory invenUI = _gameSceneUI.InvenUI;

            if (invenUI.gameObject.activeSelf) // 활성화 여부
            {
				invenUI.gameObject.SetActive(false);
				_gameSceneUI.DescriptionBox.ClosePosition();
			}
            else
            {
				invenUI.gameObject.SetActive(true);
				invenUI.RefreshUI(); // 켤 때 갱신
			}
		}
        else if (Input.GetKeyDown(KeyCode.C))
        {
			UI_Stat statUI = _gameSceneUI.StatUI;

			if (statUI.gameObject.activeSelf) // 활성화 여부
			{
				statUI.gameObject.SetActive(false);
				_gameSceneUI.DescriptionBox.ClosePosition();
			}
			else
			{
				statUI.gameObject.SetActive(true);
				statUI.RefreshUI(); // 켤 때 갱신
			}
		}
		else if(Input.GetKeyDown(KeyCode.K))
        {
			UI_Skill skillUI = _gameSceneUI.SkillUI;

			if (skillUI.gameObject.activeSelf) // 활성화 여부
			{
				skillUI.gameObject.SetActive(false);
				_gameSceneUI.DescriptionBox.ClosePosition();
			}
			else
			{
				skillUI.gameObject.SetActive(true);
			}
		}
	}

	void GetQuickSlotInput()
	{ // 단축키
	  // 채팅을 치고 있을 땐 다른 키입력 x
		if (_gameSceneUI.ChatInputBoxUI.GetInputField.isFocused)
			return;

		if (Input.GetKeyDown(KeyCode.Q))
			_gameSceneUI.ShortcutKeyUI.GetKeyQ();
		if (Input.GetKeyDown(KeyCode.W))
			_gameSceneUI.ShortcutKeyUI.GetKeyW();
		if (Input.GetKeyDown(KeyCode.E))
			_gameSceneUI.ShortcutKeyUI.GetKeyE();
		if (Input.GetKeyDown(KeyCode.R))
			_gameSceneUI.ShortcutKeyUI.GetKeyR();
		if (Input.GetKeyDown(KeyCode.A))
			_gameSceneUI.ShortcutKeyUI.GetKeyA();
		if (Input.GetKeyDown(KeyCode.S))
			_gameSceneUI.ShortcutKeyUI.GetKeyS();
		if (Input.GetKeyDown(KeyCode.D))
			_gameSceneUI.ShortcutKeyUI.GetKeyD();
		if (Input.GetKeyDown(KeyCode.F))
			_gameSceneUI.ShortcutKeyUI.GetKeyF();

	}

	void LateUpdate()
	{
		Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
	}

    protected override void MoveToNextPos()
    {
		if (!_moveKeyPressed)
		{
			State = CreatureState.Idle;
			CheckUpdatedFlag();
			return;
		}

		Vector3Int destPos = CellPos;

		switch (Dir)
		{
			case MoveDir.Up:
				destPos += Vector3Int.up;
				break;
			case MoveDir.Down:
				destPos += Vector3Int.down;
				break;
			case MoveDir.Left:
				destPos += Vector3Int.left;
				break;
			case MoveDir.Right:
				destPos += Vector3Int.right;
				break;
		}

		if (Managers.Map.CanGo(destPos))
		{
			if (Managers.Object.FindCollsion(destPos) == null)
			{
				CellPos = destPos;
			}
		}

		CheckUpdatedFlag();
	}

	protected override void CheckUpdatedFlag()
    {
		if(_updated)
        {
			C_Move movePacket = new C_Move();
			movePacket.PosInfo = PosInfo;
			Managers.Network.Send(movePacket);

			_updated = false;
		}
    }

	public void RefreshAdditionanlStat()
	{
		WeaponDamage = 0;
		ArmorDefence = 0;

		foreach (Item item in Managers.Inven.Items.Values)
		{
			if (item.Equipped == false)
				continue;

			switch (item.ItemType)
			{
				case ItemType.Weapon:
					Weapon weapon = (Weapon)item;
					if (weapon.WeaponType == WeaponType.Sword && Stat.JobClassType == JobClassType.Warrior)
						WeaponDamage += weapon.Damage;
					else if (weapon.WeaponType == WeaponType.Staff && Stat.JobClassType == JobClassType.Mage)
						WeaponDamage += weapon.Damage;
					else if (Stat.JobClassType == JobClassType.None)
						WeaponDamage += weapon.Damage;
					break;
				case ItemType.Armor:
					ArmorDefence += ((Armor)item).Defence;
					break;
			}
		}
	}

}
