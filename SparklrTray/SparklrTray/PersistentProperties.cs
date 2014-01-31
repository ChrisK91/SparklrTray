using SparklrSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparklrTray
{
    internal static class PersistentProperties
    {
        internal static Connection Conn = new Connection();
        internal static bool LoggedIn = false;

        internal static List<int> ShownIDs = new List<int>();
    }
}
