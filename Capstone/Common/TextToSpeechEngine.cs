using System;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml.Controls;

namespace Capstone.Common
{
    public static class TextToSpeechEngine
    {
        /// <summary>
        /// Speaks the passed text in an uninflected voice through the passed MediaElement
        /// </summary>
        /// <param name="media"> the media element on the page to play the sound through</param>
        /// <param name="text">the text to speak</param>
        public static async void SpeakText(MediaElement media, string text)
        {
            var synth = new SpeechSynthesizer();
            // create the audio stream from our text
            SpeechSynthesisStream stream = await synth.SynthesizeTextToStreamAsync(text);
            // prepare the media object to speak and play the audio
            media.SetSource(stream, stream.ContentType);
            media.AudioCategory = Windows.UI.Xaml.Media.AudioCategory.Speech;
            media.Play();
        }

        /// <summary>
        /// Speaks text with inflections. The inflections are defined in an xml-like format called SSML (Speech Synthesis Markup Language). To learn more, go to
        /// <a href="https://docs.microsoft.com/en-us/cortana/skills/speech-synthesis-markup-language">this documentation link</a>
        /// </summary>
        /// <param name="media">The media element to speak the text through</param>
        /// <param name="ssmlText"> the SSML markup to provide inflection data and the words to speak</param>
        public static async void SpeakInflectedText(MediaElement media, string ssmlText)
        {
            var synth = new SpeechSynthesizer();
            // create the audio stream from our text
            SpeechSynthesisStream stream = await synth.SynthesizeSsmlToStreamAsync(ssmlText);
            // prepare the media object to speak and play the audio
            media.SetSource(stream, stream.ContentType);
            media.AudioCategory = Windows.UI.Xaml.Media.AudioCategory.Speech;
            media.Play();
        }
    }

    /// <summary>
    /// A builder class to build a syntactically valid SSML markup string to be used in <see cref="TextToSpeechEngine.SpeakInflectedText(MediaElement, string)"/>.
    /// <br />
    /// To learn more about SSML, go to <a href="https://docs.microsoft.com/en-us/cortana/skills/speech-synthesis-markup-language">this documentation link provided by Mircosoft</a>
    /// </summary>
    public class SSMLBuilder
    {
        public string ssmlText { get; private set; } = "";

        public SSMLBuilder()
        {
        }

        public SSMLBuilder Add(string text)
        {
            this.ssmlText += text;
            return this;
        }

        public string Build()
        {
            this.ssmlText += "</speak>";
            return "<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>" + this.ssmlText;
        }

        /// <summary>
        /// Returns the built ssmlText without the enclosing speakElement, useful if you want to build a piece of ssml text and then wrap it all in another piece of ssml, like a prosody element for example.
        /// </summary>
        /// <returns></returns>
        public string BuildWithoutWrapperElement()
        {
            return this.ssmlText;
        }

        public SSMLBuilder Break(int duration = 0)
        {
            this.ssmlText += $"<break {(duration > 0 ? $"time='{duration}' " : "")}/>";
            return this;
        }

        public SSMLBuilder Sentence(string text)
        {
            this.ssmlText += $"<sentence>{text}</sentence>";
            return this;
        }

        public SSMLBuilder Paragraph(string text)
        {
            this.ssmlText += $"<p>{text}</p>";
            return this;
        }

        /// <summary>
        /// This is where the vocal inflections take place. It's a little complicated, and <a href="https://docs.microsoft.com/en-us/cortana/skills/speech-synthesis-markup-language#prosody-element">Microsoft's documentation</a> does a good job explaining it.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="pitch"></param>
        /// <param name="contour"></param>
        /// <param name="range"></param>
        /// <param name="rate"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public SSMLBuilder Prosody(string text, string pitch = "", string contour = "", string range = "", string rate = "", string duration = "")
        {
            this.ssmlText += $"<prosody{(pitch.Length > 0 ? $" pitch='{pitch}'" : "")}{(contour.Length > 0 ? $" contour='{contour}'" : "")}{(range.Length > 0 ? $" range='{range}'" : "")}{(rate.Length > 0 ? $" rate='{rate}'" : "")}{(duration.Length > 0 ? $" duration='{duration}'" : "")}>{text}</prosody>";
            return this;
        }

        public SSMLBuilder SayAs(string text, SayAsTypes type)
        {
            string sayType = Enum.GetName(typeof(SayAsTypes), type).ToLower();
            this.ssmlText += $"<say-as interpret-as='{sayType}'>{text}</say-as>";
            return this;
        }

        public SSMLBuilder Sub(string text, string substitute)
        {
            this.ssmlText += $"<sub alias='{substitute}'>{text}</sub>";
            return this;
        }

        public enum SayAsTypes
        {
            ADDRESS = 0,
            CARDINAL = 1,
            CHARACTERS = 2,
            DATE = 3,
            DIGITS = 4,
            FRACTION = 5,
            ORDINAL = 6,
            TELEPHONE = 7,
            TIME = 8
        }
    }
}
