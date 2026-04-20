# Unity 2022 Dynamic Variant Fix

Fix for #pragma dynamic_branch in Unity versions before 2022.3.40f1. In these versions, Unity would treat dynamic_branches as normal keywords and create keyword permutations with and without the dynamic_branch keyword. This means that unity would then compile exact copies of the shader with the dynamic keyword "set" and not "set" despite it not being a preprocessor symbol, bloating compile times and completely defeating the point of dynamic branching. This repository adds a shader variant preprocessor that removes the unnecessary duplicate variants. 

# IMPORTANT

Due to a quirk of unity's variant system, unity will refuse to set the value of dynamic_branch variable if the shader variant does not contain the corresponding keyword. Thus, **this patch removes all variants that do not contain every dynamic_branch keyword**, not the permutations that contain the keywords!

Do not put multiple dynamic_branch keywords on a single line (ie `#pragma _ FOG_LINEAR FOG_EXP FOG_EXP2`) as that will tell unity to compile variants with only one of each keyword. The patch will then remove all variants from the shader because none of them have all the keywords active at once. Instead, put each dynamic branch in a separate pragma:
```
#pragma dynamic_branch FOG_LINEAR
#pragma dynamic_branch FOG_EXP
#pragma dynamic_branch FOG_EXP2
```

Additionally any other code that removes variants, for example unity's built in fog, lightmap, and instancing stripping or other IPreprocessShaders scripts, can cause this patch to delete all variants if they run before and remove variants containing all of the dynamic_branch keywords. Fog keywords are handled explicitly by this script and should work, but others will cause issues if turned into dynamic branches. 