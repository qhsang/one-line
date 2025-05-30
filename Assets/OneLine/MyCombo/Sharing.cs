﻿using UnityEngine;
using System.Collections;
using System.IO;

namespace EasyMobile
{
    #if UNITY_IOS || UNITY_ANDROID
    using EasyMobile.SharingInternal;
    #endif

    public static class Sharing
    {
        /// <summary>
        /// Shares a text using the native sharing utility.
        /// Note that Facebook doesn't allow pre-filled sharing message,
        /// so the text will be discarded when sharing to that particular network.
        /// </summary>
        /// <param name="text">Text.</param>
        /// <param name="subject">Subject.</param>
        public static void ShareText(string text, string subject = "")
        {
            #if UNITY_EDITOR
            Debug.Log("ShareText is only available on mobile devices.");
            #elif UNITY_IOS
            iOSNativeShare.ShareText(text, subject);
            #elif UNITY_ANDROID
            AndroidNativeShare.ShareTextOrURL(text, subject);
            #else
            Debug.Log("ShareText FAILED: platform not supported.");
            #endif
        }

        /// <summary>
        /// Shares a URL using the native sharing utility.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="subject">Subject.</param>
        public static void ShareURL(string url, string subject = "")
        {
            #if UNITY_EDITOR
            Debug.Log("ShareURL is only available on mobile devices.");
            #elif UNITY_IOS
            iOSNativeShare.ShareURL(url, subject);
            #elif UNITY_ANDROID
            AndroidNativeShare.ShareTextOrURL(url, subject);
            #else
            Debug.Log("ShareURL FAILED: platform not supported.");
            #endif
        }

        /// <summary>
        /// Shares the image at the given path via native sharing utility.
        /// </summary>
        /// <param name="imagePath">Image path.</param>
        /// <param name="message">Message.</param>
        /// <param name="subject">Subject.</param>
        public static void ShareImage(string imagePath, string message, string subject = "")
        {
            #if UNITY_EDITOR
            Debug.Log("ShareImage is only available on mobile devices.");
            #elif UNITY_IOS
            iOSNativeShare.ShareImage(imagePath, message, subject);
            #elif UNITY_ANDROID
            AndroidNativeShare.ShareImage(imagePath, message, subject);
            #else
            Debug.Log("ShareImage FAILED: platform not supported.");
            #endif
        }

        /// <summary>
        /// Captures the screenshot, saves it as a PNG image and then shares it
        /// to social networks via the native sharing utility.
        /// Note that this method should be called at the end of the frame
        /// (called within a coroutine after WaitForEndOfFrame())
        /// </summary>
        /// <returns>The filepath to the saved screenshot.</returns>
        /// <param name="filename">Filename to store the resulted PNG image without the file extension.</param>
        /// <param name="message">Message.</param>
        /// <param name="subject">Subject.</param>
        public static string ShareScreenshot(string filename, string message, string subject = "")
        {
            return ShareScreenshot(0, 0, Screen.width, Screen.height, filename, message, subject);
        }

        /// <summary>
        /// Captures the specified area of the screen, saves it as a PNG image
        /// and then shares it to social networks via the native sharing utility.
        /// The parameters specify the area of the screen are in pixels (screen space).
        /// Note that this method should be called at the end of the frame
        /// (called within a coroutine after WaitForEndOfFrame())
        /// </summary>
        /// <returns>The filepath to the saved screenshot.</returns>
        /// <param name="startX">Start x.</param>
        /// <param name="startY">Start y.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="filename">Filename to store the resulted PNG image without the file extension.</param>
        /// <param name="message">Message.</param>
        /// <param name="subject">Subject.</param>
        public static string ShareScreenshot(float startX, float startY, float width, float height, string filename, string message, string subject = "")
        {
            string filepath = SaveScreenshot(startX, startY, width, height, filename);

            // Share the screenshot via the native share utility
            if (filepath != null)
                ShareImage(filepath, message, subject);

            return filepath;
        }

