using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ASE
{
    public enum SET_LABEL_METHOD
    {
        UseFolderNameAsLabel,
        UsePrefixAsLabel,
        UsePostfixAsLabel,
        UseTSVFile,
        UseCSVFile,
        NoLabel
    }

    public enum ELabel : byte
    {
        None,
        Close,
        Open,
    }


    public class ImageLoader
    {

        /// <summary>
        /// Load image data for classification.
        /// </summary>
        /// <param name="_mlContext">The Microsoft.ML Context Object</param>
        /// <param name="_folder">The folder containing data. If load from source file (TSV or CSV) is selected then the file name should be included</param>
        /// <param name="_setLabelMethod">The method for labeling(classifying) of data</param>
        /// <param name="_shuffleRows">Shuffle the order of data to make some randomness</param>
        /// <returns></returns>

        public static IDataView LoadImageData(MLContext _mlContext, string _folder, SET_LABEL_METHOD _setLabelMethod = SET_LABEL_METHOD.UseFolderNameAsLabel, bool _shuffleRows = true)
        {
            IDataView _data;

            switch (_setLabelMethod)
            {
                case SET_LABEL_METHOD.UseFolderNameAsLabel:
                case SET_LABEL_METHOD.UsePrefixAsLabel:
                case SET_LABEL_METHOD.UsePostfixAsLabel:
                case SET_LABEL_METHOD.NoLabel:
                    {
                        var _IEnumImages = LoadImagesFromDirectory(_folder, _setLabelMethod);
                        _data = _mlContext.Data.LoadFromEnumerable(_IEnumImages);
                        break;
                    }
                case SET_LABEL_METHOD.UseTSVFile:
                    {
                        _data = _mlContext.Data.LoadFromTextFile<ImageData>(path: _folder, separatorChar: '\t', hasHeader: false);
                        break;
                    }
                case SET_LABEL_METHOD.UseCSVFile:
                    {
                        _data = _mlContext.Data.LoadFromTextFile<ImageData>(path: _folder, separatorChar: ',', hasHeader: false);
                        break;
                    }
                default:
                    {
                        throw new Exception("The Set label method is invalid");
                        break;
                    }
            }

            return _mlContext.Data.ShuffleRows(_data);

        }

        private static IEnumerable<ImageData> LoadImagesFromDirectory(string _folder, SET_LABEL_METHOD _setLabelMethod)
        {
            //get all file paths from the subdirectories
            var _files = Directory.GetFiles(_folder, "*", searchOption: SearchOption.AllDirectories);
            //iterate through each file
            foreach (var _file in _files)
            {
                //Image Classification API supports .jpg and .png formats; check img formats
                if ((Path.GetExtension(_file) != ".jpg") && (Path.GetExtension(_file) != ".png"))
                    continue;
                //store filename in a variable, say ‘label’
                var _FileName = Path.GetFileName(_file);
                ELabel _Label = ELabel.None;
                /* If the useFolderNameAsLabel parameter is set to true, then name 
                   of parent directory of the image file is used as the label. Else label is expected to be the file name or a a prefix of the file name. */

                switch (_setLabelMethod)
                {
                    case SET_LABEL_METHOD.UseFolderNameAsLabel:

                        if(!Enum.TryParse<ELabel>(Directory.GetParent(_file).Name, out _Label))
                            throw new Exception("The image label is not matched");

                        break;
                    case SET_LABEL_METHOD.UsePrefixAsLabel:
                        {
                            int i = 0;

                            while (char.IsLetter(_FileName[i]))
                            {
                                i++;
                            }

                            _FileName = _FileName.Substring(0, i);

                            if(!Enum.TryParse<ELabel>(_FileName, out _Label))
                                throw new Exception("The image label is not matched");

                            break;
                        }
                    case SET_LABEL_METHOD.UsePostfixAsLabel:
                        {
                            int _firstLabelCharIndex = _FileName.LastIndexOf('_');
                            if (_firstLabelCharIndex == -1)
                                throw new Exception($"Postfix Labling Method was used but didn't find any postfix label at the file name ({_FileName})");


                            _firstLabelCharIndex++;

                            int _dotIndex = _FileName.LastIndexOf('.');

                            if (_firstLabelCharIndex == _dotIndex || _firstLabelCharIndex > _dotIndex)
                            {
                                throw new Exception($"Failed at finding label : {_FileName}");
                            }

                            _FileName = _FileName.Substring(_firstLabelCharIndex, _dotIndex - _firstLabelCharIndex);

                            if(!Enum.TryParse<ELabel>(_FileName, out _Label))
                                throw new Exception("The image label is not matched");

                            break;
                        }
                    case SET_LABEL_METHOD.NoLabel:
                        {
                            _Label = ELabel.None;
                            break;
                        }
                    default:
                        break;
                }

                //create a new instance of ImageData()
                yield return new ImageData()
                {
                    ImagePath = _file,
                    Label = (byte)_Label
                };

            }
        }
    }
}
