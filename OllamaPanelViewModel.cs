using Microsoft.VisualStudio.Extensibility.UI;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace KesifUygulamasiTemplate.KesifUygulamasi.OllamaTool
{
    [DataContract]
    internal sealed class OllamaPanelViewModel : NotifyPropertyChangedObject
    {
        private string _chatHistory = string.Empty;
        private string _prompt = string.Empty;

        public OllamaPanelViewModel()
        {
            SendCommand = new AsyncCommand(async (parameter, cancellationToken) =>
            {
                var input = Prompt?.Trim();
                if (!string.IsNullOrEmpty(input))
                {
                    ChatHistory += "Sen: " + input + Environment.NewLine;
                    Prompt = string.Empty;
                    string cevap = ">>> Bot cevabı buraya gelecek...";
                    ChatHistory += "Bot: " + cevap + Environment.NewLine;
                }
                await Task.CompletedTask;
            });
        }

        [DataMember]
        public string ChatHistory
        {
            get => _chatHistory;
            set => SetProperty(ref _chatHistory, value);
        }

        [DataMember]
        public string Prompt
        {
            get => _prompt;
            set => SetProperty(ref _prompt, value);
        }

        [DataMember]
        public AsyncCommand SendCommand { get; }
    }
}