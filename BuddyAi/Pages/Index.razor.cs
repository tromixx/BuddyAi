using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using BuddyAi.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Timers;

namespace BuddyAi.Pages
{
    public partial class Index : IDisposable
    {
        private List<ChatMessage> _messages = new();
        private string _currentMessage = string.Empty;
        private bool _isTyping = false;
        private bool _isRobotActive = false;
        private string _currentRobotImage = "/avatars/PurpleRobot.png";
        private System.Timers.Timer? _typingTimer;
        private int _responseIndex = 0;
        private string _pendingResponse = "At the moment we are under maintenance...";
        private string _confluenceContent = string.Empty;

        private ElementReference _messagesContainer;
        private MudTextField<string>? _messageInputRef;
        [Inject] private ConfluenceService ConfluenceService { get; set; } = default!;

        protected override void OnInitialized()
        {
            _messages.Add(new ChatMessage(
                "Purple Pete",
                "Hi! My name is Purple Pete and I'm here to help!",
                "/avatars/PurpleRobot.png"
            ));
        }

        private async Task OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await SendMessage();
            }
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(_currentMessage)) return;

            var userMessage = _currentMessage;
            _messages.Add(new ChatMessage("You", userMessage, null));
            _currentMessage = string.Empty;
            await ScrollToBottom();
            await Task.Delay(100); // let UI update before animation starts

            _isTyping = true;
            _isRobotActive = true;
            _currentRobotImage = "/avatars/PurpleRobotTalking.png";
            StateHasChanged();

            _responseIndex = 0;
            _typingTimer?.Dispose();
            _typingTimer = new System.Timers.Timer(60); 
            _typingTimer.Elapsed += async (_, _) => await TypeOutBotResponse();
            _typingTimer.Start();
        }

        private async Task TypeOutBotResponse()
        {
            await InvokeAsync(async () =>
            {
                if (_responseIndex < _pendingResponse.Length)
                {
                    var currentText = _pendingResponse.Substring(0, _responseIndex + 1);

                    // Remove last bot message if exists
                    if (_messages.LastOrDefault()?.Sender == "Purple Pete")
                        _messages.RemoveAt(_messages.Count - 1);

                    _messages.Add(new ChatMessage(
                        "Purple Pete",
                        currentText,
                        "/avatars/PurpleRobotTalking.png"
                    ));

                    _responseIndex++;
                    StateHasChanged();
                    await ScrollToBottom();
                }
                else
                {
                    _typingTimer?.Stop();
                    _typingTimer?.Dispose();
                    _isTyping = false;
                    _isRobotActive = false;
                    _currentRobotImage = "/avatars/PurpleRobot.png";
                    StateHasChanged();
                }
            });
        }

        private async Task ScrollToBottom()
        {
            await Task.Delay(50); 
            await JsRuntime.InvokeVoidAsync("scrollToBottom", _messagesContainer); 
        }

        public async Task FetchConfluenceData()
        {
            _confluenceContent = await ConfluenceService.GetPageContent();
            StateHasChanged();
        }

        public void Dispose()
        {
            _typingTimer?.Dispose();
        }
    }
}
