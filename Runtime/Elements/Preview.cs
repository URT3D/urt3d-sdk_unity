using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Urt3d.Utilities;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Urt3d
#pragma warning restore IDE0130 // Namespace does not match folder structure
{
    /// <summary>
    /// Preview is a wrapper for a Texture2D.
    /// </summary>
    public class Preview
    {
        #region Public Methods: Construction and Destruction

        /// <summary>
        /// Construct a new Preview from the given file.
        /// </summary>
        /// <param name="pathToPreviewFile">Fully qualified path to Preview file.</param>
        /// <returns>Fully initialized Preview object.</returns>
        public static async Task<Preview> Construct(string pathToPreviewFile)
        {
            var image = new Texture2D(1, 1, TextureFormat.RGBA32, true)
            {
                wrapMode = TextureWrapMode.Clamp
            };

            var bytes = await File.ReadAllBytesAsync(pathToPreviewFile);
            var isValid = image.LoadImage(bytes);

            if (isValid)
            {
                image.name = Path.GetFileName(pathToPreviewFile);
                return new Preview(image);
            }
            GameObjectUtils.Destroy(image);
            return null;
        }

        /// <summary>
        /// Construct a new Preview from the given file.
        /// </summary>
        /// <param name="pathToPreviewFile">Fully qualified path to Preview file.</param>
        /// <param name="callback">Callback invoked when Preview is constructed.</param>
        public static void Construct(string pathToPreviewFile, Action<Preview> callback)
        {
            var task = Construct(pathToPreviewFile);
            task.ContinueWith(action =>
            {
                if (action.IsFaulted)
                {
                    Debug.LogException(action.Exception);
                }
                callback?.Invoke(action.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Properly dispose of this object.
        /// </summary>
        public void Destroy()
        {
            GameObjectUtils.Destroy(_texture);
        }

        #endregion

        #region Public Methods: Implicit Conversions

        /// <summary>
        /// Implicit conversion from Actor to GameObject
        /// </summary>
        /// <param name="preview">Preview object from which to convert.</param>
        /// <returns>Texture2D associated with the given Preview.</returns>
        public static implicit operator Texture2D(Preview preview)
        {
            return preview._texture;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Construct a new Preview with the given Texture2D.
        /// </summary>
        /// <param name="texture">Texture2D to visually represent this Preview.</param>
        private Preview(Texture2D texture)
        {
            if (texture == null)
            {
                _texture = Texture2D.redTexture;
                Log.Error("Attempted to construct a Preview with an invalid Texture2D");
            }
            else
            {
                _texture = texture;
            }
        }

        #endregion

        #region Private Variables

        private readonly Texture2D _texture;

        #endregion
    }
}
