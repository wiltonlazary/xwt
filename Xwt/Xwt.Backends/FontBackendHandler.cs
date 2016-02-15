// 
// IFontBackendHandler.cs
//  
// Author:
//       Lluis Sanchez <lluis@xamarin.com>
// 
// Copyright (c) 2011 Xamarin Inc
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using Xwt.Drawing;
using System.Collections.Generic;

namespace Xwt.Backends
{
	public abstract class FontBackendHandler: BackendHandler
	{
		Font systemFont;
		Font systemMonospaceFont;
		Font systemSerifFont;
		Font systemSansSerifFont;

		internal Font SystemFont {
			get {
				if (systemFont == null)
					systemFont = new Font (GetSystemDefaultFont (), ApplicationContext.Toolkit);
				return systemFont;
			}
		}

		internal Font SystemMonospaceFont {
			get {
				if (systemMonospaceFont == null) {
					var f = GetSystemDefaultMonospaceFont ();
					if (f != null)
						systemMonospaceFont = new Font (f, ApplicationContext.Toolkit);
					else
					{
						switch(Desktop.DesktopType) {
							case DesktopType.Linux:
								systemMonospaceFont = SystemFont.WithFamily ("FreeMono, Nimbus Mono L, Courier New, Courier, monospace");
							break;

							case DesktopType.Mac:
								systemMonospaceFont = SystemFont.WithFamily ("Menlo, Monaco, Courier New, Courier, monospace");
							break;

							default:
								systemMonospaceFont = SystemFont.WithFamily ("Lucida Console, Courier New, Courier, monospace");
								break;
						}
					}
				}
				return systemMonospaceFont;
			}
		}

		internal Font SystemSerifFont {
			get {
				if (systemSerifFont == null) {
					var f = GetSystemDefaultSerifFont ();
					if (f != null)
						systemSerifFont = new Font (f, ApplicationContext.Toolkit);
					else
					{
						switch(Desktop.DesktopType) {
							case DesktopType.Linux:
								systemSerifFont = SystemFont.WithFamily ("FreeSerif, Bitstream Vera Serif, DejaVu Serif, Likhan, Norasi, Rekha, Times New Roman, Times, serif");
								break;

							case DesktopType.Mac:
								systemSerifFont = SystemFont.WithFamily ("Georgia, Palatino, Times New Roman, Times, serif");
								break;

							default:
								systemSerifFont = SystemFont.WithFamily ("Times New Roman, Times, serif");
								break;
						}
					}
				}
				return systemSerifFont;
			}
		}

		internal Font SystemSansSerifFont {
			get {
				if (systemSansSerifFont == null) {
					var f = GetSystemDefaultSansSerifFont ();
					if (f != null)
						systemSansSerifFont = new Font (f, ApplicationContext.Toolkit);
					else
					{
						switch(Desktop.DesktopType) {
							case DesktopType.Linux:
								systemSansSerifFont = SystemFont.WithFamily ("FreeSans, Nimbus Sans L, Garuda, Utkal, Arial, Helvetica, sans-serif");
							break;

							case DesktopType.Mac:
								systemSansSerifFont = SystemFont.WithFamily ("SF UI Text, Helvetica Neue, Helvetica, Lucida Grande, Lucida Sans Unicode, Arial, sans-serif");
							break;

							default:
								systemSansSerifFont = SystemFont.WithFamily ("Segoe UI, Tahoma, Arial, Helvetica, Lucida Sans Unicode, Lucida Grande, sans-serif");
								break;
						}
					}
				}
				return systemSansSerifFont;
			}
		}

		public abstract object GetSystemDefaultFont ();

		/// <summary>
		/// Gets the system default serif font, or null if there is no default for such font
		/// </summary>
		public virtual object GetSystemDefaultSerifFont ()
		{
			return null;
		}

		/// <summary>
		/// Gets the system default sans-serif font, or null if there is no default for such font
		/// </summary>
		public virtual object GetSystemDefaultSansSerifFont ()
		{
			return null;
		}

		/// <summary>
		/// Gets the system default monospace font, or null if there is no default for such font
		/// </summary>
		public virtual object GetSystemDefaultMonospaceFont ()
		{
			return null;
		}

		public abstract IEnumerable<string> GetInstalledFonts ();

		public abstract IEnumerable<KeyValuePair<string, object>> GetAvailableFamilyFaces (string family);

		/// <summary>
		/// Creates a new font. Returns null if the font family is not available in the system
		/// </summary>
		/// <param name="fontName">Font family name</param>
		/// <param name="size">Size in points</param>
		/// <param name="style">Style</param>
		/// <param name="weight">Weight</param>
		/// <param name="stretch">Stretch</param>
		public abstract object Create (string fontName, double size, FontStyle style, FontWeight weight, FontStretch stretch);

		/// <summary>
		/// Register a font file with the system font manager that is then accessible through Create. The font is only
		/// available during the lifetime of the process.
		/// </summary>
		/// <returns><c>true</c>, if font from file was registered, <c>false</c> otherwise.</returns>
		/// <param name="fontPath">Font path.</param>
		public abstract bool RegisterFontFromFile (string fontPath);

		public abstract object Copy (object handle);
		
		public abstract object SetSize (object handle, double size);
		public abstract object SetFamily (object handle, string family);
		public abstract object SetStyle (object handle, FontStyle style);
		public abstract object SetWeight (object handle, FontWeight weight);
		public abstract object SetStretch (object handle, FontStretch stretch);
		
		public abstract double GetSize (object handle);
		public abstract string GetFamily (object handle);
		public abstract FontStyle GetStyle (object handle);
		public abstract FontWeight GetWeight (object handle);
		public abstract FontStretch GetStretch (object handle);
		
	}
}

