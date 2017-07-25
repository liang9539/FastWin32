using System;
using System.IO;

namespace LzmaSharp
{
	/// <summary>
	/// The exception that is thrown when an error in input stream occurs during decoding.
	/// </summary>
	class DataErrorException : ApplicationException
	{
		public DataErrorException(): base("Data Error") { }
	}

	/// <summary>
	/// The exception that is thrown when the value of an argument is outside the allowable range.
	/// </summary>
	class InvalidParamException : ApplicationException
	{
		public InvalidParamException(): base("Invalid Parameter") { }
	}

    /// <summary>
    /// 
    /// </summary>
	public interface ICoder
	{
        /// <summary>
        /// Codes streams.
        /// </summary>
        /// <param name="inStream">
        /// input Stream.
        /// </param>
        /// <param name="outStream">
        /// output Stream.
        /// </param>
        /// <param name="inSize">
        /// input Size. -1 if unknown.
        /// </param>
        /// <param name="outSize">
        /// output Size. -1 if unknown.
        /// </param>
        /// <exception cref="DataErrorException">
        /// if input stream is not valid
        /// </exception>
        void Code(Stream inStream, Stream outStream, long inSize, long outSize);
	};

	/// <summary>
	/// Provides the fields that represent properties idenitifiers for compressing.
	/// </summary>
	public enum CoderPropID
	{
		/// <summary>
		/// Specifies default property.
		/// </summary>
		DefaultProp = 0,
		/// <summary>
		/// Specifies size of dictionary.
		/// </summary>
		DictionarySize,
		/// <summary>
		/// Specifies size of memory for PPM*.
		/// </summary>
		UsedMemorySize,
		/// <summary>
		/// Specifies order for PPM methods.
		/// </summary>
		Order,
		/// <summary>
		/// Specifies Block Size.
		/// </summary>
		BlockSize,
		/// <summary>
		/// Specifies number of postion state bits for LZMA (0 ~ 4).
		/// </summary>
		PosStateBits,
        /// <summary>
        /// Specifies number of literal context bits for LZMA (0 ~ 8).
        /// </summary>
        LitContextBits,
        /// <summary>
        /// Specifies number of literal position bits for LZMA (0 ~ 4).
        /// </summary>
        LitPosBits,
		/// <summary>
		/// Specifies number of fast bytes for LZ*.
		/// </summary>
		NumFastBytes,
		/// <summary>
		/// Specifies match finder. LZMA: "BT2", "BT4" or "BT4B".
		/// </summary>
		MatchFinder,
		/// <summary>
		/// Specifies the number of match finder cycles.
		/// </summary>
		MatchFinderCycles,
		/// <summary>
		/// Specifies number of passes.
		/// </summary>
		NumPasses,
		/// <summary>
		/// Specifies number of algorithm.
		/// </summary>
		Algorithm,
		/// <summary>
		/// Specifies the number of threads.
		/// </summary>
		NumThreads,
		/// <summary>
		/// Specifies mode with end marker.
		/// </summary>
		EndMarker
	};

    /// <summary>
    /// 
    /// </summary>
	public interface ISetCoderProperties
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propIDs"></param>
        /// <param name="properties"></param>
		void SetCoderProperties(CoderPropID[] propIDs, object[] properties);
	};

    /// <summary>
    /// 
    /// </summary>
	public interface IWriteCoderProperties
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="outStream"></param>
		void WriteCoderProperties(Stream outStream);
	}

    /// <summary>
    /// 
    /// </summary>
	public interface ISetDecoderProperties
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
		void SetDecoderProperties(byte[] properties);
	}
}
