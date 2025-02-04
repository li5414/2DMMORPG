﻿using Google.Protobuf.Protocol;
using Server.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class Projectile : CreatureObject
    {
        public int Damage { get; set; }
        public int Range { get; set; }
        protected Skill _skillData = null;
        protected int _skillLevel;

        public Projectile()
        {
            ObjectType = GameObjectType.Projectile;
        }

        public virtual void Init(Skill skillData, int point)
        {
            _skillData = skillData;
            _skillLevel = point;

            Speed = skillData.projectile.projectilePointInfos[point].speed;
            Damage = skillData.skillPointInfos[point].damage;
            Range = skillData.projectile.projectilePointInfos[point].range;

        }

        public override void Update()
        {

        }
    }
}
