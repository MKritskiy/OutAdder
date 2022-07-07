using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Shapes;

namespace OutAdder.resources
{
    class AudioFileInfo
    {
        string Name;
        ImageSource FileIcon;
        public string Duration { get; set; }
        public AudioFileInfo()
        {
            Name = "";
            SetIcon("default.mp3", false);
        }
        public string GetName()
        {
            return Name;
        }
        public void SetName(string path)
        {
            Name = path.Split('\\').Last();
        }

        
        public void SetIcon(string strPath, bool bSmall)
        {
            Interop.SHFILEINFO info = new Interop.SHFILEINFO(true);
            int cbFileInfo = Marshal.SizeOf(info);
            Interop.SHGFI flags;
            if (bSmall)
                flags = Interop.SHGFI.Icon | Interop.SHGFI.SmallIcon | Interop.SHGFI.UseFileAttributes;
            else
                flags = Interop.SHGFI.Icon | Interop.SHGFI.LargeIcon | Interop.SHGFI.UseFileAttributes;

            Interop.SHGetFileInfo(strPath, 256, out info, (uint)cbFileInfo, flags);

            IntPtr iconHandle = info.hIcon;
            //if (IntPtr.Zero == iconHandle) // not needed, always return icon (blank)
            //  return DefaultImgSrc;
            ImageSource img = Imaging.CreateBitmapSourceFromHIcon(
                        iconHandle,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
            Interop.DestroyIcon(iconHandle);
            FileIcon = img;
        }

        public ImageSource GetIcon()
        {
            return FileIcon;
        }
    }
}
