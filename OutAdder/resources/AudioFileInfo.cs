using System;
using System.Linq;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace OutAdder.resources
{
    class AudioFileInfo : NAudio.Wave.AudioFileReader
    {
        /// <summary>
        /// Имя файла
        /// </summary>
        string ?Name;

        ///<summary>
        ///Иконка файла
        ///</summary>
        ImageSource ?FileIcon;

        ///<summary> 
        ///Продолжительность аудиофайла
        ///</summary>
        public string ?Duration { get; set; }

        ///<summary> 
        ///Конструктор
        ///</summary>
        public AudioFileInfo(string AudioFileName) : base(AudioFileName)
        {
            SetName(AudioFileName);
            SetIcon(AudioFileName, false);
            SetDuration(this.TotalTime);
        }

        ///<summary> 
        ///Получить имя файла
        ///</summary> 
        public string GetName()
        {
            return Name;
        }

        ///<summary>
        ///Установить имя файла находящегося в path
        ///</summary>
        public void SetName(string path)
        {
            Name = path.Split('\\').Last();
        }

        ///<summary>
        ///Изменить текущую иконку
        ///</summary>
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

        ///<summary>
        ///Получить текущую иконку
        ///</summary>
        public ImageSource GetIcon()
        {
            return FileIcon;
        }

        ///<summary>
        ///Установить продолжительность аудиофайла
        ///из TimeSpan формата в String
        ///</summary>
        public void SetDuration(TimeSpan totalTime)
        {
            Duration = String.Format("{0:00}:{1:00}",
                totalTime.Minutes, totalTime.Seconds /*, audioFile.TotalTime.Milliseconds*/);
        }
    }
}
