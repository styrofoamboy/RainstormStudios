using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace RainstormStudios.Drawing
{
    public class ImageColorConverter
    {
        #region Global Objects
        //***************************************************************************
        // Private Fields
        // 
        private Bitmap
            _srcImg,
            _destImg;
        private int
            _curPixel,
            _curX,
            _curY;
        //***************************************************************************
        // Delegates
        // 
        private delegate Bitmap BeginConvertColorDelegate(Bitmap img, PixelFormat format);
        //***************************************************************************
        // Public Events
        // 
        public event EventHandler PixelProcessed;
        public event EventHandler ConvertComplete;
        #endregion

        #region Properties
        //***************************************************************************
        // Public Properties
        // 
        public int CurrentPixel
        { get { return this._curPixel; } }
        public int CurrentX
        { get { return this._curX; } }
        public int CurrentY
        { get { return this._curY; } }
        public int PixelCount
        { get { return this._srcImg.Width * this._srcImg.Height; } }
        public int PercentComplete
        { get { return (int)((this._curPixel * 100) / this.PixelCount); } }
        #endregion

        #region Class Constructors
        //***************************************************************************
        // Class Constructors
        // 
        public ImageColorConverter()
        {
            this._curPixel = 0;
            this._destImg = null;
        }
        public ImageColorConverter(Bitmap img)
            : this()
        {
            this._srcImg = img;
        }
        #endregion

        #region Public Methods
        //***************************************************************************
        // Public Methods
        // 
        public Bitmap ConvertTo(PixelFormat format)
        {
            return this.prvConvertImage(this._srcImg, format);
        }
        public void BeginConvertTo(PixelFormat format)
        {
            BeginConvertColorDelegate del = new BeginConvertColorDelegate(this.prvConvertImage);
            del.BeginInvoke(this._srcImg, format, new AsyncCallback(this.BeginConvertToCallback), del);
        }
        public Bitmap EndConvertTo()
        {
            return this._destImg;
        }
        #endregion

        #region Private Methods
        //***************************************************************************
        // Private Methods
        // 
        private Bitmap prvConvertImage(Bitmap img, PixelFormat format)
        {
            lock (img)
                lock (this)
                {
                    int imgW = img.Width;

                    using (Bitmap destImg = new Bitmap(img.Width, img.Height, format))
                    {
                        // Lock bitmap in memory.
                        BitmapData bmpData = destImg.LockBits(
                            new Rectangle(0, 0, destImg.Width, destImg.Height),
                            ImageLockMode.ReadWrite,
                            destImg.PixelFormat);

                        for (int y = 0; y < img.Height; y++)
                            for (int x = 0; x < img.Width; x++)
                            {
                                // Get the source color.
                                Color xyClr = img.GetPixel(x, y);

                                // Get the index of the similar color.
                                byte index = Imaging.GetSimilarColor(destImg.Palette, xyClr);

                                // Write the image data to the bitmap's memory.
                                this.WriteBmpData(x, y, index, bmpData, 8);

                                this._curX = x;
                                this._curY = y;
                                this._curPixel = (imgW * y) + x;
                                this.PixelProcessedEvent();
                            }

                        destImg.UnlockBits(bmpData);
                        return (Bitmap)destImg.Clone();
                    }
                }
        }
        private void BeginConvertToCallback(IAsyncResult state)
        {
            BeginConvertColorDelegate del = (BeginConvertColorDelegate)state.AsyncState;
            this._destImg = del.EndInvoke(state);
            this.ConvertCompleteEvent();
        }
        private void WriteBmpData(int x, int y, byte index, BitmapData data, int pixelSize)
        {
            double entry = pixelSize / 8;

            // Get unmanaged address of needed byte
            IntPtr realByteAddr = new IntPtr(System.Convert.ToInt32(
                                  data.Scan0.ToInt32() +
                                  (y * data.Stride) + x * entry));

            // Create array with data to copy
            byte[] dataToCopy = new byte[] { index };

            // Perfrom copy
            System.Runtime.InteropServices.Marshal.Copy(dataToCopy, 0, realByteAddr,
                          dataToCopy.Length);
        }
        #endregion

        #region Event Triggers
        //***************************************************************************
        // Event Triggers
        // 
        private void PixelProcessedEvent()
        {
            if (this.PixelProcessed != null)
                this.PixelProcessed.Invoke(this, EventArgs.Empty);
        }
        private void ConvertCompleteEvent()
        {
            if (this.ConvertComplete != null)
                this.ConvertComplete.Invoke(this, EventArgs.Empty);
        }
        #endregion
    }
}
