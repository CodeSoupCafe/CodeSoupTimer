using System;
using System.Diagnostics;
using System.Threading;
using Android.Media;
using LinearTimeCodeGenerator.Droid.Global;
using LinearTimeCodeGenerator.LTCSharper;
using Timecode4net;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidWavePlayer))]

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
        ltcDataSource.SetTimecode(
          Timecode.FromString(
          $"{timer.Elapsed.Hours:00}:" +
          $"{timer.Elapsed.Minutes:00}:" +
          $"{timer.Elapsed.Seconds:00};" +
          $"{(int)(timer.Elapsed.Milliseconds / 1000.0f * 30.0f):00}",
          frameRate: FrameRate.fps30, false));

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
    public Encoder Encoder { get; private set; }

    public LTCTimeCodeDataSource()
    {
      Encoder = new Encoder(48000, 30, FrameRate.fps30, BGFlags.NONE);
    }

    public override long Size { get; }

    public void SetTimecode(Timecode timecode)
    {
      lock (Encoder)
      {
        Encoder.Timecode = timecode;
      }
    }

    public override int ReadAt(long position, byte[] buffer, int offset, int size)
    {
      if (Encoder.Timecode == null)
        return 0;

      lock (Encoder)
      {
        Encoder.EncodeFrame();
        return Encoder.GetBuffer(buffer, offset);
        //return size;
      }
    }

    public override void Close()
    {
      //throw new NotImplementedException();
    }
  }
}