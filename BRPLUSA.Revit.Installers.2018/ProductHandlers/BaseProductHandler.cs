﻿using BRPLUSA.Revit.Installers._2018.Services;
using Squirrel;

namespace BRPLUSA.Revit.Installers._2018.ProductHandlers
{
    public abstract class BaseProductHandler : IProductHandler
    {
        public UpdateManager UpdateManager { get; set; }
        public FileInstallationService FileReplicationService { get; set; }

        protected BaseProductHandler(UpdateManager mgr, FileInstallationService frp)
        {
            UpdateManager = mgr;
            FileReplicationService = frp;
        }
    }
}