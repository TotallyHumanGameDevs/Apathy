using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apathy
{
    internal class Room
    {
        public readonly string name;
        public readonly Dictionary<string, List<string>> conjunctions = new Dictionary<string, List<string>> { { "graveyard", new List<string> { "to", "out to" } } };
        public readonly List<string> roomCmds = new List<string> { "exit" };
        public List<Object> items = new List<Object>();

        public readonly string firstTimeDescription = "Wow there is stuff in this room!";
        public bool entered = false;

        public Room(string name)
        {
            this.name = name;
        }
    }
}
