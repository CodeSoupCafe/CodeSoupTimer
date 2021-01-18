using System;
using System.Diagnostics;
using System.Threading;
using Android.Media;
using LinearTimeCodeGenerator;
using LTCSharp;
using Xamarin.Forms;

[assembly: Dependency(typeof(IWavePlayer))]

namespace LinearTimeCodeGenerator.Droid.Global
{
  public class AndroidWavePlayer : IWavePlayer
  {
    public void Play()
    {
      try
      {
        var ltcDataSource = new LTCTimeCodeDataSource();
        var mediaPlayer = new MediaPlayer();

        mediaPlayer.SetDataSource(ltcDataSource);

        mediaPlayer.Prepare();
        mediaPlayer.Start();

        PlayExample(ltcDataSource);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex);
      }
    }

    internal void PlayExample(LTCTimeCodeDataSource ltcDataSource)
    {
      Stopwatch timer = new Stopwatch();

      timer.Start();

      while (timer.Elapsed < new TimeSpan(0, 0, 5))
      {
        ltcDataSource.SetTimecode(new Timecode(
          timer.Elapsed.Hours,
          timer.Elapsed.Minutes,
          timer.Elapsed.Seconds,
          (int)(timer.Elapsed.Milliseconds / 1000.0f * 30.0f)));

        Thread.Sleep(10);
      }

      timer.Stop();
    }

    public void Stop()
    {
    }
  }

  internal class LTCTimeCodeDataSource : MediaDataSource
  {
    private readonly Encoder FEncoder;

    public LTCTimeCodeDataSource()
    {
      FEncoder = new Encoder(48000, 30, TVStandard.TV525_60i, BGFlags.NONE);
    }

    public Encoder Encoder
    {
      get
      {
        return this.FEncoder;
      }
    }

    public override long Size { get; }

    public void SetTimecode(Timecode timecode)
    {
      lock (FEncoder)
      {
        FEncoder.setTimecode(timecode);
      }
    }

    public override int ReadAt(long position, byte[] buffer, int offset, int size)
    {
      lock (FEncoder)
      {
        FEncoder.encodeFrame();
        return FEncoder.getBuffer(buffer, offset);
        //return size;
      }
    }

    public override void Close()
    {
      //throw new NotImplementedException();
    }
  }
}