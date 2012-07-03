using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using PlistCS;

namespace PlistParse
{
	class Program
	{
		static int Main(string[] args)
		{
			string CurrentProcessName = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);

			if (args.Length == 0)
			{
				Console.WriteLine("Usage: {0} <plist> <args>\n", CurrentProcessName);
				Console.WriteLine("Example: {0} \"C:\\IPSW\\RestorePlist.plist\" ProductVersion\n", CurrentProcessName);
				Console.WriteLine("Supported for Restore.plist: producttype, productversion, productbuildversion, updateramdisk, restoreramdisk, rootfilesystem, bdid, boardconfig, cpid, platform, scep.\n");
				Console.WriteLine("Supported for BuildManifest.plist: apboardid, apchipid, uniquebuildidid, buildnumber, buildtrain, deviceclass, path <imagename>, digest <imagename>, partialdigest <imagename>, buildstring, manifestversion, productbuildversion, productversion, supportedproducttypes.\n");
				return 1;
			}

			if (!File.Exists(args[0]))
			{
				Console.WriteLine("ERROR: file {0} does not exist!", args[0]);
				return 1;
			}

			switch (Path.GetFileName(args[0]))
			{
				case "Restore.plist":
					ParseRestorePlist(args);
					break;
				case "BuildManifest.plist":
					ParseBuildManifest(args);
					break;
				default:
					Console.WriteLine("Unsupported plist!\nCurrently supported: Restore.plist and BuildManifest.plist");
					break;
			}

			return 0;
		}

		public static void ParseRestorePlist(string[] args)
		{
			try
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)Plist.readPlist(args[0]);
				Dictionary<string, object> DeviceMap = (Dictionary<string, object>)((List<object>)dict["DeviceMap"])[0];
				Dictionary<string, object> RestoreRamDisks = (Dictionary<string, object>)dict["RestoreRamDisks"];
				Dictionary<string, object> SystemRestoreImages = (Dictionary<string, object>)dict["SystemRestoreImages"];

				switch (args[1].ToLower())
				{
					case "producttype":
						Console.WriteLine(dict["ProductType"]);
						break;
					case "productversion":
						Console.WriteLine(dict["ProductVersion"]);
						break;
					case "productbuildversion":
						Console.WriteLine(dict["ProductBuildVersion"]);
						break;
					case "updateramdisk":
					case "updaterd":
						Console.WriteLine(RestoreRamDisks["Update"]);
						break;
					case "restoreramdisk":
					case "restorerd":
						Console.WriteLine(RestoreRamDisks["User"]);
						break;
					case "rootfilesystem":
					case "rootfs":
						Console.WriteLine(SystemRestoreImages["User"]);
						break;
					case "bdid":
						Console.WriteLine(DeviceMap["BDID"]);
						break;
					case "boardconfig":
						Console.WriteLine(DeviceMap["BoardConfig"]);
						break;
					case "cpid":
						Console.WriteLine(DeviceMap["CPID"]);
						break;
					case "platform":
						Console.WriteLine(DeviceMap["Platform"]);
						break;
					case "scep":
						Console.WriteLine(DeviceMap["SCEP"]);
						break;
					default:
						Console.WriteLine("Not found!");
						break;
				}
			}
			catch (Exception) { }
		}

		public static void ParseBuildManifest(string[] args)
		{
			try
			{
				Dictionary<string, object> dict = (Dictionary<string, object>)Plist.readPlist(args[0]);
				List<object> BuildIdentitiesList = (List<object>)dict["BuildIdentities"];
				List<object> SupportedProductTypes = (List<object>)dict["SupportedProductTypes"];
				Dictionary<string, object> BuildIdentities = (Dictionary<string, object>)BuildIdentitiesList[0];
				Dictionary<string, object> Info = (Dictionary<string, object>)BuildIdentities["Info"];
				Dictionary<string, object> Manifest = (Dictionary<string, object>)BuildIdentities["Manifest"];

				switch (args[1].ToLower())
				{
					case "apboardid":
						Console.WriteLine(BuildIdentities["ApBoardID"].ToString());
						break;
					case "apchipid":
						Console.WriteLine((string)BuildIdentities["ApChipID"]);
						break;
					case "uniquebuildidid":
						Console.WriteLine((byte[])BuildIdentities["UniqueBuildID"]);
						break;
					case "buildnumber":
						Console.WriteLine(Info["BuildNumber"]);
						break;
					case "buildtrain":
						Console.WriteLine(Info["BuildTrain"]);
						break;
					case "deviceclass":
						Console.WriteLine(Info["DeviceClass"]);
						break;
					case "path":
						if (Manifest.ContainsKey(args[2]))
							Console.WriteLine((string)((Dictionary<string, object>)((Dictionary<string, object>)Manifest[args[2]])["Info"])["Path"]);
						else
							Console.WriteLine("{0} not found.", args[2]);
						break;
					case "digest":
						if (Manifest.ContainsKey(args[2]))
						{
							if (((Dictionary<string, object>)Manifest[args[2]]).ContainsKey("Digest"))
								Console.WriteLine((byte[])((Dictionary<string, object>)Manifest[args[2]])["Digest"]);
							else
								Console.WriteLine("Digest not found in " + args[2]);
						}
						else
						{
							Console.WriteLine("{0} not found.", args[2]);
						}
						break;
					case "partialdigest":
						if (Manifest.ContainsKey(args[2]))
						{
							if (((Dictionary<string, object>)Manifest[args[2]]).ContainsKey("PartialDigest"))
								Console.WriteLine((byte[])((Dictionary<string, object>)Manifest[args[2]])["PartialDigest"]);
							else
								Console.WriteLine("PartialDigest not found in " + args[2]);
						}
						else
						{
							Console.WriteLine("{0} not found.", args[2]);
						}
						break;
					case "buildstring":
						if (Manifest.ContainsKey("LLB"))
							Console.WriteLine((string)((Dictionary<string, object>)Manifest["LLB"])["BuildString"]);
						break;
					case "manifestversion":
						Console.WriteLine(dict["ManifestVersion"]);
						break;
					case "productbuildversion":
						Console.WriteLine(dict["ProductBuildVersion"]);
						break;
					case "productversion":
						Console.WriteLine(dict["ProductVersion"]);
						break;
					case "supportedproducttypes":
						Console.WriteLine(SupportedProductTypes[0]);
						break;
					default:
						Console.WriteLine("Not found!");
						break;
				}
			}
			catch (Exception) { }
		}
	}
}
