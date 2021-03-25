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
        public string Label;
    }

    public class InMemoryImageData
    {

        public InMemoryImageData()
        {}

        public InMemoryImageData(byte[] _image, string _label, string _imagePath)
        {
            Image = _image;
            Label = _label;
            ImagePath = _imagePath;
        }

        public readonly byte[] Image;
        public UInt32 LabelKey;
        public readonly string Label;
        public readonly string ImagePath;
    }


    public class ImagePrediction : ImageData
    {
        public string PredictedLabelValue;
    }


}
