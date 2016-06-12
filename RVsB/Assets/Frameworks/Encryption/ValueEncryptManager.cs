using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// Value encrypt manager. 内存数值加密管理
/// </summary>
public class ValueEncryptManager : ISingleton{
	private Dictionary<string, byte[]> _values = new Dictionary<string, byte[]>();

	private const string DEBUG_KEY = "DebugKey";

	private byte[] _encryptKey;

	private bool _enableFakeValue = true;
	private Dictionary<string, object> _fakeValues = new Dictionary<string, object> ();

	public static ValueEncryptManager Instance{
		get{
			return Singleton<ValueEncryptManager>.Instance;
		}
	}

	public static byte[] ValueToByteArray<T>(T obj)
	{
		if(obj == null)
			return null;
		BinaryFormatter bf = new BinaryFormatter();
		using(MemoryStream ms = new MemoryStream())
		{
			bf.Serialize(ms, obj);
			return ms.ToArray();
		}
	}

	public static T ValueFromByteArray<T>(byte[] data)
	{
		if(data == null)
			return default(T);
		BinaryFormatter bf = new BinaryFormatter();
		using(MemoryStream ms = new MemoryStream(data))
		{
			object obj = bf.Deserialize(ms);
			return (T)obj;
		}
	}

	public bool Init()
	{
		SetEncryptionKey (DEBUG_KEY);
		return true;
	}

	public void SetEncryptionKey(string encryptKey)
	{
		Debug.Assert (encryptKey != null && encryptKey.Length > 0);
		_encryptKey = ValueToByteArray<string> (encryptKey);
	}

	public void EnableFakeValue(bool v)
	{
		_enableFakeValue = v;
	}

	public bool HasKey(string key)
	{
		return _values.ContainsKey (key);
	}

	private byte[] encrypt(byte[] inValue, byte[] encryptKey)
	{
		byte[] outValue = new byte[inValue.Length];

		int keyLength = encryptKey.Length;
		for(int i=0;i<inValue.Length;i++)
		{
			outValue [i] = (byte)(inValue [i] ^ encryptKey [i % keyLength]);
		}

		return outValue;
	}

	private byte[] decrypt(byte[] inValue, byte[] encryptKey)
	{
		return encrypt (inValue, encryptKey);	
	}

	public T GetValue<T>(string key, T defaultValue = default(T))
	{
		T v = defaultValue;

		byte[] valueBytes;
		if(_values.ContainsKey(key))
		{
			valueBytes = _values [key];
			if(valueBytes!=null && valueBytes.Length>0)
			{
				valueBytes = decrypt (valueBytes, _encryptKey);
				v = ValueFromByteArray<T> (valueBytes);
			}

			Debug.Assert(v.Equals((T)_fakeValues[key]));
		}

		return v;
	}

	public void SetValue<T>(string key, T v)
	{
		byte[] valueBytes = ValueToByteArray (v);
		valueBytes = encrypt (valueBytes, _encryptKey);

		_values [key] = valueBytes;

		if(_enableFakeValue)
		{
			_fakeValues [key] = v;
		}
	}

	public static int IntValue(string key, int defaultValue = default(int))
	{
		return Instance.GetValue<int> (key, defaultValue);
	}

	public static void SetIntValue(string key, int v)
	{
		Instance.SetValue<int> (key, v);
	}

	public static string StringValue(string key, string defaultValue = default(string))
	{
		return Instance.GetValue<string> (key, defaultValue);
	}

	public static void SetStringValue(string key, string v)
	{
		Instance.SetValue<string> (key, v);
	}

	public static float FloatValue(string key, float defaultValue = default(float))
	{
		return Instance.GetValue<float> (key, defaultValue);
	}

	public static void SetFloatValue(string key, float v)
	{
		Instance.SetValue<float> (key, v);
	}

	public static object ObjectValue(string key, object defaultValue = default(object))
	{
		return Instance.GetValue<object> (key, defaultValue);
	}

	public static void SetObjectValue(string key, object v)
	{
		Instance.SetValue<object> (key, v);
	}

	public static void UnitTest()
	{
		// test int
		string key = "debug_int_value";

		{
			int v = 0;
			SetIntValue(key, v);
			Debug.LogFormat ("Test Int Value: in [{0}] => out [{1}] ", v, IntValue (key));
		}

		{
			int v = 123456789;
			SetIntValue(key, v);
			Debug.LogFormat ("Test Int Value: in [{0}] => out [{1}] ", v, IntValue (key));
		}

		{
			int v = -123456789;
			SetIntValue(key, v);
			Debug.LogFormat ("Test Int Value: in [{0}] => out [{1}] ", v, IntValue (key));
		}


		// test float
		key = "debug_float_value";
		{
			float v = 0f;
			SetFloatValue(key, v);
			Debug.LogFormat ("Test Int Value: in [{0}] => out [{1}] ", v, FloatValue (key));
		}

		{
			float v = 0.123456f;
			SetFloatValue(key, v);
			Debug.LogFormat ("Test Int Value: in [{0}] => out [{1}] ", v, FloatValue (key));
		}

		{
			float v = -123456.123f;
			SetFloatValue(key, v);
			Debug.LogFormat ("Test Int Value: in [{0}] => out [{1}] ", v, FloatValue (key));
		}

		// test string
		key = "debug_string_value";
		{
			string v = "";
			SetStringValue(key, v);
			Debug.LogFormat ("Test Int Value: in [{0}] => out [{1}] ", v, StringValue (key));
		}

		{
			string v = "012345678901234567890123456789";
			SetStringValue(key, v);
			Debug.LogFormat ("Test Int Value: in [{0}] => out [{1}] ", v, StringValue (key));
		}
	}
}
