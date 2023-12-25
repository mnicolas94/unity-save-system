//-----------------------------------------------------------------------
// <copyright file="AOTSupportUtilities.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

namespace SaveSystem.Editor.OdinSerializerExtensions
{
    public static class AOTSupportUtilities
    {
        /// <summary>
        /// Scans the project's build scenes and resources, plus their dependencies, for serialized types to support. Progress bars are shown during the scan.
        /// </summary>
        /// <param name="serializedTypes">The serialized types to support.</param>
        /// <param name="scanBuildScenes">Whether to scan the project's build scenes.</param>
        /// <param name="scanAllAssetBundles">Whether to scan all the project's asset bundles.</param>
        /// <param name="scanPreloadedAssets">Whether to scan the project's preloaded assets.</param>
        /// <param name="scanResources">Whether to scan the project's resources.</param>
        /// <param name="resourcesToScan">An optional list of the resource paths to scan. Only has an effect if the scanResources argument is true. All the resources will be scanned if null.</param>
        /// <returns>true if the scan succeeded, false if the scan failed or was cancelled</returns>
        public static bool ScanProjectForSerializedTypes(out List<Type> serializedTypes, bool scanBuildScenes = true, bool scanAllAssetBundles = true, bool scanPreloadedAssets = true, bool scanResources = true, List<string> resourcesToScan = null, bool scanAddressables = true)
        {
            using (var scanner = new AOTSupportScanner())
            {
                scanner.BeginScan();

                if (!scanner.ScanSaveSystemAssets(showProgressBar: true))
                {
                    Debug.Log("Project scan canceled while scanning assets in specified path.");
                    serializedTypes = null;
                    return false;
                }
                //
                // if (scanBuildScenes && !scanner.ScanBuildScenes(includeSceneDependencies: true, showProgressBar: true))
                // {
                //     Debug.Log("Project scan canceled while scanning scenes and their dependencies.");
                //     serializedTypes = null;
                //     return false;
                // }
                
                if (scanResources && !scanner.ScanAllResources(includeResourceDependencies: true, showProgressBar: true, resourcesPaths: resourcesToScan))
                {
                    Debug.Log("Project scan canceled while scanning resources and their dependencies.");
                    serializedTypes = null;
                    return false;
                }
                
                if (scanAllAssetBundles && !scanner.ScanAllAssetBundles(showProgressBar: true))
                {
                    Debug.Log("Project scan canceled while scanning asset bundles and their dependencies.");
                    serializedTypes = null;
                    return false;
                }
                //
                // if (scanPreloadedAssets && !scanner.ScanPreloadedAssets(showProgressBar: true))
                // {
                //     Debug.Log("Project scan canceled while scanning preloaded assets and their dependencies.");
                //     serializedTypes = null;
                //     return false;
                // }
                
                if (scanAddressables && !scanner.ScanAllAddressables(includeAssetDependencies: true, showProgressBar: true))
                {
                    Debug.Log("Project scan canceled while scanning addressable assets and their dependencies.");
                    serializedTypes = null;
                    return false;
                }

                serializedTypes = scanner.EndScan();
            }
            return true;
        }
    }
}

#endif
