using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Apathy
{
    //  "THE LAST ROOM"

    internal class Input
    {
        public MainWindow mW;
        public Room shed = new Room("shed");
        Room clearing = new Room("clearing");
        Room cemetary = new Room("cemetary");
        public Room currentRoom = new Room("test");
        Room lastRoom = null;
        readonly List<string> cmds = new List<string> {
        "go", "move", "advance", "progress", "walk", "travel", "head", "stroll", "saunter", "trudge", "amble", "plod",
        "tramp", "march", "stride", "run", "sprint", "dart", "rush", "dash", "scurry", "scamper", "bolt", "bound", "charge",
        "jog", "trot", "crawl", "dogtrot", "jogtrot", "inch", "creep", "lumber", "trek", "skip", "leave", "return", "backtrack" };
        readonly Dictionary<string, Room> roomKeys = new Dictionary<string, Room>
        {
            { "shed", new Room("shed") },
            { "the cemetary", new Room("cemetary") }
        };
        readonly Dictionary<string, Room> targetRoomCmds = new Dictionary<string, Room>();
        readonly Dictionary<string, Object> itemKeys = new Dictionary<string, Object>();
        readonly List<string> pickUpCmds = new List<string>();
        readonly List<string> putDownCmds = new List<string>();

        public void PlayerInput(string playerInput)
        {
            playerInput = playerInput.ToLower();
            playerInput = playerInput.Trim(new char[] { ' ' });
            playerInput = playerInput.TrimEnd(new char[] { '.', '!', '?' });
            List<string> inputWords = playerInput.Split(new char[] { ' ' }).ToList();

            if (playerInput == "test")
            {
                mW.GuideTalk("\n3*Testing, testing. One, two, three.", 80);
            }
            else if (inputWords[0] == "leave" && inputWords.Count == 1)
            {
                if (currentRoom != cemetary)
                {
                    TryMoveTo(inputWords, cemetary);
                }
                else
                {
                    TryMoveTo(inputWords, clearing);
                }
            }
            else if (cmds.Contains(inputWords[0]))
            {
                bool firstWordReturn = false;
                if (inputWords[0] == "return" || inputWords[0] == "backtrack")
                {
                    if (inputWords.Count == 1 && lastRoom != null)
                    {
                        TryMoveTo(inputWords, lastRoom);
                    }
                    else
                    {
                        firstWordReturn = true;
                    }
                }

                if (inputWords.Count > 1)
                {
                    if (inputWords[1] == "back" && !firstWordReturn && inputWords.Count == 2 && lastRoom != null)
                    {
                        TryMoveTo(inputWords, lastRoom);
                        return;
                    }
                    List<string> _inputWords = new List<string>(inputWords);
                    if (_inputWords[1] == "back" && !firstWordReturn)
                        _inputWords.RemoveAt(1);
                    if (!FindRoom(_inputWords, true) && _inputWords.Count > 1)
                    {
                        if (!FindItem(_inputWords))
                        {
                            if ((_inputWords[1] == "outside" || _inputWords[1] == "out")
                                && _inputWords.Count == 2)
                            {
                                if (currentRoom == shed)
                                {
                                    TryMoveTo(_inputWords, clearing);
                                }
                                else
                                {
                                    // YOURE ALREADY OUTSIDE
                                }
                            }
                            else if (_inputWords[1] == "inside" || _inputWords[1] == "in")
                            {
                                if (currentRoom != shed)
                                {
                                    TryMoveTo(inputWords, shed);
                                }
                                else
                                {
                                    // YOURE ALREADY INSIDE
                                }
                            }
                        }
                    }
                }
            }
            else if (targetRoomCmds.TryGetValue(inputWords[0], out Room roomCmdtargetRoom))
            {
                List<string> _inputWords = new List<String>(inputWords);
                _inputWords.RemoveAt(0);
                List<string> keyWords = new List<String>(_inputWords);
                while (keyWords.Count > 0)
                {
                    string combinedInputWords = String.Join(" ", keyWords);
                    if (itemKeys.TryGetValue(combinedInputWords, out Object targetItem))
                    {
                        Room targetRoom = targetItem.room;
                        List<string> conjunctionList = new List<string>(_inputWords);
                        conjunctionList.RemoveRange(_inputWords.Count - keyWords.Count, keyWords.Count);
                        string conjunction = String.Join(" ", conjunctionList);
                        if (currentRoom.conjunctions[targetRoom.name].Contains(conjunction) && targetRoom == roomCmdtargetRoom)
                        {
                            TryMoveTo(_inputWords, targetRoom);
                        }
                        else continue;
                    }
                    else if (roomKeys.TryGetValue(combinedInputWords, out Room targetRoom))
                    {
                        List<string> conjunctionList = new List<string>(_inputWords);
                        conjunctionList.RemoveRange(_inputWords.Count - keyWords.Count, keyWords.Count);
                        string conjunction = String.Join(" ", conjunctionList);
                        if (currentRoom.conjunctions[targetRoom.name].Contains(conjunction) && targetRoom == roomCmdtargetRoom)
                        {
                            TryMoveTo(_inputWords, targetRoom);
                        }
                        else continue;
                    }
                    keyWords.RemoveAt(0);
                }
            }
            else if (currentRoom.roomCmds.Contains(inputWords[0]) || inputWords[0] == "leave")
            {
                if (!FindItem(inputWords))
                {
                    List<string> _inputWords = new List<String>(inputWords);
                    _inputWords.RemoveAt(0);

                    if (currentRoom.name == roomKeys[String.Join(" ", _inputWords)].name)
                    {
                        if (currentRoom != cemetary)
                        {
                            TryMoveTo(inputWords, cemetary);
                        }
                        else
                        {
                            //  NOP YOU CANNOT
                        }
                    }
                    else
                    {
                        List<string> roomKeyWords = new List<String>(_inputWords);
                        while (roomKeyWords.Count > 0)
                        {
                            string combinedInputWords = String.Join(" ", roomKeyWords);
                            if (roomKeys.TryGetValue(combinedInputWords, out Room targetRoom))
                            {
                                List<string> conjunctionList = new List<string>(_inputWords);
                                conjunctionList.RemoveRange(_inputWords.Count - roomKeyWords.Count, roomKeyWords.Count);
                                string conjunction = String.Join(" ", conjunctionList);
                                if (currentRoom.conjunctions[targetRoom.name].Contains(conjunction) && targetRoom != currentRoom)
                                {
                                    TryMoveTo(inputWords, targetRoom);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void TryMoveTo(List<string> playerInput, Room room)
        {
            lastRoom = currentRoom;
            currentRoom = room;

            bool isBack = false;
            if (playerInput.Count > 1)
                if (playerInput[1] == "back")
                    isBack = true;
            string verb;
            if ((playerInput[0] == "backtrack" || playerInput[0] == "return") && playerInput.Count == 1)
            {
                isBack = true;
                verb = playerInput[0] + "s";
            }
            else
                verb = playerInput[0] + (playerInput[0] != "go" ? "s " : "es ");
            playerInput.RemoveAt(0);
            mW.GuideTalk("2\nThe gravedigger " + verb + String.Join(" ", playerInput) + (isBack ? " to the " + room.name : "") + "." +
                (!room.entered ? "\n" + room.firstTimeDescription : ""), 45);
            room.entered = true;
        }

        private bool FindRoom(List<string> _inputWords, bool needsConjunction)
        {
            List<string> inputWords = new List<String>(_inputWords);
            inputWords.RemoveAt(0);
            List<string> roomKeyWords = new List<String>(inputWords);
            while (roomKeyWords.Count > 0)
            {
                string combinedInputWords = String.Join(" ", roomKeyWords);
                if (roomKeys.TryGetValue(combinedInputWords, out Room targetRoom))
                {
                    List<string> conjunctionList = new List<string>(inputWords);
                    conjunctionList.RemoveRange(inputWords.Count - roomKeyWords.Count, roomKeyWords.Count);
                    string conjunction = String.Join(" ", conjunctionList);
                    if (currentRoom.conjunctions[targetRoom.name].Contains(conjunction))
                    {
                        TryMoveTo(_inputWords, targetRoom);
                    }
                    return true;
                }
                roomKeyWords.RemoveAt(0);
            }

            return false;
        }

        private bool FindItem(List<string> _inputWords)
        {
            List<string> inputWords = new List<String>(_inputWords);
            inputWords.RemoveAt(0);
            List<string> itemKeyWords = new List<String>(inputWords);
            while (itemKeyWords.Count > 0)
            {
                string itemKey = String.Join(" ", itemKeyWords);
                if (itemKeys.TryGetValue(itemKey, out Object targetItem))
                {
                    Room targetRoom = targetItem.room;

                    List<string> conjunctionList = new List<string>(_inputWords);
                    conjunctionList.RemoveRange(_inputWords.Count - itemKeyWords.Count, itemKeyWords.Count);
                    string conjunction = String.Join(" ", conjunctionList);
                    if (currentRoom.conjunctions[targetRoom.name].Contains(conjunction))
                    {
                        TryMoveTo(_inputWords, targetRoom);
                    }
                    return true;
                }
                itemKeyWords.RemoveAt(0);
            }

            return false;
        }
    }
}
