using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole {
    class Program {
        static void Main(string[] args) {
            WinApi.User.Window.GetWindowByCaption("未命名 - 記事本").First().Size = new WinApi.User.Size() { Width = 1024,Height = 768};
        }
    }
}
