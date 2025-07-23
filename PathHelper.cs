using System;
using System.IO;

namespace EncryptedNotes
{
    public class PathHelper
    {

        public static string GetFileNameWithoutExtension(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            try
            {
                return Path.GetFileNameWithoutExtension(filePath);
            }
            catch (ArgumentException)
            {                
                return null;
            }
        }
        public static string GetFileName(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            try
            {
                return Path.GetFileName(filePath);
            }
            catch (ArgumentException)
            {                
                return null;
            }
        }

        public static string GetDirectoryPath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            try
            {
                return Path.GetDirectoryName(filePath);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
