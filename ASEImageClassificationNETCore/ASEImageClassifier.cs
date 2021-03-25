using Microsoft.ML;
using Microsoft.ML.Vision;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ASE
{

    public enum EModelArchitecture
    {
        MobileNetV2,
        InceptionV3,
        ResNet250,
        ResNet2101
    }

    public enum ETrainEarlyStoppingCriteria
    {
        None,
        Accuracy,
        Loss
    }



    public class ASEImageClassifier
    {

        public ASEImageClassifier()
        {
            mMLContext = new MLContext();
        }


        /// <summary>
        /// Train the model with images. this fucntion use InMemory Data. 
        /// </summary>
        /// <param name="_dataDirectoryPath">The directory of images</param>
        /// <param name="_relativePath">Indicate that whether the specified directory is relative to the executable program file or not(absolute path)</param>
        /// <param name="_setLabelMethodOnInputData">This paramter specify how the input images should be labeled(classified).</param>
        /// <param name="_preTrainedModel">Select the pretrained model architecture</param>
        /// <param name="_usePreviousTrainingCachedData">If you are using the same data(images) for training with the different parameter values, e.g. Epoch, BatchSize, LearningRate, you can set this parameter true to use previous cached data for faster training. Be aware, in the case of training with new data this paramter should be set to false.</param>
        /// <param name="_epoch">The training iteration count</param>
        /// <param name="_batchSize">Number of samples to use for mini-batch training</param>
        /// <param name="_learningRate">Learning rate to use during optimization</param>
        /// <param name="_validationDataFraction">The fraction of input images which will be used as the validation data for the model</param>
        /// <param name="_earlyStoppingCriteria">Indicate the criteria for stopping the training iterations earlier regarding that metric. It could be disabled by setting the paramter to "None"</param>
        /// <param name="_printOutputsToConsole">If set this parameter to true the training function will print out the progress in the console.</param>
        public void Train(string _dataDirectoryPath
            , bool _relativePath
            , SET_LABEL_METHOD _setLabelMethodOnInputData
            , EModelArchitecture _preTrainedModel = EModelArchitecture.MobileNetV2
            , bool _usePreviousTrainingCachedData = false
            , int _epoch = 200
            , int _batchSize = 10
            , float _learningRate = 0.01f
            , float _validationDataFraction = 0.2f
            , ETrainEarlyStoppingCriteria _earlyStoppingCriteria = ETrainEarlyStoppingCriteria.Accuracy
            , bool _printOutputsToConsole = true)
        {

            if(_relativePath)
                _dataDirectoryPath = Path.Combine(Environment.CurrentDirectory, _dataDirectoryPath);


            var preProcessingPipeline = mMLContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Label", outputColumnName: "LabelKey")
               .Append(mMLContext.Transforms.LoadRawImageBytes(outputColumnName: "Image", imageFolder: _dataDirectoryPath, inputColumnName: "ImagePath"));

            var _inputData = ImageLoader.LoadImageData(mMLContext, _dataDirectoryPath, _setLabelMethodOnInputData);

            var _preProcessedData = preProcessingPipeline.Fit(_inputData).Transform(_inputData);

            var _trainValidationSet = mMLContext.Data.TrainTestSplit(_preProcessedData, _validationDataFraction);
            mTrainData = _trainValidationSet.TrainSet;
            var _validationData = _trainValidationSet.TestSet;

            ImageClassificationTrainer.Architecture _modelArchitecture = ImageClassificationTrainer.Architecture.MobilenetV2;
            switch (_preTrainedModel)
            {
                case EModelArchitecture.MobileNetV2:
                    _modelArchitecture = ImageClassificationTrainer.Architecture.MobilenetV2;
                    break;
                case EModelArchitecture.InceptionV3:
                    _modelArchitecture = ImageClassificationTrainer.Architecture.InceptionV3;
                    break;
                case EModelArchitecture.ResNet250:
                    _modelArchitecture = ImageClassificationTrainer.Architecture.ResnetV250;
                    break;
                case EModelArchitecture.ResNet2101:
                    _modelArchitecture = ImageClassificationTrainer.Architecture.ResnetV2101;
                    break;
                default:
                    throw new Exception("Selected PreTrained Model is not valid");
                    break;
            }


            Action<ImageClassificationTrainer.ImageClassificationMetrics> _metricCallback = null;
            if(_printOutputsToConsole)
            { 
                _metricCallback = (metrics) => Console.WriteLine(metrics);
            }


            ImageClassificationTrainer.EarlyStopping _earlyStopping = null;
            switch (_earlyStoppingCriteria)
            {
                case ETrainEarlyStoppingCriteria.None:
                    break;
                case ETrainEarlyStoppingCriteria.Accuracy:
                    _earlyStopping = new ImageClassificationTrainer.EarlyStopping(.01f, 20, ImageClassificationTrainer.EarlyStoppingMetric.Accuracy);
                    break;
                case ETrainEarlyStoppingCriteria.Loss:
                    _earlyStopping = new ImageClassificationTrainer.EarlyStopping(.01f, 20, ImageClassificationTrainer.EarlyStoppingMetric.Loss);
                    break;
                default:
                    break;
            }

            var _classifierOptions = new ImageClassificationTrainer.Options()
            {
                FeatureColumnName = "Image",
                LabelColumnName = "LabelKey",
                ValidationSet = _validationData,
                Arch = _modelArchitecture,
                MetricsCallback = _metricCallback,
                TestOnTrainSet = false,
                ReuseTrainSetBottleneckCachedValues = _usePreviousTrainingCachedData,
                ReuseValidationSetBottleneckCachedValues = _usePreviousTrainingCachedData,
                WorkspacePath = "ASEModelTrainingWorkspace",
                Epoch = _epoch,
                BatchSize = _batchSize,
                LearningRate = _learningRate,
                EarlyStoppingCriteria = _earlyStopping
            };

            var _trainingPipeline = mMLContext.MulticlassClassification.Trainers.ImageClassification(_classifierOptions)
                .Append(mMLContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"));

            mTrainedModel = _trainingPipeline.Fit(mTrainData);

            mIsTrained = true;
        }

        /// <summary>
        /// Classify the input images
        /// </summary>
        /// <param name="_dataDirectoryPath">The directory of images for classifying</param>
        /// <param name="_isPathRelative">Indicate whether the directory path is reltive to program executbale directory or not</param>
        /// <param name="_setLabelMethod">Indicate how input images should be labeled(classified). in the case of not-labeled input images set the parameter to "NoLabel". Labeled input images are usually used for testing the model.</param>
        public IEnumerable<ImagePrediction> Classify(string _dataDirectoryPath, bool _isPathRelative, SET_LABEL_METHOD _setLabelMethod)
        {
            if(_isPathRelative)
            {
                _dataDirectoryPath = Path.Combine(Environment.CurrentDirectory, _dataDirectoryPath);
            }


            if(!mIsTrained)
            {
                throw new Exception("The Model Should Trained or Load Before Prediction");
            }

            var _inputData = ImageLoader.LoadImageData(mMLContext, _dataDirectoryPath, _setLabelMethod);

            IDataView _preProcessedData;

            if (_setLabelMethod != SET_LABEL_METHOD.NoLabel)
            {
                var preProcessingPipeline = mMLContext.Transforms.Conversion.MapValueToKey(inputColumnName: "Label", outputColumnName: "LabelKey").Append(mMLContext.Transforms.LoadRawImageBytes(outputColumnName: "Image", imageFolder: _dataDirectoryPath, inputColumnName: "ImagePath"));
                _preProcessedData = preProcessingPipeline.Fit(_inputData).Transform(_inputData);
            }
            else
            {
                var preProcessingPipeline = mMLContext.Transforms.LoadRawImageBytes(outputColumnName: "Image", imageFolder: _dataDirectoryPath, inputColumnName: "ImagePath");
                _preProcessedData = preProcessingPipeline.Fit(_inputData).Transform(_inputData);
            }

            var _predictionData = mTrainedModel.Transform(_preProcessedData);

            var _predictions = mMLContext.Data.CreateEnumerable<ImagePrediction>(_predictionData, reuseRowObject: true);

            return _predictions;
        }

        /// <summary>
        /// Classify the input images
        /// </summary>
        /// <param name="_ImageData">Input Image Data (The Image Field (Array of bytes) is mandatory).</param>
        public IEnumerable<ImagePrediction> Classify(IEnumerable<InMemoryImageData> _ImageData)
        {
            if (!mIsTrained)
            {
                throw new Exception("The Model Should Trained or Load Before Prediction");
            }

            IDataView _IDataView = mMLContext.Data.LoadFromEnumerable(_ImageData);

            var _predictionData = mTrainedModel.Transform(_IDataView);

            var _predictions = mMLContext.Data.CreateEnumerable<ImagePrediction>(_predictionData, reuseRowObject: true);

            return _predictions;
        }

        /// <summary>
        /// Save the model to .zip compatible file.
        /// </summary>
        /// <param name="_filePath">The directory used to save the model</param>
        /// <param name="_fileName">The file name used for the model. The extension is not needed</param>
        /// <param name="_isPathRelative">The specified path is relative to the program executable directory or not</param>
        public void SaveModel(string _filePath, string _fileName, bool _isPathRelative = false)
        {
            if(_isPathRelative)
            { 
                _filePath = Path.Combine(Environment.CurrentDirectory, _filePath);
            }

            _filePath = _filePath + "\\" + _fileName + ".zip";

            mMLContext.Model.Save(mTrainedModel, mTrainData.Schema, _filePath);
        }

        /// <summary>
        /// Loading saved model
        /// </summary>
        /// <param name="_filePath">The directory of saved model</param>
        /// <param name="_fileName">File name of saved model. The extension is not needed</param>
        /// <param name="_isPathRelative">Specify the file path is relative to the program executable directory or not</param>
        public void LoadModel(string _filePath, string _fileName, bool _isPathRelative = false)
        {

            if (_isPathRelative)
            {
                _filePath = Path.Combine(Environment.CurrentDirectory, _filePath);
            }

            _filePath = _filePath + "\\" + _fileName + ".zip";

            DataViewSchema _inputSchema;
            mTrainedModel = mMLContext.Model.Load(_filePath, out _inputSchema);
            mIsTrained = true;
        }


        private void PrintPredictions(IEnumerable<ImagePrediction> _predictions)
        {
            foreach (var p in _predictions)
            {
                var _fileName = Path.GetFileName(p.ImagePath);

                string _printLabel = string.IsNullOrEmpty(p.Label) ? "" : $"| ActualLabel : {p.Label}";
                Console.WriteLine($"Prediction Result: {_fileName} { _printLabel } | Prediction : {p.PredictedLabelValue}");
            }
        }

        private MLContext mMLContext;
        private ITransformer mTrainedModel;
        private IDataView mTrainData = null;
        private bool mIsTrained = false;
    }
}
