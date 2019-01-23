﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Andromeda.Interfaces;
using Andromeda.Events;
using InfinityScript;
using Andromeda.Cmd;

namespace Andromeda
{
    [Plugin]
    public static partial class Common
    {
        internal static readonly string Version = "Andromeda v0.0.1";

        internal static readonly string[] Credits = new[]
            {
                "Lambder & Markus - dem bois",
                "Slvr99 - help with AbortScript"
            };

        private static IPerms perms;
        private static readonly IPerms mockPerms = new Mock.Perms();
        public static IPerms Perms
            => perms ?? mockPerms;

        private static IUtils utils;
        private static readonly IUtils mockUtils = new Mock.Utils();
        public static IUtils Utils
            => utils ?? mockUtils;

        private static IAdmin admin;
        private static readonly IAdmin mockAdmin = new Mock.Admin();
        public static IAdmin Admin
            => admin ?? mockAdmin;

        internal static readonly List<IFunctionality> functionalities = new List<IFunctionality>();
        
        internal static readonly Dictionary<string, object> Exports = new Dictionary<string, object>();

        public static void PrintException(string message, string source)
            => Exception(new[]
            {
                $"Source: {source}",
                message,
            });

        public static void PrintException(Exception exception, string source)
            => Exception(new[] {
                $"Source: {source}",
                exception.Message,
                exception.StackTrace
            });

        public static void Exception(params string[] lines)
        {
            Log.Error("----------- Exception ------------");
            foreach (var line in lines)
                Log.Error(line);
            Log.Error("----------------------------------");
        }

        public static void Warning(params string[] lines)
        {
            Log.Info("------------- Warning ------------");
            foreach (var line in lines)
                Log.Info(line);
            Log.Info("----------------------------------");
        }

        public static string Format(this string str, Dictionary<string, string> format)
        {
            var builder = new StringBuilder(str);

            foreach (var val in format)
                builder.Replace(val.Key, val.Value);

            return builder.ToString();
        }

        public static IEnumerable<string> Condense(IEnumerable<string> enumerable, int condenselevel = 40, string separator = ", ")
        {
            string all = string.Join(separator, enumerable);
            List<string> toRet = new List<string>();

            while (all.Length > 0)
            {
                string toAdd = TruncateAtWord(ref all, condenselevel, separator);

                if (toAdd == null)
                    break;

                toRet.Add(toAdd);
            }

            return toRet;
        }

        public static string TruncateAtWord(ref string input, int length, string separator)
        {
            if (input != null && input.Length > 0)
            {
                length = length > input.Length ? input.Length : length;

                int iNextSpace = input.LastIndexOf(separator, length, StringComparison.Ordinal) + 1;
                string toRet = input.Substring(0, (iNextSpace > 0) ? iNextSpace : length);


                input = input.Replace(toRet, "");

                return toRet.TrimStart();
            }

            return null;
        }

        public static void Register(IFunctionality functionality)
        {
            if(functionality is IPerms perms)
            {
                if (Common.perms == null)
                {
                    Common.perms = perms;
                    Exports[nameof(Perms)] = Perms;
                }
                else
                    Warning("Perms already assigned", $"Ignoring new register: {perms.Version}");
            }

            if(functionality is IUtils utils)
            {
                if (Common.utils == null)
                {
                    Common.utils = utils;
                    Exports[nameof(Utils)] = Utils;

                    colorScheme = utils.ColorScheme.Export();
                }
                else
                    Warning("Utils already assigned", $"Ignoring new register: {utils.Version}");
            }

            if(functionality is IAdmin admin)
            {
                if (Common.admin == null)
                {
                    Common.admin = admin;
                    Exports[nameof(Admin)] = Admin;
                }
                else
                    Warning("Admin already assigned", $"Ignoring new register: {admin.Version}");
            }

            functionalities.Add(functionality);
        }

        [EntryPoint]
        private static void Init()
        {
            Log.Info("Initializing Andromeda");

            if(perms == null)
            {
                Warning(new[]
                {
                    "IPerms has not been registered",
                    "Mock IPerms will be used",
                });
            }

            if (utils == null)
            {
                Warning(new[]
                {
                    "IUtils has not been registered",
                    "Mock IUtils will be used",
                });
            }

            if (admin == null)
            {
                Warning(new[]
{
                    "IAdmin has not been registered",
                    "Mock IAdmin will be used",
                });
            }
        }

        static Common()
        {
            // VERSION
            Command.TryRegister(SmartParse.CreateCommand(
                name: "version",
                aliases: new[] { "v" },
                argTypes: new IArgParse[0],
                action: delegate (Entity sender, object[] args)
                {
                    sender.Tell(new[]
                    {
                        "%iVersions:",
                        Version,
                    }.Concat(
                        functionalities.Select(func => func.Version)
                    ));
                },
                usage: "!version",
                description: "Shows versions of functionalities"));

            // CREDITS
            Command.TryRegister(SmartParse.CreateCommand(
                name: "credits",
                argTypes: new IArgParse[0],
                action: delegate (Entity sender, object[] args)
                {
                    var msg = new[]
                    {
                        "%iVersions:",
                        $"%h1{Version}",
                    }.Concat(Credits);

                    foreach (var func in functionalities)
                    {
                        msg = msg.Concat(new[]
                        {
                            $"%h1{func.Version}"
                        }.Concat(func.Credits));
                    }

                    sender.Tell(msg);
                },
                usage: "!credits"));
        }
    }
}
