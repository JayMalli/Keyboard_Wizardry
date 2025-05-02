using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Runtime.InteropServices;

namespace MyWpfApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class F1KeyListener : Window
{


    [DllImport("user32.dll")]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll")]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll")]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;

    private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
    private LowLevelKeyboardProc _proc;
    private IntPtr _hookId = IntPtr.Zero;

    // private bool _isCtrlPressed = false;
    // private const int WM_KEYUP = 0x0101;

    public F1KeyListener()
    {
        _proc = HookCallback;
        StartHook();
    }

    private void StartHook()
    {
        _hookId = SetHook(_proc);
    }

    private void StopHook()
    {
        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
            _hookId = IntPtr.Zero;
        }
    }

    private IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using var curProcess = System.Diagnostics.Process.GetCurrentProcess();
        using var curModule = curProcess.MainModule;
        return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        // Track When Key is Pressed
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            Key key = KeyInterop.KeyFromVirtualKey(vkCode);

            if (key == Key.F1)
            {
                MessageBox.Show("F1 key was pressed!");
                return (IntPtr)1; // Prevent further processing
            }

            // Bonus Code : 
            // if (key == Key.LeftCtrl || key == Key.RightCtrl)
            // {
            //     _isCtrlPressed = true;
            // }
            // else if (key == Key.C && _isCtrlPressed)
            // {
            //     MessageBox.Show("Ctrl+C was pressed!");
            //     _isCtrlPressed = false; // Reset the flag after handling
            //     return (IntPtr)1; // Prevent further processing
            // }
        }

        // // Track when Ctrl key is released
        // else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
        // {
        //     int vkCode = Marshal.ReadInt32(lParam);
        //     Key key = KeyInterop.KeyFromVirtualKey(vkCode);

        //     if (key == Key.LeftCtrl || key == Key.RightCtrl)
        //     {
        //         _isCtrlPressed = false;
        //     }
        // }


        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }
    
    protected override void OnClosed(EventArgs e)
    {
        StopHook();
        base.OnClosed(e);
    }
}