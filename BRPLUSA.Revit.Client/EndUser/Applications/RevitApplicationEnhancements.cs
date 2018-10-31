﻿using System;
using System.Diagnostics;
using Autodesk.Revit.UI;
using BRPLUSA.Core.Services;
using BRPLUSA.Revit.Client.EndUser.Commands;
using BRPLUSA.Revit.Client.EndUser.Commands.Mechanical;
using BRPLUSA.Revit.Client.EndUser.Services;
using BRPLUSA.Revit.Client.UI.Views;
using BRPLUSA.Revit.Services.Registration;
using BRPLUSA.Revit.Services.Web;

namespace BRPLUSA.Revit.Client.EndUser.Applications
{
    // if this doesn't debug - check this link:
    // https://blogs.msdn.microsoft.com/devops/2013/10/16/switching-to-managed-compatibility-mode-in-visual-studio-2013/
    public class RevitApplicationEnhancements : IExternalApplication
    {
        public BardWebClient Sidebar { get; set; }
        private static SocketService SocketService { get; set; }

        public Result OnStartup(UIControlledApplication app)
        {
            return Initialize(app);
        }

        public Result OnShutdown(UIControlledApplication app)
        {
            return Uninitialize(app);
        }

        private Result Initialize(UIControlledApplication app)
        {
            try
            {
                LoggingService.LogInfo("Starting up application via Revit");
                ResolveBrowserBinaries();
                RegisterSideBar(app);
                CreateRibbon(app);

                var backupAuto = new AutoModelBackupService();
                var backupManual = new ManualModelBackupService();

                UpdaterRegistrationService.AddRegisterableServices(
                    //new SpatialPropertyUpdater(app),
                    backupAuto
                    );

                SocketRegistrationService.AddRegisterableServices(
                    backupManual
                    );

                app.ControlledApplication.DocumentOpened += UpdaterRegistrationService.RegisterServices;
                app.ControlledApplication.DocumentOpened += SocketRegistrationService.RegisterServices;
                app.ControlledApplication.DocumentClosed += UpdaterRegistrationService.DeregisterServices;
                app.ControlledApplication.DocumentClosed += SocketRegistrationService.DeregisterServices;

                LoggingService.LogInfo("Application loaded successfully");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                LoggingService.LogError("Failed to load application due to internal error:", e);
                TaskDialog.Show("Fatal error", $"Failed to load application due to internal exception: {e.Message}");
                return Result.Failed;
            }
        }

        private Result Uninitialize(UIControlledApplication app)
        {
            try
            {
                LoggingService.LogInfo("Shutting down application via Revit");
                app.ControlledApplication.DocumentOpened -= UpdaterRegistrationService.RegisterServices;
                app.ControlledApplication.DocumentOpened -= SocketRegistrationService.RegisterServices;
                app.ControlledApplication.DocumentClosed -= UpdaterRegistrationService.DeregisterServices;
                app.ControlledApplication.DocumentClosed -= SocketRegistrationService.DeregisterServices;
                LoggingService.LogInfo("Application shutdown complete!");
                return Result.Succeeded;
            }
            catch (Exception e)
            {
                LoggingService.LogError("Failure to shut down correctly", e);
                return Result.Failed;
            }
        }

        public void CreateRibbon(UIControlledApplication app)
        {
            try
            {
                LoggingService.LogInfo("Creating Application ribbon within Revit");
                app.CreateRibbonTab("BR+A");

                var brpa = app.CreateRibbonPanel("BR+A", "Utilities");
                var toggleSidebar = new PushButtonData("Toggle Sidebar", "Toggle Sidebar",
                    typeof(ShowSidebar).Assembly.Location, typeof(ShowSidebar).FullName);
                //var spaceSync = new PushButtonData("Link Spaces", "Link Spaces", typeof(LinkSpaces).Assembly.Location, typeof(LinkSpaces).FullName);
                //var exportAreaToNavis = new PushButtonData("Export Area To Navisworks", "Clash Area", typeof(ExportAreaToNavis).Assembly.Location, typeof(ExportAreaToNavis).FullName);
                var findElement = new PushButtonData("Find Element By Name", "Find Element",
                    typeof(SelectByName).Assembly.Location, typeof(SelectByName).FullName);
                //var findPanel = new PushButtonData("Find Panel By Name", "Find Panel", typeof(SelectPanelFromSchedule).Assembly.Location, typeof(SelectPanelFromSchedule).FullName);
                var switchType = new PushButtonData("Switch Element Type", "Switch Element Type",
                    typeof(ForceTypeSwap).Assembly.Location, typeof(ForceTypeSwap).FullName);
                var createVentSchedule = new PushButtonData("Create Vent Schedule", "Create Vent Schedule",
                    typeof(CreateVentilationRequirementsScheduleCommand).Assembly.Location,
                    typeof(CreateVentilationRequirementsScheduleCommand).FullName);

                brpa.AddItem(toggleSidebar);
                //brpa.AddItem(spaceSync);
                //brpa.AddItem(exportAreaToNavis);
                brpa.AddItem(findElement);
                //brpa.AddItem(findPanel);
                brpa.AddItem(switchType);
                brpa.AddItem(createVentSchedule);
                LoggingService.LogInfo("Application Ribbon creation complete");
            }

            catch (Exception e)
            {
                throw new Exception("Failed to create application ribbon within Revit", e);
            }
        }

        public void ResolveBrowserBinaries()
        {
            try
            {
                LoggingService.LogInfo("Attempting to resolve browser binaries");

                AppDomain.CurrentDomain.AssemblyResolve += BardWebClient.Resolver;
                BardWebClient.InitializeCefSharp();

                LoggingService.LogInfo("Browser binary resolution complete");
            }

            catch (Exception e)
            {
                throw new Exception("Failed to resolve browser binaries", e);
            }
        }

        private void RegisterSideBar(UIControlledApplication app)
        {
            try
            {
                LoggingService.LogInfo("Attempting to register Bard Client sidebar with Revit");

                Sidebar = new BardWebClient(SocketService);
                Sidebar.RegisterEvents(app);

                app.RegisterDockablePane(BardWebClient.Id, "BR+A Revit Helper", Sidebar);

                LoggingService.LogInfo("Browser binary resolution complete");
            }

            catch (Exception e)
            {
                throw new Exception("Failed to resolve browser binaries", e);
            }
        }
    }
}