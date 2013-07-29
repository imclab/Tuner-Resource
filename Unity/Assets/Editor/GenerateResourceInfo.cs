﻿using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Text;

class GenerateResourceInfo
{
	const string menuTitle = "Tuner/Generate Resources Info Json";

		[MenuItem(menuTitle)]
		static void GenJson ()
		{

				List<FileInfo> filelist = new List<FileInfo> ();
				string bundlePath = Tuner.Resource.LocationHelper.GetBundlePath ();
				GetFileInfo (ref filelist, bundlePath, true);
				string[] ignoreKeys = new string[]{"DS_Store","resourcesinfo.json"};
				ignore (ref filelist, ignoreKeys);

				//json
				Dictionary<string,object> json_root = new Dictionary<string, object> ();
				List<object> json_assetlist = new List<object> ();
				json_root.Add ("assets", json_assetlist);

				//ergodic
				foreach (FileInfo item in filelist) {
						//Debug.Log (item);
						string subPath = item.FullName.Substring (bundlePath.Length);

						Dictionary<string,object> tempDic = new Dictionary<string, object> ();
						tempDic.Add (subPath, getMD5 (item.FullName));
						json_assetlist.Add (tempDic);
				}
				string json = MiniJSON.Json.Serialize (json_root);

				WriteFile (bundlePath + "/Info/resourcesinfo.json", json);
		}

		static void WriteFile (string path, string content)
		{
				Debug.Log ("Write Content :" + content);
				string parent = path.Substring (0, path.LastIndexOf ("/") + 1);
				AssetUtil.CreateDirectory (parent);

				FileInfo fi = new FileInfo (path);
				if (fi.Exists) {
						fi.Delete ();
				}
				File.WriteAllText (path, content);
		}

		static	string  getMD5 (string path)
		{
				FileStream fs = File.OpenRead (path);
				System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider ();
		
				byte[] md5Bytes = md5.ComputeHash (fs);
				StringBuilder sb = new StringBuilder ();
		
				foreach (byte byte_item in md5Bytes) {
						sb.Append (byte_item.ToString ("x2"));
				}
				string md5_asc = sb.ToString ();
				return md5_asc;
		}
	
		static void ignore (ref List<FileInfo> list, string[] ignoreKeys)
		{
				//ignore
				List<FileInfo> ignoreList = new List<FileInfo> ();
				foreach (FileInfo item in list) {

						foreach (string ignoreKey in ignoreKeys) {
								if (item.FullName.IndexOf (ignoreKey) != -1) {
										ignoreList.Add (item);
										break;
								}
						}
				}
				foreach (FileInfo item in ignoreList) {
						list.Remove (item);
				}

		}
	
		static void GetFileInfo (ref List<FileInfo> list, string path, bool recursive)
		{
				DirectoryInfo dirInfo = new DirectoryInfo (path);
				foreach (FileInfo item in dirInfo.GetFiles ()) {
						list.Add (item);
				}	
				if (recursive) {
						foreach (DirectoryInfo dirItem in dirInfo.GetDirectories()) {
								GetFileInfo (ref list, dirItem.FullName, recursive);
						}
				}
		}
}