        /// <summary>
        /// Generates a PNG image from the given Texture2D object, saves it to persistentDataPath using
        /// the given filename and then shares the image via the native sharing utility.
        /// </summary>
        /// <returns>The filepath to the saved image.</returns>
        /// <param name="tt">Tt.</param>
        /// <param name="filename">Filename to store the resulted PNG image without the file extension.</param>
        /// <param name="message">Message.</param>
        /// <param name="subject">Subject.</param>
        public static string ShareTexture2D(Texture2D tt, string filename, string message, string subject = "")
        {
            #if UNITY_EDITOR
            Debug.Log("ShareTexture2D is only available on mobile devices.");
            return null;
            #elif UNITY_IOS || UNITY_ANDROID
            // Encode texture into PNG
            byte[] bytes = tt.EncodeToPNG();

            // Save file to disk
            string filepath = Path.Combine(Application.persistentDataPath, filename + ".png");
            File.WriteAllBytes(filepath, bytes);

            // Share the screenshot via the native share utility
            ShareImage(filepath, message, subject);

            return filepath;
            #else
            Debug.Log("ShareImageByTexture2D FAILED: platform not supported.");
            return null;
            #endif
        }

        /// <summary>
        /// Captures the whole screen and saves the screenshot as a PNG image with the given filename.
        /// The created file is saved to the persistentDataPath if running on mobile devices,
        /// or to Assets folder if running in the editor.
        /// Note that this method should be called at the end of the frame
        /// (called within a coroutine after WaitForEndOfFrame())
        /// </summary>
        /// <returns>The filepath to the saved screenshot.</returns>
        /// <param name="filename">Filename to store the resulted PNG image without the file extension.</param>
        public static string SaveScreenshot(string filename)
        {
            return SaveScreenshot(0, 0, Screen.width, Screen.height, filename);
        }

        /// <summary>
        /// Captures the specified area of the screen and saves the screenshot as a PNG image with the given filename..
        /// The created file is saved to the persistentDataPath if running on mobile devices,
        /// or to Assets folder if running in the editor.
        /// Note that this method should be called at the end of the frame
        /// (called within a coroutine after WaitForEndOfFrame())
        /// </summary>
        /// <returns>The filepath to the saved screenshot.</returns>
        /// <param name="startX">Start x.</param>
        /// <param name="startY">Start y.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        /// <param name="filename">Filename to store the resulted PNG image without the file extension.</param>
        public static string SaveScreenshot(float startX, float startY, float width, float height, string filename)
        {
            string folder = Application.persistentDataPath;

            #if UNITY_EDITOR
            folder = Application.dataPath;
            #endif

            // Take the required portion of the screen
            Texture2D tt = CaptureScreenshot(startX, startY, width, height);

            // Encode texture into PNG
            byte[] bytes = tt.EncodeToPNG();

            // Save file to disk
            string filepath = Path.Combine(folder, filename + ".png");
            File.WriteAllBytes(filepath, bytes);

            // Destroy the temporary Texture2D object
            Object.Destroy(tt);
            tt = null;

            return filepath;
        }

        /// <summary>
        /// Captures a full-screen screenshot and returns the generated Texture2D object.
        /// Note that this method should be called at the end of the frame
        /// (called within a coroutine after WaitForEndOfFrame())
        /// </summary>
        /// <returns>The full-screen screenshot.</returns>
        public static Texture2D CaptureScreenshot()
        {
            return CaptureScreenshot(0, 0, Screen.width, Screen.height);
        }

        /// <summary>
        /// Captures the specified area of the screen and returns the generated Texture2D object.
        /// Note that this method should be called at the end of the frame
        /// (called within a coroutine after WaitForEndOfFrame())
        /// </summary>
        /// <returns>The screenshot.</returns>
        /// <param name="startX">Start x.</param>
        /// <param name="startY">Start y.</param>
        /// <param name="width">Width.</param>
        /// <param name="height">Height.</param>
        public static Texture2D CaptureScreenshot(float startX, float startY, float width, float height)
        {
            Texture2D tt = new Texture2D((int)width, (int)height, TextureFormat.RGB24, false);
            tt.ReadPixels(new Rect(startX, startY, width, height), 0, 0);
            tt.Apply();

            return tt;
        }
    }
}
