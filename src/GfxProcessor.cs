using ii.Min.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ii.Min;

public class GfxProcessor
{
	private byte[] paletteBytes = null!;

	public List<(Image<Rgba32> image, short hotspotX, short hotspotY)> Read(string filename, string palette)
	{
		const int PaletteLength = 768;

		using var paletteFs = new FileStream(palette, FileMode.Open, FileAccess.Read);
		using var paletteBr = new BinaryReader(paletteFs);
		paletteBytes = paletteBr.ReadBytes(PaletteLength);

		using var fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
		using var br = new BinaryReader(fs);

		var fileLength = (int)fs.Length;
		var imageType = DetermineImageType(br, fileLength);

		List<ImageData>? images = imageType switch
		{
			ImageType.Normal => ProcessNormalImage(br, fileLength),
			ImageType.Multiple => ProcessMultipleImage(br, fileLength) ?? ProcessMultipleShadow(br, fileLength),
			ImageType.Fullscreen => ProcessFullscreenImage(br, fileLength),
			_ => null
		};

		if (images == null || images.Count == 0)
		{
			return [];
		}

		var result = new List<(Image<Rgba32> image, short hotspotX, short hotspotY)>();
		for (var i = 0; i < images.Count; i++)
		{
			var imageData = images[i];
			var image = CreateImage(imageData);
			result.Add((image, imageData.HotspotX, imageData.HotspotY));
		}

		return result;
	}

	private ImageType DetermineImageType(BinaryReader br, int fileLength)
	{
		if (fileLength < 8)
		{
			return ImageType.Unknown;
		}

		// There doesn't seem to be a specific type field, so we have to heuristically guess
		const int MaxWidth = 640;
		const int MaxHeight = 480;
		const int PaletteLength = 768;

		br.BaseStream.Seek(0, SeekOrigin.Begin);
		var val1 = br.ReadInt16(); // 'width', 'count'
		var val2 = br.ReadInt16(); // 'height'
		var val3 = br.ReadInt16(); // 'width2'
		var val4 = br.ReadInt16(); // 'height2'

		// Dimensions are smaller than 640x480, and the file length is width*height + 8 byte (header)
		if (val1 > 0 && val1 <= MaxWidth && val2 > 0 && val2 <= MaxHeight && Math.Abs(val3) <= MaxWidth && Math.Abs(val4) <= MaxHeight && fileLength == val1 * val2 + 8)
		{
			return ImageType.Normal;
		}

		// Count should be less than 100, each image must be at least 12 bytes, + 2 byte (header)
		if (val1 > 0 && val1 < 100 && fileLength >= val1 * 12 + 2)
		{
			return ImageType.Multiple;
		}

		const int MinFullscreenLength = PaletteLength + 8;
		if (fileLength >= MinFullscreenLength && val3 > 0 && val3 <= MaxWidth && val4 > 0 && val4 <= MaxHeight)
		{
			return ImageType.Fullscreen;
		}

		return ImageType.Unknown;
	}

	// Single indexed bitmap: 8-byte header (dimensions + hotspot).
	// Payload is raw palette indices, width×height bytes; no compression.
	private List<ImageData>? ProcessNormalImage(BinaryReader br, int fileLength)
	{
		br.BaseStream.Seek(0, SeekOrigin.Begin);

		var width = br.ReadInt16();
		var height = br.ReadInt16();
		var hotspotX = br.ReadInt16();
		var hotspotY = br.ReadInt16();

		var data = br.ReadBytes(width * height);

		return
		[
			new ImageData
			{
				Width = width,
				Height = height,
				HotspotX = hotspotX,
				HotspotY = hotspotY,
				Data = data,
				Alpha = new byte[width * height]
			}
		];
	}

