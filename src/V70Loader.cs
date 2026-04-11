using ii.Min.Model;

namespace ii.Min;

public class V70Loader(BinaryReader reader)
{
	internal BinaryReader Reader { get; } = reader;
	private readonly List<IV70Serializable> _objects = [];

	public ushort ReadObjectCount() => Reader.ReadUInt16();

	internal static string DecodeName(byte[] raw)
	{
		if (raw.Length == 0)
		{
			return string.Empty;
		}

		var s = System.Text.Encoding.UTF8.GetString(raw);
		var nul = s.IndexOf('\0');
		if (nul >= 0)
		{
			s = s[..nul];
		}
		return s;
	}

	internal T? ReadObject<T>() where T : class, IV70Serializable
	{
		var idx = Reader.ReadUInt16();
		
		if (idx == 0)
		{ 
			return null;
		}

		if (idx <= _objects.Count)
		{ 
			return _objects[idx - 1] as T;
		}

		var typeIdx = Reader.ReadUInt16();
		if (idx != _objects.Count + 1)
		{ 
			throw new InvalidDataException($"Unexpected new object index {idx}; expected {_objects.Count + 1}.");
		}

		var obj = (IV70Serializable)(typeIdx switch
		{
			1 => new SaveAirPath(),
			2 => new SaveBuilderPath(),
			3 => new SaveComplex(),
			4 => new SaveGroundPath(),
			5 => new SaveUnitInfo(),
			6 => new SaveUnitValues(),
			_ => throw new InvalidDataException($"Unknown serialized class type index {typeIdx}.")
		});

		_objects.Add(obj);
		obj.Deserialize(this);
		return obj as T;
	}

	public SaveUnitPath? ReadUnitPath()
	{
		var idx = Reader.ReadUInt16();

		if (idx == 0)
		{ 
			return null;
		}

		if (idx <= _objects.Count)
		{
			if (_objects[idx - 1] is SaveUnitPath p)
			{ 
				return p;
			}

			throw new InvalidDataException($"Object at {idx} is not a path.");
		}

		var typeIdx = Reader.ReadUInt16();
		if (idx != _objects.Count + 1)
		{ 
			throw new InvalidDataException($"Unexpected path object index {idx}.");
		}

		SaveUnitPath path = typeIdx switch
		{
			1 => new SaveAirPath(),
			2 => new SaveBuilderPath(),
			4 => new SaveGroundPath(),
			_ => throw new InvalidDataException($"Not a unit path type: {typeIdx}.")
		};

		_objects.Add(path);
		path.Deserialize(this);
		return path;
	}
}