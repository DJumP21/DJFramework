using System;
using System.Collections.Generic;
using System.IO;

namespace ET
{
	/// <summary>
	/// 文件工具类
	/// </summary>
	public static class FileHelper
	{
		/// <summary>
		/// 读取所有文件
		/// </summary>
		/// <param name="files"></param>
		/// <param name="dir"></param>
		public static void GetAllFiles(List<string> files, string dir)
		{
			string[] fls = Directory.GetFiles(dir);
			foreach (string fl in fls)
			{
				files.Add(fl);
			}

			string[] subDirs = Directory.GetDirectories(dir);
			foreach (string subDir in subDirs)
			{
				GetAllFiles(files, subDir);
			}
		}
		
		/// <summary>
		/// 删除文件夹（及其包含的文件）
		/// </summary>
		/// <param name="dir">文件夹路径</param>
		public static void CleanDirectory(string dir)
		{
			foreach (string subdir in Directory.GetDirectories(dir))
			{
				Directory.Delete(subdir, true);		
			}

			foreach (string subFile in Directory.GetFiles(dir))
			{
				File.Delete(subFile);
			}
		}
		/// <summary>
		/// 复制文件夹到指定路径
		/// </summary>
		/// <param name="srcDir">源路径</param>
		/// <param name="tgtDir">目标路径</param>
		/// <exception cref="Exception"></exception>
		public static void CopyDirectory(string srcDir, string tgtDir)
		{
			DirectoryInfo source = new DirectoryInfo(srcDir);
			DirectoryInfo target = new DirectoryInfo(tgtDir);
	
			if (target.FullName.StartsWith(source.FullName, StringComparison.CurrentCultureIgnoreCase))
			{
				throw new Exception("父目录不能拷贝到子目录！");
			}
	
			if (!source.Exists)
			{
				return;
			}
	
			if (!target.Exists)
			{
				target.Create();
			}
	
			FileInfo[] files = source.GetFiles();
	
			for (int i = 0; i < files.Length; i++)
			{
				File.Copy(files[i].FullName, Path.Combine(target.FullName, files[i].Name), true);
			}
	
			DirectoryInfo[] dirs = source.GetDirectories();
	
			for (int j = 0; j < dirs.Length; j++)
			{
				CopyDirectory(dirs[j].FullName, Path.Combine(target.FullName, dirs[j].Name));
			}
		}
		/// <summary>
		/// 替换文件后缀名
		/// </summary>
		/// <param name="srcDir">文件路径</param>
		/// <param name="extensionName">原后缀</param>
		/// <param name="newExtensionName">新后缀</param>
		public static void ReplaceExtensionName(string srcDir, string extensionName, string newExtensionName)
		{
			if (Directory.Exists(srcDir))
			{
				string[] fls = Directory.GetFiles(srcDir);

				foreach (string fl in fls)
				{
					if (fl.EndsWith(extensionName))
					{
						File.Move(fl, fl.Substring(0, fl.IndexOf(extensionName)) + newExtensionName);
						File.Delete(fl);
					}
				}

				string[] subDirs = Directory.GetDirectories(srcDir);

				foreach (string subDir in subDirs)
				{
					ReplaceExtensionName(subDir, extensionName, newExtensionName);
				}
			}
		}
		/// <summary>
		/// 复制文件到指定路径
		/// </summary>
		/// <param name="sourcePath">原路径</param>
		/// <param name="targetPath">目标路径</param>
		/// <param name="overwrite">是否覆盖</param>
		/// <returns></returns>
		public static bool CopyFile(string sourcePath, string targetPath, bool overwrite)
		{
			string sourceText = null;
			string targetText = null;

			if (File.Exists(sourcePath))
			{
				sourceText = File.ReadAllText(sourcePath);
			}

			if (File.Exists(targetPath))
			{
				targetText = File.ReadAllText(targetPath);
			}

			if (sourceText != targetText && File.Exists(sourcePath))
			{
				File.Copy(sourcePath, targetPath, overwrite);
				return true;
			}

			return false;
		}
	}
}
