namespace LinearTimeCodeGenerator.LTCSharper
{
  using Timecode4net;

  public struct EncoderData
  {
    public double Fps { get; set; }
    public double SampleRate { get; set; }
    public double FilterConst { get; set; }
    public int Flags { get; set; }
    public FrameRate? Standard { get; set; }
    public byte Enc_lo { get; set; }
    public byte Enc_hi { get; set; }

    public int Offset { get; set; }

    public int BufferSize { get; set; }
    public byte[] Buffer { get; set; }
    public byte State { get; set; }

    public double SamplesPerClock { get; set; }
    public double SamplesPerClock2 { get; set; }
    public double SampleRemainder { get; set; }

    public FrameData Frame(Timecode timecode)
    {
      var frame = new FrameData();
      return frame;
    }

    public EncoderData(double sampleRate, double fps, FrameRate frameRate, int flags) : this()
    {
      Update(sampleRate: sampleRate, fps: fps, standard: frameRate, flags: flags);
    }

    public void Update
      (int? offset = null,
      byte? state = null,
      double? sampleRate = null,
      double? fps = null,
      FrameRate? standard = null,
      int? flags = null,
      double? samplesPerClock = null,
      double? samplesPerClock2 = null,
      double? sampleRemainder = null,
      double? filterConst = null)
    {
      this.State = state ?? State;
      this.Offset = offset ?? Offset;
      this.SampleRate = sampleRate ?? SampleRate;

      if (filterConst is double filterConstValue
        && filterConstValue != FilterConst)
        this.SetFilter(filterConstValue);

      this.Fps = fps ?? Fps;
      this.Flags = flags ?? Flags;
      this.Standard = standard ?? Standard;
      this.SamplesPerClock = samplesPerClock ?? SamplesPerClock;
      this.SamplesPerClock2 = samplesPerClock2 ?? SamplesPerClock2;
      this.SampleRemainder = sampleRemainder ?? SampleRemainder;
    }
  };
}