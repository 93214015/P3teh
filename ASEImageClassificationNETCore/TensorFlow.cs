using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace ASE
//{
//    public class TFInputImageData
//    {
//        //public System.Drawing.Bitmap Image;
//        public byte[] Image;
//    };

//    public class TFImagePrediction
//    {
//        [ColumnName("model_1/dense_1/Softmax")]
//        public float[] PredictedLabels;
//    }


//    public class TFImageClassifier
//    {
//        public TFImageClassifier(string _ModelFilePath, string _ImageForlder)
//        {
//            m_MLContext = new MLContext();

//            var _tfModel = m_MLContext.Model.LoadTensorFlowModel(_ModelFilePath);
//            var _Schema = _tfModel.GetInputSchema();
//            var _s = _tfModel.GetModelSchema();

//            var pipeline =
//                m_MLContext.Transforms.ResizeImages("x", 50, 50, "Image")
//                .Append(m_MLContext.Transforms.ConvertToGrayscale("x"))
//                .Append(m_MLContext.Model.LoadTensorFlowModel(_ModelFilePath).ScoreTensorFlowModel("model_1/dense_1/Softmax", "x", true));


//            string[] filesTesting = System.IO.Directory.GetFiles(_ImageForlder, "*.png", System.IO.SearchOption.AllDirectories);

//            int _ImageCount = 40;

//            TFInputImageData[] _InputDataList = new TFInputImageData[_ImageCount];

//            for (int i = 0; i < _ImageCount; i++)
//            {
//                using (var _MemoryStream = new MemoryStream())
//                {
//                    new System.Drawing.Bitmap(filesTesting[i]).Save(_MemoryStream, System.Drawing.Imaging.ImageFormat.Png);

//                    _InputDataList[i] = new TFInputImageData { Image = _MemoryStream.GetBuffer()  };
//                }
//            }

//            var _Data = m_MLContext.Data.LoadFromEnumerable<TFInputImageData>(_InputDataList);

//            m_Model = pipeline.Fit(_Data);
//        }

//        public void Predict(string _ImageForlder)
//        {

//            string[] filesTesting = System.IO.Directory.GetFiles(_ImageForlder, "*.png", System.IO.SearchOption.AllDirectories);

//            int _ImageCount = 40;

//            TFInputImageData[] _InputDataList = new TFInputImageData[_ImageCount];

//            for (int i = 0; i < _ImageCount; i++)
//            {
//                using (var _MemoryStream = new MemoryStream())
//                {
//                    new System.Drawing.Bitmap(filesTesting[i]).Save(_MemoryStream, System.Drawing.Imaging.ImageFormat.Png);

//                    _InputDataList[i] = new TFInputImageData { Image = _MemoryStream.GetBuffer() };
//                }
//            }


//            var _Data = m_MLContext.Data.LoadFromEnumerable<TFInputImageData>(_InputDataList);

//            var _Result = m_Model.Transform(_Data);

//            var R = _Result.GetColumn<float[]>("PredictedLabels");
//        }



//        private MLContext m_MLContext;
//        private ITransformer m_Model;
//    }
//}
