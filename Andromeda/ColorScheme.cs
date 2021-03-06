﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Andromeda
{
    public class ColorScheme
    {
        public readonly string Normal;
        public readonly string Info;
        public readonly string Admin;
        public readonly string Error;
        public readonly string Player;
        
        public readonly string Highlight1;
        public readonly string Highlight2;
        public readonly string Highlight3;
        public readonly string Highlight4;

        public static readonly ColorScheme None = new ColorScheme();

        public ColorScheme(string normal = "^7", string info = "^7", string admin = "^7",
            string error = "^7", string player = "^7", string highlight1 = "^7",
            string highlight2 = "^7", string highlight3 = "^7", string highlight4 = "^7")
        {
            Normal = normal;
            Info = info;
            Admin = admin;
            Error = error;
            Player = player;

            Highlight1 = highlight1;
            Highlight2 = highlight2;
            Highlight3 = highlight3;
            Highlight4 = highlight4;
        }

        public Dictionary<string, string> Export()
            => new Dictionary<string, string>()
            {
                ["%n"] = Normal,
                ["%i"] = Info,
                ["%a"] = Admin,
                ["%e"] = Error,
                ["%p"] = Player,

                ["%h1"] = Highlight1,
                ["%h2"] = Highlight1,
                ["%h3"] = Highlight2,
                ["%h4"] = Highlight3,
            };
    }
}
