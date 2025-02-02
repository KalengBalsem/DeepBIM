using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace deepbim

{

    public class App : IExternalApplication

    {

        public Result OnStartup(UIControlledApplication application)

        {

            // Create a Ribbon Panel

            RibbonPanel ribbonPanel = application.CreateRibbonPanel("DeepBIM");



            // Create a Push Button

            string thisAssemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            PushButtonData buttonData = new PushButtonData(

                "deepbimApp",

                "deepbim",

                thisAssemblyPath,

                "deepbim.Command");



            PushButton pushButton = ribbonPanel.AddItem(buttonData) as PushButton;


            // Add an icon
            //Make dynamic
            string imagePath = @"C:\Users\balse\Documents\BIM\Revit\Plugins\deepbim\deepbim\blue_whale.png";


            if (File.Exists(imagePath))
            {
                using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    BitmapImage largeImage = new BitmapImage();
                    largeImage.BeginInit();
                    largeImage.StreamSource = stream;
                    largeImage.CacheOption = BitmapCacheOption.OnLoad;
                    largeImage.EndInit();

                    // Assign the image to the button
                    pushButton.LargeImage = largeImage;
                    pushButton.Image = largeImage;
                }
            }
            else
            {
                TaskDialog.Show("Error", $"Image file not found at {imagePath}");
            }

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)

        {

            // Cleanup
            return Result.Succeeded;

        }

    }

}