	// Multi-image shadow: count, offset table, then each sub-image with per-row packed data
	// Scanline RLE fills two palette colors with alternating alpha; palette is rewritten to white/black for output
	private List<ImageData>? ProcessMultipleShadow(BinaryReader br, int fileLength)
	{
		try
		{
			const int CountHeaderBytes = 2;
			const int OffsetTableStrideBytes = 4;
			const int ImageHeaderBytes = 8; // width, height, hotspot x, hotspot y
			const byte RowTerminator = 255;
			const byte OpaqueAlpha = 255;

			br.BaseStream.Seek(0, SeekOrigin.Begin);
			var count = br.ReadInt16();

			var bounds = new int[count];
			for (var i = 0; i < count; i++)
			{
				br.BaseStream.Seek(CountHeaderBytes + i * OffsetTableStrideBytes, SeekOrigin.Begin);
				bounds[i] = br.ReadInt32();
			}

			var images = new List<ImageData>();

			for (var i = 0; i < count; i++)
			{
				var begin = bounds[i];
				var end = fileLength;

				foreach (var boundary in bounds)
				{
					if (boundary > begin && boundary < end)
					{
						end = boundary;
					}
				}

				br.BaseStream.Seek(begin, SeekOrigin.Begin);
				var width = br.ReadInt16();
				var height = br.ReadInt16();
				var hotspotX = br.ReadInt16();
				var hotspotY = br.ReadInt16();

				var data = new byte[width * height];
				var alpha = new byte[width * height];
				Array.Fill(alpha, OpaqueAlpha); // max the alpha so the image is visible (in e.g. explorer)

				br.BaseStream.Seek(begin + ImageHeaderBytes, SeekOrigin.Begin);
				var rows = new int[height];
				for (var row = 0; row < height; row++)
				{
					rows[row] = br.ReadInt32();
				}

				for (var y = 0; y < height; y++)
				{
					var x = 0;
					byte color = 0;
					byte opacity = OpaqueAlpha;
					var blockIndex = 0;

					br.BaseStream.Seek(rows[y] + blockIndex, SeekOrigin.Begin);
					var currentByte = br.ReadByte();

					while (rows[y] + blockIndex < end && currentByte != RowTerminator)
					{
						if (y * width + x + currentByte > width * height)
						{
							return null;
						}

						for (var p = 0; p < currentByte; p++)
						{
							data[y * width + x + p] = color;
							alpha[y * width + x + p] = opacity;
						}

						color = (byte)(1 - color);
						opacity = (byte)(OpaqueAlpha - opacity);
						x += currentByte;
						blockIndex++;

						br.BaseStream.Seek(rows[y] + blockIndex, SeekOrigin.Begin);
						currentByte = br.ReadByte();
					}
				}

				images.Add(new ImageData
				{
					Width = width,
					Height = height,
					HotspotX = hotspotX,
					HotspotY = hotspotY,
					Data = data,
					Alpha = alpha
				});
			}

			// Shadows only use the first two palette entries
			paletteBytes[0] = 255;
			paletteBytes[1] = 255;
			paletteBytes[2] = 255;
			paletteBytes[3] = 0;
			paletteBytes[4] = 0;
			paletteBytes[5] = 0;

			return images;
		}
		catch
		{
			return null;
		}
	}

	// Multi-image sprite: count, file offset per frame, then each sub-image (header + row pointer table + packed rows)
	// Scanline RLE alternates skip vs copy runs; alpha toggles so transparency can encode in the packed stream
	private List<ImageData>? ProcessMultipleImage(BinaryReader br, int fileLength)
	{
		try
		{
			const int CountHeaderBytes = 2;
			const int OffsetTableStrideBytes = 4;
			const int ImageHeaderBytes = 8; // width, height, hotspot x, hotspot y
			const byte RowTerminator = 255;
			const byte OpaqueAlpha = 255;

			br.BaseStream.Seek(0, SeekOrigin.Begin);
			var count = br.ReadInt16();

			var bounds = new int[count];
			for (var idx = 0; idx < count; idx++)
			{
				br.BaseStream.Seek(CountHeaderBytes + idx * OffsetTableStrideBytes, SeekOrigin.Begin);
				bounds[idx] = br.ReadInt32();
			}

			var images = new List<ImageData>();

			for (var i = 0; i < count; i++)
			{
				var begin = bounds[i];
				var end = fileLength;

				foreach (var boundary in bounds)
				{
					if (end > boundary && begin < boundary)
					{
						end = boundary;
					}
				}

				br.BaseStream.Seek(begin, SeekOrigin.Begin);
				var width = br.ReadInt16();
				var height = br.ReadInt16();
				var hotX = br.ReadInt16();
				var hotY = br.ReadInt16();

				var data = new byte[width * height];
				var alpha = new byte[width * height];
				Array.Fill(alpha, OpaqueAlpha); // max the alpha so the image is visible (in e.g. explorer)

				br.BaseStream.Seek(begin + ImageHeaderBytes, SeekOrigin.Begin);
				var rows = new int[height];
				for (var row = 0; row < height; row++)
				{
					rows[row] = br.ReadInt32();
				}

				for (var y = 0; y < height; y++)
				{
					var x = 0;
					bool isCopyRun = false;
					byte opacity = OpaqueAlpha;
					var blockIndex = 0;

					br.BaseStream.Seek(rows[y] + blockIndex, SeekOrigin.Begin);
					byte currentByte = br.ReadByte();

					while (rows[y] + blockIndex < end && currentByte != RowTerminator)
					{
						if (y * width + x + currentByte > width * height)
						{
							return null;
						}

						for (var j = 0; j < currentByte; j++)
						{
							alpha[y * width + x + j] = opacity;
						}

						if (isCopyRun)
						{
							br.BaseStream.Seek(rows[y] + blockIndex + 1, SeekOrigin.Begin);
							br.Read(data, y * width + x, currentByte);
							x += currentByte;
							blockIndex += currentByte + 1;
						}
						else
						{
							for (var j = 0; j < currentByte; j++)
							{
								data[y * width + x + j] = 0;
							}
							x += currentByte;
							blockIndex++;
						}

						isCopyRun = !isCopyRun;
						opacity = (byte)(OpaqueAlpha - opacity);

						br.BaseStream.Seek(rows[y] + blockIndex, SeekOrigin.Begin);
						currentByte = br.ReadByte();
					}
				}

				images.Add(new ImageData
				{
					Width = width,
					Height = height,
					HotspotX = hotX,
					HotspotY = hotY,
					Data = data,
					Alpha = alpha
				});
			}

			return images;
		}
		catch
		{
			return null;
		}
	}

