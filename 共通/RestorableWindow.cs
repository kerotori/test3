using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;

namespace 共通
{
    public class RestorableWindow : Window
    {
        #region WindowSettings 依存関係プロパティ

        public IWindowSettings WindowSettings
        {
            get { return (IWindowSettings)this.GetValue(WindowSettingsProperty); }
            set { this.SetValue(WindowSettingsProperty, value); }
        }
        public static readonly DependencyProperty WindowSettingsProperty =
            DependencyProperty.Register("WindowSettings", typeof(IWindowSettings), typeof(RestorableWindow), new UIPropertyMetadata(null));

        #endregion

        MultipleControl semaphore;

        protected override void OnSourceInitialized(EventArgs e)
        {
            semaphore = new MultipleControl(this);
            if (!semaphore.IsCreate())
            {
                // 他のプロセスが先にセマフォを作っていた

                this.Visibility = Visibility.Collapsed;
                return; // プログラム終了
            }

            base.OnSourceInitialized(e);

            // 外部からウィンドウ設定の保存・復元クラスが与えられていない場合は、既定実装を使用する
            if (this.WindowSettings == null)
            {
                this.WindowSettings = new WindowSettings(this);
            }

            this.WindowSettings.Reload();

            if (this.WindowSettings.Placement.HasValue)
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                var placement = this.WindowSettings.Placement.Value;
                placement.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
                placement.flags = 0;
                placement.showCmd = (placement.showCmd == SW.SHOWMINIMIZED) ? SW.SHOWNORMAL : placement.showCmd;

                NativeMethods.SetWindowPlacement(hwnd, ref placement);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // セマフォを開放
            semaphore.Releace();

            // クローズ処理
            base.OnClosing(e);

            // 画面位置・サイズを保存
            if (!e.Cancel)
            {
                WINDOWPLACEMENT placement;
                var hwnd = new WindowInteropHelper(this).Handle;
                NativeMethods.GetWindowPlacement(hwnd, out placement);

                this.WindowSettings.Placement = placement;
                this.WindowSettings.Save();
            }
        }
    }
}
