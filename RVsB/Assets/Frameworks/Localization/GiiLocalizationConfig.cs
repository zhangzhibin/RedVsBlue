using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GiiLocalizationConfig{
	[System.Serializable]
	public class LocalizedResource<RESOURCE_TYPE>
	{
		public string Key;
		public List<RESOURCE_TYPE> ResourceValues;
	}

	[System.Serializable]
	public class StringResource : LocalizedResource<string>
	{
		public List<string> Values{
			get{
				return base.ResourceValues;
			}
		}
	}

	[System.Serializable]
	public class TextrueResource : LocalizedResource<Texture>
	{
		public List<Texture> Values{
			get{
				return base.ResourceValues;
			}
		}
	}
}
