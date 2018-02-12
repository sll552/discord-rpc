using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace sharp_presence
{
  public partial class Form1 : Form
  {
    private readonly DiscordRpc.RichPresence _presence = new DiscordRpc.RichPresence();
    private readonly DiscordRpc.EventHandlers _handlers;

    public Form1()
    {
      var curdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      if (!File.Exists(curdir + "\\" + "discord-rpc.dll"))
      {
        Console.WriteLine(@"DiscordRPC library could not be found");
        Application.Exit();
      }

      _handlers.disconnectedCallback += DisconnectedCallback;
      _handlers.errorCallback += ErrorCallback;
      _handlers.joinCallback += JoinCallback;
      _handlers.readyCallback += ReadyCallback;
      _handlers.requestCallback += RequestCallback;
      _handlers.spectateCallback += SpectateCallback;

      DiscordRpc.Initialize("345229890980937739",ref _handlers, true, null);

      InitializeComponent();

      Closing += OnClosing;
    }

    private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
    {
      _presence.FreeMem();
      DiscordRpc.Shutdown();
    }

    private void SpectateCallback(string secret)
    {
      MessageBox.Show("SpectateCallback: \n secret: " + secret, @"SpectateCallback", MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }

    private void RequestCallback(ref DiscordRpc.JoinRequest request)
    {
      MessageBox.Show("RequestCallback: \n avatar: " + request.avatar
        + "\n discriminator: " + request.discriminator
        + "\n userid: " + request.userId
        + "\n username: " + request.username, @"RequestCallback", MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }

    private void ReadyCallback()
    {
      MessageBox.Show(@"ReadyCallback", @"ReadyCallback", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void JoinCallback(string secret)
    {
      MessageBox.Show("JoinCallback: \n secret: " + secret, @"JoinCallback", MessageBoxButtons.OK,
        MessageBoxIcon.Information);
    }

    private void ErrorCallback(int errorCode, string message)
    {
      MessageBox.Show("ErrorCallback: \n errorcode: " + errorCode
        + "\n message: " + message, @"ErrorCallback", MessageBoxButtons.OK,
        MessageBoxIcon.Error);
    }

    private void DisconnectedCallback(int errorCode, string message)
    {
      MessageBox.Show("DisconnectedCallback: \n errorcode: " + errorCode
        + "\n message: " + message, @"DisconnectedCallback", MessageBoxButtons.OK,
        MessageBoxIcon.Exclamation);
    }

    private new void Update()
    {
      DiscordRpc.UpdatePresence(_presence);
      // wait for callbacks
      Thread.Sleep(500);
      DiscordRpc.RunCallbacks();
    }

    private void button1_Click(object sender, EventArgs e)
    {
      _presence.state = textBoxState.Text;
      _presence.details = textBoxDetails.Text;

      var t = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1));
      _presence.startTimestamp = (long)Math.Round(t.TotalSeconds);

      _presence.partyId = textBoxPartyId.Text;
      try
      {
        _presence.partyMax = int.Parse(textBoxPartyMax.Text);
        _presence.partySize = int.Parse(textBoxPartySize.Text);
      }
      catch (Exception)
      {
        // ignored
      }

      _presence.largeImageKey = textBoxLargeImageKey.Text;
      _presence.largeImageText = textBoxLargeImageText.Text;
      _presence.smallImageKey = textBoxSmallImageKey.Text;
      _presence.smallImageText = textBoxSmallImageText.Text;

      Update();
    }
  }
}
