using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
//using System.Timers;
using BuddyAi.Models;

namespace BuddyAi.Pages
{
    public partial class Index : IDisposable
    {
        private List<ChatMessage> _messages = new();
        private string _currentMessage = string.Empty;
        private bool _isTyping = false;
        private bool _isRobotActive = false;
        private string _currentRobotImage = "/avatars/PurpleRobot.png";
        private DotNetObjectReference<Index>? _objRef;
        private Timer? _typingTimer;
        private string _pendingResponse = "At the moment we are under maintenance...";
        private int _responseIndex = 0;

        protected override void OnInitialized()
        {
            _objRef = DotNetObjectReference.Create(this);
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

            _messages.Add(new ChatMessage("You", _currentMessage, null));
            _currentMessage = string.Empty;

            _isTyping = true;
            _isRobotActive = true;
            _currentRobotImage = "/avatars/PurpleRobotTalking.png";
            StateHasChanged();

            _responseIndex = 0;

            _typingTimer = new Timer(async _ =>
            {
                await InvokeAsync(() =>
                {
                    if (_responseIndex < _pendingResponse.Length)
                    {
                        var currentText = _pendingResponse.Substring(0, _responseIndex + 1);

                        if (_messages.LastOrDefault()?.Sender == "Purple Pete")
                        {
                            _messages.RemoveAt(_messages.Count - 1);
                        }

                        _messages.Add(new ChatMessage(
                            "Purple Pete",
                            currentText,
                            "/avatars/PurpleRobotTalking.png"
                        ));

                        _responseIndex++;
                        StateHasChanged();
                    }
                    else
                    {
                        _typingTimer?.Dispose();
                        _typingTimer = null;
                        _isTyping = false;
                        _isRobotActive = false;
                        _currentRobotImage = "/avatars/PurpleRobot.png";
                        StateHasChanged();
                    }
                });
            }, null, 0, 100);
        }

        public void Dispose()
        {
            _typingTimer?.Dispose();
            _objRef?.Dispose();
        }
    }
}
