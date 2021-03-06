﻿using BRPLUSA.Revit.Installers._2018.Entities;
using Squirrel;

namespace BRPLUSA.Revit.Installers._2018.Services
{
    public class VersionService
    {
        public static VersionData GetVersionInformation(ReleaseEntry release)
        {
            if(release == null)
                return new VersionData();

            var data = release.Version.Version;
            return new VersionData(data.Major, data.Minor, data.Build, data.Revision);
        }
    }
}
