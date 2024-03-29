﻿using log4net;
using StarBlue.Communication.Packets.Outgoing.Inventory.Pets;
using StarBlue.Communication.Packets.Outgoing.Rooms.Notifications;
using StarBlue.HabboHotel.Rooms;
using StarBlue.HabboHotel.Rooms.AI;
using StarBlue.HabboHotel.Rooms.AI.Speech;
using System;
using System.Collections.Generic;

namespace StarBlue.Communication.Packets.Incoming.Rooms.AI.Pets
{
    internal class PlacePetEvent : IPacketEvent
    {
        private static readonly ILog log = LogManager.GetLogger("StarBlue.Communication.Packets.Incoming.Rooms.AI.Pets.PlacePetEvent");

        public void Parse(HabboHotel.GameClients.GameClient Session, MessageEvent Packet)
        {
            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            if (!StarBlueServer.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (Room.RoomData.AllowPets == 0 && !Room.CheckRights(Session))
            {
                Session.SendMessage(new RoomErrorNotifComposer(1));
                return;
            }

            if (Room.GetRoomUserManager().PetCount > Convert.ToInt32(StarBlueServer.GetConfig().data["room.pet.limit"]))
            {
                Session.SendMessage(new RoomErrorNotifComposer(2));//5 = I have too many.
                return;
            }

            if (!Session.GetHabbo().GetInventoryComponent().TryGetPet(Packet.PopInt(), out Pet Pet))
            {
                return;
            }

            if (Pet == null)
            {
                return;
            }

            if (Pet.PlacedInRoom)
            {
                Session.SendNotification("Este mascote já se encontra no quarto.");
                return;
            }

            int X = Packet.PopInt();
            int Y = Packet.PopInt();

            if (!Room.GetGameMap().CanWalk(X, Y, false))
            {
                Session.SendMessage(new RoomErrorNotifComposer(4));
                return;
            }

            if (Room.GetRoomUserManager().TryGetPet(Pet.PetId, out RoomUser OldPet))
            {
                Room.GetRoomUserManager().RemoveBot(OldPet.VirtualId, false);
            }

            Pet.X = X;
            Pet.Y = Y;

            Pet.PlacedInRoom = true;
            Pet.RoomId = Room.Id;

            List<RandomSpeech> RndSpeechList = new List<RandomSpeech>();
            RoomBot RoomBot = new RoomBot(Pet.PetId, Pet.RoomId, "pet", "freeroam", Pet.Name, "", Pet.Look, X, Y, 0, 0, 0, 0, 0, 0, ref RndSpeechList, "", 0, Pet.OwnerId, false, 0, false, 0);
            if (RoomBot == null)
            {
                return;
            }

            Room.GetRoomUserManager().DeployBot(RoomBot, Pet);

            Pet.DBState = DatabaseUpdateState.NeedsUpdate;
            Room.GetRoomUserManager().UpdatePets();

            if (!Session.GetHabbo().GetInventoryComponent().TryRemovePet(Pet.PetId, out Pet ToRemove))
            {
                log.Error("Error whilst removing pet: " + ToRemove.PetId);
                return;
            }

            Session.SendMessage(new PetInventoryComposer(Session.GetHabbo().GetInventoryComponent().GetPets()));
        }
    }
}
