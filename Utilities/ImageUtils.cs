using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace yourvrexperience.Utils
{
	public static class ImageUtils
	{
		public static void LoadImage(string _pathFile, Image _image, int _height, int _maximumHeightAllowed)
		{
			if (System.IO.File.Exists(_pathFile))
			{
				byte[] bytes = System.IO.File.ReadAllBytes(_pathFile);
				Texture2D textureOriginal = new Texture2D(1, 1);
				textureOriginal.LoadImage(bytes);
				TransformTexture(textureOriginal, _image, _height, _pathFile, _maximumHeightAllowed);
			}
		}

		public static void TransformTexture(Texture2D _textureOriginal, Image _image, int _height, string _pathFile, int _maximumHeightAllowed)
		{
            if (_image != null)
            {
                if ((_textureOriginal.width > 100) && (_textureOriginal.height > 100))
                {
                    float factorScale = ((float)_maximumHeightAllowed / (float)_textureOriginal.height);
                    Texture2D textureScaled = ImageUtils.ScaleTexture(_textureOriginal, (int)(_textureOriginal.width * factorScale), (int)_maximumHeightAllowed);
                    _image.overrideSprite = ToSprite(textureScaled);
                    float finalWidth = textureScaled.width * ((float)_height / (float)textureScaled.height);
                    _image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(finalWidth, _height);
                }
            }
        }

		public static Texture2D LoadTexture2D(string _pathFile, int _height)
		{
			Texture2D textureRead = null;
			if (System.IO.File.Exists(_pathFile))
			{
				byte[] bytes = System.IO.File.ReadAllBytes(_pathFile);
				Texture2D textureOriginal = new Texture2D(1, 1);
				textureOriginal.LoadImage(bytes);
				textureRead = ScaleTexture2D(textureOriginal, _height);
			}
			return textureRead;
		}

		public static Texture2D ScaleTexture2D(Texture2D _texture, int _height)
		{
			float factorScale = ((float)_height / (float)_texture.height);
			return ImageUtils.ScaleTexture(_texture, (int)(_texture.width * factorScale), (int)_height);
		}

        public static Texture2D LoadBytesTexture(byte[] _pvrtcBytes)
        {
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(_pvrtcBytes);
            return tex;
        }

        public static void LoadBytesRawImage(Image _image, byte[] _pvrtcBytes)
		{
			Texture2D tex = new Texture2D(16, 16, TextureFormat.PVRTC_RGBA4, false);
			tex.LoadRawTextureData(_pvrtcBytes);
			tex.Apply();
			_image.material.mainTexture = tex;
		}

        public static Texture2D LoadBytesRawImage(byte[] _pvrtcBytes, int _width, int _height)
        {
            Texture2D tex = new Texture2D(_width, _height, TextureFormat.PVRTC_RGBA4, false);
            tex.LoadRawTextureData(_pvrtcBytes);
            tex.Apply();
            return tex;
        }

        public static void LoadBytesImage(Image _image, byte[] _pvrtcBytes)
		{
			Texture2D tex = new Texture2D(2, 2);
			tex.LoadImage(_pvrtcBytes);
			_image.material.mainTexture = tex;
		}

		public static void LoadBytesImage(Image _image, int _with, int _height, byte[] _pvrtcBytes)
		{
			Texture2D tex = new Texture2D(_with, _height);
			tex.LoadImage(_pvrtcBytes);
			_image.overrideSprite = ToSprite(tex);
		}

		public static void LoadBytesImage(RawImage _image, int _with, int _height, byte[] _pvrtcBytes)
		{
			Texture2D tex = new Texture2D(_with, _height);
			tex.LoadImage(_pvrtcBytes);
			_image.texture = tex;
		}

		public static void LoadBytesImage(Image _image, byte[] _pvrtcBytes, int _height, int _maximumHeightAllowed)
		{
			try
			{
				Texture2D textureOriginal = new Texture2D(1, 1);
				textureOriginal.LoadImage(_pvrtcBytes);
				float factorScale = ((float)_maximumHeightAllowed / (float)textureOriginal.height);
				Texture2D textureScaled = ImageUtils.ScaleTexture(textureOriginal, (int)(textureOriginal.width * factorScale), (int)_maximumHeightAllowed);
				_image.overrideSprite = ToSprite(textureScaled);
				float finalWidth = textureScaled.width * ((float)_height / (float)textureScaled.height);
				_image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(finalWidth, _height);
				_image.gameObject.SetActive(true);
			}
			catch (Exception err)
			{
	#if DEBUG_MODE_DISPLAY_LOG
				Debug.Log(err.StackTrace);
	#endif
			};
		}

		public static byte[] GetBytesImage(Image _image)
		{
			return _image.sprite.texture.GetRawTextureData();
		}
		public static byte[] GetBytesPNG(Image _image)
		{
			return _image.overrideSprite.texture.EncodeToPNG();
		}
		public static byte[] GetBytesJPG(Image _image)
		{
			return _image.overrideSprite.texture.EncodeToJPG(75);
		}
		public static byte[] GetBytesPNG(Sprite _image)
		{
			return _image.texture.EncodeToPNG();
		}
		public static byte[] GetBytesJPG(Sprite _image)
		{
			return _image.texture.EncodeToJPG(75);
		}

		public static Texture2D ScaleTexture(Texture2D _source, int _targetWidth, int _targetHeight)
		{
			Texture2D result = new Texture2D(_targetWidth, _targetHeight, _source.format, true);
			Color[] rpixels = result.GetPixels(0);
			float incX = ((float)1 / _source.width) * ((float)_source.width / _targetWidth);
			float incY = ((float)1 / _source.height) * ((float)_source.height / _targetHeight);
			for (int px = 0; px < rpixels.Length; px++)
			{
				rpixels[px] = _source.GetPixelBilinear(incX * ((float)px % _targetWidth),
								  incY * ((float)Mathf.Floor(px / _targetWidth)));
			}
			result.SetPixels(rpixels, 0);
			result.Apply();
			return result;
		}

		public static Sprite ToSprite(Texture2D texture)
		{
			return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
		}

        public static Texture2D ToTexture2D(this Texture texture)
        {
            return Texture2D.CreateExternalTexture(
                texture.width,
                texture.height,
                TextureFormat.RGB24,
                false, false,
                texture.GetNativeTexturePtr());
        }

        public static Texture2D RenderToTexture2D(this RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }
    }
}
