using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ASFTag.Net
{
    public static class ASFTagManager
    {
        public static TagContainer ReadTag(string path)
        {
            ValidatePath(path);

            var editor = new ASFUnderlyingMetaDataEditor(path);

            var container = new TagContainer();
            container.AddRange(editor.Attributes);

            editor.Close();

            return container;
        }

        /// <summary>
        /// WriteTag overwrites an entire tag section
        /// </summary>
        /// <param name="path"></param>
        /// <param name="container"></param>
        public static void WriteTag(string path, TagContainer container)
        {
            ValidatePath(path);

            var editor = new ASFUnderlyingMetaDataEditor(path);

            foreach (var attribute in container)
                editor.AddOrModifyAttribute(attribute);

            editor.WriteToFile();
            editor.Close();
        }

        private static void ValidatePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (!File.Exists(path))
                throw new FileNotFoundException("File does not exist.", path);
        }
    }
}