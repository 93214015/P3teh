using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.ML.Data;

namespace ASE
{
    public class ImageData
    {
        [LoadColumn(0)]
        public string ImagePath;

        [LoadColumn(1)]
        public byte Label;
    }

    public class InMemoryImageData
    {

        public InMemoryImageData()
        {}

        public InMemoryImageData(byte[] _image)
        {
            Image = _image;
        }

        public readonly byte[] Image;
        public UInt32 LabelKey;
    }

    public class InMemoryImageDataLabeled
    {

        public InMemoryImageDataLabeled()
        { }

        public InMemoryImageDataLabeled(byte[] _image, ELabel _label, string _imagePath)
        {
            Image = _image;
            Label = _label;
            ImagePath = _imagePath;
        }

        public readonly byte[] Image;
        public UInt32 LabelKey;
        public readonly ELabel Label;
        public readonly string ImagePath;
    }


    public class ImagePrediction : ImageData
    {
        public byte PredictedLabelValue;
    }


}
