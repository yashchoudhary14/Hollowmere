using UnityEngine;

namespace Hollowmere
{
    /// <summary>The notice board on the square: one sealed letter per player (docs/DESIGN.md "Day phase", Your letter).</summary>
    public class NoticeBoard : MonoBehaviour
    {
        public Readable[] Letters;

        public void Post(MatchPlan plan)
        {
            for (int i = 0; i < Letters.Length; i++)
            {
                bool used = i < plan.PlayerCount;
                Letters[i].gameObject.SetActive(used);
                if (used)
                    Letters[i].Set($"To {plan.PlayerNames[i]}", plan.Letters[i], $"Read the letter addressed to {plan.PlayerNames[i]}");
            }
        }
    }
}
