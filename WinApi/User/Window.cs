using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win32 = RobertLw.Win32;

namespace WinApi.User {
    public class Window {
        public IntPtr Id { get; private set; }

        #region Window Nodes
        public Window Parent {
            get {
                IntPtr ParentId = new IntPtr(Win32.User.GetParent(Id));
                if (ParentId == IntPtr.Zero) return null;
                return new Window() { Id = ParentId};
            }
        }

        public IReadOnlyList<Window> Children {
            get {
                List<Window> result = new List<Window>();

                Win32.User.EnumChildWindows(Id, (IntPtr hwnd, int lParam) => {
                    var instance = new Window() { Id = hwnd };

                    if (instance.Parent.Id != Id) return true;

                    result.Add(instance);
                    return true;
                }, 0);

                return result;
            }
        }
        #endregion

        #region Window Status
        public string Caption {
            get {
                int captionLength = Win32.User.GetWindowTextLength(Id);

                StringBuilder builder = new StringBuilder(captionLength);
                Win32.User.GetWindowText(Id, builder, captionLength + 1);

                return builder.ToString();
            }
            set {
                Win32.User.SetWindowText(Id, value);
            }
        }

        public bool IsTopWindow {
            get {
                return Window.GetTopWindow() == this;
            }
        }

        public bool IsActiveWindow {
            get {
                return Window.GetActiveWindow() == this;
            }
        }
        #endregion

        #region Process Info
        public IntPtr ProcessId {
            get {
                int processId = 0;
                Win32.User.GetWindowThreadProcessId(Id, ref processId);
                return new IntPtr(processId);
            }
        }

        public Process Process {
            get {
                return Process.GetProcessById(ProcessId.ToInt32());
            }
        }

        public string ClassName {
            get {
                string className = "";
                StringBuilder classText = null;
                try {
                    int cls_max_length = 1000;
                    classText = new StringBuilder("", cls_max_length + 5);
                    Win32.User.GetClassName(Id, classText, cls_max_length + 2);

                    if (!String.IsNullOrEmpty(classText.ToString()) && !String.IsNullOrWhiteSpace(classText.ToString()))
                        className = classText.ToString();
                } catch (Exception ex) {
                    className = ex.Message;
                } finally {
                    classText = null;
                }
                return className;
            }
        }
        #endregion


        public override int GetHashCode() {
            return this.Id.ToInt32();
        }

        public override bool Equals(object obj) {
            Window Obj = obj as Window;
            if (obj == null) return false;
            return Id == Obj.Id;
        }

        public static IReadOnlyList<Window> GetWindows(bool OnlyRootWindow = true) {
            List<Window> result = new List<Window>();

            Win32.User.EnumWindows((IntPtr hwnd, int lParam) => {
                var instance = new Window() { Id = hwnd };

                if (OnlyRootWindow && instance.Parent != null) return true;

                result.Add(instance);
                return true;
            },0);

            return result;
        }

        public static Window GetTopWindow() {
            return new Window() { Id = new IntPtr(Win32.User.GetTopWindow(IntPtr.Zero)) };
        }

        public static Window GetActiveWindow() {
            return new Window() { Id = new IntPtr(Win32.User.GetForegroundWindow()) };
        }
    }
}
