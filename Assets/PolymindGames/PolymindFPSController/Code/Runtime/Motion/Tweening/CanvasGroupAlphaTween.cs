using UnityEngine;

namespace PolymindGames.ProceduralMotion
{
  public static partial class TweenExtensions
  {
    public static Tween<float> TweenCanvasGroupAlpha(this CanvasGroup self, float to, float duration) =>
      Tween<float, CanvasGroup>.Add<CanvasGroupAlphaTween>(self).SetTargetValue(to, duration);

    public static Tween<float> TweenCanvasGroupAlpha(this GameObject self, float to, float duration) =>
      Tween<float, CanvasGroup>.Add<CanvasGroupAlphaTween>(self).SetTargetValue(to, duration);
    }

    /// <summary>
    /// The driver is responsible for updating the tween's state.
    /// </summary>
    [System.Serializable]
    [UnityEngine.Scripting.APIUpdating.MovedFrom(true, sourceAssembly: "PolymindGames.ProceduralMotion.Tweening")]
    public sealed class CanvasGroupAlphaTween : Tween<float, CanvasGroup>
    {
        /// <summary>
        /// Overriden method which is called when the tween starts and should
        /// return the tween's initial value.
        /// </summary>
        protected override float GetDefaultFrom() => component.alpha;
        protected override float GetToRelativeToFrom(float valueFrom, float valueTo) => valueFrom + valueTo;

        /// <summary>
        /// Overriden method which is called every tween update and should be used
        /// to update the tween's value.
        /// </summary>
        /// <param name="easedTime">The current eased time of the tween's step.</param>
        protected override void OnUpdate (float easedTime)
        {
            currentValue = InterpolateValue (m_ValueFrom, valueTo, easedTime);
            component.alpha = currentValue;
        }
    }
}