using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DragonKnight2_Farmer
{
    //public enum CityHealSequence
    //{
    //    Exploring = -1, GetGoldForInn = 10, UseCityInn = 22, DepositGold = 30, DepositDragonPoints = 60, TravelToFightCity = 50
    //}
    class Character
    {
        private bool inCombat;
        private bool targetAsleep;
        private bool fightingBoss;
        //private int manaPotionCount;
        private int currentManaPoints;
        private int currentHitPoints;
        private int currentTravelPoints;
        private int currentTurns;
        private int currentGold;
        private int currentDragonPoints;
        private int currentDPBank;
        private int currentGoldBank;
        private int targetHitpoints;
        private DateTime currentTurnsTimeStamp;
        private int turnsPerMin;

        public const int MinChaosDamage = 3300;
        public const int MinHitPoints = 2900;
        public const int MinManaPoints = 250;
        public const int InnCost = 5;
        public const int CityTravelPointCost = 350;

        //public const int Heal1Cost = 5, Heal2Cost = 10, Heal3Cost = 25, Heal4Cost = 50, Heal5Cost = 75;
        //public const int Heal1Healing = 10, Heal2Healing = 25, Heal3Healing = 50, Heal4Healing = 100, Heal5Healing = 150;

        public bool InCombat
        {
            get
            {
                return inCombat;
            }
            set
            {
                inCombat = value;
            }
        }
        public bool TargetAsleep
        {
            get
            {
                return targetAsleep;
            }
            set
            {
                targetAsleep = value;
            }
        }
        public bool FightingBoss
        {
            get
            {
                return fightingBoss;
            }
            set
            {
                fightingBoss = value;
            }
        }
        public int CurrentManaPoints
        {
            get
            {
                return currentManaPoints;
            }
            set
            {
                currentManaPoints = value;
            }
        }
        public int CurrentHitPoints
        {
            get
            {
                return currentHitPoints;
            }
            set
            {
                currentHitPoints = value;
            }
        }
        public int CurrentTravelPoints
        {
            get
            {
                return currentTravelPoints;
            }
            set
            {
                currentTravelPoints = value;
            }
        }
        public int CurrentTurns
        {
            get
            {
                return currentTurns + (int)(DateTime.Now - currentTurnsTimeStamp).TotalMinutes * turnsPerMin;
            }
            set
            {
                currentTurns = value;
                currentTurnsTimeStamp = DateTime.Now;
            }
        }
        public int CurrentGold
        {
            get
            {
                return currentGold;
            }
            set
            {
                currentGold = value;
            }
        }
        public int CurrentDragonPoints
        {
            get
            {
                return currentDragonPoints;
            }
            set
            {
                currentDragonPoints = value;
            }
        }
        public int CurrentDPBank
        {
            get
            {
                return currentDPBank;
            }
            set
            {
                currentDPBank = value;
            }
        }
        public int CurrentGoldBank
        {
            get
            {
                return currentGoldBank;
            }
            set
            {
                currentGoldBank = value;
            }
        }
        public int TurnsPerMin
        {
            get
            {
                return turnsPerMin;
            }
            set
            {
                turnsPerMin = value;
            }
        }
        public int TargetHitPoints
        {
            get
            {
                return targetHitpoints;
            }
            set
            {
                targetHitpoints = value;
            }
        }

        public Point FightFromCity { get; set; }
        public Point CurrentLocation { get; set; }

        public Character()
        {
            FightFromCity = new Point(-520, 27);
            currentTurnsTimeStamp = DateTime.Now;
            turnsPerMin = 2;
        }

        public CharacterAction GetNextAction()
        {
            if (inCombat)
            {
                if (targetAsleep || targetHitpoints < MinChaosDamage || currentManaPoints < 50+60)
                {
                    return new CharacterAction(ActionID.CastDamage50, true, fightingBoss);
                }
                else
                {
                    return new CharacterAction(ActionID.CastSleep60, true, fightingBoss);
                }
            }
            else //inCombat == false
            {
                bool closeToFightCity = (CurrentLocation.X == FightFromCity.X && (CurrentLocation.Y >= FightFromCity.Y - 3) && CurrentLocation.Y <= FightFromCity.Y);
                bool cityHealNeeded = (currentHitPoints < MinHitPoints || currentManaPoints < MinManaPoints);


                if (closeToFightCity)
                {
                    if (cityHealNeeded)
                    {
                        return new CharacterAction(ActionID.GoToTown01);
                    }
                    else if (CurrentLocation.Y <= FightFromCity.Y - 2)
                    {
                        return new CharacterAction(ActionID.ExploreNorth, false, false);
                    }
                    else
                    {
                        return new CharacterAction(ActionID.ExploreSouth, false, false);
                    }
                }
                else // !closeToFightCity
                {
                    if (CurrentLocation.X != 0 || CurrentLocation.Y != 0)
                    {
                        return new CharacterAction(ActionID.GoToTown01);
                    }
                    else if (cityHealNeeded || currentTravelPoints < CityTravelPointCost)
                    {
                        if (currentGold < InnCost)
                        {
                            return new CharacterAction(ActionID.WithdrawGold, false, false, InnCost);
                        }
                        else if (currentDragonPoints > 0)
                        {
                            return new CharacterAction(ActionID.DepositDragonPoints, false, false, currentDragonPoints);
                        }
                        else
                        {
                            return new CharacterAction(ActionID.SleepAtInn);
                        }
                    }
                    else
                    {
                        if (currentGold > 0)
                        {
                            return new CharacterAction(ActionID.DepositGold, false, false, currentGold);
                        }
                        else
                        {
                            return new CharacterAction(ActionID.GoToTown13);
                        }
                    }
                }
            }
        }
    }
}
