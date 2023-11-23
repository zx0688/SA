using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SecurePlayerPrefs
{

    public static List<T> GetListOrEmpty<T>(string key) where T : class
    {
        List<T> list = new();
        int count = PlayerPrefs.GetInt($"lst_{key}_ct", 0);
        for (int i = 0; i < count; i++)
        {
            try
            {
                string keylist = $"lst_{key}_{i}";
                if (!PlayerPrefs.HasKey(keylist))
                    continue;
                string json = decrypt(PlayerPrefs.GetString(keylist)) as string;
                T item = JsonUtility.FromJson<T>(json);
                list.Add(item);
            }
            catch (Exception) { }
        }
        //fix lenght
        if (count != list.Count)
        {
            PlayerPrefs.SetInt($"lst_{key}_ct", list.Count);
        }

        return list;
    }

    public static void ClearList(string key)
    {
        string keylist = $"lst_{key}_ct";
        int count = PlayerPrefs.GetInt(keylist, 0);
        try
        {
            for (int i = 0; i < count; i++)
                PlayerPrefs.DeleteKey($"lst_{key}_{i}");
            PlayerPrefs.DeleteKey(keylist);
        }
        catch (Exception) { }
    }

    public static void AddToList<T>(string key, T item) where T : class
    {
        string keylist = $"lst_{key}_ct";
        try
        {
            int count = PlayerPrefs.GetInt(keylist, 0);
            PlayerPrefs.SetString($"lst_{key}_{count}", encrypt(JsonUtility.ToJson(item)));
            PlayerPrefs.SetInt(keylist, count + 1);
        }
        catch (Exception) { }
    }

    public static void SetString(string key, string value)
    {
        PlayerPrefs.SetString(md5(key), encrypt(value));
    }

    public static string GetString(string key, string defaultValue)
    {
        if (!HasKey(key))
            return defaultValue;
        try
        {
            string s = decrypt(PlayerPrefs.GetString(md5(key)));
            return s;
        }
        catch
        {
            return defaultValue;
        }
    }

    public static string GetString(string key)
    {
        return GetString(key, "");
    }

    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetString(md5(key), encrypt(value.ToString()));
    }

    public static int GetInt(string key, int defaultValue)
    {
        if (!HasKey(key))
            return defaultValue;
        try
        {
            string s = decrypt(PlayerPrefs.GetString(md5(key)));
            int i = int.Parse(s);
            return i;
        }
        catch
        {
            return defaultValue;
        }
    }

    public static int GetInt(string key)
    {
        return GetInt(key, 0);
    }


    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetString(md5(key), encrypt(value.ToString()));
    }


    public static float GetFloat(string key, float defaultValue)
    {
        if (!HasKey(key))
            return defaultValue;
        try
        {
            string s = decrypt(PlayerPrefs.GetString(md5(key)));
            float f = float.Parse(s, System.Globalization.CultureInfo.InvariantCulture);
            return f;
        }
        catch
        {
            return defaultValue;
        }
    }

    public static float GetFloat(string key)
    {
        return GetFloat(key, 0);
    }

    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(md5(key));
    }

    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(md5(key));
    }

    public static void Save()
    {
        PlayerPrefs.Save();
    }

    private static string secretKey = "fper";
    private static byte[] key = new byte[8] { 12, 11, 18, 27, 56, 208, 65, 14 };
    private static byte[] iv = new byte[8] { 35, 63, 12, 43, 43, 43, 56, 12 };

    private static string encrypt(string s)
    {
        byte[] inputbuffer = Encoding.Unicode.GetBytes(s);
        byte[] outputBuffer = DES.Create().CreateEncryptor(key, iv).TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);

        //compress
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gzipStream.Write(outputBuffer, 0, outputBuffer.Length);
                gzipStream.Close();
            }
            return System.Convert.ToBase64String(memoryStream.ToArray());
        }
    }

    private static string decrypt(string s)
    {
        byte[] inputbuffer = System.Convert.FromBase64String(s);

        //decompress
        using (MemoryStream inputStream = new MemoryStream(inputbuffer))
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
                {
                    gzipStream.CopyTo(outputStream);
                    gzipStream.Close();
                }

                var output = outputStream.ToArray();

                byte[] outputBuffer = DES.Create().CreateDecryptor(key, iv).TransformFinalBlock(output, 0, output.Length);
                return Encoding.Unicode.GetString(outputBuffer);
            }
        }
    }

    private static string md5(string s)
    {
        byte[] hashBytes = new MD5CryptoServiceProvider().ComputeHash(new UTF8Encoding().GetBytes(s + secretKey + SystemInfo.deviceUniqueIdentifier));
        string hashString = "";
        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }
        return hashString.PadLeft(32, '0');
    }
}