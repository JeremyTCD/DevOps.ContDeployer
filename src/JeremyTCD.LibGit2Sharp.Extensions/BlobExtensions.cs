using System;
using System.IO;
using System.Text;

namespace LibGit2Sharp.Extensions
{
    public static class BlobExtensions
    {
        public static string ReadAsString(this Blob blob)
        {
            if (blob == null)
            {
                throw new ArgumentNullException(nameof(blob));
            }

            Stream contentStream = blob.GetContentStream();

            using (StreamReader reader = new StreamReader(contentStream, Encoding.UTF8))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
