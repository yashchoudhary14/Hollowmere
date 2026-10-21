using UnityEngine;

namespace Hollowmere
{
    /// <summary>
    /// Something to read: a note in a house, a letter on the notice board, the Ritual Book.
    /// MatchDirector writes the text from the MatchPlan; empty text hides the prompt.
    /// </summary>
    public class Readable : Interactable
    {
        public string Title = "A note";
        [TextArea(3, 12)] public string Text = "";
        public string Verb = "Read the note";

        public override string Prompt(PlayerAgent player) => string.IsNullOrEmpty(Text) ? null : Verb;

        public override void Interact(PlayerAgent player, InteractKind kind) => player.OpenPanel(Title, Text);

        public void Set(string title, string text, string verb = null)
        {
            Title = title;
            Text = text ?? "";
            if (verb != null) Verb = verb;
        }
    }
}
