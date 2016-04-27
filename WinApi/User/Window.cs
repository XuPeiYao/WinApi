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
            set {
                Win32.User.SetParent(this.Id, value.Id);
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

        public Position Position {
            get {
                Win32.RECT rect = GetWindowRect();
                return new Position() { X = rect.Left, Y = rect.Top };
            }
            set {
                var rect = GetWindowRect();
                Win32.User.SetWindowPos(this.Id, IntPtr.Zero, value.X, value.Y, 0, 0, Win32.User.SWP_NOSIZE | Win32.User.SWP_NOOWNERZORDER);
            }
        }

        public Size Size {
            get {
                Win32.RECT rect = default(Win32.RECT);
                Win32.User.GetClientRect(this.Id, ref rect);
                return new Size() { Width = rect.Right, Height = rect.Bottom };
            }
            set {
                Win32.RECT rect = default(Win32.RECT);
                Win32.User.GetClientRect(this.Id, ref rect);
                Win32.User.SetWindowPos(this.Id, IntPtr.Zero, Position.X, Position.Y, value.Width, value.Height, Win32.User.SWP_NOOWNERZORDER);
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
        
        
        public void Focus() {
            Win32.User.SetForegroundWindow(this.Id);
            Win32.User.SetActiveWindow(this.Id);
        }

        public void Flash() {
            Win32.User.FlashWindow(this.Id, 1);
        }

        public void Maximized() {
            Win32.User.ShowWindow(this.Id, Win32.User.SW_SHOWMAXIMIZED);
        }

        public void Minimized() {
            Win32.User.ShowWindow(this.Id, Win32.User.SW_SHOWMINIMIZED);
        }

        public void Restore() {
            Win32.User.ShowWindow(this.Id, Win32.User.SW_RESTORE);
        }

        public void Close() {
            Win32.User.SendMessage(this.Id, Win32.User.WM_CLOSE, 0, IntPtr.Zero);
        }

        #region private method
        private Win32.RECT GetWindowRect() {
            Win32.RECT result = default(Win32.RECT);
            Win32.User.GetWindowRect(this.Id, ref result);
            return result;
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

        public static Window GetWindowById(IntPtr Id) {
            return new Window() { Id = new IntPtr(Win32.User.GetWindow(Id, 0))};
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

        public static IReadOnlyList<Window> GetWindowByCaption(string Caption) {
            return GetWindows().Where(x => x.Caption == Caption).ToList();
        }

        public static IReadOnlyList<Window> GetWindowByClassName(string ClassName) {
            return GetWindows().Where(x => x.ClassName == ClassName).ToList();
        }

        public static Window GetTopWindow() {
            return new Window() { Id = new IntPtr(Win32.User.GetTopWindow(IntPtr.Zero)) };
        }

        public static Window GetActiveWindow() {
            return new Window() { Id = new IntPtr(Win32.User.GetForegroundWindow()) };
        }
    }
}
