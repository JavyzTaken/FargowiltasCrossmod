using System;
using System.IO;
using Terraria.ModLoader;

namespace FargowiltasCrossmod.Content.Calamity.Bosses.ExoMechs.Draedon.Dialogue
{
    /// <summary>
    /// Represents an instance of a subtitle created by Draedon.
    /// </summary>
    /// <param name="SoundPath">The path to the sound file to be played out by this subtitle.</param>
    /// <param name="Duration">The total play duration of the subtitle, in frames.</param>
    public record DraedonSubtitle(string SoundPath, int Duration)
    {
        /// <summary>
        /// Calculates how long a sound takes to play, in seconds, from a given sound data stream.
        /// </summary>
        /// <param name="stream">The stream to read.</param>
        private static double CalculateSoundDataDuration(Stream stream)
        {
            // Sample data.
            byte[] data;

            // WaveFormatEx data.
            ushort wFormatTag;
            ushort nChannels;
            uint nSamplesPerSec;
            uint nAvgBytesPerSec;
            ushort nBlockAlign;
            ushort wBitsPerSample;

            int samplerLoopStart;
            int samplerLoopEnd;

            using (BinaryReader reader = new(stream))
            {
                // RIFF Signature
                string signature = new(reader.ReadChars(4));
                if (signature != "RIFF")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                reader.ReadUInt32(); // Riff Chunk Size.

                string wformat = new(reader.ReadChars(4));
                if (wformat != "WAVE")
                    throw new NotSupportedException("Specified stream is not a wave file.");

                // WAVE Header.
                string formatSignature = new string(reader.ReadChars(4));
                while (formatSignature != "fmt ")
                {
                    reader.ReadBytes(reader.ReadInt32());
                    formatSignature = new string(reader.ReadChars(4));
                }

                int formatChunkSize = reader.ReadInt32();
                wFormatTag = reader.ReadUInt16();
                nChannels = reader.ReadUInt16();
                nSamplesPerSec = reader.ReadUInt32();
                nAvgBytesPerSec = reader.ReadUInt32();
                nBlockAlign = reader.ReadUInt16();
                wBitsPerSample = reader.ReadUInt16();

                // Read residual bytes.
                if (formatChunkSize > 16)
                    reader.ReadBytes(formatChunkSize - 16);

                // data Signature
                string dataSignature = new(reader.ReadChars(4));
                while (!dataSignature.Equals("data", StringComparison.InvariantCultureIgnoreCase))
                {
                    reader.ReadBytes(reader.ReadInt32());
                    dataSignature = new string(reader.ReadChars(4));
                }
                if (dataSignature != "data")
                    throw new NotSupportedException("Specified wave file is not supported.");

                int waveDataLength = reader.ReadInt32();
                data = reader.ReadBytes(waveDataLength);

                // Scan for other chunks.
                while (reader.PeekChar() != -1)
                {
                    char[] chunkIDChars = reader.ReadChars(4);
                    if (chunkIDChars.Length < 4)
                        break; // EOL!

                    byte[] chunkSizeBytes = reader.ReadBytes(4);
                    if (chunkSizeBytes.Length < 4)
                        break; // EOL!

                    string chunkSignature = new string(chunkIDChars);
                    int chunkDataSize = BitConverter.ToInt32(chunkSizeBytes, 0);
                    if (chunkSignature == "smpl") // "smpl", Sampler Chunk Found.
                    {
                        reader.ReadUInt32(); // Manufacturer.
                        reader.ReadUInt32(); // Product.
                        reader.ReadUInt32(); // Sample Period.
                        reader.ReadUInt32(); // MIDI Unity Note.
                        reader.ReadUInt32(); // MIDI Pitch Fraction.
                        reader.ReadUInt32(); // SMPTE Format.
                        reader.ReadUInt32(); // SMPTE Offset.
                        uint numSampleLoops = reader.ReadUInt32();
                        int samplerData = reader.ReadInt32();

                        for (int i = 0; i < numSampleLoops; i += 1)
                        {
                            reader.ReadUInt32(); // Cue Point ID.
                            reader.ReadUInt32(); // Type.
                            int start = reader.ReadInt32();
                            int end = reader.ReadInt32();
                            reader.ReadUInt32(); // Fraction.
                            reader.ReadUInt32(); // Play Count.

                            // Grab loopStart and loopEnd from first sample loop.
                            if (i == 0)
                            {
                                samplerLoopStart = start;
                                samplerLoopEnd = end;
                            }
                        }

                        // Read Sampler Data if it exists.
                        if (samplerData != 0)
                            reader.ReadBytes(samplerData);
                    }

                    // Read unwanted chunk data and try again.
                    else
                        reader.ReadBytes(chunkDataSize);
                }
            }

            return (double)data.Length / nChannels / (wBitsPerSample / 8) / nSamplesPerSec;
        }

        public DraedonSubtitle(string soundPath) : this(soundPath, 0)
        {
            using Stream stream = ModContent.GetInstance<FargowiltasCrossmod>().GetFileStream(SoundPath);
            double giveMeTheDamnSoundDurationInMultiplayerYouStupidGame = CalculateSoundDataDuration(stream);
            int durationInFrames = (int)(giveMeTheDamnSoundDurationInMultiplayerYouStupidGame * 60D);
            Duration = durationInFrames;
        }
    }
}
