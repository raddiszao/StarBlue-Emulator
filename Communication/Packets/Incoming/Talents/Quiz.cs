﻿using System;

namespace StarBlue.Communication.Packets.Incoming.Talents
{
    class Quiz
    {
        internal static Boolean CorrectAnswer(int Quiz, int Answer)
        {
            switch (Quiz)
            {
                case 0:
                    {
                        if (Answer == 2)
                        {
                            return true;
                        }

                        break;
                    }
                case 1:
                    {
                        if (Answer == 1)
                        {
                            return true;
                        }

                        break;
                    }
                case 2:
                    {
                        if (Answer == 2)
                        {
                            return true;
                        }

                        break;
                    }
                case 3:
                    {
                        if (Answer == 0)
                        {
                            return true;
                        }

                        break;
                    }
                case 4:
                    {
                        if (Answer == 1)
                        {
                            return true;
                        }

                        break;
                    }
                case 5:
                    {
                        if (Answer == 3)
                        {
                            return true;
                        }

                        break;
                    }
                case 6:
                    {
                        if (Answer == 0)
                        {
                            return true;
                        }

                        break;
                    }
                case 7:
                    {
                        if (Answer == 3)
                        {
                            return true;
                        }

                        break;
                    }
                case 8:
                    {
                        if (Answer == 0)
                        {
                            return true;
                        }

                        break;
                    }
                case 9:
                    {
                        if (Answer == 0)
                        {
                            return true;
                        }

                        break;
                    }
            }

            return false;
        }
    }
}
