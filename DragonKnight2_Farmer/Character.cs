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
        private bool breakOnEnchanter;
        //private int manaPotionCount;
        private int currentLevel;
        private int currentManaPoints;
        private int currentHitPoints;
        private int currentTravelPoints;
        private int currentTurns;
        private int currentGold;
        private int currentDragonPoints;
        private int currentDPBank;
        private int currentGoldBank;
        private int currentPotionHPCount;
        private int currentPotionMPCount;
        private int currentPotionTPCount;
        private int targetHitpoints;
        private DateTime currentTurnsTimeStamp;
        private int turnsPerMin;

        public const int MinChaosDamage = 3800;
        public const int MinHitPoints = 3000;
        public const int MinManaPoints = 250;
        public const int MaxDragonPoints = 2000;
        public const int MinDragonPoints = 1000;
        public const int MaxGold = 200000;
        //public const int InnCost = 5;
        public const int CityTravelPointCost = 300;
        public const int EnchantDPCost = 500;
        public const int MinPotionCount = 500;

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
        public bool BreakOnEnchanter
        {
            get
            {
                return breakOnEnchanter;
            }
            set
            {
                breakOnEnchanter = value;
            }
        }
        public int CurrentLevel
        {
            get
            {
                return currentLevel;
            }
            set
            {
                currentLevel = value;
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
        public int CurrentPotionHPCount
        {
            get
            {
                return currentPotionHPCount;
            }
            set
            {
                currentPotionHPCount = value;
            }
        }
        public int CurrentPotionMPCount
        {
            get
            {
                return currentPotionMPCount;
            }
            set
            {
                currentPotionMPCount = value;
            }
        }
        public int CurrentPotionTPCount
        {
            get
            {
                return currentPotionTPCount;
            }
            set
            {
                currentPotionTPCount = value;
            }
        }
        public int CurrentWoodCount { get; set; }
        public int CurrentFishCount { get; set; }
        public int CurrentStoneCount { get; set; }
        public int CurrentIronCount { get; set; }
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
            FightFromCity = new Point(120, -580);
            currentTurnsTimeStamp = DateTime.Now;
            turnsPerMin = 2;
        }

        public CharacterAction GetNextMonsterFarmAction()
        {
            bool closeToFightCity = (CurrentLocation.X == FightFromCity.X && (CurrentLocation.Y >= FightFromCity.Y - 3) && CurrentLocation.Y <= FightFromCity.Y);
            bool atInnCity = (CurrentLocation.X == 0 && CurrentLocation.Y == 0);
            int workingMinDP = (breakOnEnchanter ? MinDragonPoints : 0);

            if (!inCombat)
            {
                if (atInnCity)
                {
                    if (currentTravelPoints <= CityTravelPointCost)
                    {
                        if (currentGold < currentLevel * 10)
                        {
                            return new CharacterAction(ActionID.WithdrawGold, false, false, currentLevel * 10);
                        }
                        else
                        {
                            return new CharacterAction(ActionID.SleepAtInn, false, false, currentLevel * 10);
                        }
                    }
                    else if (currentPotionHPCount < MinPotionCount || currentPotionMPCount < MinPotionCount)
                    {
                        if (currentGold < 100 * 20)
                        {
                            return new CharacterAction(ActionID.WithdrawGold, false, false, 100 * 20);
                        }
                        else if (currentPotionHPCount < MinPotionCount)
                        {
                            return new CharacterAction(ActionID.BuyPotionHP);
                        }
                        else
                        {
                            return new CharacterAction(ActionID.BuyPotionMP);
                        }
                    }
                    else if (currentDragonPoints > workingMinDP)
                    {
                        return new CharacterAction(ActionID.DepositDragonPoints, false, false, currentDragonPoints - workingMinDP);
                    }
                    else if (currentGold > 0)
                    {
                        return new CharacterAction(ActionID.DepositGold, false, false, currentGold);
                    }
                    else
                    {
                        return new CharacterAction(ActionID.GoToTown16);
                    }
                }
                else if (!closeToFightCity || currentDragonPoints > MaxDragonPoints || currentGold > MaxGold)
                {
                    return new CharacterAction(ActionID.GoToTown01);
                }
                else if (currentHitPoints < MinHitPoints)
                {
                    return new CharacterAction(ActionID.UsePotionHP);
                }
                else if (currentManaPoints < MinManaPoints)
                {
                    return new CharacterAction(ActionID.UsePotionMP);
                }
                else
                {
                    if (CurrentLocation.Y <= FightFromCity.Y - 2)
                    {
                        return new CharacterAction(ActionID.ExploreNorth, false, false);
                    }
                    else
                    {
                        return new CharacterAction(ActionID.ExploreSouth, false, false);
                    }
                }
            }
            else //if (inCombat)
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
        }

        public CharacterAction GetNextRangerFarmAction()
        {
            if (currentLevel > 250 && currentPotionTPCount < MinPotionCount)
            {
                if (currentGold < 500 * 20)
                {
                    return new CharacterAction(ActionID.WithdrawGold, false, false, 500 * 20);
                }
                else
                {
                    return new CharacterAction(ActionID.BuyPotionTP);
                }
            }
            else if (CurrentTravelPoints < 5)
            {
                if (currentLevel > 250)
                {
                    return new CharacterAction(ActionID.UsePotionTP);
                }
                else if (currentGold < currentLevel * 10)
                {
                    return new CharacterAction(ActionID.WithdrawGold, false, false, currentLevel * 10);
                }
                else
                {
                    return new CharacterAction(ActionID.SleepAtInn, false, false, currentLevel * 10);
                }
            }
            else
            {
                int lowestCount = CurrentWoodCount;
                if (CurrentFishCount < lowestCount)
                    lowestCount = CurrentFishCount;
                if (CurrentStoneCount < lowestCount)
                    lowestCount = CurrentStoneCount;
                if (CurrentIronCount < lowestCount)
                    return new CharacterAction(ActionID.RangerIron50);

                if (CurrentWoodCount == lowestCount)
                    return new CharacterAction(ActionID.RangerWood50);
                if (CurrentFishCount == lowestCount)
                    return new CharacterAction(ActionID.RangerFish50);
                return new CharacterAction(ActionID.RangerStone50);
            }
        }
    }
}
