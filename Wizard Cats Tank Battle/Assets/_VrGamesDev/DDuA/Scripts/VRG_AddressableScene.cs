using System;

using UnityEngine;

using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

using VrGamesDev.BHEL;

// Remember to add the following using statemnt to the top of your class. This will give you access to all of Odin's attributes.
//using Sirenix.OdinInspector;


namespace VrGamesDev.DDuA
{
    [System.Serializable]
    public class VRG_AddressableScene : VRG_AddressableSerializable
    {
        [Tooltip("Async Data of the Handler from Scene")]
        [SerializeField] public AsyncOperationHandle<SceneInstance> m_SceneHandle = new AsyncOperationHandle<SceneInstance>();




        /// <summary>
        /// Empty Creator, everything is true and zero
        /// </summary>
        public VRG_AddressableScene()
        {
            this.Reset();

            this.m_IsScene = true;
        }

        /// <summary>
        /// Empty Creator, everything is true and zero
        /// </summary>
        public VRG_AddressableScene(string nameLocal)
        {
            this.Reset();

            this.m_IsScene = true;

            // assign the new name
            this.address = nameLocal;
        }










        // delegated function to load the Addressable
        protected override void GetDownload(AsyncOperationHandle obj)
        {
            if (this.m_ProgressHandle.Status == AsyncOperationStatus.Succeeded)
            {
                if (this.m_Size > 0)
                {
                    if (this.verbose >= ENUM_Verbose.ALL && this.address != VRG_DDuA.m_SceneProxy)
                    {
                        // log and inform i am done
                        VRG_Bhel.Do
                        (
                            "<color=green>" + this.address + "</color> | <b>DOWNLOADED: </b>\n" + this.m_Path,
                            "VRG_AddressableScene->GetDownload()",
                            ENUM_Verbose.ALL,
                            this.m_ObjectInScene
                        );
                    }
                }

                // It is already downloaded, update the status 
                this.m_Status = ENUM_AddressableStatus.DOWNLOADED;

                // if needs to be created
                if (this.evolution == ENUM_AddressableEvolution.CREATE)
                {
                    Addressables.LoadSceneAsync(this.address).Completed += Scene_Completed;
                }
            }

            // if failed
            else
            {
                if (this.verbose >= ENUM_Verbose.ERROR)
                {
                     // log and inform i couldn't load the asset
                    VRG_Bhel.Do
                    (
                        "<color=green>" + this.address + "</color> | " + "<b> DIDN'T</b> download = " + this.m_ProgressHandle.Status,
                        "VRG_AddressableScene->GetDownload()",
                        ENUM_Verbose.ERROR,
                        this.m_ObjectInScene
                    );
               }

                // invoke the events delegateds
                this.InvokeEvent_WhenComplete();
            }
        }

        // delegated function to load the Addressable
        protected void Scene_Completed(AsyncOperationHandle<SceneInstance> obj)
        {
            bool bFail = false;

            string sLogs = "";

            //Debug.Log(Time.frameCount + ") Class Scene_Completed");

            // save the scene for later reference
            this.m_SceneHandle = obj;

            // if the try was valid
            if (this.m_SceneHandle.IsValid())
            {
                // ... and successful
                if (this.m_SceneHandle.Status == AsyncOperationStatus.Succeeded)
                {
                    this.m_Status = ENUM_AddressableStatus.SCENE_LOADED;
/*
                    if (this.verbose >= ENUM_Verbose.DEBUG && this.address != VRG_DDuA.m_SceneProxy)
                    {
                        // log and inform the size of the asset
                        VRG_Bhel.Do
                        (
                            "Scene: <color=blue><i>" + this.address + "</i></color> | <b>DOWNLOADED:</b> 100%",
                            "VRG_AddressableScene->Scene_Completed()",
                            ENUM_Verbose.DEBUG,
                            this.objectInScene
                        );
                    }
*/
                }

                // damn! we failed...
                else
                {
                    // this failed couldn't create it
                    bFail = true;

                    sLogs = "Scene: <color=blue><i>" + this.address + "</i></color> | " + this.m_SceneHandle.Status;
                }
            }

            // ok, we failed...
            else
            {
                // this failed couldn't create it
                bFail = true;

                sLogs = "Scene: <color=blue><i>" + this.address + "</i></color> | IsValid(false) = " + this.m_SceneHandle.Status;
            }


            if (bFail)
            {
                // reset the addressable
                this.Release();

                // update it failed
                this.m_Status = ENUM_AddressableStatus.FAILED;

                if (this.verbose >= ENUM_Verbose.ERROR)
                {
                    // log and inform i could load the asset
                    VRG_Bhel.Do
                    (
                        sLogs,
                        "VRG_AddressableScene->Scene_Completed()",
                        ENUM_Verbose.ERROR,
                        this.m_ObjectInScene
                    );
                }
            }

            // invoke the events delegateds
            this.InvokeEvent_WhenComplete();
        }

        // delegated function to load the Addressable
        public new void Release()
        {
            // if the try was valid
            if (this.m_SceneHandle.IsValid())
            {
                /*
                [DEPRECATED in Addressables 1.18.2]
                try
                {
                    // release the handler
                    Addressables.Release(this.m_SceneHandle);
                }
                catch (Exception e)
                {
                    if (this.verbose >= ENUM_Verbose.ERROR)
                    {
                        // log and inform the error
                        VRG_Bhel.Do
                        (
                            "<color=green>" + this.address + "</color> | " + "(this.m_SceneHandle) = " + e.ToString(),
                            "VRG_AddressableScene->Destroy()",
                            ENUM_Verbose.ERROR,
                            this.m_ObjectInScene
                        );
                    }
                }
                */
            }

            // reset it
            this.m_SceneHandle = new AsyncOperationHandle<SceneInstance>();

            // call daddy
            base.Release();
        }

    }
}