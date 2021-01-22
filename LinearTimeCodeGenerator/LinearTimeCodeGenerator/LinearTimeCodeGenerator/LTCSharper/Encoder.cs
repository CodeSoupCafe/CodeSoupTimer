namespace LinearTimeCodeGenerator.LTCSharper
{
  using System;
  using Timecode4net;

  public class Encoder
  {
    public Encoder(double sampleRate, double fps, FrameRate standard, BGFlags flags)
    {
      EncoderData = new EncoderData(sampleRate, fps, standard, (int)flags);
    }

    private int Init(double sampleRate, double fps, FrameRate standard, BGFlags flags)
    {
      if (sampleRate < 1)
        return -1;

      int bufferSize = 1 + (int)Math.Ceiling(sampleRate / fps);

      if (bufferSize > EncoderData.BufferSize)
        return -1;

      var samplesPerClock = sampleRate / (fps * 80.0);

      EncoderData.Update(
        0,
        0,
        sampleRate,
        fps,
        standard,
        (int)flags,
        samplesPerClock,
        samplesPerClock / 2.0,
        0.5,
        40.0);
      /*
      if (flags & LTC_BGF_DONT_TOUCH)
      {
        EncoderData.f.col_frame = 0;
        if (flags & LTC_TC_CLOCK)
        {
          EncoderData.f.binary_group_flag_bit1 = 1;
        }
        else
        {
          EncoderData.f.binary_group_flag_bit1 = 0;
        }
        switch (standard)
        {
          case LTC_TV_625_50: 25 fps mode
            EncoderData.f.biphase_mark_phase_correction = 0; // BGF0
            EncoderData.f.binary_group_flag_bit0 = (flags & LTC_USE_DATE) ? 1 : 0; // BGF2
            break;

          default:
            EncoderData.f.binary_group_flag_bit0 = 0;
            EncoderData.f.binary_group_flag_bit2 = (flags & LTC_USE_DATE) ? 1 : 0;
            break;
        }
      }
      if ((flags & LTC_NO_PARITY) == 0)
      {
        ltc_frame_set_parity(&EncoderData.f, standard);
      }

      if ((int)rint(fps * 100.0) == 2997)
        EncoderData.f.dfbit = 1;
      else
        EncoderData.f.dfbit = 0;
  */
      return 0;
    }

    public Timecode Timecode { get; set; }
    public FrameData Frame { get; set; }

    public int GetBuffer(byte[] buffer, int offset)
    {
      /*      const int len = EncoderData.offset;
            memcpy(buf, EncoderData.buf, len * sizeof(ltcsnd_sample_t));
            EncoderData.offset = 0;
            return len;
       */
      int len = EncoderData.Offset;
      Buffer.BlockCopy(EncoderData.Buffer, 0, buffer, 0, len);
      EncoderData.Update(offset: 0);
      return len;
    }

    public EncoderData EncoderData { get; }

    public void IncrementFrame()
    {
    }

    public void DecrementFrame()
    {
    }

    public void FlushBuffer()
    {
    }

    public void SetBufferSize(double sampleRate, double fps)
    {
      /*
       * free(e->buf);
           e->offset = 0;
           e->bufsize = 1 + ceil(sample_rate / fps);
           e->buf = (ltcsnd_sample_t*)calloc(e->bufsize, sizeof(ltcsnd_sample_t));
           if (!e->buf)
           {
             return -1;
           }
           return 0;
     */
    }

    public void SetVolume(double deciBelFullScale)
    {
    }

    public void EncodeFrame() => EncoderData.EncodeFrame();
  };
}