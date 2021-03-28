using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Assets.Scripts
{
    /// <summary>
    /// Provides static utility, helper, and extension methods.
    /// Authors: Scott Clarke and Daniel Darnell.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// A mapping of physical keys from their associated virtual key.
        /// </summary>
        public static readonly Dictionary<VirtualKey, (KeyCode key, KeyCode? alt)> KeyMap
            = new Dictionary<VirtualKey, (KeyCode key, KeyCode? alt)>
            {
            { VirtualKey.RotateRight    , (KeyCode.D        , null) },
            { VirtualKey.RotateLeft     , (KeyCode.A        , null) },
            { VirtualKey.Accelerate     , (KeyCode.W        , null) },
            { VirtualKey.Brake          , (KeyCode.S        , null) },
            { VirtualKey.Up             , (KeyCode.W        , null) },
            { VirtualKey.Down           , (KeyCode.S        , null) },
            { VirtualKey.Left           , (KeyCode.A        , null) },
            { VirtualKey.Right          , (KeyCode.D        , null) },
            { VirtualKey.Primary        , (KeyCode.Mouse0   , null) },
            { VirtualKey.Secondary      , (KeyCode.Mouse1   , null) },
            { VirtualKey.Face           , (KeyCode.Mouse2   , null) },
            { VirtualKey.Teleport       , (KeyCode.Space    , null) },
            { VirtualKey.Accept         , (KeyCode.Space    , null) },
            { VirtualKey.Previous       , (KeyCode.Escape   , null) },
            { VirtualKey.Pause          , (KeyCode.Escape   , null) },
            { VirtualKey.Unpause        , (KeyCode.Escape   , null) },
            { VirtualKey.OpenUpgrade    , (KeyCode.Tab      , null) },
            };

        /// <summary>
        /// Returns whether a physical key mapped to the specified virtual key is currently held.
        /// </summary>
        /// <param name="vKey">The virtual key whose physical keys to check.</param>
        /// <returns><see langword="true"/>, if the virtual key is held; <see langword="false"/> otherwise.</returns>
        public static bool Check(this VirtualKey vKey, Func<KeyCode, bool> condition)
        {
            var keyset = KeyMap[vKey];
            return condition(keyset.key)
                || (keyset.alt is KeyCode alt && condition(alt));
        }

        /// <summary>
        /// Returns whether a physical key mapped to the specified virtual key is currently held.
        /// </summary>
        /// <param name="vKey">The virtual key whose physical keys to check.</param>
        /// <returns><see langword="true"/>, if the virtual key is held; <see langword="false"/> otherwise.</returns>
        public static bool IsHeld(this VirtualKey vKey) => vKey.Check(x => Input.GetKey(x));

        /// <summary>
        /// Returns whether a physical key mapped to the specified virtual key was just pressed.
        /// </summary>
        /// <param name="vKey">The virtual key whose physical keys to check.</param>
        /// <returns><see langword="true"/>, if the virtual key was just pressed; <see langword="false"/> otherwise.</returns>
        public static bool JustPressed(this VirtualKey vKey) => vKey.Check(x => Input.GetKeyDown(x));

        /// <summary>
        /// Returns whether a physical key mapped to the specified virtual key was just released.
        /// </summary>
        /// <param name="vKey">The virtual key whose physical keys to check.</param>
        /// <returns><see langword="true"/>, if the virtual key was just released; <see langword="false"/> otherwise.</returns>
        public static bool JustReleased(this VirtualKey vKey) => vKey.Check(x => Input.GetKeyUp(x));

        /// <summary>
        /// Prints the specified objects to the console window.
        /// </summary>
        /// <param name="objects">The objects to print.</param>
        public static void Print(params dynamic[] objects)
        {
            Debug.Log(string.Join(" ", objects));
        }

        /// <summary>
        /// Prints the specified objects to the console window.
        /// </summary>
        /// <param name="objects">The objects to print.</param>
        public static void Print(IEnumerable<dynamic> objects)
        {
            Debug.Log(string.Join(" ", objects));
        }

        /// <summary>
        /// Performs the specified action on each element of the enumerable.
        /// </summary>
        /// <typeparam name="TSource">The type of each element.</typeparam>
        /// <param name="source">The enumerable on whose elements the action will be performed.</param>
        /// <param name="action">The action delegate to perform on each element.</param>
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var element in source)
            {
                action?.Invoke(element);
            }
        }

        /// <summary>
        /// Executes all the provided coroutines in parallel.
        /// </summary>
        /// <param name="mono">The <see cref="MonoBehaviour"/> object who will start the coroutines.</param>
        /// <param name="coroutines">An enumerable of coroutines to be executed in parallel.</param>
        /// <returns>Yield returns until all coroutines have been completed.</returns>
        public static IEnumerator Parallel(MonoBehaviour mono, IEnumerable<Func<IEnumerator>> coroutines)
        {
            // Number of coroutines to run in parallel
            int expectedCount = coroutines.Count();

            // Number of completed coroutines
            int doneCount = 0;

            foreach (var coroutine in coroutines)
            {
                mono.StartCoroutine(wrap(coroutine));
            }

            // Yield returns until all coroutines are complete
            yield return new WaitUntil(() => doneCount >= expectedCount);

            // Wraps a coroutine with an update to completion counter
            IEnumerator wrap(Func<IEnumerator> coroutine)
            {
                yield return coroutine();

                // Once a coroutine is complete, increment completion counter
                doneCount++;
            }
        }

        public static Vector2 RotateBy(this Vector2 vector, float degrees) => Quaternion.AngleAxis(degrees, Vector2.up) * vector;
    }
}