namespace LinearTimeCodeGenerator.LTCSharper
{
  using System;
  using System.Runtime.InteropServices;

  public static class Extensions
  {
    public static void EncodeFrame(this EncoderData e)
    {
      int byteCounter;

      for (byteCounter = 0; byteCounter < 10; byteCounter++)
        e.EncodeByte(byteCounter, 1.0);
    }

    public static byte[] GetBytes<T>(this T encoderData) where T : struct
    {
      int size = Marshal.SizeOf(encoderData);
      byte[] arr = new byte[size];

      IntPtr ptr = Marshal.AllocHGlobal(size);
      Marshal.StructureToPtr(encoderData, ptr, true);
      Marshal.Copy(ptr, arr, 0, size);
      Marshal.FreeHGlobal(ptr);
      return arr;
    }

    public static int EncodeByte(this EncoderData e, int byteCounter, double speed)
    {
      if (byteCounter < 0 || byteCounter > 9) return -1;
      if (speed == 0) return -1;

      int err = 0;
      byte c = new byte(); // e.Frame.GetBytes()[byteCounter]; <<<<<<<<<<<<-------------------------------------
      char b = (speed < 0) ? Convert.ToChar(128) : Convert.ToChar(1); // bit
      double spc = e.SamplesPerClock * Math.Abs(speed);
      double sph = e.SamplesPerClock2 * Math.Abs(speed);

      do
      {
        int n;
        if ((c & b) == 0)
        {
          n = (int)(spc + e.SampleRemainder);
          e.SampleRemainder = spc + e.SampleRemainder - n;
          e.State = ReturnBit(e);
          err |= e.AddValues(n);
        }
        else
        {
          n = (int)(sph + e.SampleRemainder);
          e.SampleRemainder = sph + e.SampleRemainder - n;
          e.State = ReturnBit(e);
          err |= e.AddValues(n);

          n = (int)(sph + e.SampleRemainder);
          e.SampleRemainder = sph + e.SampleRemainder - n;
          e.State = ReturnBit(e);
          err |= e.AddValues(n);
        }
        /* this is based on the assumption that with every compiler
         * ((unsigned char) 128)<<1 == ((unsigned char 1)>>1) == 0
         */
        if (speed < 0)
          b >>= 1;
        else
          b <<= 1;
      } while (b != 0);

      return err;
    }

    private static byte ReturnBit(EncoderData e) => (byte)(e.State == 0 ? 1 : 0);

    /**
     * add values to the output buffer
     */

    public static int AddValues(this EncoderData e, int n)
    {
      var tgtval = e.State > 0 ? e.Enc_hi : e.Enc_lo;

      if (e.Offset + n >= e.BufferSize)
        return 1;

      ref byte wave = ref e.Buffer[e.Offset];
      double tcf = e.FilterConst;

      if (tcf > 0)
      {
        /* low-pass-filter
         * LTC signal should have a rise time of 40 us +/- 10 us.
         *
         * rise-time means from <10% to >90% of the signal.
         * in each call to addvalues() we start at 50%, so
         * here we need half-of it. (0.000020 sec)
         *
         * e.cutoff = 1.0 -exp( -1.0 / (sample_rate * .000020 / exp(1.0)) );
         */
        int i;
        byte val = 128;
        int m = (n + 1) >> 1;

        for (i = 0; i < m; i++)
        {
          //val = val + tcf * (tgtval - val);
          val = Convert.ToByte(val + Convert.ToByte(tcf) * (tgtval - val));
          //wave[n - i - 1] = wave[i] = val;
          wave ^= Convert.ToByte((-val ^ wave) & 1 << (n - i - 1));
          wave ^= val;
        }
      }
      else
      {
        /* perfect square wave */
        //MemSet(wave, tgtval, n);
        wave ^= Convert.ToByte((-tgtval ^ wave) & n);
      }

      e.Offset += n;

      return 0;
    }

    public static void SetFilter(this EncoderData encoderData, double riseTime)
    {
      if (riseTime <= 0)
        encoderData.Update(filterConst: 0);
      else
        encoderData.Update(filterConst: 1.0 - Math.Exp(-1.0 / (encoderData.SampleRate * riseTime / 2000000.0 / Math.Exp(1.0))));
    }

    public static void MemSet(byte[] input, byte value)
    {
      int block = 32, index = 0;
      int length = Math.Min(block, input.Length);

      // Fill the initial array
      while (index < length)
      {
        input[index++] = value;
      }

      length = input.Length;

      while (index < length)
      {
        Buffer.BlockCopy(input, 0, input, index, Math.Min(block, length - index));
        index += block;
        block *= 2;
      }
    }
  }
}

/*

#include <stdio.h>

typedef unsigned char Pants;

int main()
{
  int i = 3;
  int n = 4;
  int offset = 2;
  unsigned char valw = 4;
  Pants* original = &valw;
  unsigned char val = 4;
  Pants* wave = &(original[offset]);

  printf("%d, %d", wave[n - i - 1], val);

  wave[n - i - 1] = wave[i] = val;

  printf("Hello, world!, %d", wave);
  return 0;
}
*/