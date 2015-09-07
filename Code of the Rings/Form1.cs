using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Code_of_the_Rings
{
    public partial class Form1 : Form
    {
        private string[] codes = {
                "MINAS",
                "UMNE TALMAR RAHTAINE NIXENEN UMIR",
                "OOOOOOOOOOOOOOO",
                "BABCDEDCBABCDCB",
                "ZAZYA YAZ",
                "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
                "NONONONONONONONONONONONONONONONONONONONO",
                "GUZ MUG ZOG GUMMOG ZUMGUM ZUM MOZMOZ MOG ZOGMOG GUZMUGGUM",
                "SSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSSEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE",
                "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
                "GAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLUGAAVOOOLLU",
                "O OROFARNE LASSEMISTA CARNIMIRIE O ROWAN FAIR UPON YOUR HAIR HOW WHITE THE BLOSSOM LAY",
                "ALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG BALROG B",
                "OYLO Y OOOYYY LLLYOOYY O YO YLOO O OLY YL OY L YY L YOO LYL YYYOOYLOL L Y O YYYLLOY O L YYYOOYLOL YOLOLOY",
                "TUVWXYZ ABCDEFGHIJ",
                "ABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZAABCDEFGHIJKLMNOPQRSTUVWXYZA",
                "OROZOLOKOTONOFOGOMOJOHOFOTOLOPO ODOYOWOAOZO OPOJOTO OROXOVOXO OC",
                "A B C D E F G H I J K L M N O P Q R S T U V W X Y Z",
                "FIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFOFIFO FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM FUM",
                "GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY GY HIJIHIJIJIJIHIHIHIJIHIJIHIJIJIJIHHHIJIJHIHH",
                "MELLON MORIAMELLON MORIAMORIAMORIAMELLON MELLON MELLON MORIAMORIAMELLON MELLON MORIA",
                "ZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZAZACEGIKMOQSUWY BDFHJLNPRTVXZACEGIKMOQSUWY BDFHJLNPRTVXZA",
                "THREE RINGS FOR THE ELVEN KINGS UNDER THE SKY SEVEN FOR THE DWARF LORDS IN THEIR HALLS OF STONE NINE FOR MORTAL MEN DOOMED TO DIE ONE FOR THE DARK LORD ON HIS DARK THRONEIN THE LAND OF MORDOR WHERE THE SHADOWS LIE ONE RING TO RULE THEM ALL ONE RING TO FIND THEM ONE RING TO BRING THEM ALL AND IN THE DARKNESS BIND THEM IN THE LAND OF MORDOR WHERE THE SHADOWS LIE"
            };

        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < codes.Length; i++)
            {
                codes[i] = codes[i].Replace(' ', '@');
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            char[] board = new char[30];
            for (int i = 0; i < 30; i++)
            {
                board[i] = '@';
            }
            
            int pos = 14;
            board[13] = '@';
            string cmd = ChangeCost(board, 'G', 13, pos);

            string printedText;
            tbOut.Text = ProcessCommand(cmd, board, pos, out printedText) + Environment.NewLine;

            for (int i = 0; i < codes.Length; i++)
            {
                tbOut.Text += ":: " + codes[i] + Environment.NewLine;
                tbOut.Text += FindSeqRepeatingSequences(codes[i]);
                tbOut.Text += Environment.NewLine + Environment.NewLine;
            }

        }

        private void btnExpand_Click(object sender, EventArgs e)
        {
            char[] board = ResetBoard();

            StringBuilder sb = new StringBuilder();

            string spell;// = codes[0];
            string cmd;// = GenerateNormalCommands(spell);

            for (int i = 0; i < codes.Length; i++)
            {
                int t;
                ResetBoard(board);
                spell = codes[i];
                cmd = GenerateNormalCommands(board, spell, 0, out t);
                sb.AppendLine(spell);
                sb.AppendLine(cmd);
                sb.AppendLine(ProcessCommand(cmd, board, 0, out spell));
                sb.AppendLine(spell);
                sb.AppendLine();
            }

            tbOut.Text = sb.ToString();
        }

        private void btnMod_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < codes.Length; i++)
            {
                string cmd = GenerateBestCommands(codes[i]);
                string genSpell;
                ProcessCommand(cmd, ResetBoard(), 0, out genSpell);
                sb.AppendLine(codes[i]);
                sb.AppendLine(genSpell);
                sb.AppendLine(cmd);
                sb.AppendLine("LEN = " + cmd.Length);
                sb.AppendLine("Valid Answer = " + (genSpell == codes[i]));
                sb.AppendLine();
            }

            tbOut.Text = sb.ToString();
        }

        private string ChangeCost(char[] board, char destChar, int changeIdx, int pos, bool onlySimple = false)
        {
            char rotChar = '+';
            char iRotChar = '-';
            int difference = destChar - board[changeIdx];
            if (difference > 13)
            {
                difference -= 27;
                difference *= -1;
                rotChar = '-';
                iRotChar = '+';
            }
            else if (difference < -13)
            {
                difference += 27;
            }
            else if (difference < 0)
            {
                difference *= -1;
                rotChar = '-';
                iRotChar = '+';
            }

            StringBuilder sb = new StringBuilder();

            int moveDir = changeIdx - pos;
            if (moveDir > 15)
            {
                moveDir -= 30;
            }
            else if (moveDir < -15)
            {
                moveDir += 30;
            }
            if (!onlySimple && difference > 11)
            {
                if (moveDir < 0)
                {
                    if (board[(changeIdx + 1) % 30] == '@')
                    {
                        sb.Append('<', (moveDir + 1) * -1);
                        sb.Append("+[<");
                        sb.Append(rotChar);
                        sb.Append(">++]<");
                        sb.Append(iRotChar, 13 - difference);
                    }
                    else if (board[(changeIdx + 29) % 30] == '@')
                    {
                        sb.Append('<', (moveDir - 1) * -1);
                        sb.Append("+[>");
                        sb.Append(rotChar);
                        sb.Append("<++]>");
                        sb.Append(iRotChar, 13 - difference);
                    }
                }
                else if (moveDir >= 0)
                {
                    if (board[(changeIdx + 29) % 30] == '@')
                    {
                        if (moveDir == 0)
                        {
                            sb.Append('<');
                        }
                        else
                        {
                            sb.Append('>', moveDir - 1);
                        }
                        sb.Append("+[>");
                        sb.Append(rotChar);
                        sb.Append("<++]>");
                        sb.Append(iRotChar, 13 - difference);
                    }
                    else if (board[(changeIdx + 1) % 30] == '@')
                    {
                        sb.Append('>', moveDir + 1);
                        sb.Append("+[<");
                        sb.Append(rotChar);
                        sb.Append(">++]<");
                        sb.Append(iRotChar, 13 - difference);
                    }
                }
            }
            if (difference <= 11 || sb.Length == 0)
            {
                char moveChar = '>';
                if (moveDir < 0)
                {
                    moveChar = '<';
                    moveDir *= -1;
                }
                sb.Append(moveChar, moveDir);
                sb.Append(rotChar, difference);
            }

            return sb.ToString();
        }


        struct SeqRepeat
        {
            public int StartIndex, Length, Count;

            public SeqRepeat(int StartIndex, int Length, int Count)
            {
                this.StartIndex = StartIndex;
                this.Length = Length;
                this.Count = Count;
            }
        }
        private List<SeqRepeat> FindSeqRepeatingSequences(string spell)
        {
            //string rtn = "";
            List<SeqRepeat> r = new List<SeqRepeat>();

            for (int i = 0; i < spell.Length; i++)
            {
                int nxtStart = spell.IndexOf(spell[i], i + 1);
                if (nxtStart > 0)
                {
                    int repeatLength = nxtStart - i;
                    int successCount = 0;
                    string subcheck = spell.Substring(i, repeatLength);
                    while (nxtStart + repeatLength <= spell.Length && subcheck == spell.Substring(nxtStart, repeatLength))
                    {
                        successCount++;
                        nxtStart += repeatLength;
                    }
                    if (successCount > 0)
                    {
                        successCount++;
                        //rtn += "Inline Repeat Found @ char " + i + " that has length " + repeatLength + " and repeats " + successCount + " times." + Environment.NewLine;
                        r.Add(new SeqRepeat(i, repeatLength, successCount));
                        i = nxtStart;
                    }
                }
            }

            return r;
        }

        private string ProcessCommand(string command, char[] board, int pos, out string printedText)
        {
            int cmdIdx = 0;
            StringBuilder ptxt = new StringBuilder();

            while (cmdIdx < command.Length)
            {
                switch (command[cmdIdx])
                {
                    case '>':
                        pos = (pos + 1) % 30;
                        break;
                    case '<':
                        pos = (pos + 29) % 30;
                        break;
                    case '+':
                        if (board[pos] == 'Z')
                        {
                            board[pos] = '@';
                        }
                        else
                        {
                            board[pos]++;
                        }
                        break;
                    case '-':
                        if (board[pos] == '@')
                        {
                            board[pos] = 'Z';
                        }
                        else
                        {
                            board[pos]--;
                        }
                        break;
                    case '[':  // nested brackets not accounted for
                        if (board[pos] == '@') // skip to corresponding last bracket in command
                        {
                            int stack = 1;
                            while (stack > 0)
                            {
                                cmdIdx++;
                                if (command[cmdIdx] == '[')
                                {
                                    stack++;
                                }
                                else if (command[cmdIdx] == ']')
                                {
                                    stack--;
                                }
                            }
                        }
                        else
                        {
                            // continue with command
                        }
                        break;
                    case ']':  // nested brackest not fully accounted for
                        if (board[pos] != '@')  // return to corresponding first bracket in command
                        {
                            int stack = 1;
                            while (stack > 0)
                            {
                                cmdIdx--;
                                if (command[cmdIdx] == ']')
                                {
                                    stack++;
                                }
                                else if (command[cmdIdx] == '[')
                                {
                                    stack--;
                                }
                            }
                        }
                        else
                        {
                            // continue with command
                        }
                        break;
                    case '.':
                        ptxt.Append(board[pos]);
                        break;
                }
                cmdIdx++;
            }

            StringBuilder sb = new StringBuilder(command);
            sb.AppendLine();
            for (int i = 0; i < board.Length; i++)
            {
                sb.Append(board[i]);
                sb.Append(' ');
            }

            printedText = ptxt.ToString();
            return sb.ToString();
        }

        private char[] ResetBoard(char[] board = null)
        {
            if (board == null)
                board = new char[30];

            for (int i = 0; i < 30; i++)
            {
                board[i] = '@';
            }
            return board;
        }

        private class BoardState
        {
            public const int SuggestedDomain = 4; // store the best 4 cases;
            public string Spell;
            public int SpellIdx;
            public char[] Board;
            public string CmdPart;
            public int Pos;
            public BoardState[] SubStates;

            public BoardState(string spell, int spellIdx, int pos, string cmdPart)
            {
                Spell = spell;
                SpellIdx = spellIdx;
                Pos = pos;
                CmdPart = cmdPart;
                //SubStates = new List<BoardState>(SuggestedDomain);
            }

            public int GetShortestSubStateIndex()
            {
                int best = int.MaxValue;
                int bestIdx = -1;
                for (int i = 0; i < SubStates.Length; i++)
                {
                    int tmp = SubStates[i].GetShortestSubStateLen();
                    if (tmp < best)
                    {
                        best = tmp;
                        bestIdx = i;
                    }
                }
                return bestIdx;
            }

            private int GetShortestSubStateLen()
            {
                if (SubStates == null)
                    return 0;

                int best = int.MaxValue;
                for (int i = 0; i < SubStates.Length; i++)
                {
                    int tmp = SubStates[i].GetShortestSubStateLen();
                    if (tmp < best)
                    {
                        best = tmp;
                    }
                }
                return CmdPart.Length + best;
            }
        }

        private string GenerateNormalCommands(char[] board, string spell, int pos, out int finalPos)
        {
            //char[] board = new char[30];
            //for (int i = 0; i < 30; i++)
            //{
            //    board[i] = '@';
            //}

            StringBuilder fullCommand = new StringBuilder();
            for (int i = 0; i < spell.Length; i++)
            {
                int updatedIndex;
                
                fullCommand.Append(FindShortestNextLetter(board, spell[i], pos, out updatedIndex));
                pos = updatedIndex;
                board[updatedIndex] = spell[i];
                fullCommand.Append('.');
            }

            finalPos = pos;
            return fullCommand.ToString();
        }

        private BoardState[] FindSubStates(char[] board, string spell, int spellIdx, int pos, int depth)
        {
            const int domain = BoardState.SuggestedDomain;
            BoardState[] bestCmds = new BoardState[domain];

            spellIdx++;
            char nextChar = spell[spellIdx];
            string originCmd = ChangeCost(board, nextChar, pos, pos);
            bestCmds[0] = new BoardState(spell, spellIdx, pos, originCmd);
            int updatedIndex = pos;
            int maxRange = originCmd.Length;
            if (maxRange < 3)
            {
                maxRange = 3;
            }
            if (maxRange > 6)
            {
                maxRange = 6;
            }
            int low = (pos - maxRange + 30) % 30;
            int high = (pos + maxRange + 1) % 30;

            int i = low;
            while (i != high)
            {
                if (i == pos)
                {
                    i = (i + 1) % 30;
                }
                string tCmd = ChangeCost(board, nextChar, i, pos);
                BoardState tstate = new BoardState(spell, spellIdx, i, tCmd);
                int ii = 0;
                while (ii < domain && tCmd.Length > bestCmds[ii].CmdPart.Length)
                {
                    ii++;
                }
                while (ii < domain)
                {
                    BoardState tmp = bestCmds[ii];
                    bestCmds[ii] = tstate;
                    tstate = tmp;
                }
                i = (i + 1) % 30;
            }

            for (i = 0; i < domain; i++)
            {
                bestCmds[i].Board = (char[])board.Clone();
                bestCmds[i].Board[bestCmds[i].Pos] = nextChar;
                if (depth > 0)
                {
                    bestCmds[i].SubStates = FindSubStates(bestCmds[i].Board, spell, spellIdx, bestCmds[i].Pos, depth - 1);
                }
            }

            return bestCmds;
        }

        private string FindShortestNextLetter(char[] board, char nextChar, int pos, out int updatedIndex)
        {
            //const int rangeReducer = 2;
            string bestCmd;
            string originCmd = ChangeCost(board, nextChar, pos, pos);
            bestCmd = originCmd;
            updatedIndex = pos;
            int maxRange = originCmd.Length;
            if (maxRange > 1)
            {
                if (maxRange > 14)
                {
                    maxRange = 14;
                }
                int low = (pos - maxRange + 30) % 30;
                int high = (pos + maxRange + 1) % 30;

                int i = low;
                while (i != high)
                {
                    if (i == pos)
                    {
                        i = (i + 1) % 30;
                    }
                    string tCmd = ChangeCost(board, nextChar, i, pos);
                    if (tCmd.Length < bestCmd.Length)
                    {
                        bestCmd = tCmd;
                        updatedIndex = i;
                    }
                    i = (i + 1) % 30;
                }
            }
            return bestCmd;
        }

        private string SeqRepeatOuterStructure(char[] board, int outerStartPos, int pos, int repeatCount, string inner)
        {
            StringBuilder sb = new StringBuilder();
            if (repeatCount > 26)
                throw new Exception();
            char destChar = (char)('[' - repeatCount);
            sb.Append(ChangeCost(board, destChar, outerStartPos, pos, true));
            sb.Append("[>");
            sb.Append(inner);
            sb.Append("<+]");

            return sb.ToString();
        }

        private void SeqRepeatInnerStructure(char[] board, string spell, int innerStartPos, int pos, out string setup, out string inner, out char[] finalBoard)
        {
            finalBoard = (char[])board.Clone();

            StringBuilder resetCode = new StringBuilder();
            resetCode.Append(spell[0]);
            finalBoard[innerStartPos] = spell[0];
            StringBuilder sbInner = new StringBuilder(".");
            StringBuilder sbSetup = new StringBuilder(ChangeCost(board, spell[0], innerStartPos, pos, false));
            pos = innerStartPos;
            for (int i = 1; i < spell.Length; i++)
            {
                string tcmd = ChangeCost(finalBoard, spell[i], pos, pos, true);
                if (tcmd.Length > 3)
                {
                    resetCode.Append(spell[i]);
                    sbInner.Append(">.");
                    sbSetup.Append(ChangeCost(finalBoard, spell[i], (pos + 1) % 30, pos, true));
                    pos = (pos + 1) % 30;
                }
                else
                {
                    sbInner.Append(tcmd);
                    sbInner.Append('.');
                }
                finalBoard[pos] = spell[i];
            }
            int move = 0;
            for (int i = resetCode.Length - 1; i >= 0; i--)
            {
                sbSetup.Append('<');
                sbInner.Append(ChangeCost(finalBoard, resetCode[i], (pos + move) % 30, pos, true));
                pos = (pos + move) % 30;
                finalBoard[pos] = resetCode[i];
                move = 29;
            }
            setup = sbSetup.ToString();
            inner = sbInner.ToString();
        }

        private string GenerateBestCommands(string spell)
        {
            StringBuilder sb = new StringBuilder();
            //string spell = codes[15];   // 2 8 9 15
            char[] board = ResetBoard();

            int pos = 0;

            List<SeqRepeat> seqList = FindSeqRepeatingSequences(spell);
            int seqIdx = 0;

            List<string> subSpell = new List<string>();
            if (seqList.Count == 0)
            {
                //subSpell.Add(spell);
                seqIdx = int.MaxValue;
            }

            int spellIdx = 0;
            for (int i = 0; i < seqList.Count; i++)
            {
                int len;
                if (spellIdx != seqList[i].StartIndex)
                {
                    len = seqList[i].StartIndex - spellIdx;
                    subSpell.Add(spell.Substring(spellIdx, len));
                    spellIdx += len;
                }
                len = seqList[i].Length * seqList[i].Count;
                subSpell.Add(spell.Substring(spellIdx, len));
                spellIdx += len;
            }
            if (spellIdx < spell.Length)
            {
                subSpell.Add(spell.Substring(spellIdx));
            }

            spellIdx = 0;
            for (int i = 0; i < subSpell.Count; i++)
            {
                if (seqIdx < seqList.Count && seqList[seqIdx].StartIndex == spellIdx)
                {
                    int finalPos1, finalPos2;
                    char[] b1 = (char[])board.Clone();
                    char[] b2 = (char[])board.Clone();
                    string cmd1 = GenerateNormalCommands(b1, subSpell[i], pos, out finalPos1);
                    string cmd2 = GenerateSeqCommands(b2, subSpell[i], pos, seqList[seqIdx].Length, seqList[seqIdx].Count, out finalPos2);

                    if (cmd1.Length < cmd2.Length)
                    {
                        sb.Append(cmd1);
                        pos = finalPos1;
                        board = b1;
                    }
                    else
                    {
                        sb.Append(cmd2);
                        pos = finalPos2;
                        board = b2;
                    }
                    seqIdx++;
                }
                else
                {
                    int finalPos;
                    sb.Append(GenerateNormalCommands(board, subSpell[i], pos, out finalPos));
                    pos = finalPos;
                }
                spellIdx += subSpell[i].Length;
            }

            return sb.ToString();
        }

        private string GenerateSeqCommands(char[] board, string spell, int pos, int seqLen, int seqCount, out int finalPos)
        {
            spell = spell.Substring(0, seqLen);
            string setup = null, inner = null;
            char[] tboard;
            StringBuilder sb = new StringBuilder();
            while (seqCount > 26)
            {
                sb.Append(GenerateSeqCommands(board, spell, pos, seqLen, 26, out pos));
                seqCount -= 26;
            }

            if (seqCount * seqLen < 13)
            {
                string newSpell = "";
                for (int i = 0; i < seqCount; i++)
                {
                    newSpell += spell;
                }
                int posf;
                sb.Append(GenerateNormalCommands(board, newSpell, pos, out posf));
                pos = posf;
            }
            else
            {
                SeqRepeatInnerStructure(board, spell, (pos + 1) % 30, pos, out setup, out inner, out tboard);
                sb.Append(setup);
                //board = tboard;
                //pos++;

                sb.Append(SeqRepeatOuterStructure(tboard, pos, pos, seqCount, inner));
                //pos = (pos + 29) % 30;
                tboard[pos] = '@';

                Array.Copy(tboard, board, 30);
            }
            finalPos = pos;
            return sb.ToString();
        }
    }
}
