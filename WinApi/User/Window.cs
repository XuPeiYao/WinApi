using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32 = RobertLw.Win32;

namespace WinApi.User {
    public class Window {
        public IntPtr Id { get; private set; }

        public static Window[] GetWindows() {
            List<Window> result = new List<Window>();

            Win32.User.EnumWindows((IntPtr hwnd, int lParam) => {
                if (Win32.User.GetParent(hwnd).Equals(IntPtr.Zero)) return true;
                
                var instance = new Window() { Id = hwnd };
                result.Add(instance);
                return true;
            },0);

            return result.ToArray();
        }
    }
}
