﻿using StarBlue.HabboHotel.Items.Utilities;
using StarBlue.HabboHotel.RewaStarBlue.Rooms.AI.Types;
using StarBlue.HabboHotel.Rooms.AI.Speech;
using StarBlue.HabboHotel.Rooms.AI.Types;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace StarBlue.HabboHotel.Rooms.AI
{
    public class RoomBot
    {
        public int Id;
        public int BotId;
        public int VirtualId;

        public BotAIType AiType;

        public int DanceId;
        public int CurrentEffect;
        public string Gender;

        public string Look;
        public string Motto;
        public string Name;
        public int RoomId;
        public int Rot;


        public string WalkingMode;

        public int X;
        public int Y;
        public double Z;
        public int maxX;
        public int maxY;
        public int minX;
        public int minY;

        public int ownerID;

        public bool AutomaticChat;
        public int SpeakingInterval;
        public bool MixSentences;

        public RoomUser RoomUser;
        public List<RandomSpeech> RandomSpeech;

        private int _chatBubble;
        public bool ForcedMovement { get; set; }
        public int ForcedUserTargetMovement { get; set; }
        public Point TargetCoordinate { get; set; }

        public int TargetUser { get; set; }

        public RoomBot(int BotId, int RoomId, string AiType, string WalkingMode, string Name, string Motto, string Look, int X, int Y, double Z, int Rot,
            int minX, int minY, int maxX, int maxY, ref List<RandomSpeech> Speeches, string Gender, int Dance, int ownerID,
            bool AutomaticChat, int SpeakingInterval, bool MixSentences, int ChatBubble)
        {
            Id = BotId;
            this.BotId = BotId;
            this.RoomId = RoomId;

            this.Name = Name;
            this.Motto = Motto;
            this.Look = Look;
            this.Gender = Gender.ToUpper();

            this.AiType = BotUtility.GetAIFromString(AiType);
            this.WalkingMode = WalkingMode;

            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.Rot = Rot;
            this.minX = minX;
            this.minY = minY;
            this.maxX = maxX;
            this.maxY = maxY;

            VirtualId = -1;
            RoomUser = null;
            DanceId = Dance;
            CurrentEffect = 0;

            LoadRandomSpeech(Speeches);
            //this.LoadResponses(Responses);

            this.ownerID = ownerID;

            this.AutomaticChat = AutomaticChat;
            this.SpeakingInterval = SpeakingInterval;
            this.MixSentences = MixSentences;

            _chatBubble = ChatBubble;
            ForcedMovement = false;
            TargetCoordinate = new Point();
            TargetUser = 0;
        }

        public bool IsPet => (AiType == BotAIType.PET);

        #region Speech Related
        public void LoadRandomSpeech(List<RandomSpeech> Speeches)
        {
            RandomSpeech = new List<RandomSpeech>();
            foreach (RandomSpeech Speech in Speeches)
            {
                if (Speech.BotID == BotId)
                {
                    RandomSpeech.Add(Speech);
                }
            }
        }


        public RandomSpeech GetRandomSpeech()
        {
            var rand = new Random();

            if (RandomSpeech.Count < 1)
            {
                return new RandomSpeech("", 0);
            }

            return RandomSpeech[rand.Next(0, (RandomSpeech.Count - 1))];
        }
        #endregion

        #region AI Related
        public BotAI GenerateBotAI(int VirtualId)
        {
            switch (AiType)
            {
                case BotAIType.PET:
                    return new PetBot(VirtualId);
                case BotAIType.GENERIC:
                    return new GenericBot(VirtualId);
                case BotAIType.BARTENDER:
                    return new BartenderBot(VirtualId);
                case BotAIType.WELCOME:
                    return new WelcomeBot(VirtualId);
                case BotAIType.VISITORLOGGER:
                    return new VisitorLogger(VirtualId);
                case BotAIType.CASINO_BOT:
                    return new CasinoCounter(VirtualId);
                case BotAIType.ROULLETE_BOT:
                    return new CasinoRoullete(VirtualId);
                case BotAIType.ROULETTEBOTDUCKET:
                    return new RouleteBotDucket(VirtualId);
                case BotAIType.ROULETTEBOTHONOR:
                    return new RouletteBotHonor(VirtualId);
                default:
                    return new GenericBot(VirtualId);
            }
        }
        #endregion

        public int ChatBubble
        {
            get => _chatBubble;
            set => _chatBubble = value;
        }
    }
}