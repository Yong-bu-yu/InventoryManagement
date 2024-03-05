using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using InventoryManagement.Service.Drawing;
using InventoryManagement.Views.Template;
using Microsoft.Maui.Controls.Shapes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: Dependency(typeof(DrawingService))]
namespace InventoryManagement.Service.Drawing
{
    internal class DrawingService : IDrawingService
    {
        public DrawingPage DrawingPage { get; private set; } = new DrawingPage();
        public bool IsEnabled { get => DrawingPage.IsEnabled; set => DrawingPage.IsEnabled = value; }
        private DrawingView DrawingView { get => DrawingPage.DrawingView; }
        private ObservableCollection<IDrawingLine> Lines { get => DrawingView.Lines; set => DrawingView.Lines = value; }
        public string FilePath
        {
            get
            {
                string filePath = $"{FileSystem.AppDataDirectory}/Personnel_Credentials";
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                return filePath;
            }
        }

        public async Task<bool> TryLoadImage(string fileName)
        {
            try
            {
                string lineFile = await File.ReadAllTextAsync($"{FilePath}/{fileName}.json");
                IList<DrawingLine> drawingLines = JsonConvert.DeserializeObject<List<DrawingLine>>(lineFile);
                Lines = new ObservableCollection<IDrawingLine>(drawingLines);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task LoadImage(string fileName)
        {
            string lineFile = await File.ReadAllTextAsync($"{FilePath}/{fileName}.json");
            IList<DrawingLine> drawingLines = JsonConvert.DeserializeObject<List<DrawingLine>>(lineFile);
            Lines = new ObservableCollection<IDrawingLine>(drawingLines);
        }

        public async Task SaveImage(string fileName)
        {
            Stream stream = await DrawingView.GetImageStream(350, 550);
            byte[] buffer = new byte[stream.Length];
            await stream.ReadAsync(buffer);
            await stream.FlushAsync();
            stream.Close();
            FileStream fileImage = new FileStream($"{FilePath}/{fileName}.png", FileMode.OpenOrCreate);
            await fileImage.WriteAsync(buffer);
            await fileImage.FlushAsync();
            fileImage.Close();

            StreamWriter fileJson = File.CreateText($"{FilePath}/{fileName}.json");
            await fileJson.WriteAsync(JsonConvert.SerializeObject(Lines));
            await fileJson.FlushAsync();
            fileJson.Close();
        }

        public void Clear()
        {
            DrawingPage = new DrawingPage();
        }
    }
}
