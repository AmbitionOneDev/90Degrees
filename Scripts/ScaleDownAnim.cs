using UnityEngine;

public class ScaleDownAnim : MonoBehaviour
{
    public void ScaleDown(GameObject clickedButton) {
        float duration = 0.05f;
        Vector3 downScale = new Vector3(0.7f, 0.7f, 1f);

        // Loop ping pong allows for reversal of the animation
        // Repeat set to 2 -> scale down, scale back up
        LeanTween.scale(clickedButton, downScale, duration).setLoopPingPong().setRepeat(2);
    }
}