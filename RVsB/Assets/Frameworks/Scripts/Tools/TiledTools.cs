using UnityEngine;
using System.Collections;

public struct TileIndex
{
	public string ImageName;
	public int IndexInImage;
}

/// <summary>
/// Tiled map parser
/// https://github.com/bjorn/tiled/wiki/JSON-Map-Format
/// </summary>
[System.Serializable]
public class TiledMap{
    public int height;
    public TiledLayer[] layers;
    public int nextobjectid;
    public string orientation;
    public TiledProperty[] properties;
    public string renderorder;
    public int tileheight;
    public TiledSet[] tilesets;
    public int tilewidth;
    public int width;
    public int version;

	static public TiledMap FromFile(string file){
		TiledMap map = null;
		string jsonString;
        bool errCode = IOTools.LoadTextFromFile (file, out jsonString);
		if (jsonString==null || jsonString.Length <= 0) {
			Debug.LogAssertion ("Can't open json file: " + file);
		} else {
			map = JsonUtility.FromJson<TiledMap> (jsonString);
			map.Init ();
		}
		return map;
	}

	public void Init()
	{
		int idxStart = 1;
		foreach (var tileSet in tilesets) 
		{
			if (tileSet.StartId <= 0) 
			{
				tileSet.StartId = idxStart;
			}

			if (tileSet.TileCountX <= 0) 
			{
				tileSet.TileCountX = tileSet.tilewidth / tilewidth;	
			}

			if (tileSet.TileCountY <= 0) 
			{
				tileSet.TileCountY = tileSet.tileheight / tileheight;		
			}

			idxStart += tileSet.TileCount;
		}
	}

	public TiledLayer GetLayer(string layerName)
	{
		TiledLayer layer = null;
		foreach(var tempLayer in layers)
		{
			if(tempLayer.name == layerName)
			{
				layer = tempLayer;
				break;
			}
		}

		return layer;
	}

	public bool TileIdToIndex(int id, out TileIndex tileIndex)
	{
		tileIndex = new TileIndex();
		bool found = false;

		int idxStart = 0;
		foreach(var tileSet in tilesets)
		{
			idxStart = tileSet.StartId;
			if(id>=idxStart && id<idxStart+tileSet.TileCount){
				// in this tileset
				tileIndex.ImageName = tileSet.name;
				tileIndex.IndexInImage = id - idxStart;
				found = true;
				break;
			}
			else
			{
				// not this one, go next tileset
				idxStart += tileSet.TileCount;
			}
		}

		return found;
	}

	public bool GetTileSetOfTileId(int id, out TiledSet tileSet)
	{
		tileSet = null;

		bool found = false;

		int idxStart = 0;
		foreach(var tempTileSet in tilesets)
		{
			idxStart = tempTileSet.StartId;
			if(id>=idxStart && id<idxStart+tempTileSet.TileCount){
				tileSet = tempTileSet;
				found = true;
				break;
			}
			else
			{
				// not this one, go next tileset
				idxStart += tempTileSet.TileCount;
			}
		}

		return found;
	}
}

[System.Serializable]
public class TiledProperty{
	public string key;
	public string value;
}

[System.Serializable]
public class TiledSet
{
    public int columns;
    public int firstgid;
    public string image;
    public int imageheight;
    public int imagewidth;
    public int margin;
    public string name;
    public TiledProperty[] properties;
    public int spacing;
    public int tilecount;
    public int tileheight;
    public int tilewidth;

	public int TileCount
	{
		get{
			if(tilecount<=0)
			{
				int c = imagewidth / tilewidth;
				int r = imageheight / tileheight;

				tilecount = c * r;
			}	
			return tilecount;
		}
	}

	private int _startId = 0;
	public int StartId
	{
		get{
			return _startId;
		}
		set{
			_startId = value;
		}
	}

	private int _tileCountX = 0;
	public int TileCountX
	{
		get{
			return _tileCountX;
		}
		set{
			_tileCountX = value;
		}
	}

	private int _tileCountY = 0;
	public int TileCountY
	{
		get{
			return _tileCountY;
		}
		set{
			_tileCountY = value;
		}
	}
}

//public class TiledObject{
//	public int id;
//	public int width;
//	public int height;
//	public string name;
//	public string type;
//	public TiledProperty[] properties;
//	public bool visible;
//	public int x;
//	public int y;
//	public float rotation;
//	public int gid;
//}

[System.Serializable]
public class TiledLayer{
    /// <summary>
    /// tilelayer 名称定义
    /// </summary>
    public const string NAME_TERRAIN = "00-terrain",
                        NAME_PARTS   = "01-props",
                        NAME_BUILDING = "02-building",
                        NAME_UNITS = "03-units",
                        NAME_MARKS = "04-marks";

    public int[] data;
    public int height;
    public string name;
    public int opacity;
    public string type; // "tilelayer", "objectgroup", or "imagelayer"
    public bool visible;
    public int width;
	public int x;
	public int y;
	public TiledProperty[] properties;
}

//public class TiledObjectLayer{
//	public int width;
//	public int height;
//	public string name;
//	public bool visible;
//	public int x;
//	public int y;
//}
