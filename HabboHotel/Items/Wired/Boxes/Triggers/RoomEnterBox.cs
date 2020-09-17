using StarBlue.Communication.Packets.Incoming;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Users;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace StarBlue.HabboHotel.Items.Wired.Boxes.Triggers
{
    internal class RoomEnterBox : IWiredItem
    {
        public Room Instance { get; set; }
        public Item Item { get; set; }
        public WiredBoxType Type => WiredBoxType.TriggerRoomEnter;
        public ConcurrentDictionary<int, Item> SetItems { get; set; }
        public string StringData { get; set; }
        public bool BoolData { get; set; }
        public string ItemsData { get; set; }

        public RoomEnterBox(Room Instance, Item Item)
        {
            this.Instance = Instance;
            this.Item = Item;
            SetItems = new ConcurrentDictionary<int, Item>();
        }

        public void HandleSave(MessageEvent Packet)
        {
            int Unknown = Packet.PopInt();
            string User = Packet.PopString();

            StringData = User;
        }

        public bool Execute(params object[] Params)
        {
            Instance.GetWired().OnEvent(Item);

            Habbo Player = (Habbo)Params[0];

            if (!string.IsNullOrWhiteSpace(StringData) && Player.Username != StringData)
            {
                return false;
            }

            ICollection<IWiredItem> Effects = Instance.GetWired().GetEffects(this);
            ICollection<IWiredItem> Conditions = Instance.GetWired().GetConditions(this);

            bool Success = false;
            bool HasAnyConditionValid = Effects.Where(x => x.Type == WiredBoxType.AddonAnyConditionValid).ToList().Count() > 0;
            foreach (IWiredItem Condition in Conditions.ToList())
            {
                if (!Condition.Execute(Player))
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
                    if (!Effect.Execute(Player))
                    {
                        return false;
                    }

                    Instance.GetWired().OnEvent(Effect.Item);
                }
            }

            return true;
        }
    }
}
