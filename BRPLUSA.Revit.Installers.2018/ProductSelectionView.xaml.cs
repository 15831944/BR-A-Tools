﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using BRPLUSA.Revit.Installers._2018.Services;
using RSG;

namespace BRPLUSA.Revit.Installers._2018
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ProductSelectionView : Window
    {
        private const string _updateAvailable = "Update Available!";
        private const string _updateNotAvailable = "Up to date";
        private const string _productInstalled = "Installed";
        private const string _productInstalling = "Installing...";
        private const string _productNeedsInstall = "Install";
        private const string _productCanBeUpgraded = "Upgrade";
        private const string _installerHeadline =
            "BR+A's Revit Enhancements are designed to help with a great deal of " +
            "modeling and engineering tasks. " +
            "\n" +
            "\n" +
            "Use the buttons below to install or upgrade " +
            "the application and watch your productivity skyrocket!";

        private InstallManager Manager { get; set; }

        private bool Revit2018Installed { get; set; }
        private bool AppFor2018Installed { get; set; }
        private bool AppFor2018Installing { get; set; }
        private bool AppFor2018CanInstall { get; set; }
        private bool AppFor2018HasUpdateAvailable { get; set; }

        public ProductSelectionView(InstallManager mgr)
        {
            InitializeComponent();
            InitializeServices(mgr);
        }

        private void InitializeServices(InstallManager mgr)
        {
            Manager = mgr;
            HandlePreInstallationChecks();
            ContentRendered += SetInstallationStatuses;
            //ContentRendered += SetInstallationStatusesAsync;
        }

        private void HandlePreInstallationChecks()
        {
            SetRevit2018InstallStatus(Manager.Revit2018Installed);

            if (Revit2018Installed)
                return;

            ShowRevit2018NotInstalled();
        }

        private void SetInstallationStatuses(object sender, EventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                SetInstallerHeadlineContent();
                SetApplicationVersion();
                SetAppFor2018InstallStatus(Manager.AppFor2018Installed);
                SetAppFor2018UpdateAvailability(Manager.AppFor2018HasUpdateAvailable);
            });
        }

        private void SetApplicationVersion()
        {
            //var version = Manager.AppVersion
        }
        private void SetInstallerHeadlineContent()
        {
            InstallerHeadline.Text = _installerHeadline;
        }

        private void SetRevit2018InstallStatus(bool status)
        {
            Revit2018Installed = status;
            AppFor2018CanInstall = status;
        }

        private void ShowAppFor2018InstallationComplete()
        {
            AppFor2018Installing = false;
            AppFor2018CanInstall = false;
            ButtonRevit2018AppInstallStatus.Background = new SolidColorBrush(Color.FromRgb(51,157,255));
            ButtonRevit2018AppInstallStatus.Content = _productInstalled;
        }

        private void ShowAppFor2018InstallationInProcess()
        {
            AppFor2018Installing = true;
            AppFor2018CanInstall = false;
            ButtonRevit2018AppInstallStatus.Background = Brushes.Gray;
            ButtonRevit2018AppInstallStatus.Content = _productInstalling;
        }

        private void ShowAppFor2018InstallationFailed()
        {
            AppFor2018Installing = false;
            AppFor2018CanInstall = true;
            ButtonRevit2018AppInstallStatus.Background = Brushes.Crimson;
            ButtonRevit2018AppInstallStatus.Foreground = Brushes.White;
            ButtonRevit2018AppInstallStatus.Content = "Failed";
        }

        private void ShowRevit2018NotInstalled()
        {
            ButtonRevit2018AppInstallStatus.Background = Brushes.Gray;
            ButtonRevit2018AppInstallStatus.Foreground = Brushes.White;
            ButtonRevit2018AppInstallStatus.Content = "Can't Install";

            Revit2018UpdateStatus.Foreground = Brushes.Crimson;
            Revit2018UpdateStatus.Text = "Revit 2018 Not Installed";

            AppFor2018CanInstall = false;
        }

        private void SetAppFor2018InstallStatus(bool status)
        {
            AppFor2018Installed = status;
            AppFor2018CanInstall = !status;
            ButtonRevit2018AppInstallStatus.Content = status
                ? _productInstalled
                : _productNeedsInstall;
        }

        private void SetAppFor2018UpdateAvailability(bool status)
        {
            AppFor2018HasUpdateAvailable = status;

            if (AppFor2018Installed)
            {
                ButtonRevit2018AppInstallStatus.Content = status
                    ? _productCanBeUpgraded
                    : _productInstalled;

                Revit2018UpdateStatus.Text = status
                ? _updateAvailable
                : _updateNotAvailable;
            }
        }

        private void ShutdownPage(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnDragRequest(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private async void InstallRevit2018(object sender, RoutedEventArgs e)
        {
            if (!AppFor2018CanInstall)
                return;

            ShowAppFor2018InstallationInProcess();

            //var success = await Manager.HandleRevit2018ApplicationInstallation();

            var promise = new Promise(
                async (resolve, reject) =>
                {
                    var success = await Manager.HandleRevit2018ApplicationInstallation();

                    if (success)
                    {
                        resolve();
                    }
                    else
                    {
                        reject(null);
                    }
                }
            );

            promise.Done(
                ShowAppFor2018InstallationComplete,
                (err) => ShowAppFor2018InstallationFailed()
                );
        }
    }
}