	// Fullscreen indexed image: header and 768-byte embedded palette, then pixel block is compressed.
	// 16-bit RLE stream: count ≤ 0 means repeat the next byte -count times; count > 0 copies count literal bytes (standard packbits-style).
	private List<ImageData>? ProcessFullscreenImage(BinaryReader br, int fileLength)
	{
		try
		{
			const int PaletteLength = 768;
			const int HeaderLength = 8;

			br.BaseStream.Seek(0, SeekOrigin.Begin);
			var hotX = br.ReadInt16();
			var hotY = br.ReadInt16();
			var width = br.ReadInt16();
			var height = br.ReadInt16();

			br.BaseStream.Seek(8, SeekOrigin.Begin);
			paletteBytes = br.ReadBytes(PaletteLength);

			var data = new byte[width * height];

			var startPosition = PaletteLength + HeaderLength;
			var writePosition = 0;

			while (startPosition + 1 < fileLength && writePosition < width * height)
			{
				br.BaseStream.Seek(startPosition, SeekOrigin.Begin);
				var count = br.ReadInt16();
				startPosition += 2;

				if (count <= 0)
				{
					count = (short)-count;
					if (startPosition >= fileLength || count + writePosition > width * height)
					{ 
						return null;
					}

					br.BaseStream.Seek(startPosition, SeekOrigin.Begin);
					byte value = br.ReadByte();

					for (var i = 0; i < count; i++)
					{ 
						data[writePosition + i] = value;
					}

					startPosition++;
					writePosition += count;
				}
				else
				{
					if (count + startPosition > fileLength || count + writePosition > width * height)
					{ 
						return null;
					}

					br.BaseStream.Seek(startPosition, SeekOrigin.Begin);
					br.Read(data, writePosition, count);

					startPosition += count;
					writePosition += count;
				}
			}

			if (startPosition != fileLength || writePosition != width * height)
			{ 
				return null;
			}

			return
            [
                new ImageData
				{
					Width = width,
					Height = height,
					HotspotX = hotX,
					HotspotY = hotY,
					Data = data,
					Alpha = new byte[width * height]
				}
			];
		}
		catch
		{
			return null;
		}
	}

	private Image<Rgba32> CreateImage(ImageData imageData)
	{
		var image = new Image<Rgba32>(imageData.Width, imageData.Height);

		for (var y = 0; y < imageData.Height; y++)
		{
			for (var x = 0; x < imageData.Width; x++)
			{
				var index = y * imageData.Width + x;
				var paletteIndex = imageData.Data[index] * 3;
				var alpha = imageData.Alpha[index] == 0 ? (byte)255 : (byte)0;

				image[x, y] = new Rgba32(
					paletteBytes[paletteIndex],     // r
					paletteBytes[paletteIndex + 1], // g
					paletteBytes[paletteIndex + 2], // b
					alpha                           // a
				);
			}
		}

		return image;
	}
}