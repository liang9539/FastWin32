using System;
using System.IO;
using LzmaDecoder = LzmaSharp.Compression.LZMA.Decoder;
using LzmaEncoder = LzmaSharp.Compression.LZMA.Encoder;

namespace LzmaSharp
{
    /// <summary>
    /// Lzma算法压缩与解压
    /// </summary>
    public static class LzmaCompressor
    {
        /// <summary>
        /// 压缩（默认字典大小32MB）
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] buffer)
        {
            return Compress(buffer, 32 * 1024 * 1024);
        }

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <param name="dictionarySize">字典大小</param>
        /// <returns></returns>
        public static byte[] Compress(byte[] buffer, int dictionarySize)
        {
            if (buffer == null)
                throw new ArgumentNullException();
            if (dictionarySize <= 0)
                throw new ArgumentOutOfRangeException();

            CoderPropID[] propIDs;
            object[] properties;
            LzmaEncoder encoder;

            propIDs = new CoderPropID[] { CoderPropID.DictionarySize, CoderPropID.PosStateBits, CoderPropID.LitContextBits, CoderPropID.LitPosBits, CoderPropID.NumFastBytes, CoderPropID.MatchFinder, CoderPropID.EndMarker };
            properties = new object[] { dictionarySize, 4, 8, 0, 16, "bt2", true };
            encoder = new LzmaEncoder();
            encoder.SetCoderProperties(propIDs, properties);
            //设置压缩参数
            using (MemoryStream outStream = new MemoryStream())
            {
                encoder.WriteCoderProperties(outStream);
                //写入压缩属性
                using (MemoryStream inStream = new MemoryStream(buffer))
                {
                    encoder.Code(inStream, outStream, -1, -1);
                    //编码
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// 解压
        /// </summary>
        /// <param name="buffer">字节数组</param>
        /// <returns></returns>
        public static byte[] Decompress(byte[] buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException();
            if (buffer.Length <= 5)
                throw new ArgumentOutOfRangeException();

            byte[] properties;
            LzmaDecoder decoder;

            properties = new byte[5];
            Array.Copy(buffer, properties, 5);
            //获取属性
            decoder = new LzmaDecoder();
            //实例化解码器
            decoder.SetDecoderProperties(properties);
            //设置解码器属性
            using (MemoryStream inStream = new MemoryStream(buffer))
            {
                inStream.Seek(5, SeekOrigin.Current);
                //将当前流字节数提升5
                using (MemoryStream outStream = new MemoryStream())
                {
                    decoder.Code(inStream, outStream, -1, -1);
                    //解码
                    return outStream.ToArray();
                }
            }
        }
    }
}
