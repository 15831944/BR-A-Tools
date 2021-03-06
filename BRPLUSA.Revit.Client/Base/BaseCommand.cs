﻿using System;
using System.Diagnostics;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BRPLUSA.Revit.Core.Exceptions;

namespace BRPLUSA.Revit.Client.Base
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public abstract class BaseCommand : IExternalCommand
    {
        protected ExternalCommandData ExternalCommandData { get; set; }
        protected string MainMessage { get; set; }
        protected ElementSet ElementSet { get; set; }
        protected UIApplication CurrentApplication => ExternalCommandData.Application;
        protected Document CurrentDocument => CurrentApplication.ActiveUIDocument.Document;
        protected UIDocument UiDocument => CurrentApplication.ActiveUIDocument;

        // necessary method that Revit needs to call
        Result IExternalCommand.Execute(
            ExternalCommandData excmd,
            ref string mainmessage,
            ElementSet elemset)
        {
            MainMessage = mainmessage;
            ExternalCommandData = excmd;
            ElementSet = elemset;

            Result result = Result.Failed;

            try
            {
                result = InternalExecute();
            }

            catch (Exception e)
            {
                TaskDialog.Show("Unhandled Failure", $"The command failed due to an unhandled error ");
            }

            finally
            {
                UiDocument.Selection?.Dispose();
                //CurrentDocument.Regenerate();
            }

            return result;
        }

        // Internal method that allows this class to use this private fields it contains
        // without having to set them necessarily.
        protected Result InternalExecute()
        {
            try
            {
                // defined in derived classes
                return Work();
            }

            catch (CancellableException e)
            {
                return Result.Cancelled;
            }

            catch(Exception e)
            {
                Debug.WriteLine("Command failed because of an unknown exception");
                TaskDialog.Show("Command Failed",
                    $"There was an error behind the scenes that caused the command to fail horribly and die. {e.Message}");
            }

            return Result.Failed;
        }

        protected abstract Result Work();
    }
}