﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfinityScript;
using System.Collections;
using Andromeda;
using System.Net;
using Newtonsoft.Json;

namespace Updater
{
    [Plugin]
    public class Main
    {
        private const string versionsStringUrl = "http://www.oofmate.test";

        private static byte[] downloadBytes(string url)
        {
            try
            {
                var stream = WebRequest.Create(url).GetResponse().GetResponseStream();

                byte[] buf = new byte[stream.Length];
                int i = 0;
                int x;
                while ((x = stream.ReadByte()) != -1)
                    buf[i++] = (byte)x;

                return buf;
            }
            catch(Exception)
            {
                return null;
            }
        }

        [EntryPoint]
        private static void Init()
        {
            GSCFunctions.SetDvarIfUninitialized("andromeda_updates", 1);

            if (GSCFunctions.GetDvarInt("andromeda_updates") == 1)
                Async.Start(Update());
            else
                Log.Info("Andromeda update checks disabled");
        }

        private static IEnumerator Update()
        {
            Log.Info("Checking for updates...");

            yield return Async.Detach();

            var versionBytes = downloadBytes(versionsStringUrl);

            if(versionBytes == null)
            {
                Log.Info("Failed downloading versions file");
                yield break;
            }

            var versions = JsonConvert.DeserializeObject<VersionInfo[]>(Encoding.UTF8.GetString(versionBytes));

            var md5 = System.Security.Cryptography.MD5.Create();

            var updates = versions.Where(update =>
            {
                var currentMd5 = md5.ComputeHash(System.IO.File.ReadAllBytes(update.FilePath));

                return currentMd5 != update.Md5Hash;
            }).ToList();

            if (updates.Any())
            {
                Log.Info("Updates found:");
                foreach (var update in versions)
                {
                    var name = update.Name;
                    Log.Info($"Updating to {name}");

                    var bytes = downloadBytes(update.DownloadUrl);

                    if(bytes == null)
                    {
                        Log.Info($"Failed updating to {name}");
                        Log.Info("Failed downloading new file");

                        continue;
                    }

                    if (md5.ComputeHash(versionBytes) != update.Md5Hash)
                    {
                        Log.Info($"Failed updating to {name}");
                        Log.Info("Mismatching md5 hash");

                        continue;
                    }

                    var oldFilePath = $"{update.FilePath}.old";

                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);

                    System.IO.File.Move(update.FilePath, oldFilePath);

                    System.IO.File.WriteAllBytes(update.FilePath, downloadBytes(update.DownloadUrl));

                    Log.Info($"Updated {name}");
                }
            }
            else
            {
                Log.Info("No updates found!");
            }

            yield return Async.Attach();

            Log.Info("Update checking complete");
        }
    }
}
