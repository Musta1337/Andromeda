﻿using Andromeda;
using Andromeda.Events;
using Andromeda.Events.EventArguments;
using InfinityScript;
using InfinityScript.PBase;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AntiCheat.ACModules
{
    public class Aimbot
    {
        int MaxAngleChange = 60;
        int MaxAngleChangeTime = 100;

        int MaxTagHit = 5;
        int MaxTagHitTime = 10;

        public Aimbot()
        {
            Log.Debug("Registered Aimbot Listener");
            RegisterEvents();
        }

        private void RegisterEvents()
        {
            Script.PlayerDamage.Add((sender, args) =>
            {
                Entity entity = sender as Entity;
                string tag = args.Hitloc;

                long time = GetTimeAndRegisterNewKill(entity);
                int change = GetChangeAndRegisterNewKill(entity);

                if (time < MaxAngleChangeTime)
                {              
                    if (change > MaxAngleChange)
                    {
                        entity.Tell($"Hax? Time: {time}. Change: {change}");
                    }
                }

                Log.Debug(GetTagHitAndRegisterNewKill(entity, tag).ToString());
            });
        }

        #region Tags
        private int GetHighestTagHitCount(Entity entity)
        {
            if (entity.HasField("TagHits"))
                return entity.GetField<Dictionary<string, int>>("TagHits").OrderByDescending(x => x.Key).FirstOrDefault().Value;

            return 0;
        }

        private int GetTagHitAndRegisterNewKill(Entity entity, string tag)
        {
            int hits = GetHighestTagHitCount(entity);

            if (entity.HasField("TagHits"))
            {
                if (entity.GetField<Dictionary<string, int>>("TagHits").ContainsKey(tag))
                    entity.GetField<Dictionary<string, int>>("TagHits")[tag]++;
                else
                    entity.GetField<Dictionary<string, int>>("TagHits")[tag] = 1;
            }
            else
            {
                Dictionary<string, int> hit = new Dictionary<string, int>
                {
                    { tag, 1 }
                };

                entity.SetFieldT("TagHits", hit);
            }

            return hits;
        }
        #endregion

        #region Angle
        private int GetLongestVectorChange(Entity entity)
        {
            if (entity.HasField("LastKillAngle"))
            {          
                Vector3 lastKillAngle = entity.GetField<Vector3>("LastKillAngle");

                return (int)lastKillAngle.DistanceToAngle(entity.GetPlayerAngles());
                
            }

            return 0;
        }

        private int GetChangeAndRegisterNewKill(Entity entity)
        {
            int change = GetLongestVectorChange(entity);

            entity.SetField("LastKillAngle", entity.GetPlayerAngles());

            return change;
        }

        private long GetTimeSinceLastKill(Entity entity) => entity.GetFieldOrVal<Stopwatch>("AngleStopwatch").ElapsedMilliseconds;

        private long GetTimeAndRegisterNewKill(Entity entity)
        {
            if (entity.HasField("AngleStopwatch"))
            {
                long last = GetTimeSinceLastKill(entity);

                entity.GetField<Stopwatch>("AngleStopwatch").Restart();

                return last;
            }
            else
            {
                entity.SetFieldT("AngleStopwatch", new Stopwatch());

                entity.GetField<Stopwatch>("AngleStopwatch").Start();

                return -1;
            }
        }
        #endregion
    }
}