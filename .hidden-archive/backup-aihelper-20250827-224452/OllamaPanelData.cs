using Microsoft.VisualStudio.Extensibility.UI;
using System.Runtime.Serialization;

namespace KesifUygulamasiTemplate.KesifUygulamasi.AIHelper.KesifUygulamasi.OllamaTool
{
    /// <summary>
    /// ViewModel for the OllamaPanelContent remote user control.
    /// </summary>
    [DataContract]
    internal class OllamaPanelData : NotifyPropertyChangedObject
    {
        public OllamaPanelData()
        {
            HelloCommand = new AsyncCommand((parameter, clientContext, cancellationToken) =>
            {
                Text = $"Hello {parameter as string}!";
                return Task.CompletedTask;
            });
        }

        private string _name = string.Empty;
        [DataMember]
        public string Name
        {
            get => _name;
            set => SetProperty(ref this._name, value);
        }

        private string _text = string.Empty;
        [DataMember]
        public string Text
        {
            get => _text;
            set => SetProperty(ref this._text, value);
        }

        [DataMember]
        public AsyncCommand HelloCommand { get; }
    }
}
