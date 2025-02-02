using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Threading;

namespace deepbim
{
    public partial class ChatWindow : Window
    {
        private readonly UIApplication _uiapp;
        private readonly List<ChatMessage> _conversationHistory = new();
        private CancellationTokenSource _cancellationTokenSource = new();

        public ChatWindow(UIApplication application)
        {
            InitializeComponent();
            _uiapp = application ?? throw new ArgumentNullException(nameof(application));
            Dispatcher.ShutdownStarted += (s, e) => _cancellationTokenSource.Cancel();
            ChatHistory.Document.Blocks.Clear();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var userInput = UserInput.Text.Trim();
                if (string.IsNullOrWhiteSpace(userInput)) return;

                // Clear input and disable UI
                UserInput.Text = string.Empty;
                SendButton.IsEnabled = false;
                _cancellationTokenSource = new CancellationTokenSource();

                // Add user message
                AddMessage(userInput, isUser: true);
                _conversationHistory.Add(new ChatMessage("user", userInput));

                // Get and display response
                var response = await DeepSeekService.GetResponse(
                    userInput,
                    _cancellationTokenSource.Token
                );

                if (!string.IsNullOrWhiteSpace(response))
                {
                    AddMessage(response, isUser: false);
                    _conversationHistory.Add(new ChatMessage("assistant", response));
                }
            }
            catch (OperationCanceledException)
            {
                AddMessage("[Request cancelled]", isUser: false, isError: true);
            }
            catch (Exception ex)
            {
                AddMessage($"Assistant Error: {ex.Message}", isUser: false, isError: true);
            }
            finally
            {
                SendButton.IsEnabled = true;
            }
        }

        private void AddMessage(string message, bool isUser, bool isError = false)
        {
            Dispatcher.Invoke(() =>
            {
                var container = new FlowDocument
                {
                    PagePadding = new Thickness(0),
                    Background = Brushes.Transparent
                };

                // Add message blocks
                foreach (var block in MarkdownToInlineConverter.ConvertToBlocks(message))
                {
                    container.Blocks.Add(block);
                }

                // Create bubble container
                var bubble = new RichTextBox
                {
                    Document = container,
                    IsReadOnly = true,
                    BorderThickness = new Thickness(0),
                    Background = isError
                        ? new SolidColorBrush(Colors.DarkRed)
                        : isUser
                            ? new SolidColorBrush(Color.FromRgb(0x52, 0x8A, 0xFF))
                            : new SolidColorBrush(Color.FromRgb(0x2D, 0x2A, 0x36)),
                    Margin = new Thickness(5),
                    Padding = new Thickness(8),
                    HorizontalAlignment = isUser ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 480
                };

                // Add to chat history
                ChatHistory.Document.Blocks.Add(new BlockUIContainer(bubble));
                ChatHistory.ScrollToEnd();
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    public class ChatMessage
    {
        public string Role { get; }
        public string Content { get; }

        public ChatMessage(string role, string content)
        {
            Role = role;
            Content = content;
        }
    }
}