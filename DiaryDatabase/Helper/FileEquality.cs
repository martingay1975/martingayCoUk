using System.IO;

namespace DiaryDatabase.Helper
{
	internal class FileEquality
	{

		/// <summary>
		/// This method accepts two strings the represent two files to 
		/// compare. A return value of true indicates that the contents of the files
		/// are the same. 
		/// </summary>
		public static bool FileCompare(string file1, string file2)
		{
			// Determine if the same file was referenced two times.
			if (file1 == file2)
			{
				// Return true to indicate that the files are the same.
				return true;
			}

			// Open the two files.
			using (var fs1 = new FileStream(file1, FileMode.Open, FileAccess.Read))
			using (var fs2 = new FileStream(file2, FileMode.Open, FileAccess.Read))
			{

				// Check the file sizes. If they are not the same, the files 
				// are not the same.
				if (fs1.Length != fs2.Length)
				{
					// Return false to indicate files are different
					return false;
				}

				// Read and compare a byte from each file until either a
				// non-matching set of bytes is found or until the end of
				// file1 is reached.
				int file1byte;
				int file2byte;
				do
				{
					// Read one byte from each file.
					file1byte = fs1.ReadByte();
					file2byte = fs2.ReadByte();

					if (file1byte != file2byte)
					{
						return false;
					}
				}
				while ((file1byte == file2byte) && (file1byte != -1));

				return true;
			}
		}
	}
}