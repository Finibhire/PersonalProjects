using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonKnight2_Farmer
{
    public enum ActionID : int
    {
        UserDefined = 0,
        SleepAtInn = 10, DepositGold = 20, DepositDragonPoints = 30, WithdrawGold = 40,

        ExploreNorth = 110, ExploreEast = 120, ExploreSouth = 130, ExploreWest = 140,

        //CastAttack = 210, 
        //CastHeal05 = 251, CastHeal10 = 252, CastHeal25 = 253, CastHeal50 = 254, CastHeal75 = 255,
        CastDamage50 = 265, CastSleep60 = 279, Run = -1,

        GoToTown01 = 2001, GoToTown02 = 2002, GoToTown03 = 2003, GoToTown04 = 2004, GoToTown05 = 2005,
        GoToTown06 = 2006, GoToTown07 = 2007, GoToTown08 = 2008, GoToTown09 = 2009, GoToTown10 = 2010,
        GoToTown11 = 2011, GoToTown12 = 2012, GoToTown13 = 2013, GoToTown14 = 2014, GoToTown15 = 2015,
        GoToTown16 = 2016, GoToTown17 = 2017, GoToTown18 = 2018,

        UsePotionHP = 3001, UsePotionMP = 3002, UsePotionTP = 3003,
        BuyPotionHP = 3501, BuyPotionMP = 3502, BuyPotionTP = 3503,

        //RangerWood05 = 1105, RangerWood10 = 1110, RangerWood15 = 1115, RangerWood20 = 1120, RangerWood30 = 1130, 
        RangerWood50 = 1150,
        //RangerFish05 = 1205, RangerFish10 = 1210, RangerFish15 = 1215, RangerFish20 = 1220, RangerFish30 = 1230, 
        RangerFish50 = 1250,
        //RangerStone05 = 1305, RangerStone10 = 1310, RangerStone15 = 1315, RangerStone20 = 1320, RangerStone30 = 1330, 
        RangerStone50 = 1350,
        //RangerIron05 = 1405, RangerIron10 = 1410, RangerIron15 = 1415, RangerIron20 = 1420, RangerIron30 = 1430, 
        RangerIron50 = 1450
    }
    class CharacterAction
    {
        private ActionID actionid;
        private string actionURL;
        private string actionRefererURL;
        private string actionPOST;
        private bool inCombat;
        private bool fightingBoss;
        private int goldMove;


        public ActionID Action
        {
            get
            {
                return actionid;
            }
            set
            {
                actionid = value;
            }
        }
        public string ActionURL
        {
            get
            {
                return actionURL;
            }
            set
            {
                if (actionURL != value)
                {
                    actionid = ActionID.UserDefined;
                    actionURL = value;
                }
            }
        }
        public string ActionRefererURL
        {
            get
            {
                return actionRefererURL;
            }
            set
            {
                if (actionURL != value)
                {
                    actionid = ActionID.UserDefined;
                    actionRefererURL = value;
                }
            }
        }
        public string ActionPOST
        {
            get
            {
                return actionPOST;
            }
            set
            {
                if (actionPOST != value)
                {
                    actionid = ActionID.UserDefined;
                    actionPOST = value;
                }
            }
        }
        public bool InCombat
        {
            get
            {
                return inCombat;
            }
            set
            {
                string url = LookupActionURL(actionid, fightingBoss);
                if (url != null)
                {
                    actionURL = url;
                }
                inCombat = value;
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
        public int GoldMove
        {
            get
            {
                return goldMove;
            }
            set
            {
                goldMove = value;
            }
        }

        public CharacterAction(ActionID action = ActionID.UserDefined, bool inCombat = false, bool fightingBoss = false, int goldMove = 0)
        {
            actionid = action;
            this.inCombat = inCombat;
            this.fightingBoss = fightingBoss;
            this.goldMove = goldMove;
            actionURL = LookupActionURL(action, fightingBoss);
            actionRefererURL = LookupRefererURL(action, fightingBoss);
            actionPOST = LookupActionPOST(action, goldMove);
        }
        public CharacterAction(string ActionURL, string ActionPOST, bool inCombat = false, bool fightingBoss = false)
        {
            actionid = ActionID.UserDefined;
            this.inCombat = inCombat;
            this.fightingBoss = fightingBoss;
            actionURL = ActionURL;
            actionPOST = ActionPOST;
        }

        public static string LookupRefererURL(ActionID action, bool fightingBoss)
        {
            switch (action)
            {
                case ActionID.SleepAtInn:
                    return "https://dknight2.com/index.php";
                case ActionID.BuyPotionHP:
                case ActionID.BuyPotionMP:
                case ActionID.BuyPotionTP:
                    return "https://dknight2.com/index.php?do=buypotions";
            }
            if (LookupActionPOST(action) == null)
            {
                return "https://dknight2.com/index.php";
            }
            return LookupActionURL(action, fightingBoss);
        }

        public static string LookupActionURL(ActionID action, bool fightingBoss)
        {
            switch (action)
            {
                case ActionID.GoToTown01:
                    return "https://dknight2.com/index.php?do=gotown:1";
                case ActionID.GoToTown02:
                    return "https://dknight2.com/index.php?do=gotown:2";
                case ActionID.GoToTown03:
                    return "https://dknight2.com/index.php?do=gotown:3";
                case ActionID.GoToTown04:
                    return "https://dknight2.com/index.php?do=gotown:4";
                case ActionID.GoToTown05:
                    return "https://dknight2.com/index.php?do=gotown:5";
                case ActionID.GoToTown06:
                    return "https://dknight2.com/index.php?do=gotown:6";
                case ActionID.GoToTown07:
                    return "https://dknight2.com/index.php?do=gotown:7";
                case ActionID.GoToTown08:
                    return "https://dknight2.com/index.php?do=gotown:8";
                case ActionID.GoToTown09:
                    return "https://dknight2.com/index.php?do=gotown:9";
                case ActionID.GoToTown10:
                    return "https://dknight2.com/index.php?do=gotown:10";
                case ActionID.GoToTown11:
                    return "https://dknight2.com/index.php?do=gotown:11";
                case ActionID.GoToTown12:
                    return "https://dknight2.com/index.php?do=gotown:12";
                case ActionID.GoToTown13:
                    return "https://dknight2.com/index.php?do=gotown:13";
                case ActionID.GoToTown14:
                    return "https://dknight2.com/index.php?do=gotown:14";
                case ActionID.GoToTown15:
                    return "https://dknight2.com/index.php?do=gotown:15";
                case ActionID.GoToTown16:
                    return "https://dknight2.com/index.php?do=gotown:16";
                case ActionID.GoToTown17:
                    return "https://dknight2.com/index.php?do=gotown:17";
                case ActionID.GoToTown18:
                    return "https://dknight2.com/index.php?do=gotown:18";
                case ActionID.UsePotionHP:
                    return "https://dknight2.com/index.php?do=potion:1";
                case ActionID.UsePotionMP:
                    return "https://dknight2.com/index.php?do=potion:2";
                case ActionID.UsePotionTP:
                    return "https://dknight2.com/index.php?do=potion:3";
                case ActionID.BuyPotionHP:
                    return "https://dknight2.com/index.php?do=buypotions&buy=1&amount=20";
                case ActionID.BuyPotionMP:
                    return "https://dknight2.com/index.php?do=buypotions&buy=2&amount=20";
                case ActionID.BuyPotionTP:
                    return "https://dknight2.com/index.php?do=buypotions&buy=3&amount=20";
                case ActionID.RangerWood50:
                    return "https://dknight2.com/index.php?do=woodcut";
                case ActionID.RangerFish50:
                    return "https://dknight2.com/index.php?do=fish";
                case ActionID.RangerStone50:
                    return "https://dknight2.com/index.php?do=stone";
                case ActionID.RangerIron50:
                    return "https://dknight2.com/index.php?do=iron";
                case ActionID.WithdrawGold:
                case ActionID.DepositGold:
                    return "https://dknight2.com/index.php?do=bank";
                case ActionID.DepositDragonPoints:
                    return "https://dknight2.com/index.php?do=dpbank";
                case ActionID.ExploreNorth:
                case ActionID.ExploreSouth:
                case ActionID.ExploreWest:
                case ActionID.ExploreEast:
                    return "https://dknight2.com/index.php?do=move";
                case ActionID.SleepAtInn:
                    return "https://dknight2.com/index.php?do=inn";
                case ActionID.Run:
                case ActionID.CastSleep60:
                case ActionID.CastDamage50:
                    if (fightingBoss)
                    {
                        return "https://dknight2.com/index.php?do=fightboss";
                    }
                    return "https://dknight2.com/index.php?do=fight";
            }
            return null;
        }

        public static string LookupActionPOST(ActionID action, int goldMove = 0)
        {
            switch (action)
            {
                case ActionID.SleepAtInn:
                    return "choice=" + goldMove + "+Gold&heal=Full+Heal";
                case ActionID.WithdrawGold:
                    return "bank=Withdraw&withdraw=" + goldMove;
                case ActionID.DepositGold:
                    return "bank=Deposit&deposit=" + goldMove;
                case ActionID.DepositDragonPoints:
                    return "dpbank=Deposit&deposit=" + goldMove;
                case ActionID.ExploreNorth:
                    return "north=North&keydir=";
                case ActionID.ExploreSouth:
                    return "south=South&keydir=";
                case ActionID.ExploreWest:
                    return "west=West&keydir=";
                case ActionID.ExploreEast:
                    return "east=East&keydir=";
                case ActionID.CastSleep60:
                    return "spell=Spell&userspell=13";
                case ActionID.CastDamage50:
                    return "spell=Spell&userspell=10";
                case ActionID.RangerWood50:
                    return "ac=t6&submit=Yes";
                case ActionID.RangerFish50:
                case ActionID.RangerStone50:
                case ActionID.RangerIron50:
                    return "ac=t6&submit=Yes!";
            }
            return null;
        }
    }
}
