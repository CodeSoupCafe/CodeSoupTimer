namespace LinearTimeCodeGenerator
{
  using System.Windows.Input;
  using Xamarin.Forms;

  public partial class MainPage : ContentPage
  {
    private readonly IWavePlayer wavePlayer;

    public MainPage()
    {
      wavePlayer = DependencyService.Get<IWavePlayer>();

      InitializeComponent();
    }

    public ICommand TryStartSoundCommand => new Command(() =>
    {
      wavePlayer.Play();
    });
  }
}