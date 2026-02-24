using System.Collections.Generic;
using System;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using System.Text;
using UnityEditor;
using System.Reflection;

namespace emdl
{

    /// <summary>
    /// Fixes bug with dynamic variants in pre-2022.3.40 unity versions.
    /// In these versions unity compiles two copies of the shader for dynamic_branch keywords as though they are normal preprocessor branches.
    /// In order to fix this, we have to remove the variants that do not have every dynamic branch keyword in their set. For some reason unity will not
    /// set the value of the dynamic branch variable if it can only find a variant without that keyword "set". 
    /// WARNING: After 39f1, this will just remove all keywords as it no longer generates variants with dynamic branch variables as keywords.
    /// </summary>

    // Thanks c# for not having simple numeric logic in preprocessor branches, I love checking 40 bools instead of comparing two numbers
#if  UNITY_2022_3_0 || UNITY_2022_3_1 || UNITY_2022_3_2 || UNITY_2022_3_3 || UNITY_2022_3_4 || UNITY_2022_3_5 || UNITY_2022_3_6 || UNITY_2022_3_7 || UNITY_2022_3_8 || UNITY_2022_3_9 || UNITY_2022_3_10 || UNITY_2022_3_11 || UNITY_2022_3_12 || UNITY_2022_3_13 || UNITY_2022_3_14 || UNITY_2022_3_15 || UNITY_2022_3_16 || UNITY_2022_3_17 || UNITY_2022_3_18 || UNITY_2022_3_19 || UNITY_2022_3_20 || UNITY_2022_3_21 || UNITY_2022_3_22 || UNITY_2022_3_23 || UNITY_2022_3_24 || UNITY_2022_3_25 || UNITY_2022_3_26 || UNITY_2022_3_27 || UNITY_2022_3_28 || UNITY_2022_3_29 || UNITY_2022_3_30 || UNITY_2022_3_31 || UNITY_2022_3_32 || UNITY_2022_3_33 || UNITY_2022_3_34 || UNITY_2022_3_35 || UNITY_2022_3_36 || UNITY_2022_3_37 || UNITY_2022_3_38 || UNITY_2022_3_39
    class RemoveDynamicVariantDuplicates : IPreprocessShaders
    {
        public int callbackOrder { get { return 0; } }
        
        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> keywordPermutations)
        {
            // dynamic_branches do NOT work in the meta pass! use normal keywords!
            if (snippet.passType == PassType.Meta)
            {
                return;
            }

            // Find all dynamic keywords

            LocalKeyword[] localKW = ShaderUtil.GetPassKeywords(shader, snippet.pass, snippet.shaderType);
            List<LocalKeyword> dynamicKWs = new List<LocalKeyword>(localKW.Length);

            for (int i = 0; i < localKW.Length; i++)
            {
                if (localKW[i].isDynamic)
                {
                    dynamicKWs.Add(localKW[i]);
                }
            }

            int numDynamic = dynamicKWs.Count;

            // do nothing if there's no dynamic keywords
            if (numDynamic == 0)
            {
                return;
            }

            int numPermutations = keywordPermutations.Count;
            int currentSize = 0;

            // move the keywords which have every dynamic keyword turned on to the front of the array
            for (int pIdx = 0; pIdx < numPermutations; ++pIdx)
            {
                bool isDynVariant = true;

                for (int dIdx = 0; dIdx < numDynamic; ++dIdx)
                {
                    isDynVariant = isDynVariant && keywordPermutations[pIdx].shaderKeywordSet.IsEnabled(dynamicKWs[dIdx]);
                }

                if (isDynVariant)
                {
                    keywordPermutations[currentSize] = keywordPermutations[pIdx];
                    currentSize++;
                }
            }

            // Trim off the end of the array, removing all variants without every dynamic keyword set
            keywordPermutations.TryRemoveElementsInRange(currentSize, numPermutations - currentSize, out System.Exception ex);
            if (ex != null) Debug.LogException(ex);
        } 
    }
#endif
}
