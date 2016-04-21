using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole {
    class Program {
        static void Main(string[] args) {
            var ws = WinApi.User.Window.GetWindows(true).Where(x=>x.ClassName?.Length > 0).ToArray();
        }
    }
}
