// Copied from https://github.com/Unity-Technologies/Graphics/blob/master/Packages/com.unity.render-pipelines.core/Runtime/Common/RemoveRange.Extensions.cs
// 
// Copyright © 2020 Unity Technologies ApS
// 
// Licensed under the Unity Companion License for Unity-dependent projects--see Unity Companion License.
// 
// Unless expressly provided otherwise, the Software under this license is made available strictly on an “AS IS” BASIS WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED. Please review the license for details on these and other terms and conditions.

#if !UNITY_CORE_RP
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace UnityEngine.Rendering
{
    /// <summary>
    /// A set of extension methods for collections
    /// </summary>
    public static class RemoveRangeExtensions
    {
        /// <summary>
        /// Tries to remove a range of elements from the list in the given range.
        /// </summary>
        /// <param name="list">The list to remove the range</param>
        /// <param name="index">The zero-based starting index of the range of elements to remove</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <param name="error">The exception raised by the implementation</param>
        /// <typeparam name="TValue">The value type stored on the list</typeparam>
        /// <returns>True if succeed, false otherwise</returns>
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        [MustUseReturnValue]
        public static bool TryRemoveElementsInRange<TValue>([DisallowNull] this IList<TValue> list, int index, int count, [NotNullWhen(false)] out Exception error)
        {
            try
            {
                if (list is List<TValue> genericList)
                {
                    genericList.RemoveRange(index, count);
                }
                else
                {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
                    if (index < 0) throw new ArgumentOutOfRangeException(nameof(index));
                    if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));
                    if (list.Count - index < count) throw new ArgumentException("index and count do not denote a valid range of elements in the list");
#endif

                    for (var i = count; i > 0; --i)
                        list.RemoveAt(index);
                }
            }
            catch (Exception e)
            {
                error = e;
                return false;
            }

            error = null;
            return true;
        }
    }
}
#endif