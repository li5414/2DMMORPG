﻿using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class PoisonSmoke : Summoning
    {
        public CreatureObject Owner { get; set; } // 주인

        IJob _job = null;
        public override void Update()
        {
            if (Owner == null || Room == null)
                return;

            // 0.5초마다 체크
            int tick = (int)(1000 / 500);
            _job = Room.PushAfter(tick, Update);


            if (Duration > Environment.TickCount) // 지속 시간일 때
            {
                HashSet<CreatureObject> objects = Room.Map.LoopByCircle<CreatureObject>(CellPos, Radian);

                foreach (CreatureObject co in objects)
                {
                    if(Owner is Player)
                    {
                        Player player = Owner as Player;
                        if(co != Owner && player.Communication.Party == null)
                            co.Condition.Poison(_skillData, _skillLevel, Owner);
                        else if (co != Owner && player.Communication.Party.FindPlayerById(co.Id) == null)
                            co.Condition.Poison(_skillData, _skillLevel, Owner);
                    }
                    else if(Owner is BanBan)
                    {
                        BanBan banban = Owner as BanBan;
                        if (co != Owner && !(co is Monster))
                            co.Condition.Poison(_skillData, _skillLevel, Owner);
                    }

                }
            }
            else // 지속시간이 끝낫을 때
            {
                _job.Cancel = true;
                Room.Push(Room.LeaveGame, Id);
                return;
            }
        }

        public override GameObject GetOwner()
        {
            return Owner;
        }
    }
}
