using System;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using System.Diagnostics;

namespace ASETensorflow
{
    public class TFImageClassifier
    {

        public TFImageClassifier(string _ModelFilePath, bool _IsPathRelative)
        {
            if (_IsPathRelative)
            {
                _ModelFilePath = Path.Combine(Environment.CurrentDirectory, _ModelFilePath);
            }

            byte[] _Buffer = System.IO.File.ReadAllBytes(_ModelFilePath);
            m_Graph = new TensorFlow.TFGraph();
            m_Graph.Import(_Buffer);
            m_Session = new TensorFlow.TFSession(m_Graph);

            string _SampleImagePath = @"Images\ImagePredictionSampleData\p3teClose.png";
            _SampleImagePath = Path.Combine(Environment.CurrentDirectory, _SampleImagePath);
            Bitmap _SampleImage = new Bitmap(_SampleImagePath);

            var _Runner = m_Session.GetRunner();
            var _Tensor = ImageToTensorGrayScale(_SampleImage);
            _Runner.AddInput(m_Graph["x"][0], _Tensor);
            _Runner.Fetch(m_Graph["model_1/dense_1/Softmax"][0]);
            var _Output = _Runner.Run();
            var _VectorResult = _Output[0].GetValue();
            float[,] _Result = (float[,])_VectorResult;
            int[] quantized = QuantizedF(_Result);
        }

        public int[] Predict(Bitmap[] _Images)
        {
            int[] _ResultList = new int[_Images.Length];

            Parallel.For(0, _Images.Length
                , _Index =>
                {
                    var _Runner = m_Session.GetRunner();
                    var _Tensor = ImageToTensorGrayScale(_Images[_Index]);
                    _Runner.AddInput(m_Graph["x"][0], _Tensor);
                    _Runner.Fetch(m_Graph["model_1/dense_1/Softmax"][0]);
                    var _Output = _Runner.Run();
                    var _VectorResult = _Output[0].GetValue();
                    float[,] _Result = (float[,])_VectorResult;
                    int[] quantized = QuantizedF(_Result);
                    _ResultList[_Index] = quantized[1];
                });

            return _ResultList;
        }

        public int[] Predict(string _ImageFolderPath, string _ImageFormat, bool _IsPathRelative, out long PredictionTime, int _LimitImageCount = -1)
        {
            if (_IsPathRelative)
            {
                _ImageFolderPath = Path.Combine(Environment.CurrentDirectory, _ImageFolderPath);
            }

            string[] filesTesting = System.IO.Directory.GetFiles(_ImageFolderPath, "*." + _ImageFormat, System.IO.SearchOption.AllDirectories);

            int _ImageCount = _LimitImageCount != -1 ? _LimitImageCount : filesTesting.Length;

            Bitmap[] _Images = new Bitmap[_ImageCount];

            for (int i = 0; i < _ImageCount; i++)
            {
                _Images[i] = new Bitmap(filesTesting[i]);
            }

            int[] _ResultList = new int[_ImageCount];

            Stopwatch _SW = new Stopwatch();
            _SW.Start();            

            Parallel.For(0, _ImageCount
                , _Index =>
                {
                    var _Runner = m_Session.GetRunner();
                    var _Tensor = ImageToTensorGrayScale(_Images[_Index]);
                    _Runner.AddInput(m_Graph["x"][0], _Tensor);
                    _Runner.Fetch(m_Graph["model_1/dense_1/Softmax"][0]);
                    var _Output = _Runner.Run();
                    var _VectorResult = _Output[0].GetValue();
                    float[,] _Result = (float[,])_VectorResult;
                    int[] quantized = QuantizedF(_Result);
                    _ResultList[_Index] = quantized[1];
                });

            _SW.Stop();

            PredictionTime = _SW.ElapsedMilliseconds;

            return _ResultList;
        }

        /*****************************************
         ************ Private methods
         *****************************************/

        private static TensorFlow.TFTensor ImageToTensorGrayScale(Bitmap image)
        {
            int temp = 0;
            var matrix = new float[1, image.Size.Height, image.Size.Width, 3];
            for (var iy = 0; iy < image.Size.Height; iy++)
            {
                for (int ix = 0, index = iy * image.Size.Width; ix < image.Size.Width; ix++, index++)
                {
                    System.Drawing.Color pixel = image.GetPixel(ix, iy);
                    matrix[0, iy, ix, 0] = pixel.B / 255.0f;//(float)((pixel.B-min)/149.0f);
                    matrix[0, iy, ix, 1] = pixel.G / 255.0f;
                    matrix[0, iy, ix, 2] = pixel.R / 255.0f;
                    temp++;
                }
            }
            return new TensorFlow.TFTensor(matrix);
        }

        private static int[] QuantizedF(float[,] results)
        {
            int[] q = new int[]
            {
                results[0,0]>0.5?1:0,
                results[0,1]>0.5?1:0,
            };
            return q;
        }

        /*****************************************
         ************ Data memebers
         *****************************************/

        private TensorFlow.TFGraph m_Graph;
        private TensorFlow.TFSession m_Session;

    }
}
