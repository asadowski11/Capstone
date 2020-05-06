using Microsoft.Data.Sqlite;

namespace BobTheDigitalAssistant.Models
{
    public class Joke
    {
        public string Text { get; private set; } = "";

        // because of the FromDataRow method, there's no reason to have a public constructor
        private Joke(string Text)
        {
            this.Text = Text;
        }

        private Joke() : this("")
        {
        }

        public static Joke FromDataRow(SqliteDataReader reader)
        {
            return new Joke(reader["jokeText"].ToString());
        }
    }
}