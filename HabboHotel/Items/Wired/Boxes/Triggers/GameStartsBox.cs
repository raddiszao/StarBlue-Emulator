using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Triggers
{
    internal class GameStartsBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.TriggerGameStarts;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public GameStartsBox(Room Instance, Item Item)
        {
            this.Item = Item;
            this.Instance = Instance;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(MessageEvent Packet)
        {

        }

        public bool Execute(params object[] Params)
        {
            ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(this);
            ICollection<IWiredItem> Conditions = Instance.GetWired().GetConditions(this);

            bool Success = false;
            bool HasAnyConditionValid = Effects.Where(x => x.Type == WiredBoxType.AddonAnyConditionValid).ToList().Count() > 0;
            foreach (IWiredItem Condition in Conditions.ToList())
            {
                if (!Condition.Execute(Condition.Item))
                {
                    if (!HasAnyConditionValid)
                        return false;

                    continue;
                }

                Success = true;
                Instance.GetWired().OnEvent(Condition.Item);
            }

            if (!Success && Conditions.Count > 0)
            {
                return false;
            }

            //Check the ICollection to find the random addon effect.
            bool HasRandomEffectAddon = Effects.Where(x => x.Type == WiredBoxType.AddonRandomEffect).ToList().Count() > 0;
            if (HasRandomEffectAddon)
            {
                //Okay, so we have a random addon effect, now lets get the IWiredItem and attempt to execute it.
                IWiredItem RandomBox = Effects.FirstOrDefault(x => x.Type == WiredBoxType.AddonRandomEffect);
                if (!RandomBox.Execute())
                {
                    return false;
                }

                //Success! Let's get our selected box and continue.
                IWiredItem SelectedBox = Instance.GetWired().GetRandomEffect(Effects.ToList());
                if (!SelectedBox.Execute(Params))
                {
                    return false;
                }

                //Woo! Almost there captain, now lets broadcast the update to the room instance.
                if (Instance != null)
                {
                    Instance.GetWired().OnEvent(RandomBox.Item);
                    if (SelectedBox != null)
                        Instance.GetWired().OnEvent(SelectedBox.Item);
                }
            }
            else
            {
                foreach (IWiredItem Effect in Effects)
                {
                    foreach (RoomUser User in Instance.GetRoomUserManager().GetRoomUsers().ToList())
                    {
                        if (User == null || User.GetClient() == null || User.GetClient().GetHabbo() == null)
                        {
                            continue;
                        }

                        Effect.Execute(User.GetClient().GetHabbo());
                    }

                    Instance.GetWired().OnEvent(Effect.Item);
                }
            }

            return true;
        }
    }
}
