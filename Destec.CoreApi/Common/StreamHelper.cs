using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Destec.CoreApi.Common
{
    public static class StreamHelper
    {
        /// <summary>
        /// Retorna um stream separado por ";" com o conteúdo de um repositório.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MemoryStream GetStream<T>(IEnumerable<T> value) where T : class
        {
            var content = new StringBuilder();
            content.AppendLine(string.Join(";", typeof(T).GetProperties().Where(p => p.CanRead).Select(c => c.Name)));
            foreach (var item in value)
                content.AppendLine(string.Join(";", typeof(T).GetProperties().Where(p => p.CanRead).Select(c => c.GetValue(item, null))));

            return new MemoryStream(content.ToString().GetBytes(), false);
        }

        /// <summary>
        /// Transforma uma string em array de bytes.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static byte[] GetBytes(this string value)
        {
            byte[] bytes = new byte[value.Length * sizeof(char)];
            Buffer.BlockCopy(value.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
    }
}
