using Database_Manager.Database.Session_Details.Interfaces;
using StarBlue.Communication.Packets.Outgoing.Rooms.Avatar;
using StarBlue.HabboHotel.Rooms;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace StarBlue.HabboHotel.Users.Effects
{
    public sealed class EffectsComponent
    {
        private Habbo _habbo;
        public int _currentEffect;

        /// <summary>
        /// Effects stored by ID > Effect.
        /// </summary>
        private readonly ConcurrentDictionary<int, AvatarEffect> _effects = new ConcurrentDictionary<int, AvatarEffect>();

        public EffectsComponent()
        {
        }

        /// <summary>
        /// Initializes the EffectsComponent.
        /// </summary>
        /// <param name="UserId"></param>
        public bool Init(Habbo Habbo)
        {
            if (_effects.Count > 0)
            {
                return false;
            }

            DataTable GetEffects = null;
            using (IQueryAdapter dbClient = StarBlueServer.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `user_effects` WHERE `user_id` = @id;");
                dbClient.AddParameter("id", Habbo.Id);
                GetEffects = dbClient.GetTable();

                if (GetEffects != null)
                {
                    foreach (DataRow Row in GetEffects.Rows)
                    {
                        if (_effects.TryAdd(Convert.ToInt32(Row["id"]), new AvatarEffect(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["user_id"]), Convert.ToInt32(Row["effect_id"]), Convert.ToDouble(Row["total_duration"]), StarBlueServer.EnumToBool(Row["is_activated"].ToString()), Convert.ToDouble(Row["activated_stamp"]), Convert.ToInt32(Row["quantity"]))))
                        {
                            //umm?
                        }
                    }
                }
            }

            _habbo = Habbo;
            _currentEffect = 0;
            return true;
        }

        public bool TryAdd(AvatarEffect Effect)
        {
            return _effects.TryAdd(Effect.Id, Effect);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SpriteId"></param>
        /// <param name="ActivatedOnly"></param>
        /// <param name="UnactivatedOnly"></param>
        /// <returns></returns>
        public bool HasEffect(int SpriteId, bool ActivatedOnly = false, bool UnactivatedOnly = false)
        {
            return (GetEffectNullable(SpriteId, ActivatedOnly, UnactivatedOnly) != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SpriteId"></param>
        /// <param name="ActivatedOnly"></param>
        /// <param name="UnactivatedOnly"></param>
        /// <returns></returns>
        public AvatarEffect GetEffectNullable(int SpriteId, bool ActivatedOnly = false, bool UnactivatedOnly = false)
        {
            foreach (AvatarEffect Effect in _effects.Values.ToList())
            {
                if (!Effect.HasExpired && Effect.SpriteId == SpriteId && (!ActivatedOnly || Effect.Activated) && (!UnactivatedOnly || !Effect.Activated))
                {
                    return Effect;
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Habbo"></param>
        public void CheckEffectExpiry(Habbo Habbo)
        {
            foreach (AvatarEffect Effect in _effects.Values.ToList())
            {
                if (Effect.HasExpired)
                {
                    Effect.HandleExpiration(Habbo);
                }
            }
        }

        public void ApplyEffect(int EffectId)
        {
            if (_habbo == null || _habbo.CurrentRoom == null)
            {
                return;
            }

            RoomUser User = _habbo.CurrentRoom.GetRoomUserManager().GetRoomUserByHabbo(_habbo.Id);
            if (User == null)
            {
                return;
            }

            _currentEffect = EffectId;

            if (User.IsDancing)
            {
                _habbo.CurrentRoom.SendMessage(new DanceComposer(User, 0));
            }

            _habbo.CurrentRoom.SendMessage(new AvatarEffectComposer(User.VirtualId, EffectId));
        }

        public ICollection<AvatarEffect> GetAllEffects
        {
            get { return _effects.Values; }
        }

        public int CurrentEffect
        {
            get { return _currentEffect; }
            set { _currentEffect = value; }
        }

        /// <summary>
        /// Disposes the EffectsComponent.
        /// </summary>
        public void Dispose()
        {
            _effects.Clear();
        }
    }
}
